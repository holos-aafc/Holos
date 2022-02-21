namespace H.Core.Enumerations
{
    public static class SoilCategoryTypeExtensions
    {
        /// <summary>
        /// Some lookups require a 'reduced' soil type and not specific soil type (i.e. Brown not DarkBrown)
        /// </summary>
        public static SoilFunctionalCategory GetSimplifiedSoilCategory(this SoilFunctionalCategory soilFunctionalCategory)
        {
            switch (soilFunctionalCategory)
            {
                case SoilFunctionalCategory.Brown:
                case SoilFunctionalCategory.DarkBrown:
                case SoilFunctionalCategory.BrownChernozem:
                case SoilFunctionalCategory.DarkBrownChernozem:
                    return SoilFunctionalCategory.Brown;

                case SoilFunctionalCategory.Black:
                case SoilFunctionalCategory.BlackGrayChernozem:
                    return SoilFunctionalCategory.Black;

                // Other types cannot be reduced/simplified (i.e. Organic, Eastern Canada, etc.)
                default:
                    return soilFunctionalCategory;
            }
        }

        /// <summary>
        /// Economic soil regions for AB and SK need to distiguish between brown and darkbrown to return the correct crop type for economic results
        /// </summary>
        /// <param name="soilFunctionalCategory">soil category of the farm</param>
        /// <returns></returns>
        public static SoilFunctionalCategory GetBaseSoilFunctionalCategory(
            this SoilFunctionalCategory soilFunctionalCategory)
        {
            switch (soilFunctionalCategory)
            {
                case SoilFunctionalCategory.Brown:
                case SoilFunctionalCategory.BrownChernozem:
                    return SoilFunctionalCategory.Brown;

                case SoilFunctionalCategory.DarkBrown:
                case SoilFunctionalCategory.DarkBrownChernozem:
                    return SoilFunctionalCategory.DarkBrown;

                case SoilFunctionalCategory.Black:
                case SoilFunctionalCategory.BlackGrayChernozem:
                    return SoilFunctionalCategory.Black;

                // Other types cannot be reduced/simplified (i.e. Organic, Eastern Canada, etc.)
                default:
                    return soilFunctionalCategory;
            }
        }
    }
}
