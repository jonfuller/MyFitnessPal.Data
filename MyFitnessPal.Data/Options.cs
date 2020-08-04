using System;
using System.IO;
using CommandLine;
using MyFitnessPal.Data.Utility.Output;
using NodaTime;

namespace MyFitnessPal.Data
{
    public class LoginOptions
    {
        [Option('u', "username", Required = true, HelpText = "MyFitnessPal username.")]
        public string Username { get; set; }

        [Option('p', "password", Required = true, HelpText = "MyFitnessPal password.")]
        public string Password { get; set; }
    }

    public class OutputOptions : LoginOptions
    {
        [Option('f', "format", Required = false, Default = OutputType.csv, HelpText = "Format of output.")]
        public OutputType OutputFormat { get; set; }

        public enum OutputType
        {
            csv,
            table
        }

        public IOutputWriter OutputWriter(TextWriter output) => OutputFormat == OutputType.csv
            ? (IOutputWriter)new CsvOutputWriter(output)
            : new TableOutputWriter(output);
    }

    public class DateRangeOptions : OutputOptions
    {
        [Option('d', "date", Required = true, HelpText = "The date used as basis for fetching nutrition history from MyFitnessPal.")]
        public DateTime AnchorDateRaw { get; set; }

        [Option('n', "num-days", Required = false, HelpText = "Number of days to fetch history for, starting at \"date\" and going back in time.", Default = 1)]
        public int DaysOfHistory { get; set; }

        public DateInterval DateRange => new DateInterval(
            start: LocalDate.FromDateTime(AnchorDateRaw).PlusDays(-DaysOfHistory),
            end: LocalDate.FromDateTime(AnchorDateRaw));
    }

    [Verb("export", HelpText = "Full export of nutrition information.")]
    public class FullExportOptions : DateRangeOptions
    {

    }

    [Verb("daily", HelpText = "Daily macro summary.")]
    public class DailySummaryOptions : DateRangeOptions
    {
    }
}