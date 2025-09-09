using H.Core.Enumerations;

namespace H.Core.Models
{
    public static class ComponentCategoryExtensions
    {
        public static AnimalType GetAnimalTypeFromComponentCategory(this ComponentCategory category)
        {
            if (category == ComponentCategory.BeefProduction) return AnimalType.Beef;

            if (category == ComponentCategory.Dairy) return AnimalType.Dairy;

            if (category == ComponentCategory.Swine) return AnimalType.Swine;

            if (category == ComponentCategory.Sheep) return AnimalType.Sheep;

            if (category == ComponentCategory.Poultry) return AnimalType.Poultry;

            return AnimalType.OtherLivestock;
        }

        public static bool isAnimalComponent(this ComponentCategory category)
        {
            switch (category)
            {
                case ComponentCategory.BeefProduction:
                case ComponentCategory.Dairy:
                case ComponentCategory.Swine:
                case ComponentCategory.Sheep:
                case ComponentCategory.Poultry:
                case ComponentCategory.OtherLivestock:
                    return true;

                default:
                    return false;
            }
        }
    }
}