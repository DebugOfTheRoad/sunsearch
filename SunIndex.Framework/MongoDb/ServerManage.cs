using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MongoDB.Driver;
using SunIndex.Core.Hash;
using SunIndex.Framework.Index;

namespace SunIndex.Framework.MongoDb
{

    public class ServerManage
    {
        static Dictionary<string, MongoClient> clientsDict = new Dictionary<string, MongoClient>();
        static ReaderWriterLockSlim slimLock = new ReaderWriterLockSlim();
        static ConsistentHash MongoServers = new ConsistentHash();
        static List<ServerHost> ServerHosts = new List<ServerHost>();
        /// <summary>
        /// 1:Mongodb服务地址需要更新,0:不需要更新
        /// </summary>
        public bool HostNeedUpdate
        {
            get;
            set;
        }
        public ServerManage(Config config)
        {
            HostNeedUpdate = true;
            ServerHosts = config.ServerHosts;
            List<string> serverNames = new List<string>();
            foreach (var server in config.ServerHosts)
            {
                serverNames.Add(server.GroupName);
            }
            MongoServers.Init(serverNames, 10);
        }
        /// <summary>
        /// 获取所有节点
        /// </summary>
        /// <returns></returns>
        public List<MongoClient> GetAllNode()
        {
            return clientsDict.Values.ToList();
        }
        /// <summary>
        /// 通过哈希一致性获取节点
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public MongoClient GetNode(string code)
        {
            return MongoDbClient(MongoServers.GetNode(code));
        }
        /// <summary>
        /// 获取一个随机文件clent
        /// </summary>
        public  MongoClient MongoDbClient(string groupName)
        {
            if (HostNeedUpdate)
            {
                slimLock.EnterWriteLock();
                clientsDict.Clear();

                foreach (var h in ServerHosts)
                {
                    if (h.State != 0)
                    {
                        var setting = new MongoClientSettings();
                        if (h.Hosts.Count > 1)
                        {
                            var servers = new List<MongoServerAddress>();
                            foreach (var o in h.Hosts)
                            {
                                var pindex = o.LastIndexOf(':');
                                var host = o.Substring(0, pindex);
                                var port = int.Parse(o.Substring(pindex + 1));
                                servers.Add(new MongoServerAddress(host, port));
                            }
                            setting.Servers = servers;
                            setting.ReplicaSetName = h.GroupName;
                            setting.ReadPreference = new ReadPreference(ReadPreferenceMode.Secondary);
                        }
                        else
                        {
                            var pindex = h.Hosts[0].LastIndexOf(':');
                            var host = h.Hosts[0].Substring(0, pindex);
                            var port = int.Parse(h.Hosts[0].Substring(pindex + 1));
                            setting.Server = new MongoServerAddress(host, port);
                        }
                        if (!string.IsNullOrEmpty(h.UserId))
                            setting.Credentials = new[] { MongoCredential.CreateMongoCRCredential("admin", h.UserId, h.PassWord) };
                        setting.MaxConnectionPoolSize = 1000;
                        setting.MinConnectionPoolSize = 50;
                        var mongo = new MongoClient(setting);
                        clientsDict.Add(h.GroupName, mongo);
                    }
                }
                slimLock.ExitWriteLock();
                HostNeedUpdate = false;
            }
            MongoClient client;
            if (clientsDict.TryGetValue(groupName, out client))
            {
                return client;
            }
            else
            {
                throw new Exception("mongo群组不存在");
            }
        }
    }
}
