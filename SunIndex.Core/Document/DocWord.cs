using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunIndex.Core.Hash;

namespace SunIndex.Core.Document
{
    /// <summary>
    /// 文档粗词(用以保存文档分词完成后的单词信息)
    /// </summary>
    public class DocWord : WordAtom
    {
        public DocWord(string word, int position, int rank) : base(word)
        {
            this.Position = position;
            this.Rank = rank;
        }
        /// <summary>
        /// 单词位置
        /// </summary> 
        public int Position
        {
            get; set;
        }
        /// <summary>
        /// 单词权重
        /// </summary>
        public int Rank
        {
            get;
            set;
        }
    }
}
