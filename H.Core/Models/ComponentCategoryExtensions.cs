using H.Core.Enumerations;

namespace H.Core.Models
{
    public static class ComponentCategoryExtensions
    {
        public static AnimalType GetAnimalTypeFromComponentCategory(this ComponentCategory category)
        {
            if (category == ComponentCategory.BeefProduction)
            {
                return AnimalType.Beef;
            }
            else if (category == ComponentCategory.Dairy)
            {
                return AnimalType.Dairy;
            }
            else if (category == ComponentCategory.Swine)
            {
                return AnimalType.Swine;
            }
            else if(category == ComponentCategory.Sheep)
            {
                return AnimalType.Sheep;
            }
            else if (category == ComponentCategory.Poultry)
            {
                return AnimalType.Poultry;
            }
            else
            {
                return AnimalType.OtherLivestock;
            }
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