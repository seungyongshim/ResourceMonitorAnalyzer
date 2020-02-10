using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static LanguageExt.Prelude;

namespace ResourceMonitorAnalyzer
{
    public class ParserCsv
    {
        public static IEnumerable<string> ModifiedHeader(string source)
        {
            return source.Split(',')
                .Select(x => x.Trim())
                .Select(x => Regex.Replace(x, @"^[""][\\][\\]([^\\]*)[\\]", @""""))
                .Select(x => x.Replace('[', '(').Replace(']', ')').Replace('\\','.'));
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
    }
}
