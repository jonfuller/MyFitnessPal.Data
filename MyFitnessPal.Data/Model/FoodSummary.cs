using UnitsNet;

namespace MyFitnessPal.Data.Model
{
    public class FoodSummary : FoodItem
    {
        public FoodSummary(Energy energy, Mass protein, Mass fat, Mass carbohydrates, Mass cholesterol, Mass sodium, Mass sugars, Mass fiber) : base("TOTALS", energy, protein, fat, carbohydrates, cholesterol, sodium, sugars, fiber)
        {
        }
    }
}