using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using CommandLine;
using MyFitnessPal.Data.Pages;
using NodaTime;

namespace MyFitnessPal.Data
{
    class Program
    {
        public class Options
        {
            [Option('d', "date", Required = true, HelpText = "The date used as basis for fetching nutrition history from MyFitnessPal.")]
            public DateTime AnchorDate { get; set; }

            [Option('n', "num-days", Required = false, HelpText = "Number of days to fetch history for, starting at \"date\" and going back in time.", Default = 1)]
            public int DaysOfHistory { get; set; }

            [Option('u', "username", Required = true, HelpText = "MyFitnessPal username.")]
            public string Username { get; set; }

            [Option('p', "password", Required = true, HelpText = "MyFitnessPal password.")]
            public string Password { get; set; }
        }

        static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args)
               .WithParsedAsync(async opts =>
                {
                    var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader().WithDefaultCookies());
                    var loginPage = new LoginPage(context);

                    await loginPage.Login(opts.Username, opts.Password);

                    var lastWeek = Enumerable.Range(0, opts.DaysOfHistory)
                       .Select(i => LocalDate.FromDateTime(opts.AnchorDate).Minus(Period.FromDays(i)))
                       .Select(async d =>
                        {
                            var x = await new PrintableDiaryPage(context, d).Fetch();
                            return new
                            {
                                x.Date,
                                x.Totals.Energy.Calories,
                                Protein = x.Totals.Protein.Grams,
                                Carbs = x.Totals.Carbohydrates.Grams,
                                Fat = x.Totals.Fat.Grams
                            };
                        })
                       .Select(x => x.Result)
                       .ToList();

                    foreach (var item in lastWeek)
                    {
                        Console.WriteLine($"{item.Date} - {item.Calories} - {item.Protein} - {item.Carbs} - {item.Fat}");
                    }
                });
        }
    }
}
