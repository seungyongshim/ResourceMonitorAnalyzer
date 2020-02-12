using LanguageExt;
using NLog;
using System;
using System.Text;

namespace ResourceMonitorAnalyzer
{
    public class AnalyzedResult: Record<AnalyzedResult>
    {
        public readonly string MachineName;
        public readonly DateTime Date;
        public readonly string Type;
        public readonly decimal Average90;
        public readonly decimal Average90H10;
        public readonly decimal Average90L10;

        public AnalyzedResult(string machineName, DateTime date, string type, decimal average90, decimal average90H10, decimal average90L10)
        {
            MachineName = machineName;
            Date = date;
            Type = type;
            Average90 = average90;
            Average90H10 = average90H10;
            Average90L10 = average90L10;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendLine($"{MachineName} - {Date.ToShortDateString()} - {Type}")
                .AppendLine($"  Avg90: {Average90}")
                .AppendLine($"  Avg90H10: {Average90H10}")
                .AppendLine($"  Avg90L10: {Average90L10}")
                .ToString();
        }

        public string GetCsvValue()
        {
            return new StringBuilder()
            .Append(Date.ToShortDateString())
            .Append(',').Append(MachineName)
            .Append(',').Append(Type)
            .Append(',').Append(Average90)
            .Append(',').Append(Average90H10)
            .Append(',').Append(Average90L10)
            .ToString();
        }

        public static string GetCsvHeader()
        {
            return new StringBuilder()
                .Append(nameof(Date))
                .Append(',').Append(nameof(MachineName))
                .Append(',').Append(nameof(Type))
                .Append(',').Append(nameof(Average90))
                .Append(',').Append(nameof(Average90H10))
                .Append(',').Append(nameof(Average90L10))
                .ToString();
        }
    }
}
