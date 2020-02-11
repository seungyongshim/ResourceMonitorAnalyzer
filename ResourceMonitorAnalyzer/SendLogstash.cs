using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace ResourceMonitorAnalyzer
{
    public class BluecatsHelper
    {
        public static void SaveFileCsv(string targetPath, IEnumerable<AnalyzedResult> results)
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

        public static void SaveFileJson(string targetPath, IEnumerable<AnalyzedResult> results)
        {
            using (var csv = File.Open(targetPath, FileMode.Create))
            using (var sw = new StreamWriter(csv))
            {
                var json = JsonConvert.SerializeObject(results);
                sw.WriteLine(json);
            }
        }

        public static void SendBluecats(string bluecatsUrl, IEnumerable<AnalyzedResult> result)
        {
            HttpClient hc = new HttpClient
            {
                BaseAddress = new Uri(bluecatsUrl),
                Timeout = TimeSpan.FromSeconds(5)
            };

            var json = JsonConvert.SerializeObject(result);
            var content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
            hc.PutAsync("/logs.resourcemonitor.11", content).Wait();
        }
    }
}
