using LanguageExt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static LanguageExt.Prelude;
using static ResourceMonitorAnalyzer.ParserCsv;

namespace ResourceMonitorAnalyzer.ConsoleApp
{
    public class AnalyzedResult: Record<AnalyzedResult>
    {
        public readonly string Header;
        public readonly decimal Average90;
        public readonly decimal Average90H10;
        public readonly decimal Average90L10;

        public AnalyzedResult(string header, decimal average90, decimal average90H10, decimal average90L10)
        {
            Header = header;
            Average90 = average90;
            Average90H10 = average90H10;
            Average90L10 = average90L10;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendLine(Header)
                .AppendLine($"  Average90: {Average90}")
                .AppendLine($"  Average90H10: {Average90H10}")
                .AppendLine($"  Average90L10: {Average90L10}")
                .ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using (var csv = File.Open("data.csv", FileMode.Open))
            using (var sr = new StreamReader(csv))
            {
                var headers = ModifiedHeader(sr.ReadLine()).ToSeq(); // header

                var rows = new List<Seq<decimal>>();

                while (sr.EndOfStream != true)
                {
                    var value = sr.ReadLine();
                    var result = value.Split(',')
                        .Select(x => string.Concat(x.AsEnumerable().Where(y => y != '\"')))
                        .Select(x => parseDecimal(x)
                            .Match(_ => _, () => -1))
                        .ToSeq();

                    rows.Add(result);
                }

                var totalCount = rows.Count;

                for (int i = 1; i < headers.Count; i++)
                {
                    var columns = rows.Map(x => x.Skip(i).FirstOrDefault()).ToSeq();
                    var ordered = columns.OrderBy(x => x).ToSeq();
                    var low05 = ordered.Skip(Convert.ToInt32(totalCount * 0.05)).FirstOrDefault();
                    var low14 = ordered.Skip(Convert.ToInt32(totalCount * 0.14)).FirstOrDefault();
                    var high05 = ordered.Skip(Convert.ToInt32(totalCount * (1 - 0.05))).FirstOrDefault();
                    var high14 = ordered.Skip(Convert.ToInt32(totalCount * (1 - 0.14))).FirstOrDefault();

                    // 상위 5%, 하위 5%를 제외한 평균
                    var average90 = columns.Filter(x => x > low05 && x < high05).DefaultIfEmpty().Average();
                    // 90%중 상위 10%의 평균
                    var average90H10 = columns.Filter(x => x > high14 && x < high05).DefaultIfEmpty().Average();
                    // 90%중 하위 10%의 평균
                    var average90L10 = columns.Filter(x => x > low05 && x < low14).DefaultIfEmpty().Average();

                    var result = new AnalyzedResult
                    (
                        headers[i],
                        average90,
                        average90H10,
                        average90L10
                    );

                    Console.WriteLine(result);
                }
            }
        }
    }
}
