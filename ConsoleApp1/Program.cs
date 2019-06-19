using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Simhash.NetCore;
using TinyCsvParser;
using TinyCsvParser.Mapping;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var targetDataSet = GetTargetDataset();
//            BuildResultDataset(targetDataSet);
            var resultDataSet = GetResultDataset();
            var sw = new Stopwatch();
            ITextAnalyser analyser = new SimHashAnalyser();
            sw.Start();
            var testText = "您好呀，我是叶敏华";
//            foreach (var item in resultDataSet)
//            {
//                Console.WriteLine($"正在和{item.QID}比对......");
//                var similarityValue = analyser.GetSimilarityValue(testText, item.TextHashVector);
//                Console.WriteLine($"海明距离：{similarityValue}");
//            }
            var tagItem = targetDataSet.First().Content;
            Console.WriteLine($"目标内容是:{tagItem}");
            var result = analyser.GetSimilarityValue(testText, tagItem);
            Console.WriteLine($"海明距离：{result}");

            sw.Stop();
            Console.WriteLine($"用时：{sw.ElapsedMilliseconds} ms");
        }

        static string ReplaceHtmlTag(string html)
        {
            var strText = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", "");
            strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");
            return strText;
        }

        static void BuildResultDataset(IEnumerable<SpamWords> spamWordses)
        {
            using (var sw = new StreamWriter("result.txt"))
            {
                foreach (var r in spamWordses)
                {
                    var result = "";
                    var text = ReplaceHtmlTag(r.Content);
                    ITextAnalyser analyser = new SimHashAnalyser();
                    var textHash = analyser.GetTextHashVector(text);
                    result = $"{textHash.ToString()},{r.QID}";
                    sw.WriteLine(result);
                }

                sw.Close();
            }

            Console.WriteLine("生成样例数据成功......");
        }

        static List<SpamWords> GetTargetDataset()
        {
            var csvParserOptions = new CsvParserOptions(true, ',');
            var csvParser = new CsvParser<SpamWords>(csvParserOptions, new CsvSpamWordsMapping());
            var records = csvParser.ReadFromFile("helloworld.txt", Encoding.UTF8);

            var result = new List<SpamWords>();
            foreach (var record in records)
            {
                if (!record.IsValid)
                    continue;
                var item = new SpamWords()
                {
                    QID = record.Result.QID,
                    Content = record.Result.Content
                };
                result.Add(item);
            }

            return result;
        }

        static List<Item> GetResultDataset()
        {
            var result = new List<Item>();
            var csvParserOptions = new CsvParserOptions(false, ',');
            var csvParser = new CsvParser<Item>(csvParserOptions, new CsvItemMapping());
            var records = csvParser.ReadFromFile("result.txt", Encoding.UTF8);
            foreach (var record in records)
            {
                if (!record.IsValid)
                    continue;
                var item = new Item()
                {
                    QID = record.Result.QID,
                    TextHashVector = record.Result.TextHashVector
                };
                result.Add(item);
            }

            return result;
        }
    }

    public class CsvSpamWordsMapping : CsvMapping<SpamWords>
    {
        public CsvSpamWordsMapping() : base()
        {
            MapProperty(0, x => x.QID);
            MapProperty(1, x => x.Content);
            MapProperty(2, x => x.ConvertedContent);
        }
    }

    public class CsvItemMapping : CsvMapping<Item>
    {
        public CsvItemMapping() : base()
        {
            MapProperty(0, x => x.TextHashVector);
            MapProperty(1, x => x.QID);
        }
    }
}