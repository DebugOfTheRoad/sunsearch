using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using SunIndex.Core.Query;
using SunIndex.Core.Document;
using SunIndex.Core.Index;

namespace SunIndex.Framework.Query
{
   public class Query : Iquery
    {
        ITokenizer Tokenizer;
        IIndex IndexOp;
        public Query(ITokenizer tokenizer,IIndex indexOp)
        {
            Tokenizer = tokenizer;
            IndexOp = indexOp;
        }
        public async Task<Page<string>> Select(string query, int page, int size)
        {
            var words = Tokenizer.GetWords(query).Where(w=>w.Word.Length>1).ToList();
            int skip = (page - 1) * size;
            List<Task<IndexList>> lists = new List<Task<IndexList>>();
            foreach (var word in words)
            {
                lists.Add(IndexOp.GetOrAddIndexList(word));
            }
            var indexs = await Task.WhenAll(lists);
            List<QIndexAtom> atomList = new List<QIndexAtom>();//查询结果
            List<Result> mResults = new List<Result>();//未排序计算完成结果
            if (indexs.Count() > 0)
            {
                for (int i = 0; i < indexs.Count(); i++)
                {
                    var currentWord = words[i];
                    foreach (var atom in indexs[i].IndexAtoms)
                    {
                        var qAtom = new QIndexAtom();
                        qAtom.Word = currentWord;
                        qAtom.DocId = atom.Key;
                        qAtom.Atom = atom.Value;
                        atomList.Add(qAtom);
                    }
                }
            }
            var groups = atomList.GroupBy(a => a.DocId);
            foreach (var group in groups)
            {
                Result r = new Result();
                r.DocId = group.Key;
                if (group.Count() == 1)
                {
                    var first = group.First();
                    r.Position = first.Atom.Position;
                    r.Rank = first.Atom.Rank;
                    r.Rank += GetFrequencyRank(first.Atom.Frequency);
                    r.Rank += first.Word.Rank;//把查询权重加到原权重上干扰结果排序
                }
                else
                {
                    float max=0,rangeRank=0;
                    var list = group.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        var idata = list[i];
                        if (idata.Word.Rank > max)
                            max = idata.Word.Rank;
                        r.Rank += GetFrequencyRank(idata.Atom.Frequency);
                        if (idata.Atom.Position < r.Position)
                            r.Position = idata.Atom.Position;
                        for (int j = i + 1; j < list.Count(); j++)
                        {
                            var jdata = list[j];
                            var wordRange = jdata.Word.Position-idata.Word.Position-idata.Word.Word.Length;
                            var docRange = jdata.Atom.Position-idata.Atom.Position-idata.Word.Word.Length;
                            float rank = 3;
                            if (wordRange != docRange)
                            {
                                rank =rank + (wordRange - docRange)/10.0f;
                                if (rank < 0.3f)
                                    rank = 0.3f;
                                if (rank > 3.3f)
                                    rank = 3.3f;
                            }
                            rangeRank += rank;
                        }
                    }
                    r.Rank +=max;//把查询权重加到原权重上干扰结果排序
                    r.Rank += rangeRank;
                }
                
                mResults.Add(r);
            }
            Page<string> result = new Page<string>();
            result.CurrentPage = page;
            result.Items= mResults.OrderByDescending(d => d.Rank).OrderBy(d => d.Position).Skip(skip).Take(size).Select(d=>d.DocId).ToList();
            result.TotalItems = mResults.Count();
            result.PageSize = size;
            return result;
        }
        private float GetFrequencyRank(float frequency)
        {
            if (frequency >= 8)
            {
                return 3;
            }
            else
            {
                return (float)(frequency * 0.37);
            }
        }
    }
}
