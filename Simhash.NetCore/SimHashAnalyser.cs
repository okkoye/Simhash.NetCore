using System;
using System.Collections.Generic;
using System.Linq;

namespace Simhash.NetCore
{
    public class SimHashAnalyser : ITextAnalyser
    {
        private static int fpSize = 64;

        public ulong GetSimilarityValue(string leftText, ulong rightHashVector)
        {
            var leftTextSimHash = GetTextHashVector(leftText);
            return HammingDistance(leftTextSimHash, rightHashVector);
        }

        public ulong GetSimilarityValue(string leftText, string rightText)
        {
            var leftTextSimHash = GetTextHashVector(leftText);
            var rightTextSimHash = GetTextHashVector(rightText);

            return HammingDistance(leftTextSimHash, rightTextSimHash);
        }

        public ulong GetTextHashVector(string text)
        {
            ITokeniser tokeniser = new JieBaTokeniser();
            return GetTextHashVector(tokeniser.Analytical(text).ToList());
        }

        private static ulong GetTextHashVector(List<string> features)
        {
            var frequencies = features.GroupBy(w => w).ToDictionary(w => w.Key, w => w.Count());
            var documentHashVector = new int[fpSize];
            features.ForEach(f =>
            {
                var wordHashVector = GetWordHashVector(f, frequencies[f]);
                documentHashVector = AddWordHashVector(documentHashVector, wordHashVector);
            });
            documentHashVector = NormalizeHashVector(documentHashVector);
            var returnString = documentHashVector.Aggregate("", (current, t) => current + t);
            return Convert.ToUInt64(returnString, 2);
        }

        private static int[] GetWordHashVector(string feature, int weight)
        {
            var v = new int[fpSize];
            var featureHashCode = CalculateHash(feature);
            var featureString = Convert.ToString(featureHashCode, 2);
            for (var i = 0; i < fpSize - featureString.Length; i++)
            {
                if (featureString[i] == '1')
                {
                    v[i] = weight;
                }
                else
                {
                    v[i] = -weight;
                }
            }

            return v;
        }

        private static int[] AddWordHashVector(IReadOnlyList<int> left, IReadOnlyList<int> right)
        {
            var result = new int[fpSize];
            for (var i = 0; i < fpSize; i++)
            {
                result[i] = left[i] + right[i];
            }

            return result;
        }

        private static int[] NormalizeHashVector(IReadOnlyList<int> hashVector)
        {
            var result = new int[fpSize];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = hashVector[i] > 0 ? 1 : 0;
            }

            return result;
        }

        /// <summary>
        /// 生成64位的hash值
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private static long CalculateHash(string feature)
        {
            var s1 = feature.Substring(0, feature.Length / 2);
            var s2 = feature.Substring(feature.Length / 2);

            var x = (long) s1.GetHashCode() << 0x20 | s2.GetHashCode();

            return x;
        }

        private static ulong HammingDistance(ulong left, ulong right)
        {
            var bb = left ^ right;
            const ulong c55 = 0x5555555555555555ul;
            const ulong c33 = 0x3333333333333333ul;
            const ulong c0F = 0x0f0f0f0f0f0f0f0ful;
            const ulong c01 = 0x0101010101010101ul;

            bb -= (bb >> 1) & c55;
            bb = (bb & c33) + ((bb >> 2) & c33);
            bb = (bb + (bb >> 4)) & c0F;
            return (bb * c01) >> 56;
        }
    }
}