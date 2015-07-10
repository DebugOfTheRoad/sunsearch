using System;
using System.Collections.Generic;

namespace SunIndex.Core.Document
{
    public interface ITokenizer
    {
        /// <summary>
        /// 对指定的索引内容进行分词处理
        /// </summary>
        /// <param name="contents">索引内容</param>
        /// <returns>粗词列表</returns>
        List<DocWord> GetWords(List<PropertyInfo> contents);
        /// <summary>
        /// 对指定的文本内容进行分词处理
        /// </summary>
        /// <param name="content">文本内容</param>
        /// <returns>粗词列表</returns>
        List<DocWord> GetWords(string content);
    }
}
