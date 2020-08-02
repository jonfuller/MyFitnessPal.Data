using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AngleSharp;
using CsvHelper;
using CsvHelper.Configuration;
using MyFitnessPal.Data.Model;
using MyFitnessPal.Data.Pages;
using NodaTime;

namespace MyFitnessPal.Data
{
    public class Runner
    {
        private readonly TextWriter _stdOut;

        public Runner(TextWriter stdOut)
        {
            _stdOut = stdOut;
        }

        public ExitCode FullExport(FullExportOptions opts)
        {
            WriteCsv(FetchData(opts.Username, opts.Password, opts.DateRange).SelectMany(day => day.All.Select(food => new
                {
                    Date = day.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    food.Meal,
                    Food = food.Name,
                    food.Energy.Calories,
                    Protein = food.Protein.Grams,
                    Carbs = food.Carbohydrates.Grams,
                    Fat = food.Fat.Grams,
                    Cholesterol = food.Cholesterol.Milligrams,
                    Sodium = food.Sodium.Milligrams,
                    Sugars = food.Sugars.Grams,
                    Fiber = food.Fiber.Grams,
                }))
               .ToList());

            return ExitCode.Success;
        }

        public ExitCode DailySummary(DailySummaryOptions opts)
        {
            WriteCsv(FetchData(opts.Username, opts.Password, opts.DateRange).Select(x => new
                {
                    Date = x.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    x.Totals.Energy.Calories,
                    Protein = x.Totals.Protein.Grams,
                    Carbs = x.Totals.Carbohydrates.Grams,
                    Fat = x.Totals.Fat.Grams
                })
               .ToList());

            return ExitCode.Success;
        }

        private static IEnumerable<DayOfFood> FetchData(string username, string password, DateInterval dates)
        {
            var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader().WithDefaultCookies());
            var loginPage = new LoginPage(context);

            loginPage.Login(username, password);

            return dates
               .Select(d => new PrintableDiaryPage(context, d).Fetch())
               .Somes();
        }

        private void WriteCsv<T>(ICollection<T> records)
        {
            using (var csv = new CsvWriter(_stdOut, new CsvConfiguration(CultureInfo.InvariantCulture), true))
            {
                csv.WriteRecords(records);
            }
        }
    }
}