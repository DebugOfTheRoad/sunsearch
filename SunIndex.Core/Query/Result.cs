using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunIndex.Core.Query
{
    public class Result
    {
        /// <summary>
        /// 文档编号
        /// </summary>
        public string DocId
        {
            get;
            set;
        }
        /// <summary>
        /// 最终得分
        /// </summary>
        public float Rank
        {
            get;
            set;
        }
        /// <summary>
        /// 第一次出现搜索词的位置
        /// </summary>
        public int Position
        {
            get;
            set;
        }
    }
}
