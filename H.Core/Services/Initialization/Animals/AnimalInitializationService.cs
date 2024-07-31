using H.Core.Providers.Animals;
using H.Core.Providers.Climate;
using System.Collections.Generic;
using H.Core.Calculators.Nitrogen;

namespace H.Core.Services.Initialization.Animals
{
    public partial class AnimalInitializationService : IAnimalInitializationService
    {
        #region Fields

        private readonly IIndoorTemperatureProvider _indoorTemperatureProvider;
        private readonly Table_38_OtherLivestock_Default_CH4_Emission_Factors_Provider _otherLivestockDefaultCh4EmissionFactorsProvider;
        private readonly Table_31_Swine_VS_Excretion_For_Diets_Provider _volatileExcretionForDietsProvider;
        private readonly Table_27_Enteric_CH4_Swine_Poultry_OtherLivestock_Provider _entericMethaneProvider;
        private readonly Table_35_Methane_Producing_Capacity_Default_Values_Provider _defaultMethaneProducingCapacityProvider;
        private readonly Table_30_Default_Bedding_Material_Composition_Provider _beddingMaterialCompositionProvider;
        private readonly Table_44_Fraction_OrganicN_Mineralized_As_Tan_Provider _fractionOrganicNMineralizedAsTanProvider;
        private readonly Table_29_Default_Manure_Excreted_Provider _defaultManureExcretionRateProvider;
        private readonly Table_21_Average_Milk_Production_Dairy_Cows_Provider _averageMilkProductionDairyCowsProvider;
        private readonly Table_6_Manure_Types_Default_Composition_Provider _defaultManureCompositionProvider;
        private readonly Table_23_Feeding_Activity_Coefficient_Sheep_Provider _feedingActivityCoefficientSheepProvider;
        private readonly Table_22_Livestock_Coefficients_Sheep_Provider _sheepProvider;
        private readonly Table_36_Livestock_Emission_Conversion_Factors_Provider _livestockEmissionConversionFactorsProvider;
        private readonly Table_17_Beef_Dairy_Cattle_Feeding_Activity_Coefficient_Provider _beefDairyCattleFeedingActivityCoefficientProvider;
        private readonly INitrogenInputCalculator _nitrogenInputCalculator;

        #endregion

        #region Constructors

        public AnimalInitializationService()
        {
            _otherLivestockDefaultCh4EmissionFactorsProvider = new Table_38_OtherLivestock_Default_CH4_Emission_Factors_Provider();
            _volatileExcretionForDietsProvider = new Table_31_Swine_VS_Excretion_For_Diets_Provider();
            _entericMethaneProvider = new Table_27_Enteric_CH4_Swine_Poultry_OtherLivestock_Provider();
            _defaultMethaneProducingCapacityProvider = new Table_35_Methane_Producing_Capacity_Default_Values_Provider();
            _beddingMaterialCompositionProvider = new Table_30_Default_Bedding_Material_Composition_Provider();
            _fractionOrganicNMineralizedAsTanProvider = new Table_44_Fraction_OrganicN_Mineralized_As_Tan_Provider();
            _defaultManureExcretionRateProvider = new Table_29_Default_Manure_Excreted_Provider();
            _indoorTemperatureProvider = new Table_63_Indoor_Temperature_Provider();
            _averageMilkProductionDairyCowsProvider = new Table_21_Average_Milk_Production_Dairy_Cows_Provider();
            _beefDairyCattleFeedingActivityCoefficientProvider = new Table_17_Beef_Dairy_Cattle_Feeding_Activity_Coefficient_Provider();
            _livestockEmissionConversionFactorsProvider = new Table_36_Livestock_Emission_Conversion_Factors_Provider();
            _defaultManureCompositionProvider = new Table_6_Manure_Types_Default_Composition_Provider();
            _feedingActivityCoefficientSheepProvider = new Table_23_Feeding_Activity_Coefficient_Sheep_Provider();
            _nitrogenInputCalculator = new ICBMNitrogenInputCalculator();
            _sheepProvider = new Table_22_Livestock_Coefficients_Sheep_Provider();
        }

        #endregion
    }
}