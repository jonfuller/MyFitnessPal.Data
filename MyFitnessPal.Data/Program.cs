using System;
using System.Collections.Generic;
using CommandLine;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using MyFitnessPal.Data.Utility;

namespace MyFitnessPal.Data
{
    class Program
    {
        public const string InstrumentationKey = "1ded601c-6f94-47de-af35-6801ce8dc0fb";

        static int Main(string[] args)
        {
            var serviceProvider = ConfigureContainer();
            var telemetry = serviceProvider.GetRequiredService<TelemetryClient>();

            var parsed = Parser.Default.ParseArguments<FullExportOptions, DailySummaryOptions>(args);

            var runner = new AnalyticsRunner(telemetry, new Runner(Console.Out, GetLoggerFactory(GetLogLevel(parsed))));

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

        private static IServiceProvider ConfigureContainer()
        {
            return new ServiceCollection()
               .AddSingleton<ITelemetryChannel>(new InMemoryChannel())
               .AddApplicationInsightsTelemetryWorkerService(opts =>
                {
                    opts.InstrumentationKey = InstrumentationKey;
                    opts.ApplicationVersion = typeof(Program).Assembly.GetName().Version.ToString();
                })
               .AddSingleton(typeof(ITelemetryInitializer), TelemetryInitializer.Create(x => {
                    x.Context.User.Id = AnalyticsHelpers.GetUserId();
                    x.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
                }))
               .BuildServiceProvider();
        }
    }
}
