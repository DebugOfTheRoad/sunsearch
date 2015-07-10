using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunIndex.Core.Hash;

namespace SunIndex.Core.Document
{
    /// <summary>
    /// 单词原子
    /// </summary>
    public class WordAtom 
    {
        public WordAtom(string word)
        {
            this.Word = word;
        }
        /// <summary>
        /// 单词
        /// </summary>
        public string Word
        {
            get;
            set;
        }
        public override int GetHashCode()
        {
            return (int)MurmurHash2.Hash(Encoding.UTF8.GetBytes(Word));
        }
        public override bool Equals(object obj)
        {
            if (obj == null||this==null)
                return false;
            return this.Word.Equals(((WordAtom)obj).Word);
        }
    }
}
