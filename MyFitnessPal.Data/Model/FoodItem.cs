using LanguageExt;
using UnitsNet;

namespace MyFitnessPal.Data.Model
{
    public class FoodItem : Record<FoodItem>
    {
        public readonly string Name;
        public readonly Energy Energy;
        public readonly Mass Protein;
        public readonly Mass Fat;
        public readonly Mass Carbohydrates;
        public readonly Mass Cholesterol;
        public readonly Mass Sodium;
        public readonly Mass Sugars;
        public readonly Mass Fiber;

        public FoodItem(string name, Energy energy, Mass protein, Mass fat, Mass carbohydrates, Mass cholesterol, Mass sodium, Mass sugars, Mass fiber)
        {
            Name = name;
            Energy = energy;
            Protein = protein;
            Fat = fat;
            Carbohydrates = carbohydrates;
            Cholesterol = cholesterol;
            Sodium = sodium;
            Sugars = sugars;
            Fiber = fiber;
        }
    }
}