using System;
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

                var value = sr.ReadLine();
                var result = value.Split(',')
                    .Skip(1)
                    .Take(1)
                    .Select(x => x.Trim('\"'))
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => Convert.ToDouble(x))
                    .FirstOrDefault();

                Console.WriteLine(value);
            }
        }
    }
}
