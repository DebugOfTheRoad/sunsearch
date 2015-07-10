using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace SunIndex.Core.Index
{
    /// <summary>
    /// 倒排列表
    /// </summary>
    public class IndexList
    {
        /// <summary>
        /// 列表编号
        /// </summary>
        public string Id
        {
            get;
            set;
        }
        /// <summary>
        /// 集合列表(key为文档编号,value为单词附带信息)
        /// </summary>
        public ConcurrentDictionary<string,IndexAtom> IndexAtoms
        {
            get;set;
        }
    }
}
