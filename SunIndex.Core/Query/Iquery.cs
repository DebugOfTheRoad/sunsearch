using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunIndex.Core.Query
{
    public interface Iquery
    {
       Task<Page<string>> Select(string query, int page, int size);
    }
}
