using System;
using System.Collections.Generic;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using LanguageExt;
using Microsoft.Extensions.Logging;
using MyFitnessPal.Data.Model;
using MyFitnessPal.Data.Utility;
using NodaTime;
using NodaTime.Text;
using static LanguageExt.Prelude;

namespace MyFitnessPal.Data.Pages
{
    internal class PrintableDiaryPage
    {
        private readonly IBrowsingContext _context;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<PrintableDiaryPage> _logger;

        public PrintableDiaryPage(IBrowsingContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<PrintableDiaryPage>();
        }

        public Option<IEnumerable<FoodItem>> Fetch(LocalDate forDate)
        {
            var urlForDate = UrlForDate(forDate);
            var reportsPage = _context.OpenAsync(urlForDate).Result;
            reportsPage.WaitForReadyAsync().Wait(TimeSpan.FromSeconds(5));

            _logger.LogDebug($"fetching data from {urlForDate}");
            _logger.LogTrace(reportsPage.ToHtml());
            var datePattern = LocalDatePattern.CreateWithInvariantCulture("MMMM d, yyyy");
            var dateRaw = reportsPage.QuerySelector<IHtmlHeadingElement>(Selectors.DateHeader).TextContent;

            var parsed = datePattern.Parse(dateRaw);
            if (parsed.Success)
            {
                var foodTable = reportsPage.QuerySelector<IHtmlTableElement>(Selectors.FoodTable);

                if (foodTable == null)
                {
                    _logger.LogWarning($"skipping {forDate}. No food table found (table: {Selectors.FoodTable})");
                    return None;
                }

                return Some(FoodTableParser.ParseTable(foodTable, parsed.Value, _loggerFactory.CreateLogger(nameof(FoodTableParser))));
            }

            _logger.LogWarning($"skipping {forDate}. couldn't parse date text: {dateRaw}");
            return None;
        }

        private static string UrlForDate(LocalDate date) => $"https://www.myfitnesspal.com/reports/printable_diary?from={date:yyyy-MM-dd}&to={date:yyyy-MM-dd}";

        public static class Selectors
        {
            public const string DateHeader = "#date";
            public const string FoodTable = "#food";

            public const string MealSectionTableRowClass = "title";
        }
    }
}