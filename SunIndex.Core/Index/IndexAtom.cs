using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunIndex.Core.Index
{
    /// <summary>
    /// 索引最小单位
    /// </summary>
    public class IndexAtom
    {
        /// <summary>
        /// 单词首次出现位置
        /// </summary>
        public int Position
        {
            get; set;
        }
        /// <summary>
        /// 单词权重
        /// </summary>
        public float Rank
        {
            get;
            set;
        } 
        /// <summary>
        /// 单词出现频次（使用的时候需要除以100）
        /// </summary>
        public UInt16 Frequency 
        {
            get;
            set;
        }
    }
}
