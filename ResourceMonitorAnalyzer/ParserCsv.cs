using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using static System.Convert;


namespace ResourceMonitorAnalyzer
{
    public class ParserCsv
    {
        public static IEnumerable<string> ModifiedHeader(string source)
        {
            return source.Split(',')
                .Select(x => x.Trim())
                .Select(x => Regex.Replace(x, @"^[""][\\][\\]([^\\]*)[\\]", @""""))
                .Select(x => x
                    .Replace('[', '(')
                    .Replace(']', ')')
                    .Replace('\\','.')
                    .Replace("(0)", "(00)")
                    .Replace("(1)", "(01)")
                    .Replace("(2)", "(02)")
                    .Replace("(3)", "(03)")
                    .Replace("(4)", "(04)")
                    .Replace("(5)", "(05)")
                    .Replace("(6)", "(06)")
                    .Replace("(7)", "(07)")
                    .Replace("(8)", "(08)")
                    .Replace("(9)", "(09)")
                        )
                .Select(x => string.Concat(x.Filter(_ => _ != '\"'))
                );
        }

        public static string GetMachineName(string source)
        {
            return string.Concat(
                Regex.Match(
                        source.Split(',')
                            .Skip(1)
                            .DefaultIfEmpty("")
                            .FirstOrDefault()
                            .Trim()
                        , @"^[""][\\][\\]([^\\]*)[\\]")
                    .Value
                    .Filter(x => x != '\\')
                    .Filter(x => x != '\"')
            );
        }

        

        public static IEnumerable<AnalyzedResult> CsvParseAndAnalysis(string sourcePath)
        {
            using (var csv = File.Open(sourcePath, FileMode.Open))
            using (var sr = new StreamReader(csv))
            {
                var rawHeader = sr.ReadLine();
                var headers = ModifiedHeader(rawHeader).ToSeq(); // header
                var machineName = GetMachineName(rawHeader);

                var rows = new List<(DateTime, Seq<decimal>)>();

                while (sr.EndOfStream != true)
                {
                    var value = sr.ReadLine()?.Split(',');
                    var dateTime = DateTime.Parse(string.Concat(value?[0].Filter(y => y != '\"')));
                    var result = value.DefaultIfEmpty("")
                        .Select(x => string.Concat(x.AsEnumerable().Filter(y => y != '\"')))
                        .Select(x => parseDecimal(x)
                            .Match(_ => _, () => decimal.MinValue))
                        .ToSeq();

                    rows.Add((dateTime, result));
                }

                var ret = new List<AnalyzedResult>();

                for (int i = 1; i < headers.Count; i++)
                {
                    var date = rows.Map(x => x.Head()).OrderBy(x => x).Skip(ToInt32(rows.Count * 0.5)).FirstOrDefault();
                    var columns = rows.Map(x => x.Last().Skip(i).FirstOrDefault()).ToSeq();
                    var ordered = columns.Filter(x => x != decimal.MinValue).OrderBy(x => x).ToSeq();
                    var low05 = ordered.Skip(ToInt32(ordered.Count * 0.05)).FirstOrDefault();
                    var low14 = ordered.Skip(ToInt32(ordered.Count * 0.14)).FirstOrDefault();
                    var high05 = ordered.Skip(ToInt32(ordered.Count * (1 - 0.05))).FirstOrDefault();
                    var high14 = ordered.Skip(ToInt32(ordered.Count * (1 - 0.14))).FirstOrDefault();

                    // 상위 5%, 하위 5%를 제외한 평균
                    var average90 = columns.Filter(x => x > low05 && x < high05).DefaultIfEmpty().Average();
                    // 90%중 상위 10%의 평균
                    var average90H10 = columns.Filter(x => x > high14 && x < high05).DefaultIfEmpty().Average();
                    // 90%중 하위 10%의 평균
                    var average90L10 = columns.Filter(x => x > low05 && x < low14).DefaultIfEmpty().Average();

                    ret.Add(new AnalyzedResult
                    (
                        machineName,
                        date.Date,
                        headers[i],
                        average90,
                        average90H10,
                        average90L10
                    ));
                }
                return ret;
            }
        }
    }
}
