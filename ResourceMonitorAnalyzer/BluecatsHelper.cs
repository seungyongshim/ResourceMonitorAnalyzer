using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using LanguageExt;
using Newtonsoft.Json;
using NLog;
using static LanguageExt.Prelude;

namespace ResourceMonitorAnalyzer
{
    public static class BluecatsHelper
    {
        static Logger Logger = LogManager.GetCurrentClassLogger();

        public static void SaveFileCsv(string targetPath, IEnumerable<AnalyzedResult> results)
        {
            try
            {
                using (var csv = File.Open(targetPath, FileMode.Create))
                using (var sw = new StreamWriter(csv))
                {
                    sw.WriteLine(AnalyzedResult.GetCsvHeader());
                    foreach (var item in results.OrderBy(x => x.Type))
                    {
                        sw.WriteLine(item.GetCsvValue());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static void SaveFileJson(string targetPath, IEnumerable<AnalyzedResult> results)
        {
            try
            {
                using (var csv = File.Open(targetPath, FileMode.Create))
                using (var sw = new StreamWriter(csv))
                {
                    var json = JsonConvert.SerializeObject(results);
                    sw.WriteLine(json);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static void SendBluecats(string bluecatsUrl, IEnumerable<AnalyzedResult> result)
        {
            try
            {
                var hc = new HttpClient
                {
                    BaseAddress = new Uri(bluecatsUrl),
                    Timeout = TimeSpan.FromSeconds(5)
                };

                var json = JsonConvert.SerializeObject(result);
                var content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                hc.PutAsync("/logs.resourcemonitor.11", content).Wait();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
