﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunIndex.Core.Query
{
    /// <summary>
    /// 分页相关信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Page<T>
    {
        public long CurrentPage { get; set; }
        public long TotalItems { get; set; }
        public long PageSize { get; set; }//perPage:每页
        public List<T> Items { get; set; }
    }
}
