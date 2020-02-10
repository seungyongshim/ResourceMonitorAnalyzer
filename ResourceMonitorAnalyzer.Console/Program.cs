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
        public readonly string MachineName;
        public readonly string Header;
        public readonly decimal Average90;
        public readonly decimal Average90H10;
        public readonly decimal Average90L10;

        public AnalyzedResult(string machineName, string header, decimal average90, decimal average90H10, decimal average90L10)
        {
            MachineName = machineName;
            Header = header;
            Average90 = average90;
            Average90H10 = average90H10;
            Average90L10 = average90L10;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendLine(Header)
                .AppendLine($"  Avg90: {Average90}")
                .AppendLine($"  Avg90H10: {Average90H10}")
                .AppendLine($"  Avg90L10: {Average90L10}")
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
                var rawHeader = sr.ReadLine();
                var headers = ModifiedHeader(rawHeader).ToSeq(); // header
                var machineName = GetMachineName(rawHeader);

                var rows = new List<Seq<decimal>>();

                while (sr.EndOfStream != true)
                {
                    var value = sr.ReadLine().Split(',');
                    var dateTime = DateTime.Parse(string.Concat(value?[0].Filter(y => y != '\"')));
                    var result = value.DefaultIfEmpty("")
                        .Select(x => string.Concat(x.AsEnumerable().Filter(y => y != '\"')))
                        .Select(x => parseDecimal(x)
                            .Match(_ => _, () => decimal.MinValue))
                        .ToSeq();

                    rows.Add(result);
                }

                for (int i = 1; i < headers.Count; i++)
                {
                    var columns = rows.Map(x => x.Skip(i).FirstOrDefault()).ToSeq();
                    var ordered = columns.Filter(x => x != decimal.MinValue).OrderBy(x => x).ToSeq();
                    var low05 = ordered.Skip(Convert.ToInt32(ordered.Count * 0.05)).FirstOrDefault();
                    var low14 = ordered.Skip(Convert.ToInt32(ordered.Count * 0.14)).FirstOrDefault();
                    var high05 = ordered.Skip(Convert.ToInt32(ordered.Count * (1 - 0.05))).FirstOrDefault();
                    var high14 = ordered.Skip(Convert.ToInt32(ordered.Count * (1 - 0.14))).FirstOrDefault();

                    // 상위 5%, 하위 5%를 제외한 평균
                    var average90 = columns.Filter(x => x > low05 && x < high05).DefaultIfEmpty().Average();
                    // 90%중 상위 10%의 평균
                    var average90H10 = columns.Filter(x => x > high14 && x < high05).DefaultIfEmpty().Average();
                    // 90%중 하위 10%의 평균
                    var average90L10 = columns.Filter(x => x > low05 && x < low14).DefaultIfEmpty().Average();

                    var result = new AnalyzedResult
                    (
                        machineName,
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
