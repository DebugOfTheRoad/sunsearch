using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunIndex.Core.Index
{
    public class DictAndState
    {
        public bool IsNew
        {
            get;
            set;
        }
        public HashDict Dict
        {
            get;
            set;
        }
    }
}
