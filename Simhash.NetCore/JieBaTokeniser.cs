using System.Collections.Generic;
using System.IO;
using System.Linq;
using JiebaNet.Segmenter;

namespace Simhash.NetCore
{
    public class JieBaTokeniser : ITokeniser
    {
        public IEnumerable<string> Analytical(string input)
        {
            var segment = new JiebaSegmenter();
            var features = new List<string>();
            var stopWordsList = GetStopWords();
            foreach (var feature in segment.Cut(input))
            {
                if (stopWordsList.Any(s => s.Contains(feature)))
                    continue;
                features.Add(feature);
            }

            return features;
        }

        private List<string> GetStopWords()
        {
            var stopWordsList = new List<string>();
            using (var sr = new StreamReader("./stopwords.txt"))
            {
                while (!sr.EndOfStream)
                {
                    stopWordsList.Add(sr.ReadLine());
                }
            }

            return stopWordsList;
        }
    }
}