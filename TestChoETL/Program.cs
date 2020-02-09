using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestChoETL
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var csv = File.Open("data.csv", FileMode.Open))
            using (var sr = new StreamReader(csv))
            {
                sr.ReadLine(); // header
                sr.ReadLine();

                List<double> RawData = new List<double>();

                while (sr.EndOfStream != true)
                {
                    var value = sr.ReadLine();
                    var result = value.Split(',')
                        .Skip(1)
                        .Take(1)
                        .Select(x => x.Trim('\"'))
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => Convert.ToDouble(x))
                        .FirstOrDefault();

                    RawData.Add(result);
                }

                var totalCount = RawData.Count;
                var average = RawData.Average();
                var ordered = RawData.OrderBy(x => x).ToList();
                var low05 = ordered.Skip(Convert.ToInt32(totalCount * 0.05)).First();
                var low14 = ordered.Skip(Convert.ToInt32(totalCount * 0.14)).First();
                var high05 = ordered.Skip(Convert.ToInt32(totalCount * (1 - 0.05))).First();
                var high14 = ordered.Skip(Convert.ToInt32(totalCount * (1 - 0.14))).First();

                // 상위 5%, 하위 5%를 제외한 평균
                var average90 = RawData.Where(x => x > low05 && x < high05).Average();
                // 90%중 상위 10%의 평균
                var average90H10 = RawData.Where(x => x > high14 && x < high05).Average();
                // 90%중 하위 10%의 평균
                var average90L10 = RawData.Where(x => x > low05 && x < low14).Average();


            }
        }
    }
}
