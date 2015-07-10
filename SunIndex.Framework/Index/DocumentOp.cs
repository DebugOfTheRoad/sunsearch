using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SunIndex.Core.Document;
using MongoDB.Bson;
using MongoDB.Driver;
using SunIndex.Core.Index;
using SunIndex.Framework.MongoDb;
using SunIndex.Core.Hash;
using SunIndex.Core.Ioc;
using Autofac;

namespace SunIndex.Framework.Index
{
    public class DocumentOp : IDocument
    {
        ServerManage Mongo;
        const string idType = "doctid";
        const string DocDatabase = "Serch_Docs";
        ConsistentHash ListHash = new ConsistentHash();
        
        public DocumentOp(ServerManage mongo)
        {
            Mongo = mongo;
            
            List<string> IndexListNodes = new List<string>() { "Doclist_1", "Doclist_2", "Doclist_3", "Doclist_4", "Doclist_5", "Doclist_6", "Doclist_7", "Doclist_8", "Doclist_9", "Doclist_10" };
            ListHash.Init(IndexListNodes,10);
        }

        public async Task InitDocData()
        {
            var nodes = Mongo.GetAllNode();
            foreach (var node in nodes)
            {
               await node.DropDatabaseAsync(DocDatabase);
            }
        }
        /// <summary>
        /// 保存数据并返回DocId
        /// </summary>
        /// <param name="propertys"></param>
        /// <returns></returns>
        public async Task<string> save(List<PropertyInfo> propertys)
        {
            var id = ObjectId.GenerateNewId().ToString();
            BsonDocument bson = new BsonDocument("_id",id);
            foreach (var property in propertys)
            {
                if (property.Save)
                {
                    bson.Add(property.Name, property.Value);
                }
            }
            var collection=Mongo.GetNode(id.ToString()).GetDatabase(DocDatabase).GetCollection<BsonDocument>(ListHash.GetNode(id.ToString()));
            await collection.InsertOneAsync(bson);
            return id;
        }

        public async Task<BsonDocument> GetBsonById(string docId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id",docId);
            var colName = ListHash.GetNode(docId.ToString());
            var collection = Mongo.GetNode(docId.ToString()).GetDatabase(DocDatabase).GetCollection<BsonDocument>(colName);
            return await collection.Find(filter).SingleOrDefaultAsync();
        }
    }
}
