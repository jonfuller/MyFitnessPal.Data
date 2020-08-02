using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
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

        public PrintableDiaryPage(IBrowsingContext context, LocalDate forDate)
        {
            _context = context;
            _forDate = forDate;
        }

        public async Task<DayOfFood> Fetch()
        {
            var reportsPage = await _context.OpenAsync(UrlForDate(_forDate));
            await reportsPage.WaitForReadyAsync();

            var datePattern = LocalDatePattern.CreateWithInvariantCulture("MMMM d, yyyy");
            var dateRaw = reportsPage.QuerySelector<IHtmlHeadingElement>(Selectors.DateHeader).TextContent;
            var foodTable = reportsPage.QuerySelector<IHtmlTableElement>(Selectors.FoodTable);

            return FoodTableParser.ParseTable(foodTable, datePattern.Parse(dateRaw).GetValueOrThrow());
        }

        private static string UrlForDate(LocalDate date) => $"https://www.myfitnesspal.com/reports/printable_diary?from={date:yyyy-MM-dd}&to={date:yyyy-MM-dd}";

        private static class Selectors
        {
            public const string DateHeader = "#date";
            public const string FoodTable = "#food";
        }
    }
}