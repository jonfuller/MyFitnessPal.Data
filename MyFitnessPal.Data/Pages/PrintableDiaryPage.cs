using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Microsoft.Extensions.Logging;
using MyFitnessPal.Data.Model;
using MyFitnessPal.Data.Utility;
using NodaTime;
using NodaTime.Text;

namespace MyFitnessPal.Data.Pages
{
    internal class PrintableDiaryPage
    {
        private readonly IBrowsingContext _context;
        private readonly LocalDate _forDate;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<PrintableDiaryPage> _logger;

        public PrintableDiaryPage(IBrowsingContext context, LocalDate forDate, ILoggerFactory loggerFactory)
        {
            _context = context;
            _forDate = forDate;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<PrintableDiaryPage>();
        }

        public IEnumerable<FoodItem> Fetch()
        {
            var urlForDate = UrlForDate(_forDate);
            var reportsPage = _context.OpenAsync(urlForDate).Result;
            reportsPage.WaitForReadyAsync().Wait(TimeSpan.FromSeconds(5));

            _logger.LogDebug($"fetching data from {urlForDate}");
            var datePattern = LocalDatePattern.CreateWithInvariantCulture("MMMM d, yyyy");
            var dateRaw = reportsPage.QuerySelector<IHtmlHeadingElement>(Selectors.DateHeader).TextContent;

            var parsed = datePattern.Parse(dateRaw);
            if (parsed.Success)
            {
                var foodTable = reportsPage.QuerySelector<IHtmlTableElement>(Selectors.FoodTable);
                return FoodTableParser.ParseTable(foodTable, parsed.Value, _loggerFactory.CreateLogger(nameof(FoodTableParser)));
            }

            _logger.LogWarning($"skipping {_forDate}. couldn't parse date text: {dateRaw}");
            return Enumerable.Empty<FoodItem>();
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