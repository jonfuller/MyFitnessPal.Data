using System;
using System.IO;
using CommandLine;
using Microsoft.Extensions.Logging;
using MyFitnessPal.Data.Utility.Output;
using NodaTime;

namespace MyFitnessPal.Data
{
    public class Options
    {
        [Option('v', "verbosity", Required = false, Default = LogLevel.None, HelpText = "Log level. (https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel)")]
        public LogLevel LogVerbosity { get; set; }
    }

    public class LoginOptions : Options
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
        [Option('d', "date", Required = false, HelpText = "(Default: yesterday) The date used as basis for fetching nutrition history from MyFitnessPal.")]
        public DateTime? AnchorDateRaw { get; set; }

        [Option('n', "num-days", Required = false, HelpText = "Number of days to fetch history for, starting at \"date\" and going back in time.", Default = 1)]
        public int DaysOfHistory { get; set; }

        private LocalDate DefaultedAnchorDate => LocalDate.FromDateTime(AnchorDateRaw ?? DateTime.Today.Subtract(TimeSpan.FromDays(1)));

        public DateInterval DateRange => new DateInterval(
            start: DefaultedAnchorDate.PlusDays(-DaysOfHistory + 1),
            end: DefaultedAnchorDate);
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