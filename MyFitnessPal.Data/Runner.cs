using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AngleSharp;
using MyFitnessPal.Data.Model;
using MyFitnessPal.Data.Pages;
using MyFitnessPal.Data.Utility;
using NodaTime;

namespace MyFitnessPal.Data
{
    public class Runner
    {
        private readonly TextWriter _output;

        public Runner(TextWriter output)
        {
            _output = output;
        }

        public ExitCode FullExport(FullExportOptions opts)
        {
            FetchData(opts.Username, opts.Password, opts.DateRange)
               .Then(Flatten)
               .ThenMap(x => new
                {
                    Date = x.date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    x.food.Meal,
                    Food = x.food.Name,
                    x.food.Energy.Calories,
                    Protein = x.food.Protein.Grams,
                    Carbs = x.food.Carbohydrates.Grams,
                    Fat = x.food.Fat.Grams,
                    Cholesterol = x.food.Cholesterol.Milligrams,
                    Sodium = x.food.Sodium.Milligrams,
                    Sugars = x.food.Sugars.Grams,
                    Fiber = x.food.Fiber.Grams,
                })
               .Then(opts.OutputWriter(_output).Write);

            return ExitCode.Success;

            static IEnumerable<(LocalDate date, MealFoodItem food)> Flatten(IEnumerable<DayOfFood> data) =>
                data.SelectMany(day => day.All.Select(food => (day.Date, food)));
        }

        public ExitCode DailySummary(DailySummaryOptions opts)
        {
            FetchData(opts.Username, opts.Password, opts.DateRange)
               .ThenMap(x => new
                {
                    Date = x.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    x.Totals.Energy.Calories,
                    Protein = x.Totals.Protein.Grams,
                    Carbs = x.Totals.Carbohydrates.Grams,
                    Fat = x.Totals.Fat.Grams
                })
               .Then(opts.OutputWriter(_output).Write);

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
    }
}