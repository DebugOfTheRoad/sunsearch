using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunIndex.Core.Db
{
   public class IndexSql
    {
        /// <summary>
        /// 连接名
        /// </summary>
        public string ConName
        {
            get;
            set;
        }
        public string Sql
        {
            get;
            set;
        }
        public List<Column> Columns
        {
            get;
            set;
        }
    }
}
