using System.Collections.Generic;
using System.Threading.Tasks;
using SunIndex.Core.Document;
using SunIndex.Core.Db;

namespace SunIndex.Core.Index
{
    /// <summary>
    /// 索引接口
    /// </summary>
    public interface IIndex
    {
        /// <summary>
        /// 初始化索引信息(会导致已有索引信息丢失)
        /// </summary>
        Task InitIndexData();
        /// <summary>
        /// 监控并清理索引缓存
        /// </summary>
        /// <returns></returns>
        Task Monitor();
        /// <summary>
        /// 重新构建索引
        /// </summary>
        /// <param name="needIndexs"></param>
        /// <returns></returns>
        Task RebuilIndex(List<IndexSql> needIndexs);
        /// <summary>
        /// 附加索引
        /// </summary>
        /// <param name="items"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        Task AppendIndex(List<dynamic> items,List<Column> columns);
        /// <summary>
        /// 把最终索引词写入所属的倒排列表
        /// </summary>
        /// <param name="indexWord">索引词</param>
        Task AppendIndex(List<IndexWord> indexWords);
        /// <summary>
        /// 获取或添加索引列表
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        Task<IndexList> GetOrAddIndexList(WordAtom word);
        /// <summary>
        /// 通知索引已经完成(保存索引数据)
        /// </summary>
        /// <returns></returns>
        Task SaveAllData();
        /// <summary>
        /// 从文档分词后的粗词结果中提取最终索引词
        /// </summary>
        /// <param name="docWords">粗词列表</param>
        /// <param name="docId">粗词所属文档编号</param>
        /// <returns>最终索引词列表</returns>
        List<IndexWord> Extraction(List<DocWord> docWords, string docId);
    }
}
