using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AngleSharp;
using MyFitnessPal.Data.Model;
using MyFitnessPal.Data.Pages;
using MyFitnessPal.Data.Utility;
using NodaTime;
using UnitsNet;

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
               .ThenMap(x => new
                {
                    Date = x.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    x.Meal,
                    Food = x.Name,
                    x.Energy.Calories,
                    Protein = x.Protein.Grams,
                    Carbs = x.Carbohydrates.Grams,
                    Fat = x.Fat.Grams,
                    Cholesterol = x.Cholesterol.Milligrams,
                    Sodium = x.Sodium.Milligrams,
                    Sugars = x.Sugars.Grams,
                    Fiber = x.Fiber.Grams,
                })
               .Then(opts.OutputWriter(_output).Write);

            return ExitCode.Success;
        }

        public ExitCode DailySummary(DailySummaryOptions opts)
        {
            FetchData(opts.Username, opts.Password, opts.DateRange)
               .Then(x => x.GroupBy(y => y.Date))
               .ThenMap(grp => new
                {
                    Date = grp.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Calories = SumEnergy(grp.Select(x => x.Energy)).Calories,
                    Protein = SumMass(grp.Select(x => x.Protein)).Grams,
                    Carbs= SumMass(grp.Select(x => x.Carbohydrates)).Grams,
                    Fat = SumMass(grp.Select(x => x.Fat)).Grams,
               })
               .Then(opts.OutputWriter(_output).Write);

            return ExitCode.Success;

            static Energy SumEnergy(IEnumerable<Energy> source) => source.Aggregate(Energy.Zero, (a, b) => a + b);
            static Mass SumMass(IEnumerable<Mass> source) => source.Aggregate(Mass.Zero, (a, b) => a + b);
        }

        private static IEnumerable<FoodItem> FetchData(string username, string password, DateInterval dates)
        {
            var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader().WithDefaultCookies());
            var loginPage = new LoginPage(context);

            loginPage.Login(username, password);

            return dates.SelectMany(d => new PrintableDiaryPage(context, d).Fetch());
        }
    }
}