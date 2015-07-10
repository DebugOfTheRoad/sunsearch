using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunIndex.Core.Index;
using PanGu;
using PanGu.Dict;
using SunIndex.Core.Document;

namespace SunIndex.Framework.Index
{
    public class PanguTokenizer : ITokenizer
    {
        PanGu.Match.MatchOptions _Options;
        PanGu.Match.MatchParameter _Parameters;
        Segment segment;
        public PanguTokenizer(PanGu.Match.MatchOptions options, PanGu.Match.MatchParameter parameters)
        {
            _Options = options;_Parameters = parameters;
            segment = new Segment();
        }
        public List<DocWord> GetWords(List<PropertyInfo> contents)
        {
            List<DocWord> result = new List<DocWord>();
            foreach (var filed in contents)
            {
                ICollection<WordInfo> words = segment.DoSegment(filed.Value, _Options, _Parameters);
                foreach (var word in words)
                {
                    if (word != null)
                    {
                        result.Add(new DocWord(word.Word,word.Position,word.Rank+filed.Rank));
                    }
                }
            }
            return result;
        }
        public List<DocWord> GetWords(string content)
        {
            ICollection<WordInfo> words = segment.DoSegment(content, _Options, _Parameters);
            List<DocWord> result = new List<DocWord>();
            foreach (var word in words)
            {
                if (word != null)
                {
                    result.Add(new DocWord(word.Word, word.Position, word.Rank));
                }
            }
            return result;
        }
    }
}
