namespace SunIndex.Core.Document
{
    /// <summary>
    /// 需要索引的列
    /// </summary>
    public class PropertyInfo
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 字段权重(-1为该字段不需要索引)
        /// </summary>
        public int Rank
        {
            get;
            set;
        }
        /// <summary>
        /// 字段值
        /// </summary>
        public string Value
        {
            get;
            set;
        } 
        /// <summary>
        /// 字段是否需要存储到文档数据里
        /// </summary>
        public bool Save
        {
            get;
            set;
        }
    }
}
