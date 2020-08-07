using System;
using System.Collections.Generic;
using CommandLine;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace MyFitnessPal.Data
{
    class Program
    {
        static int Main(string[] args)
        {
            var parsed = Parser.Default.ParseArguments<FullExportOptions, DailySummaryOptions>(args);

            var runner = new Runner(Console.Out, GetLoggerFactory(GetLogLevel(parsed)));

            var exitCode = parsed
               .MapResult(
                    (FullExportOptions opts) => runner.FullExport(opts),
                    (DailySummaryOptions opts) => runner.DailySummary(opts),
                    Err);

            if (exitCode != ExitCode.Success)
            {
                Console.Error.WriteLine($"{exitCode.Value} - {exitCode.Message}");
            }
            return exitCode.Value;

            static LogLevel GetLogLevel(ParserResult<object> parseResult) => parseResult.MapResult(
                (Options x) => x.LogVerbosity,
                _ => LogLevel.Information);

            static ILoggerFactory GetLoggerFactory(LogLevel level) => LoggerFactory.Create(builder =>
                {
                    builder.SetMinimumLevel(level);
                    builder
                       .AddFilter("Microsoft", LogLevel.Warning)
                       .AddFilter("System", LogLevel.Warning)
                       .AddConsole(opts =>
                        {
                            opts.IncludeScopes = true;
                            opts.Format = ConsoleLoggerFormat.Default;
                            opts.LogToStandardErrorThreshold = LogLevel.Trace; // put all logging to STDERR
                        });
                });
        }

        static ExitCode Err(IEnumerable<Error> parseErrors)
        {
            return ExitCode.GeneralError;
        }
    }
}
