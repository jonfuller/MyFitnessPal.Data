using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using MyFitnessPal.Data.Utility;
using static MyFitnessPal.Data.Utility.AnalyticsHelpers;

namespace MyFitnessPal.Data
{
    public class AnalyticsRunner
    {
        private readonly TelemetryClient _telemetry;
        private readonly Runner _other;

        public AnalyticsRunner(TelemetryClient telemetry, Runner other)
        {
            _telemetry = telemetry;
            _other = other;
        }

        public ExitCode FullExport(FullExportOptions opts) => TrackEvent(opts, () => _other.FullExport(opts));
        public ExitCode DailySummary(DailySummaryOptions opts) => TrackEvent(opts, () => _other.DailySummary(opts));

        private ExitCode TrackEvent<TOpts>(TOpts opts, Func<ExitCode> theCall) where TOpts : Options
        {
            var operationName = GetVerb<TOpts>();

            using (_telemetry.StartOperation<RequestTelemetry>(operationName))
            {
                return theCall().Then(exit => {
                    _telemetry.TrackEvent(operationName, GetEventData(opts));
                    return exit;
                });
            }
        }
    }
}