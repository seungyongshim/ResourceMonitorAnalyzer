using System;
using Xunit;
using FluentAssertions;

namespace ResourceMonitorAnalyzer.Tests
{
    public class ParserCsvSpec
    {
        [Fact]
        public void ModifiedHeaderString()
        {
            // Arrange
            var parserCsv = new ParserCsv();
            var source =
                @"""(PDH-CSV 4.0) ("",""\\DM50-LOADER-00\Processor(_Total)\% Processor Time"", ""\\DM50-LOADER-00\Network Interface(Intel[R] I350 Gigabit Network Connection)\Packets Received/sec""";
            var expect = new[] {
                "timestamp",
                "Machine",
                @"Processor(_Total).% Processor Time",
                @"Network Interface(Intel(R) I350 Gigabit Network Connection)\Packets Received/sec"
            };

            // Act
            string [] result = parserCsv.ModifiedHeader(source);


            // Asserts
            result.Should().BeEquivalentTo(expect);
        }
    }
}
