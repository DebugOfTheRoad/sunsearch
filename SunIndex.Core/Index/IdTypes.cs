using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunIndex.Core.Index
{
    public enum IdTypes
    {
        /// <summary>
        /// 字典编号
        /// </summary>
        DictId=0,
        /// <summary>
        /// 链表编号
        /// </summary>
        ListId=1,
        /// <summary>
        /// 文档编号
        /// </summary>
        DocId=2,
        /// <summary>
        /// 自编号
        /// </summary>
        Self=3
    }
}
