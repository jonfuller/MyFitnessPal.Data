using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using AngleSharp.Common;
using AngleSharp.Html.Dom;
using Microsoft.Extensions.Logging;
using MyFitnessPal.Data.Model;
using MyFitnessPal.Data.Pages;
using NodaTime;
using UnitsNet;

namespace MyFitnessPal.Data.Utility
{
    public static class FoodTableParser
    {
        public static IEnumerable<FoodItem> ParseTable(IHtmlTableElement foodTable, LocalDate date, ILogger logger)
        {
            var allRows = foodTable.Bodies.First().Rows;
            var seed = (currentSection: "none", rows: Enumerable.Empty<FoodItem>());

            logger.LogDebug($"found {allRows.Length} row(s) in food table");
            logger.LogTrace($"parsing food table {foodTable.ToHtml()}");

            return allRows.Aggregate(seed,
                (acc, currentRow) =>
                {
                    var (currentSection, rows) = acc;

                    return IsSectionRow(currentRow)
                        ? (currentRow.TextContent.Trim(), rows)
                        : (currentSection, rows.Concat(ParseMealFoodItem(currentRow, currentSection, date, logger)));
                },
                result =>
                {
                    var foodItems = result.rows.ToList();
                    var numMeals = result.rows.Select(f => f.Meal).Distinct().Count();

                    logger.LogDebug($"found {foodItems.Count} foods, {numMeals} meals for {date}");

                    return foodItems;
                });

            static bool IsSectionRow(IHtmlTableRowElement row) =>
                row.ClassName == PrintableDiaryPage.Selectors.MealSectionTableRowClass &&
                row.Cells.Length == 1;
        }

        private static FoodItem ParseMealFoodItem(IHtmlTableRowElement row, string meal, LocalDate date, ILogger logger)
        {
            logger.LogTrace($"parsing food row {row.ToHtml()}");

            var name = row.Cells[0].TextContent;
            var calories = row.Cells[1].TextContent;
            var carbs = row.Cells[2].TextContent;
            var fat = row.Cells[3].TextContent;
            var protein = row.Cells[4].TextContent;
            var cholesterol = row.Cells[5].TextContent;
            var sodium = row.Cells[6].TextContent;
            var sugars = row.Cells[7].TextContent;
            var fiber = row.Cells[8].TextContent;

            return new FoodItem(
                date,
                meal,
                name,
                Energy.FromCalories(int.Parse(Clean(calories))),
                Mass.Parse(Clean(protein)),
                Mass.Parse(Clean(fat)),
                Mass.Parse(Clean(carbs)),
                Mass.Parse(Clean(cholesterol)),
                Mass.Parse(Clean(sodium)),
                Mass.Parse(Clean(sugars)),
                Mass.Parse(Clean(fiber)));

            static string Clean(string raw) => raw
               .Replace("--", "0")
               .Replace(",", "")
               .Replace("g", " g")
               .Replace("m g", " mg");
        }
    }
}