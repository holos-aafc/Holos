#region Imports

using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Infrastructure;

#endregion


namespace H.Core.Models
{
    /// <summary>
    /// </summary>
    public class Defaults : ModelBase
    {
        #region Constructors

        public Defaults()
        {
            TimeFrame = TimeFrame.TwoThousandToCurrent;

            DefaultTillageTypeForFallow = TillageType.NoTill;

            CarbonConcentration = CoreConstants.CarbonConcentration;
            NumberOfYearsInCarRegionAverage = 3;

            // Default location is Lethbridge, AB
            Latitude = 49.69999;
            Longitude = -112.81856;

            EmergenceDay = 141;
            RipeningDay = 197;
            Variance = 300;

            EmergenceDayForPerennials = 75;
            RipeningDayForPerennnials = 300;
            VarianceForPerennials = 1500;

            Alfa = 0.7;
            DecompositionMinimumTemperature = -3.78;
            DecompositionMaximumTemperature = 30;
            MoistureResponseFunctionAtSaturation = 0.42;
            MoistureResponseFunctionAtWiltingPoint = 0.18;

            // Annual crops
            PercentageOfProductReturnedToSoilForAnnuals = 2;
            PercentageOfStrawReturnedToSoilForAnnuals = 100;
            PercentageOfRootsReturnedToSoilForAnnuals = 100;

            // Silage crops
            PercentageOfProductYieldReturnedToSoilForSilageCrops = 2;
            PercentageOfRootsReturnedToSoilForSilageCrops = 100;

            // Cover crops
            PercentageOfProductYieldReturnedToSoilForCoverCrops = 100;
            PercentageOfProductYieldReturnedToSoilForCoverCropsForage = 35;
            PercentageOfProductYieldReturnedToSoilForCoverCropsProduce = 0;
            PercentageOfStrawReturnedToSoilForCoverCrops = 100;
            PercetageOfRootsReturnedToSoilForCoverCrops = 100;

            // Root crops
            PercentageOfProductReturnedToSoilForRootCrops = 0;
            PercentageOfStrawReturnedToSoilForRootCrops = 100;

            // Perennial crops
            PercentageOfProductReturnedToSoilForPerennials = 35;
            PercentageOfStrawReturnedToSoilForPerennials = 0;
            PercentageOfRootsReturnedToSoilForPerennials = 100;
            EstablishmentGrowthFactorPercentageForPerennials = 50;
            DefaultSupplementalFeedingLossPercentage = 20;

            // Rangeland
            PercentageOfProductReturnedToSoilForRangelandDueToHarvestLoss = 35;
            PercentageOfProductReturnedToSoilForRangelandDueToGrazingLoss = CoreConstants.ValueNotDetermined;
            PercentageOfRootsReturnedToSoilForRangeland = 100;

            // Fodder corn
            PercentageOfProductReturnedToSoilForFodderCorn = 35;
            PercentageOfRootsReturnedToSoilForFodderCorn = 100;

            HumificationCoefficientAboveGround = 0.125;
            HumificationCoefficientBelowGround = 0.3;
            HumificationCoefficientManure = 0.31;

            DecompositionRateConstantYoungPool = 0.8;
            DecompositionRateConstantOldPool = 0.00605;

            OldPoolCarbonN = 0.1; // 1/10
            SlowCarbonN = 0.05; // 1/20
            ActiveCarbonN = 0.025; // 1/40
            NORatio = 0.1;
            EmissionFactorForLeachingAndRunoff = 0.011; // Updated to IPCC 2019 value
            EmissionFactorForVolatilization = 0.01;
            CustomN2OEmissionFactor = 0.003;

            FractionOfNLostByVolatilization = 0.21;
            OtherAnimalsVolitilizationFraction = 0.2;
            PoultryVolitilizationFraction = 0.4;

            MicrobeDeath = 0.2;
            Denitrification = 0.5;
            FertilizerEfficiency = 0.5;
            FTopo = 14.03;
            DefaultNitrogenFixation = 0.7;

            UseClimateParameterInsteadOfManagementFactor = true;

            // Energy
            ConversionOfElectricityToCo2 = 0.2;
            ConversionOfGjOfDieselToCo2 = 70;
            ConversionOfGjForHerbicideProduction = 5.8;
            NitrogenFertilizerConversionFactor = 3.59;
            PhosphorusFertilizerConversionFactor = 0.5699;
            ConversionOfAreaIrrigated = 367;
            ElectricityHousedBeef = 65.7;
            ElectricityDairy = 968;
            ElectricitySwine = 1.06;
            ElectricityPoultry = 2.88;
            LiquidManureSpreading = 0.0248;
            SolidManureSpreading = 0.0248;

            LiquidManureSwineConcentrationEnergy = 3.5;
            LiquidManureDairyConcentrationEnergy = 3.4;
            LiquidManurePoultryConcentrationEnergy = 6;

            SolidManureSwineConcentrationEnergy = 8;
            SolidManureDairyConcentrationEnergy = 5;
            SolidManurePoultryConcentrationEnergy = 24.1;
            SolidManureBeefConcentrationEnergy = 10;
            SolidManureSheepConcentrationEnergy = 10;

            // Previous run-in period of 5 years was too small and resulted in very high starting/equilibrium states. Using a higher value is needed.
            DefaultRunInPeriod = 15;
            RunInPeriodTillageType = TillageType.Reduced;
            CarbonModellingStrategy = CarbonModellingStrategies.IPCCTier2;

            // AD
            DefaultBiodegradableFractionDairyManure = 0.025;
            DefaultBiodegradableFractionGreenWaste = 0.024;
            DefaultBiodegradableFractionSwineManure = 0.024;
            DefaultBiodegradableFractionOtherManure = 0.550;

            ResidueInputCalculationMethod = ResidueInputCalculationMethod.Default;
            SoilDataAcquisitionMethod = SoilDataAcquisitionMethod.Default;
        }

        #endregion

        #region Fields

        private double _carbonConcentration;
        private TimeFrame _timeFrame;
        private CarbonModellingStrategies _carbonModellingStrategy;
        private ResidueInputCalculationMethod _residueInputCalculationMethod;
        private SoilDataAcquisitionMethod _soilDataAcquisitionMethod;

        private double _latitude;
        private double _longitude;

        private TillageType _defaultTillageTypeForFallow;

        private int _emergenceDay;
        private int _emergenceDayForPerennials;
        private int _numberOfYearsInCarRegionAverage;

        private int _ripeningDay;
        private int _ripeningDayForPerennnials;

        private double _variance;
        private double _varianceForPerennials;

        private bool _useClimateParameterInsteadOfManagementFactor;

        private double _alfa;
        private double _decompositionMinimumTemperature;
        private double _decompositionMaximumTemperature;
        private double _moistureResponseFunctionAtSaturation;
        private double _moistureResponseFunctionAtWiltingPoint;

        // Annual crops
        private double _percentageOfProductReturnedToSoilForAnnuals;
        private double _percentageOfStrawReturnedToSoilForAnnuals;
        private double _percentageOfRootsReturnedToSoilForAnnuals;

        // Silage crops
        private double _percentageOfProductYieldReturnedToSoilForSilageCrops;
        private double _percentageOfRootsReturnedToSoilForSilageCrops;

        // Cover crops
        private double _percentageOfProductYieldReturnedToSoilForCoverCrops;
        private double _percentageOfProductYieldReturnedToSoilForCoverCropsForage;
        private double _percentageOfProductYieldReturnedToSoilForCoverCropsProduce;
        private double _percentageOfStrawReturnedToSoilForCoverCrops;
        private double _percetageOfRootsReturnedToSoilForCoverCrops;

        // Perennial crops
        private double _percentageOfProductReturnedToSoilForPerennials;
        private double _percentageOfStrawReturnedToSoilForPerennials;
        private double _percentageOfRootsReturnedToSoilForPerennials;
        private double _establishmentGrowthFactorPercentageForPerennials;
        private double _defaultSupplementalFeedingLossPercentage;

        // Rangeland
        private double _percentageOfProductReturnedToSoilForRangelandDueToHarvestLoss;
        private double _percentageOfProductReturnedToSoilForRangelandDueToGrazingLoss;
        private double _percentageOfRootsReturnedToSoilForRangeland;

        // Fodder corn
        private double _percentageOfProductReturnedToSoilForFodderCorn;
        private double _percentageOfRootsReturnedToSoilForFodderCorn;

        // Root crops
        private double _percentageOfProductReturnedToSoilForRootCrops;
        private double _percentageOfStrawReturnedToSoilForRootCrops;

        private double _humificationCoefficientAboveGround;
        private double _humificationCoefficientBelowGround;
        private double _humificationCoefficientManure;
        private double _decompositionRateConstantYoungPool;
        private double _decompositionRateConstantOldPool;
        private double _oldPoolCarbonN;
        private double _NORatio;
        private double _otherAnimalsVolitilizationFraction;
        private double _poultryVolitilizationFraction;
        private double _microbeDeath;
        private double _denitrification;
        private double _fertilizerEfficiency;
        private double _fTopo;
        private double _defaultNitrogenFixation;

        // N2O
        private double _emissionFactorForLeachingAndRunoff;
        private double _emissionFactorForVolatilization;
        private double _fractionOfNLostByVolatilization;
        private double _customN2OEmissionFactor;

        private bool _useCustomN2OEmissionFactor;

        private EquilibriumCalculationStrategies _equilibriumCalculationStrategy;

        // Energy
        private double _conversionOfElectricityToCO2;
        private double _conversionOfGJOfDieselToCO2;
        private double _conversionOfGJForHerbicideProduction;
        private double _nitrogenFertilizerConversionFactor;
        private double _phosphorusFertilizerConversionFactor;
        private double _potassiumConversionFactor;
        private double _conversionOfAreaIrrigated;
        private double _electricityHousedBeef;
        private double _electricityDairy;
        private double _electricitySwine;
        private double _electricityPoultry;
        private double _liquidManureSpreading;
        private double _solidManureSpreading;

        private double _liquidManureSwineConcentrationEnergy;
        private double _liquidManureDairyConcentrationEnergy;
        private double _liquidManurePoultryConcentrationEnergy;

        private double _solidManureSwineConcentrationEnergy;
        private double _solidManureDairyConcentrationEnergy;
        private double _solidManurePoultryConcentrationEnergy;
        private double _solidManureBeefConcentrationEnergy;
        private double _solidManureSheepConcentrationEnergy;

        private bool _useCustomNitrogenFertilizerConversionFactor;
        private bool _useCustomPhosphorusFertilizerConversionFactor;
        private bool _useCustomPotassiumConversionFactor;

        private bool _useCustomElectricityConversionFactor;
        private bool _useCustomElectricityConversionFactorForBeef;
        private bool _useCustomElectricityConversionFactorForDairy;
        private bool _useCustomElectricityConversionFactorForPoultry;
        private bool _useCustomElectricityConversionFactorForSwine;

        // IPCC Tier 2 Carbon
        private int _defaultRunInPeriod;
        private TillageType _runInPeriodTillageType;

        private double _defaultBiodegradableFractionDairyManure;
        private double _defaultBiodegradableFractionSwineManure;
        private double _defaultBiodegradableFractionOtherManure;
        private double _defaultBiodegradableFractionGreenWaste;

        // Irrigation
        private PumpType _defaulPumpType;
        private double _pumpEmissionsFactor;

        private bool _scaleUpEmissionsEnabled;

        #endregion

        #region Properties

        /// <summary>
        ///     Indicates if the system should display/output run in period items when displaying final results
        /// </summary>
        public bool OutputRunInPeriodItems { get; set; }

        public CarbonModellingStrategies CarbonModellingStrategy
        {
            get => _carbonModellingStrategy;
            set => SetProperty(ref _carbonModellingStrategy, value,
                () => { RaisePropertyChanged(nameof(UseICBMStrategy)); }
            );
        }

        public bool UseICBMStrategy => CarbonModellingStrategy == CarbonModellingStrategies.ICBM;

        /// <summary>
        ///     The period of time in which climate normals are calculated from a set of daily climate data values
        /// </summary>
        public TimeFrame TimeFrame
        {
            get => _timeFrame;
            set => SetProperty(ref _timeFrame, value);
        }

        public bool UseClimateParameterInsteadOfManagementFactor
        {
            get => _useClimateParameterInsteadOfManagementFactor;
            set => SetProperty(ref _useClimateParameterInsteadOfManagementFactor, value);
        }

        /// <summary>
        ///     (kg kg^-1)
        /// </summary>
        public double CarbonConcentration
        {
            get => _carbonConcentration;
            set => SetProperty(ref _carbonConcentration, value);
        }

        public double Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        public double Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        public int EmergenceDay
        {
            get => _emergenceDay;
            set => SetProperty(ref _emergenceDay, value);
        }

        public int RipeningDay
        {
            get => _ripeningDay;
            set => SetProperty(ref _ripeningDay, value);
        }

        public double Variance
        {
            get => _variance;
            set => SetProperty(ref _variance, value);
        }

        public double Alfa
        {
            get => _alfa;
            set => SetProperty(ref _alfa, value);
        }

        public double DecompositionMinimumTemperature
        {
            get => _decompositionMinimumTemperature;
            set => SetProperty(ref _decompositionMinimumTemperature, value);
        }

        public double DecompositionMaximumTemperature
        {
            get => _decompositionMaximumTemperature;
            set => SetProperty(ref _decompositionMaximumTemperature, value);
        }

        public double MoistureResponseFunctionAtSaturation
        {
            get => _moistureResponseFunctionAtSaturation;
            set => SetProperty(ref _moistureResponseFunctionAtSaturation, value);
        }

        public double MoistureResponseFunctionAtWiltingPoint
        {
            get => _moistureResponseFunctionAtWiltingPoint;
            set => SetProperty(ref _moistureResponseFunctionAtWiltingPoint, value);
        }

        public double PercentageOfProductReturnedToSoilForAnnuals
        {
            get => _percentageOfProductReturnedToSoilForAnnuals;
            set => SetProperty(ref _percentageOfProductReturnedToSoilForAnnuals, value);
        }

        public double PercentageOfStrawReturnedToSoilForAnnuals
        {
            get => _percentageOfStrawReturnedToSoilForAnnuals;
            set => SetProperty(ref _percentageOfStrawReturnedToSoilForAnnuals, value);
        }

        public double PercentageOfRootsReturnedToSoilForAnnuals
        {
            get => _percentageOfRootsReturnedToSoilForAnnuals;
            set => SetProperty(ref _percentageOfRootsReturnedToSoilForAnnuals, value);
        }

        public double PercentageOfProductReturnedToSoilForPerennials
        {
            get => _percentageOfProductReturnedToSoilForPerennials;
            set => SetProperty(ref _percentageOfProductReturnedToSoilForPerennials, value);
        }

        public double PercentageOfRootsReturnedToSoilForPerennials
        {
            get => _percentageOfRootsReturnedToSoilForPerennials;
            set => SetProperty(ref _percentageOfRootsReturnedToSoilForPerennials, value);
        }

        public double PercentageOfProductReturnedToSoilForFodderCorn
        {
            get => _percentageOfProductReturnedToSoilForFodderCorn;
            set => SetProperty(ref _percentageOfProductReturnedToSoilForFodderCorn, value);
        }

        public double PercentageOfRootsReturnedToSoilForFodderCorn
        {
            get => _percentageOfRootsReturnedToSoilForFodderCorn;
            set => SetProperty(ref _percentageOfRootsReturnedToSoilForFodderCorn, value);
        }

        public double PercentageOfProductReturnedToSoilForRootCrops
        {
            get => _percentageOfProductReturnedToSoilForRootCrops;
            set => SetProperty(ref _percentageOfProductReturnedToSoilForRootCrops, value);
        }

        public double PercentageOfStrawReturnedToSoilForRootCrops
        {
            get => _percentageOfStrawReturnedToSoilForRootCrops;
            set => SetProperty(ref _percentageOfStrawReturnedToSoilForRootCrops, value);
        }

        public double HumificationCoefficientAboveGround
        {
            get => _humificationCoefficientAboveGround;
            set => SetProperty(ref _humificationCoefficientAboveGround, value);
        }

        public double HumificationCoefficientBelowGround
        {
            get => _humificationCoefficientBelowGround;
            set => SetProperty(ref _humificationCoefficientBelowGround, value);
        }

        public double DecompositionRateConstantYoungPool
        {
            get => _decompositionRateConstantYoungPool;
            set => SetProperty(ref _decompositionRateConstantYoungPool, value);
        }

        public double DecompositionRateConstantOldPool
        {
            get => _decompositionRateConstantOldPool;
            set => SetProperty(ref _decompositionRateConstantOldPool, value);
        }

        public double HumificationCoefficientManure
        {
            get => _humificationCoefficientManure;
            set => SetProperty(ref _humificationCoefficientManure, value);
        }

        public double PercentageOfProductYieldReturnedToSoilForSilageCrops
        {
            get => _percentageOfProductYieldReturnedToSoilForSilageCrops;
            set => SetProperty(ref _percentageOfProductYieldReturnedToSoilForSilageCrops, value);
        }

        public double PercentageOfRootsReturnedToSoilForSilageCrops
        {
            get => _percentageOfRootsReturnedToSoilForSilageCrops;
            set => SetProperty(ref _percentageOfRootsReturnedToSoilForSilageCrops, value);
        }

        public double PercentageOfProductYieldReturnedToSoilForCoverCrops
        {
            get => _percentageOfProductYieldReturnedToSoilForCoverCrops;
            set => SetProperty(ref _percentageOfProductYieldReturnedToSoilForCoverCrops, value);
        }

        public double PercentageOfProductYieldReturnedToSoilForCoverCropsForage
        {
            get => _percentageOfProductYieldReturnedToSoilForCoverCropsForage;
            set => SetProperty(ref _percentageOfProductYieldReturnedToSoilForCoverCropsForage, value);
        }

        public double PercentageOfProductYieldReturnedToSoilForCoverCropsProduce
        {
            get => _percentageOfProductYieldReturnedToSoilForCoverCropsProduce;
            set => SetProperty(ref _percentageOfProductYieldReturnedToSoilForCoverCropsProduce, value);
        }

        public double PercentageOfStrawReturnedToSoilForCoverCrops
        {
            get => _percentageOfStrawReturnedToSoilForCoverCrops;
            set => SetProperty(ref _percentageOfStrawReturnedToSoilForCoverCrops, value);
        }

        public double PercetageOfRootsReturnedToSoilForCoverCrops
        {
            get => _percetageOfRootsReturnedToSoilForCoverCrops;
            set => SetProperty(ref _percetageOfRootsReturnedToSoilForCoverCrops, value);
        }

        public double PercentageOfProductReturnedToSoilForRangelandDueToHarvestLoss
        {
            get => _percentageOfProductReturnedToSoilForRangelandDueToHarvestLoss;
            set => SetProperty(ref _percentageOfProductReturnedToSoilForRangelandDueToHarvestLoss, value);
        }

        public double PercentageOfProductReturnedToSoilForRangelandDueToGrazingLoss
        {
            get => _percentageOfProductReturnedToSoilForRangelandDueToGrazingLoss;
            set => SetProperty(ref _percentageOfProductReturnedToSoilForRangelandDueToGrazingLoss, value);
        }

        public double PercentageOfRootsReturnedToSoilForRangeland
        {
            get => _percentageOfRootsReturnedToSoilForRangeland;
            set => SetProperty(ref _percentageOfRootsReturnedToSoilForRangeland, value);
        }

        /// <summary>
        ///     Section 2.3.3
        /// </summary>
        public double OldPoolCarbonN
        {
            get => _oldPoolCarbonN;
            set => SetProperty(ref _oldPoolCarbonN, value);
        }

        /// <summary>
        ///     Section 2.3.4.1
        /// </summary>
        public double NORatio
        {
            get => _NORatio;
            set => SetProperty(ref _NORatio, value);
        }

        /// <summary>
        ///     EF_leach
        ///     (kg N2O-N (kg N)^-1)
        /// </summary>
        public double EmissionFactorForLeachingAndRunoff
        {
            get => _emissionFactorForLeachingAndRunoff;
            set => SetProperty(ref _emissionFactorForLeachingAndRunoff, value);
        }

        /// <summary>
        ///     EF_volatilization (IPCC 2006)
        ///     Section 2.3.5.3
        /// </summary>
        public double EmissionFactorForVolatilization
        {
            get => _emissionFactorForVolatilization;
            set => SetProperty(ref _emissionFactorForVolatilization, value);
        }

        /// <summary>
        ///     Frac_volatilizationsoil
        ///     (IPCC 2019)
        /// </summary>
        public double FractionOfNLostByVolatilization
        {
            get => _fractionOfNLostByVolatilization;
            set => SetProperty(ref _fractionOfNLostByVolatilization, value);
        }

        /// <summary>
        ///     Section 2.3.6.3
        /// </summary>
        public double MicrobeDeath
        {
            get => _microbeDeath;
            set => SetProperty(ref _microbeDeath, value);
        }

        /// <summary>
        ///     Section 2.3.6.3
        /// </summary>
        public double Denitrification
        {
            get => _denitrification;
            set => SetProperty(ref _denitrification, value);
        }

        public double FertilizerEfficiency
        {
            get => _fertilizerEfficiency;
            set => SetProperty(ref _fertilizerEfficiency, value);
        }

        public double FTopo
        {
            get => _fTopo;
            set => SetProperty(ref _fTopo, value);
        }

        public double OtherAnimalsVolitilizationFraction
        {
            get => _otherAnimalsVolitilizationFraction;
            set => SetProperty(ref _otherAnimalsVolitilizationFraction, value);
        }

        public double PoultryVolitilizationFraction
        {
            get => _poultryVolitilizationFraction;
            set => SetProperty(ref _poultryVolitilizationFraction, value);
        }

        public int EmergenceDayForPerennials
        {
            get => _emergenceDayForPerennials;
            set => SetProperty(ref _emergenceDayForPerennials, value);
        }

        public int RipeningDayForPerennnials
        {
            get => _ripeningDayForPerennnials;
            set => SetProperty(ref _ripeningDayForPerennnials, value);
        }

        public double VarianceForPerennials
        {
            get => _varianceForPerennials;
            set => SetProperty(ref _varianceForPerennials, value);
        }

        public EquilibriumCalculationStrategies EquilibriumCalculationStrategy
        {
            get => _equilibriumCalculationStrategy;
            set => SetProperty(ref _equilibriumCalculationStrategy, value);
        }

        public int NumberOfYearsInCarRegionAverage
        {
            get => _numberOfYearsInCarRegionAverage;
            set => SetProperty(ref _numberOfYearsInCarRegionAverage, value);
        }

        /// <summary>
        ///     (kg CO2 kWh^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsCO2PerKiloWattHour)]
        public double ConversionOfElectricityToCo2
        {
            get => _conversionOfElectricityToCO2;
            set => SetProperty(ref _conversionOfElectricityToCO2, value);
        }

        /// <summary>
        ///     (kg CO2 GJ^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsCO2PerGigaJoule)]
        public double ConversionOfGjOfDieselToCo2
        {
            get => _conversionOfGJOfDieselToCO2;
            set => SetProperty(ref _conversionOfGJOfDieselToCO2, value);
        }

        /// <summary>
        ///     (kg CO2 GJ^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsCO2PerGigaJoule)]
        public double ConversionOfGjForHerbicideProduction
        {
            get => _conversionOfGJForHerbicideProduction;
            set => SetProperty(ref _conversionOfGJForHerbicideProduction, value);
        }

        /// <summary>
        ///     (kg CO2 (kg N)^-1)
        /// </summary>
        public double NitrogenFertilizerConversionFactor
        {
            get => _nitrogenFertilizerConversionFactor;
            set => SetProperty(ref _nitrogenFertilizerConversionFactor, value);
        }

        /// <summary>
        ///     (kg CO2 (kg P2O5)^-1)
        /// </summary>
        public double PhosphorusFertilizerConversionFactor
        {
            get => _phosphorusFertilizerConversionFactor;
            set => SetProperty(ref _phosphorusFertilizerConversionFactor, value);
        }

        /// <summary>
        ///     (kg CO2 (kg P2O5)^-1)
        /// </summary>
        public double PotassiumConversionFactor
        {
            get => _potassiumConversionFactor;
            set => SetProperty(ref _potassiumConversionFactor, value);
        }

        /// <summary>
        ///     User can indicate if a custom nitrogen fertilizer conversion factor should be used.
        /// </summary>
        public bool UseCustomNitrogenFertilizerConversionFactor
        {
            get => _useCustomNitrogenFertilizerConversionFactor;
            set => SetProperty(ref _useCustomNitrogenFertilizerConversionFactor, value);
        }

        /// <summary>
        ///     User can indicate if a custom phosphorus fertilizer conversion factor should be used.
        /// </summary>
        public bool UseCustomPhosphorusFertilizerConversionFactor
        {
            get => _useCustomPhosphorusFertilizerConversionFactor;
            set => SetProperty(ref _useCustomPhosphorusFertilizerConversionFactor, value);
        }

        /// <summary>
        ///     User can indicate if a custom potassium conversion factor should be used.
        /// </summary>
        public bool UseCustomPotassiumConversionFactor
        {
            get => _useCustomPotassiumConversionFactor;
            set => SetProperty(ref _useCustomPotassiumConversionFactor, value);
        }

        /// <summary>
        /// User can indicate if a custom electricity conversion factor should be used.
        /// </summary>
        public bool UseCustomElectricityConversionFactor
        {
            get => _useCustomElectricityConversionFactor;
            set => SetProperty(ref _useCustomElectricityConversionFactor, value);
        }

        public bool UseCustomElectricityConversionFactorForBeef
        {
            get => _useCustomElectricityConversionFactorForBeef;
            set => SetProperty(ref _useCustomElectricityConversionFactorForBeef, value);
        }

        public bool UseCustomElectricityConversionFactorForDairy
        {
            get => _useCustomElectricityConversionFactorForDairy;
            set => SetProperty(ref _useCustomElectricityConversionFactorForDairy, value);
        }

        public bool UseCustomElectricityConversionFactorForPoultry
        {
            get => _useCustomElectricityConversionFactorForPoultry;
            set => SetProperty(ref _useCustomElectricityConversionFactorForPoultry, value);
        }

        public bool UseCustomElectricityConversionFactorForSwine
        {
            get => _useCustomElectricityConversionFactorForSwine;
            set => SetProperty(ref _useCustomElectricityConversionFactorForSwine, value);
        }

        /// <summary>
        /// Allow the user to set a custom N2O emission factor instead of calculated value (Eq. 2.5.1-8)
        /// </summary>
        public bool UseCustomN2OEmissionFactor
        {
            get => _useCustomN2OEmissionFactor;
            set => SetProperty(ref _useCustomN2OEmissionFactor, value);
        }

        /// <summary>
        ///     Custom user-specified N2O emission factor. Overrides calculated value from Eq. 2.5.1-8
        ///     (kg N2O-N kg^-1 N)
        /// </summary>
        public double CustomN2OEmissionFactor
        {
            get => _customN2OEmissionFactor;
            set => SetProperty(ref _customN2OEmissionFactor, value);
        }

        /// <summary>
        ///     (kg CO2 ha^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsPerHectare)]
        public double ConversionOfAreaIrrigated
        {
            get => _conversionOfAreaIrrigated;
            set => SetProperty(ref _conversionOfAreaIrrigated, value);
        }

        /// <summary>
        ///     (kWh cattle^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KiloWattHourPerAnimal)]
        public double ElectricityHousedBeef
        {
            get => _electricityHousedBeef;
            set => SetProperty(ref _electricityHousedBeef, value);
        }

        /// <summary>
        ///     (kWh dairy^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KiloWattHourPerAnimal)]
        public double ElectricityDairy
        {
            get => _electricityDairy;
            set => SetProperty(ref _electricityDairy, value);
        }

        /// <summary>
        ///     (kWh pig^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KiloWattHourPerAnimal)]
        public double ElectricitySwine
        {
            get => _electricitySwine;
            set => SetProperty(ref _electricitySwine, value);
        }

        /// <summary>
        ///     (kWh poultry^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilowattHourPerPoultryPlacement)]
        public double ElectricityPoultry
        {
            get => _electricityPoultry;
            set => SetProperty(ref _electricityPoultry, value);
        }

        /// <summary>
        ///     (GJ 1000 litre^1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.GigaJoulesPer1000Litres)]
        public double LiquidManureSpreading
        {
            get => _liquidManureSpreading;
            set => SetProperty(ref _liquidManureSpreading, value);
        }

        /// <summary>
        ///     (GJ 1000 litre^1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.GigaJoulesPer1000Litres)]
        public double SolidManureSpreading
        {
            get => _solidManureSpreading;
            set => SetProperty(ref _solidManureSpreading, value);
        }

        public double LiquidManureSwineConcentrationEnergy
        {
            get => _liquidManureSwineConcentrationEnergy;
            set => SetProperty(ref _liquidManureSwineConcentrationEnergy, value);
        }

        public double LiquidManureDairyConcentrationEnergy
        {
            get => _liquidManureDairyConcentrationEnergy;
            set => SetProperty(ref _liquidManureDairyConcentrationEnergy, value);
        }

        public double LiquidManurePoultryConcentrationEnergy
        {
            get => _liquidManurePoultryConcentrationEnergy;
            set => SetProperty(ref _liquidManurePoultryConcentrationEnergy, value);
        }

        public double SolidManureSwineConcentrationEnergy
        {
            get => _solidManureSwineConcentrationEnergy;
            set => SetProperty(ref _solidManureSwineConcentrationEnergy, value);
        }

        public double SolidManureDairyConcentrationEnergy
        {
            get => _solidManureDairyConcentrationEnergy;
            set => SetProperty(ref _solidManureDairyConcentrationEnergy, value);
        }

        public double SolidManurePoultryConcentrationEnergy
        {
            get => _solidManurePoultryConcentrationEnergy;
            set => SetProperty(ref _solidManurePoultryConcentrationEnergy, value);
        }

        public double SolidManureBeefConcentrationEnergy
        {
            get => _solidManureBeefConcentrationEnergy;
            set => SetProperty(ref _solidManureBeefConcentrationEnergy, value);
        }

        public double SolidManureSheepConcentrationEnergy
        {
            get => _solidManureSheepConcentrationEnergy;
            set => SetProperty(ref _solidManureSheepConcentrationEnergy, value);
        }

        /// <summary>
        ///     Used when calculating above ground inputs for perennials
        ///     (%)
        /// </summary>
        public double EstablishmentGrowthFactorPercentageForPerennials
        {
            get => _establishmentGrowthFactorPercentageForPerennials;
            set => SetProperty(ref _establishmentGrowthFactorPercentageForPerennials, value);
        }

        public double EstablishmentGrowthFactorFractionForPerennials =>
            EstablishmentGrowthFactorPercentageForPerennials / 100;

        public TillageType DefaultTillageTypeForFallow
        {
            get => _defaultTillageTypeForFallow;
            set => SetProperty(ref _defaultTillageTypeForFallow, value);
        }

        /// <summary>
        ///     The amount of feed that is lost when grazing animals are fed a supplemental forage/feed while grazing on pasture
        ///     (%)
        /// </summary>
        public double DefaultSupplementalFeedingLossPercentage
        {
            get => _defaultSupplementalFeedingLossPercentage;
            set => SetProperty(ref _defaultSupplementalFeedingLossPercentage, value);
        }

        /// <summary>
        ///     Default run in period in years for IPCC Tier 2 carbon model
        /// </summary>
        public int DefaultRunInPeriod
        {
            get => _defaultRunInPeriod;
            set => SetProperty(ref _defaultRunInPeriod, value);
        }

        /// <summary>
        ///     The type of pump used for irrigation in the farm. Can be changed by the user in the User Settings menu of the UI.
        /// </summary>
        public PumpType DefaultPumpType
        {
            get => _defaulPumpType;
            set => SetProperty(ref _defaulPumpType, value);
        }

        public double PumpEmissionFactor
        {
            get
            {
                if (DefaultPumpType == PumpType.ElectricPump)
                    _pumpEmissionsFactor = 0.266;
                else if (DefaultPumpType == PumpType.NaturalGasPump)
                    _pumpEmissionsFactor = 1.145;
                else
                    _pumpEmissionsFactor = 1;

                return _pumpEmissionsFactor;
            }
        }

        public double ActiveCarbonN { get; set; }

        public double SlowCarbonN { get; set; }

        #endregion

        #region AD

        public double DefaultBiodegradableFractionDairyManure
        {
            get => _defaultBiodegradableFractionDairyManure;
            set => SetProperty(ref _defaultBiodegradableFractionDairyManure, value);
        }

        public double DefaultBiodegradableFractionSwineManure
        {
            get => _defaultBiodegradableFractionSwineManure;
            set => SetProperty(ref _defaultBiodegradableFractionSwineManure, value);
        }

        public double DefaultBiodegradableFractionOtherManure
        {
            get => _defaultBiodegradableFractionOtherManure;
            set => SetProperty(ref _defaultBiodegradableFractionOtherManure, value);
        }

        public double DefaultBiodegradableFractionGreenWaste
        {
            get => _defaultBiodegradableFractionGreenWaste;
            set => SetProperty(ref _defaultBiodegradableFractionGreenWaste, value);
        }

        public double DefaultNitrogenFixation
        {
            get => _defaultNitrogenFixation;
            set => SetProperty(ref _defaultNitrogenFixation, value);
        }

        public TillageType RunInPeriodTillageType
        {
            get => _runInPeriodTillageType;
            set => SetProperty(ref _runInPeriodTillageType, value);
        }

        public bool ScaleUpEmissionsEnabled
        {
            get => _scaleUpEmissionsEnabled;
            set => SetProperty(ref _scaleUpEmissionsEnabled, value);
        }

        public double PercentageOfStrawReturnedToSoilForPerennials
        {
            get => _percentageOfStrawReturnedToSoilForPerennials;
            set => SetProperty(ref _percentageOfStrawReturnedToSoilForPerennials, value);
        }

        public ResidueInputCalculationMethod ResidueInputCalculationMethod
        {
            get => _residueInputCalculationMethod;
            set => SetProperty(ref _residueInputCalculationMethod, value);
        }

        /// <summary>
        ///     Used with the CLI to specify how soil data should be initialized (by settings file or by SLC database)
        /// </summary>
        public SoilDataAcquisitionMethod SoilDataAcquisitionMethod
        {
            get => _soilDataAcquisitionMethod;
            set => SetProperty(ref _soilDataAcquisitionMethod, value);
        }

        #endregion
    }
}