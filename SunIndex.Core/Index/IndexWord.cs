using SunIndex.Core.Document;

namespace SunIndex.Core.Index
{
    public class IndexWord : WordAtom
    {
        public IndexWord(string docId,string word, IndexAtom indexAtom) : base(word)
        {
            this.WordIndexAtom = indexAtom;
            this.DoctId = docId;
        }
        /// <summary>
        /// 文档编号
        /// </summary>
        public string DoctId
        {
            get;set;
        }
        public IndexAtom WordIndexAtom
        {
            get; set;
        }
    }
}
