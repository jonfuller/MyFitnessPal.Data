using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<MealFoodItem> All => FoodFor(Breakfast, Meal.Breakfast)
           .ConcatFast(FoodFor(Lunch, Meal.Lunch))
           .ConcatFast(FoodFor(Dinner, Meal.Dinner))
           .ConcatFast(FoodFor(Snacks, Meal.Snack));

        private IEnumerable<MealFoodItem> FoodFor(IEnumerable<FoodItem> foods, Meal meal)
        {
            return foods.Select(f => new MealFoodItem(meal, f.Name, f.Energy, f.Protein, f.Fat, f.Carbohydrates, f.Cholesterol, f.Sodium, f.Sugars, f.Fiber));
        }

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