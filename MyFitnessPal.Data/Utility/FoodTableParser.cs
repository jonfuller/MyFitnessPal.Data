using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;
using MyFitnessPal.Data.Model;
using MyFitnessPal.Data.Pages;
using NodaTime;
using UnitsNet;

namespace MyFitnessPal.Data.Utility
{
    public static class FoodTableParser
    {
        public static DayOfFood ParseTable(IHtmlTableElement foodTable, LocalDate date)
        {
            return new DayOfFood(
                date,
                GetFoodFor(PrintableDiaryPage.Selectors.BreakfastSectionText),
                GetFoodFor(PrintableDiaryPage.Selectors.LunchSectionText),
                GetFoodFor(PrintableDiaryPage.Selectors.DinnerSectionText),
                GetFoodFor(PrintableDiaryPage.Selectors.SnackSectionText),
                ParseFooter(foodTable.Foot));

            IEnumerable<FoodItem> GetFoodFor(string sectionName) => FindRowsForSection(sectionName, foodTable.Bodies.First().Rows).Select(ParseFoodItem);

            static IEnumerable<IHtmlTableRowElement> FindRowsForSection(string section, IEnumerable<IHtmlTableRowElement> rows)
            {
                return rows
                   .SkipWhile(r => !IsSectionRow(r, section))
                   .Skip(1) // skip the section row
                   .TakeWhile(r => r.Cells.Length > 1) // take until the next section row
                   .ToList();
            }

            static bool IsSectionRow(IHtmlTableRowElement row, string section) =>
                row.ClassName == PrintableDiaryPage.Selectors.MealSectionTableRowClass &&
                row.Cells.Length == 1 &&
                row.Cells.First().TextContent == section;
        }

        private static FoodSummary ParseFooter(IHtmlTableSectionElement footer)
        {
            var item = ParseFoodItem(footer.Rows.First());

            return new FoodSummary(
                item.Energy,
                item.Protein,
                item.Fat,
                item.Carbohydrates,
                item.Cholesterol,
                item.Sodium,
                item.Sugars,
                item.Fiber);
        }

        private static FoodItem ParseFoodItem(IHtmlTableRowElement row)
        {
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