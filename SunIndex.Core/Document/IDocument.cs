using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace SunIndex.Core.Document
{
    public interface IDocument
    {
        /// <summary>
        /// 初始化所有文档数据(会导致原来所有文档数据丢失)
        /// </summary>
        Task InitDocData();
        Task<string> save(List<PropertyInfo> propertys);
        Task<BsonDocument> GetBsonById(string docId);
    }
}
