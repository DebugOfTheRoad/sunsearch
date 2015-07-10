using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunIndex.Core.Db
{
   public class Column
    {
        public string Name
        {
            get;
            set;
        }
        public int Rank
        {
            get;
            set;
        }
        public bool Save
        {
            get;
            set;
        }
    }
}
