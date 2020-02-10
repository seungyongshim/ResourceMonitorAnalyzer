using LanguageExt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static ResourceMonitorAnalyzer.ParserCsv;
using static LanguageExt.Prelude;
using static System.Convert;
using CommandLine;
using System.Threading.Tasks;

namespace ResourceMonitorAnalyzer.ConsoleApp
{
    class Program
    {
        public class Options
        {
            [Option('s', "source", Required = true, HelpText = "Source CSV file")]
            public string Source { get; set; }

            [Option('t', "target", Required = false, HelpText = "Target CSV file")]
            public string Target { get; set; }


        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(async o =>
                {
                    if (!File.Exists(o.Source))
                    {
                        Console.WriteLine($"Not Exist File : {o.Source}");
                        return;
                    }

                    if (o.Target == null) o.Target = string.Format($"{Path.GetFileName(o.Source)}_ret.csv");

                    var result = CsvParseAndAnalysis(o.Source);

                    await CsvSaveFile(o.Target, result);


                });
        }

        
    }
}
