using System;
using System.IO;
using System.Text.RegularExpressions;
using Xunit;
using FluentAssertions;
using System.Linq;
using NReco.Csv;

namespace ResourceMonitorAnalyzer.Tests
{
    public class ChoEtlSpec
    {
        [Fact]
        void ReadCsv()
        {
            foreach (dynamic e in new ChoCSVReader("Emp.csv").WithFirstLineHeader())
                Console.WriteLine("Id: " + e.Id + " Name: " + e.Name);
        }
    }
}
