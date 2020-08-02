using System;
using System.Collections.Generic;
using CommandLine;

namespace MyFitnessPal.Data
{
    class Program
    {
        static int Main(string[] args)
        {
            var runner = new Runner(Console.Out);

            var exitCode = Parser.Default.ParseArguments<FullExportOptions, DailySummaryOptions>(args)
               .MapResult(
                    (FullExportOptions opts) => runner.FullExport(opts),
                    (DailySummaryOptions opts) => runner.DailySummary(opts),
                    Err);

            if (exitCode != ExitCode.Success)
            {
                Console.Error.WriteLine($"{exitCode.Value} - {exitCode.Message}");
            }
            return exitCode.Value;
        }

        static ExitCode Err(IEnumerable<Error> parseErrors)
        {
            return ExitCode.GeneralError;
        }
    }
}
