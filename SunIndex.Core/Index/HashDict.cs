using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace SunIndex.Core.Index
{
    /// <summary>
    /// 单词字典
    /// </summary>
    public class HashDict
    {
        /// <summary>
        /// 字典hash值
        /// </summary>
        public int Hashcode
        {
            get;
            set;
        }
        /// <summary>
        /// 冲突哈希词库
        /// </summary>
        public ConcurrentDictionary<string,string> Words
        {
            get;set;
        }
    }
}
