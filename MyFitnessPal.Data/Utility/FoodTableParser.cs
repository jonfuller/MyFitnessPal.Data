using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;
using MyFitnessPal.Data.Model;
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
                GetFoodFor("Breakfast"),
                GetFoodFor("Lunch"),
                GetFoodFor("Dinner"),
                GetFoodFor("Snacks"),
                ParseFooter(foodTable.Foot));

            IEnumerable<FoodItem> GetFoodFor(string sectionName) => FindRowsForSection(sectionName, foodTable.Rows).Select(ParseFoodItem);

            static IEnumerable<IHtmlTableRowElement> FindRowsForSection(string section, IEnumerable<IHtmlTableRowElement> rows)
            {
                var rowsLeft = rows.SkipWhile(r => r.ClassName != "title" && r.Cells.First().TextContent == section);
                var sectionRows = rowsLeft
                   .Skip(1)
                   .TakeWhile(r => r.Cells.Length > 1);

                return sectionRows;
            }
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