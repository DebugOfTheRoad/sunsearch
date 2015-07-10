using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using SunIndex.Core.Index;
using SunIndex.Framework.MongoDb;
using MongoDB.Driver;
using MongoDB.Bson;
using SunIndex.Core.Hash;
using System.Threading;
using SunIndex.Core.Document;
using SunIndex.Core.Db;
using SunIndex.Framework.Db;
using MsgPack.Serialization;

namespace SunIndex.Framework.Index
{
    public class Index : IIndex
    {
        Config Config;
        ServerManage Mongo;
        ITokenizer Tokenizer;
        IDocument DocOp;
        ConsistentHash ListHash=new ConsistentHash();
       
        IMongoCollection<BsonDocument> DictCollection=null;
        IMongoCollection<BsonDocument> IncrementCollection = null;
        MongoClient baseClient = null;

        const string DictIdTypeName = "dictlist";
        const string IndexListIdTypeName = "indexlist";
        const string DictColName = "HDict";
        int indexListUsers = 0;

        ConcurrentDictionary<int, DictAndState> DictList = new ConcurrentDictionary<int, DictAndState>();

        ConcurrentDictionary<string, HashIndexList> CurrentIndexList = new ConcurrentDictionary<string, HashIndexList>();

        MessagePackSerializer<ConcurrentDictionary<string, string>> DictSerializer=SerializationContext.Default.GetSerializer<ConcurrentDictionary<string, string>>();
        MessagePackSerializer<Dictionary<string, string>> DictDerializer = SerializationContext.Default.GetSerializer<Dictionary<string, string>>();
        MessagePackSerializer<Dictionary<string, IndexAtom>> AtomDerializer = SerializationContext.Default.GetSerializer<Dictionary<string, IndexAtom>>();
        MessagePackSerializer<ConcurrentDictionary<string, IndexAtom>> AtomSerializer = SerializationContext.Default.GetSerializer<ConcurrentDictionary<string, IndexAtom>>();
        public Index(Config config, ServerManage mongo, ITokenizer tokenizer, IDocument docOp)
        {
            this.Config = config;
            this.Mongo = mongo;
            Tokenizer = tokenizer;
            DocOp = docOp;

            List<string> IndexListCollectionNodes = new List<string>() { "Ilist_1", "Ilist_2", "Ilist_3", "Ilist_4", "Ilist_5", "Ilist_6", "Ilist_7", "Ilist_8", "Ilist_9", "Ilist_10" };
            ListHash.Init(IndexListCollectionNodes, 20);

            var baseServer = config.ServerHosts.SingleOrDefault(s => s.IsBase);
            if (baseServer == null)
                baseServer = config.ServerHosts[0];
            baseClient = Mongo.MongoDbClient(baseServer.GroupName);
            DictCollection = baseClient.GetDatabase(Config.BaseDbName).GetCollection<BsonDocument>(DictColName);
            IncrementCollection = baseClient.GetDatabase(Config.BaseDbName).GetCollection<BsonDocument>("Increment");
            LoadDict();
            //单词字典索引结束
        }
        /// <summary>
        /// 加载字典数据
        /// </summary>
        private void LoadDict()
        {
            var findFilter = Builders<BsonDocument>.Filter.Type("_id", BsonType.ObjectId);
            var listTask = DictCollection.Find(findFilter).ToListAsync();
            listTask.Wait();
            Parallel.ForEach<BsonDocument>(listTask.Result, dict =>
            {
                var hashcode = dict["Hashcode"].AsInt32;
                DictList.TryAdd(hashcode, new DictAndState()
                {
                    IsNew = false,
                    Dict = new HashDict {Hashcode = hashcode, Words =new ConcurrentDictionary<string, string>(DictDerializer.UnpackSingleObject(dict["Words"].AsByteArray)) }
                });
            });
        }
        /// <summary>
        /// 清空所有索引数据以便重新开始索引
        /// </summary>
        /// <returns></returns>
        public async Task InitIndexData()
        {
            var nodes = Mongo.GetAllNode();
            foreach (var node in nodes)
            {
                await node.DropDatabaseAsync(Config.IndexListDbName);
            }
            await baseClient.GetDatabase(Config.BaseDbName).DropCollectionAsync(DictColName);
            await DocOp.InitDocData();
            DictList.Clear();
            CurrentIndexList.Clear();
        }
        /// <summary>
        /// 根据单词获取一个倒排列表
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public async Task<IndexList> GetOrAddIndexList(WordAtom word)
        {
            HashIndexList hashIndex = null;
            if (CurrentIndexList.TryGetValue(word.Word, out hashIndex))
            {
                return hashIndex.List;
            }
            int hashcode = word.GetHashCode();
            DictAndState dict;
            MongoClient Client = Mongo.GetNode(hashcode.ToString());

            if (!DictList.TryGetValue(hashcode, out dict))
            {
                dict = new DictAndState()
                {
                    IsNew = true,
                    Dict = new HashDict()
                    {
                        Hashcode = hashcode,
                        Words = new ConcurrentDictionary<string, string>()
                    }
                };
                hashIndex = new HashIndexList() { Hashcode = hashcode, List = CreateEmptyIndexList(word), IsNew = true };
                dict.Dict.Words.TryAdd(word.Word, hashIndex.List.Id);
                if (!DictList.TryAdd(hashcode, dict))
                {
                    return null;
                }
            }
            string listId;
            if (dict.Dict.Words.TryGetValue(word.Word, out listId))
            {
                if (hashIndex == null || listId != hashIndex.List.Id)
                {
                    var indexListDb = Client.GetDatabase(Config.IndexListDbName).GetCollection<BsonDocument>(ListHash.GetNode(hashcode.ToString()));
                    var index = await indexListDb.Find(Builders<BsonDocument>.Filter.Eq("_id", listId)).SingleOrDefaultAsync();
                    hashIndex = new HashIndexList()
                    {
                        Hashcode = hashcode,
                        IsNew = false,
                        List = new IndexList()
                        {
                            Id = listId,
                            IndexAtoms = index != null ? new ConcurrentDictionary<string, IndexAtom>(AtomDerializer.UnpackSingleObject(index["IndexAtoms"].AsByteArray)) : new ConcurrentDictionary<string, IndexAtom>()
                        }
                    };
                }
            }
            if (!CurrentIndexList.TryAdd(word.Word, hashIndex))
                return null;
            return hashIndex.List;
        }
        /// <summary>
        /// 清理索引列表缓存
        /// </summary>
        private void ClearCache()
        {
            lock(CurrentIndexList)
            {
                while (true)
                {
                    if (indexListUsers == 0)
                    {
                        foreach (var list in CurrentIndexList.Values)
                        {
                            var collection = Mongo.GetNode(list.Hashcode.ToString()).GetDatabase(Config.IndexListDbName).GetCollection<BsonDocument>(ListHash.GetNode(list.Hashcode.ToString()));
                            byte[] data = AtomSerializer.PackSingleObject(list.List.IndexAtoms);
                            if (!list.IsNew)
                            {
                                var filter = Builders<BsonDocument>.Filter.Eq("_id", list.List.Id);
                                var update = Builders<BsonDocument>.Update.Set("IndexAtoms",data);
                                collection.UpdateOneAsync(filter, update).Wait();
                            }
                            else
                            {

                                var bd = new BsonDocument().Add("_id", list.List.Id).Add("IndexAtoms", data);
                                collection.InsertOneAsync(bd).Wait();
                            }
                        }
                        CurrentIndexList.Clear();
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// 监控索引字典数据
        /// </summary>
        /// <returns></returns>
        public async Task Monitor()
        {
            while (true)
            {
                if (CurrentIndexList.Count() > Config.MaxIndexListCache)
                {
                    ClearCache();
                }
                await Task.Delay(1000);
            }
        }
        /// <summary>
        /// 通知索引已经完成(保存索引数据)
        /// </summary>
        /// <returns></returns>
        public async Task SaveAllData()
        {
            List<Task> tasks = new List<Task>();
            List<Task<UpdateResult>> updateTasks = new List<Task<UpdateResult>>();
            foreach (var dict in DictList)
            {
                byte[] data = DictSerializer.PackSingleObject(dict.Value.Dict.Words);
                if (!dict.Value.IsNew)
                {
                    var findFilter = Builders<BsonDocument>.Filter.Eq("Hashcode", dict.Key);
                    var update = Builders<BsonDocument>.Update.Set("Words",data);
                    updateTasks.Add(DictCollection.UpdateOneAsync(findFilter, update));
                }
                else
                {
                    var bd = new BsonDocument().Add("Hashcode",dict.Value.Dict.Hashcode).Add("Words", data);
                    tasks.Add(DictCollection.InsertOneAsync(bd));
                }
            }
            foreach (var list in CurrentIndexList.Values)
            {
                var collection = Mongo.GetNode(list.Hashcode.ToString()).GetDatabase(Config.IndexListDbName).GetCollection<BsonDocument>(ListHash.GetNode(list.Hashcode.ToString()));

                byte[] data = AtomSerializer.PackSingleObject(list.List.IndexAtoms);
                if (!list.IsNew)
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", list.List.Id);
                    var update = Builders<BsonDocument>.Update.Set("IndexAtoms", data);
                    updateTasks.Add(collection.UpdateOneAsync(filter, update));
                }
                else
                {
                    var bd = new BsonDocument().Add("_id", list.List.Id).Add("IndexAtoms", data);
                    tasks.Add(collection.InsertOneAsync(bd));
                }
            }
            await Task.WhenAll(tasks);
            await Task.WhenAll(updateTasks);
        }
        /// <summary>
        /// 创建空倒排列表
        /// </summary>
        /// <param name="word">单词</param>
        /// <returns></returns>
        private IndexList CreateEmptyIndexList(WordAtom word)
        {
            var posList = new IndexList();
            posList.Id =ObjectId.GenerateNewId().ToString();
            posList.IndexAtoms = new ConcurrentDictionary<string, IndexAtom>();
            return posList;
        }

        /// <summary>
        /// 把内容分词结果整理并返回结果
        /// </summary>
        /// <param name="docWords"></param>
        /// <param name="docId"></param>
        /// <returns></returns>
        public List<IndexWord> Extraction(List<DocWord> docWords, string docId)
        {
            List<IndexWord> result = new List<IndexWord>();
            var groups = docWords.GroupBy(o => o.Word);
            foreach (var group in groups)
            {
                var atom = new IndexAtom();
                atom.Position = group.Min(d => d.Position);
                atom.Rank = group.Max(d => d.Rank);
                atom.Frequency = Convert.ToUInt16((group.Count() * 1.0 / docWords.Count()) * 100);
                result.Add(new IndexWord(docId, group.Key, atom));
            }
            return result;
        }

        /// <summary>
        /// 重建索引
        /// </summary>
        /// <param name="needIndexs"></param>
        /// <returns></returns>
        public async Task RebuilIndex(List<IndexSql> needIndexs)
        {
            List<Task> tasks = new List<Task>();
            foreach (var need in needIndexs)
            {
                long page = 0, totalPage = 1;
                var database = DbContainer.GetDatabase(need.ConName);
                while (page < totalPage)
                {
                    var pageData = database.Page<dynamic>((page + 1), 2000, need.Sql);
                    totalPage = pageData.TotalPages;
                    page = pageData.CurrentPage;
                    tasks.Add(AppendIndex(pageData.Items, need.Columns));
                }
            }
            await Task.WhenAll(tasks);
        }
        /// <summary>
        /// 附加索引数据
        /// </summary>
        /// <param name="items">数据列表</param>
        /// <param name="columns">需要出列的列</param>
        /// <returns></returns>
        public async Task AppendIndex(List<dynamic> items, List<Column> columns)
        {
            List<IndexWord> wordList = new List<IndexWord>();
            foreach (var item in items)
            {
                List<PropertyInfo> fileds = new List<PropertyInfo>();
                foreach (var property in (IDictionary<String, Object>)item)
                {
                    var columun = columns.SingleOrDefault(c => c.Name.ToLower() == property.Key.ToLower());
                    if (columun != null)
                    {
                        fileds.Add(new PropertyInfo() { Name = columun.Name, Rank = columun.Rank, Value = property.Value.ToString(), Save = columun.Save });
                    }
                }
                var docId = await DocOp.save(fileds);
                var words = Tokenizer.GetWords(fileds);
                var indexList = Extraction(words, docId);
                wordList.AddRange(indexList);
            };
            await AppendIndex(wordList);
        }
        /// <summary>
        /// 附加索引数据
        /// </summary>
        /// <param name="indexWords">索引词列表</param>
        /// <returns></returns>
        public async Task AppendIndex(List<IndexWord> indexWords)
        {
            var groups = indexWords.GroupBy(i => i.Word);
            foreach (var group in groups)
            {
                IndexList postList = null;
                while (postList == null)
                {
                    postList = await GetOrAddIndexList(new WordAtom(group.Key));
                };
                Interlocked.Increment(ref indexListUsers);
                foreach (var indexWord in group)
                {
                    postList.IndexAtoms.TryAdd(indexWord.DoctId, indexWord.WordIndexAtom);
                }
                Interlocked.Decrement(ref indexListUsers);
            };
        }
    }
}
