using System;
using CommandLine;
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

    [Verb("export", HelpText = "Full export of nutrition information.")]
    public class FullExportOptions : DailySummaryOptions
    {

    }

    [Verb("daily", HelpText = "Daily macro summary.")]
    public class DailySummaryOptions : LoginOptions
    {
        [Option('d', "date", Required = true, HelpText = "The date used as basis for fetching nutrition history from MyFitnessPal.")]
        public DateTime AnchorDateRaw { get; set; }

        [Option('n', "num-days", Required = false, HelpText = "Number of days to fetch history for, starting at \"date\" and going back in time.", Default = 1)]
        public int DaysOfHistory { get; set; }

        public DateInterval DateRange => new DateInterval(
            start: LocalDate.FromDateTime(AnchorDateRaw).PlusDays(-DaysOfHistory),
            end: LocalDate.FromDateTime(AnchorDateRaw));
    }
}