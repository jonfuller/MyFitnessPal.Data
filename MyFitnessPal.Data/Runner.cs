﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AngleSharp;
using LanguageExt;
using Microsoft.Extensions.Logging;
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
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<Runner> _logger;

        public Runner(TextWriter output, ILoggerFactory loggerFactory)
        {
            _output = output;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<Runner>();
        }

        public ExitCode FullExport(FullExportOptions opts)
        {
            return FetchData(opts.Username, opts.Password, opts.DateRange)
               .Map(foods => foods
                   .Select(x => new
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
                   .Then(opts.OutputWriter(_output).Write))
               .Match(
                    Left: _ => _,
                    Right: _ => ExitCode.Success);
        }

        public ExitCode DailySummary(DailySummaryOptions opts)
        {
            return FetchData(opts.Username, opts.Password, opts.DateRange)
               .Map(foods => foods
                   .GroupBy(y => y.Date)
                   .Select(grp => new
                    {
                        Date = grp.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Calories = Math.Round(SumEnergy(grp.Select(x => x.Energy)).Calories),
                        Protein = Math.Round(SumMass(grp.Select(x => x.Protein)).Grams),
                        Carbs= Math.Round(SumMass(grp.Select(x => x.Carbohydrates)).Grams),
                        Fat = Math.Round(SumMass(grp.Select(x => x.Fat)).Grams),
                   })
                   .Then(opts.OutputWriter(_output).Write))
               .Match(
                    Left: _ => _,
                    Right: _ => ExitCode.Success);

            static Energy SumEnergy(IEnumerable<Energy> source) => source.Aggregate(Energy.Zero, (a, b) => a + b);
            static Mass SumMass(IEnumerable<Mass> source) => source.Aggregate(Mass.Zero, (a, b) => a + b);
        }

        private Either<ExitCode, IEnumerable<FoodItem>> FetchData(string username, string password, DateInterval dates)
        {
            var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader().WithDefaultCookies());
            var loginPage = new LoginPage(context, _loggerFactory);

            _logger.LogInformation($"ready to get data for {username} for dates: {dates.Start} - {dates.End}.");
            _logger.LogInformation($"attempting login for user {username}.");
            return loginPage
               .Login(username, password)
               .ToEither(ExitCode.BadLogin)
               .Map(_ => GetData());

            IEnumerable<FoodItem> GetData()
            {
                return dates
                   .Select(d =>
                    {
                        _logger.LogInformation($"fetching printable diary for {d}.");
                        return new PrintableDiaryPage(context, _loggerFactory).Fetch(d);
                    })
                   .Somes()
                   .SelectMany(x => x);
            }
        }
    }
}