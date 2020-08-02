using System.Collections.Generic;
using LanguageExt;
using NodaTime;

namespace MyFitnessPal.Data.Model
{
    public class DayOfFood : Record<DayOfFood>
    {
        public readonly LocalDate Date;
        public readonly IEnumerable<FoodItem> Breakfast;
        public readonly IEnumerable<FoodItem> Lunch;
        public readonly IEnumerable<FoodItem> Dinner;
        public readonly IEnumerable<FoodItem> Snacks;
        public readonly FoodSummary Totals;

        public DayOfFood(LocalDate date, IEnumerable<FoodItem> breakfast, IEnumerable<FoodItem> lunch, IEnumerable<FoodItem> dinner, IEnumerable<FoodItem> snacks, FoodSummary totals)
        {
            Date = date;
            Breakfast = breakfast;
            Lunch = lunch;
            Dinner = dinner;
            Snacks = snacks;
            Totals = totals;
        }
    }
}