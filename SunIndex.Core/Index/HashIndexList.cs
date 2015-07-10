using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunIndex.Core.Index
{
   public class HashIndexList
    {
        public int Hashcode
        {
            get;
            set;
        }
        /// <summary>
        /// 是否为新建列表
        /// </summary>
        public bool IsNew
        {
            get;set;
        }
        public IndexList List
        {
            get;set;
        }
    }
}
