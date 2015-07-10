using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PanGu;
using PanGu.Dict;
using System.Configuration;
using SunIndex.Core.Index;
using SunIndex.Framework.Index;
using Newtonsoft.Json;
using System.Diagnostics;
using SunIndex.Core.Document;
using SunIndex.Framework.MongoDb;
using SunIndex.Framework.Db;
using SunIndex.Core.Ioc;
using SunIndex.Core.Db;
using MongoDB.Bson;
using Autofac;
using SunIndex.Core.Query;
using MsgPack.Serialization;


namespace RevertedIndex
{

    class Program
    {
        
        int id = 1;
        static IIndex indexOp = null;
        static object lockobj = new object();
        static void Main(string[] args)
        {
            Start.Init();

            List<IndexSql> needIndexs = JsonConvert.DeserializeObject<List<IndexSql>>(ConfigurationManager.AppSettings["NeedIndes"].ToString());
            indexOp = CoreIoc.Container.Resolve<IIndex>();
            Iquery qOp = CoreIoc.Container.Resolve<Iquery>();
            IDocument docOp = CoreIoc.Container.Resolve<IDocument>();
            //Console.WriteLine("开始清理数据");
            //indexOp.Monitor();
            //indexOp.InitIndexData().Wait();
            //Console.WriteLine("开始创建索引");
            //var time = Stopwatch.StartNew();
            //indexOp.RebuilIndex(needIndexs).Wait();
            //indexOp.SaveAllData();
            //time.Stop();
            //Console.WriteLine("重建索引完成,处理文档数量:{0},耗费时间：{1}ms", 1375000, time.ElapsedMilliseconds);
            while (true)
            {
                string str = Console.ReadLine();
                var three = Stopwatch.StartNew();
                Console.WriteLine("开始查询文档内容");
                var rtask = qOp.Select(str, 1, 10);
                rtask.Wait();
                three.Stop();
                foreach (var r in rtask.Result.Items)
                {
                    var task = docOp.GetBsonById(r);
                    task.Wait();
                    BsonDocument bson = task.Result;
                    Console.WriteLine(bson.ToJson());
                }
                Console.WriteLine("文档查询完成,耗时{0}ms,结果总数为为:{1}", three.ElapsedMilliseconds, rtask.Result.TotalItems);
                Console.ReadLine();
            }
        }
    }
}
