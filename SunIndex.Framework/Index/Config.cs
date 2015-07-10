using System.Collections.Generic;
using SunIndex.Framework.MongoDb;

namespace SunIndex.Framework.Index
{
    public class Config
    {
        /// <summary>
        /// mongoDb的连接端
        /// </summary>
        public List<ServerHost> ServerHosts
        {
            get;
            set;
        }
        /// <summary>
        /// 基础数据数据库名称(自增Id等)
        /// </summary>
        public string BaseDbName
        {
            get;
            set;
        }
        /// <summary>
        /// 索引列表数据库名称
        /// </summary>
        public string IndexListDbName
        {
            get;
            set;
        }
        int _maxIndexListCache = 20000;
        public int MaxIndexListCache
        {
            get {
                return _maxIndexListCache;
            }
            set
            {
                _maxIndexListCache = value;
            }
        }
    }
}
