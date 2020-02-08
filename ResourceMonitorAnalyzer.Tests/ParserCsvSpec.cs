using System;
using System.Text.RegularExpressions;
using Xunit;
using FluentAssertions;
using System.Linq;

namespace ResourceMonitorAnalyzer.Tests
{
    public class ParserCsvSpec
    {
        [Theory]
        [InlineData(@"""\\DM50-LOADER-00\Processor(_Total)\% Processor Time""", @"""Processor(_Total)\% Processor Time""")]
        [InlineData(@"""\\DM50-LOADER-00\Network Interface(Intel[R] I350 Gigabit Network Connection)\Packets Received/sec""", @"""Network Interface(Intel[R] I350 Gigabit Network Connection)\Packets Received/sec""")]
        public void RegexTest(string source, string expect)
        {
            // Act
            var result = Regex.Replace(source, @"^[""][\\][\\]([^\\]*)[\\]", @"""");

            // Assert
            result.Should().Be(expect);
        }
        [Fact]
        public void ModifiedHeaderString()
        {
            // Arrange
            var source =
                @"""(PDH-CSV 4.0) ("",""\\DM50-LOADER-00\Processor(_Total)\% Processor Time"", ""\\DM50-LOADER-00\Network Interface(Intel[R] I350 Gigabit Network Connection)\Packets Received/sec""";
            var expect = new[] {
                @"""Processor(_Total).% Processor Time""",
                @"""Network Interface(Intel(R) I350 Gigabit Network Connection).Packets Received/sec"""
            };

            // Act
            var result = ParserCsv.ModifiedHeader(source).ToList();


            // Assert
            result.Should().BeEquivalentTo(expect);
        }
    }
}
