using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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
    }
}
