using LanguageExt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static ResourceMonitorAnalyzer.ParserCsv;
using static ResourceMonitorAnalyzer.BluecatsHelper;
using static LanguageExt.Prelude;
using static System.Convert;
    
using CommandLine;
using System.Threading.Tasks;
using NLog;

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

            [Option('u', "url_bluecats", Required = false, HelpText = "BLUE CATS url (ex) http://192.168.xxx.xxx:7780")]
            public string BluecatsUrl { get; set; }

            [Option('f', "target_format", Required = false, HelpText = "default:csv, json")]
            public string TargetFormat { get; set; }

        }

        static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();

            logger.Info("Start App...");

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    if (!File.Exists(o.Source))
                    {
                        Console.WriteLine($"Not Exist File : {o.Source}");
                        return;
                    }

                    if (o.Target == null) o.Target = string.Format($"{Path.GetFileName(o.Source)}_ret");

                    var result = CsvParseAndAnalysis(o.Source).ToSeq();

                    if (o.TargetFormat == "json")
                    {
                        SaveFileJson($"{o.Target}.json", result);
                    }
                    else
                    {
                        SaveFileCsv($"{o.Target}.csv", result);
                    }

                    if (o.BluecatsUrl !=null)
                    {
                        SendBluecats(o.BluecatsUrl, result);
                    }
                });

            logger.Info("End App...");
        }

        
    }
}
