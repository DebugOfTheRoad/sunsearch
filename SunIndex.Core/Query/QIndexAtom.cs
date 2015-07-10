using SunIndex.Core.Index;
using SunIndex.Core.Document;

namespace SunIndex.Core.Query
{
    public class QIndexAtom
    {
        /// <summary>
        /// 归属词
        /// </summary>
        public DocWord Word
        {
            get;
            set;
        }
        /// <summary>
        /// 文档编号
        /// </summary>
        public string DocId
        {
            get;
            set;
        }
        /// <summary>
        /// 文档附加信息
        /// </summary>
        public IndexAtom Atom
        {
            get;
            set;
        }
    }
}
