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
        #region Fields

        private double _carbonConcentration;
        private TimeFrame _timeFrame;
        private CarbonModellingStrategies _carbonModellingStrategy;

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

        #region Constructors

        public Defaults()
        {
            this.TimeFrame = TimeFrame.TwoThousandToCurrent;

            this.DefaultTillageTypeForFallow = TillageType.NoTill;

            this.CarbonConcentration = CoreConstants.CarbonConcentration;
            this.NumberOfYearsInCarRegionAverage = 3;

            // Default location is Lethbridge, AB
            this.Latitude = 49.69999;
            this.Longitude = -112.81856;

            this.EmergenceDay = 141;
            this.RipeningDay = 197;
            this.Variance = 300;

            this.EmergenceDayForPerennials = 75;
            this.RipeningDayForPerennnials = 300;
            this.VarianceForPerennials = 1500;

            this.Alfa = 0.7;
            this.DecompositionMinimumTemperature = -3.78;
            this.DecompositionMaximumTemperature = 30;
            this.MoistureResponseFunctionAtSaturation = 0.42;
            this.MoistureResponseFunctionAtWiltingPoint = 0.18;

            // Annual crops
            this.PercentageOfProductReturnedToSoilForAnnuals = 2;
            this.PercentageOfStrawReturnedToSoilForAnnuals = 100;
            this.PercentageOfRootsReturnedToSoilForAnnuals = 100;

            // Silage crops
            this.PercentageOfProductYieldReturnedToSoilForSilageCrops = 35;
            this.PercentageOfRootsReturnedToSoilForSilageCrops = 100;

            // Cover crops
            this.PercentageOfProductYieldReturnedToSoilForCoverCrops = 100;
            this.PercentageOfProductYieldReturnedToSoilForCoverCropsForage = 35;
            this.PercentageOfProductYieldReturnedToSoilForCoverCropsProduce = 0;
            this.PercentageOfStrawReturnedToSoilForCoverCrops = 100;
            this.PercetageOfRootsReturnedToSoilForCoverCrops = 100;

            // Root crops
            this.PercentageOfProductReturnedToSoilForRootCrops = 0;
            this.PercentageOfStrawReturnedToSoilForRootCrops = 100;

            // Perennial crops
            this.PercentageOfProductReturnedToSoilForPerennials = 35;
            this.PercentageOfStrawReturnedToSoilForPerennials = 0;
            this.PercentageOfRootsReturnedToSoilForPerennials = 100;
            this.EstablishmentGrowthFactorPercentageForPerennials = 50;
            this.DefaultSupplementalFeedingLossPercentage = 20;

            // Rangeland
            this.PercentageOfProductReturnedToSoilForRangelandDueToHarvestLoss = 35;
            this.PercentageOfProductReturnedToSoilForRangelandDueToGrazingLoss = CoreConstants.ValueNotDetermined;
            this.PercentageOfRootsReturnedToSoilForRangeland = 100;

            // Fodder corn
            this.PercentageOfProductReturnedToSoilForFodderCorn = 35;
            this.PercentageOfRootsReturnedToSoilForFodderCorn = 100;

            this.HumificationCoefficientAboveGround = 0.125;
            this.HumificationCoefficientBelowGround = 0.3;
            this.HumificationCoefficientManure = 0.31;

            this.DecompositionRateConstantYoungPool = 0.8;
            this.DecompositionRateConstantOldPool = 0.00605;

            this.OldPoolCarbonN = 0.1;      // 1/10
            this.SlowCarbonN = 0.05;        // 1/20
            this.ActiveCarbonN = 0.025;     // 1/40
            this.NORatio = 0.1;
            this.EmissionFactorForLeachingAndRunoff = 0.011; // Updated to IPCC 2019 value
            this.EmissionFactorForVolatilization = 0.01;
            this.CustomN2OEmissionFactor = 0.003;

            this.FractionOfNLostByVolatilization = 0.21;
            this.OtherAnimalsVolitilizationFraction = 0.2;
            this.PoultryVolitilizationFraction = 0.4;

            this.MicrobeDeath = 0.2;
            this.Denitrification = 0.5;
            this.FertilizerEfficiency = 0.75;
            this.FTopo = 14.03;
            this.DefaultNitrogenFixation = 0.7;

            this.UseClimateParameterInsteadOfManagementFactor = true;

            // Energy
            this.ConversionOfElectricityToCo2 = 0.2;
            this.ConversionOfGjOfDieselToCo2 = 70;
            this.ConversionOfGjForHerbicideProduction = 5.8;
            this.NitrogenFertilizerConversionFactor = 3.59;
            this.PhosphorusFertilizerConversionFactor = 0.5699;
            this.ConversionOfAreaIrrigated = 367;
            this.ElectricityHousedBeef = 65.7;
            this.ElectricityDairy = 968;
            this.ElectricitySwine = 1.06;
            this.ElectricityPoultry = 2.88;
            this.LiquidManureSpreading = 0.0248;
            this.SolidManureSpreading = 0.0248;

            this.LiquidManureSwineConcentrationEnergy = 3.5;
            this.LiquidManureDairyConcentrationEnergy = 3.4;
            this.LiquidManurePoultryConcentrationEnergy = 6;

            this.SolidManureSwineConcentrationEnergy = 8;
            this.SolidManureDairyConcentrationEnergy = 5;
            this.SolidManurePoultryConcentrationEnergy = 24.1;
            this.SolidManureBeefConcentrationEnergy = 10;
            this.SolidManureSheepConcentrationEnergy = 10;

            // Previous run-in period of 5 years was too small and resulted in very high starting/equilibrium states. Using a higher value is needed.
            this.DefaultRunInPeriod = 15;
            this.RunInPeriodTillageType = TillageType.Reduced;
            this.CarbonModellingStrategy = CarbonModellingStrategies.IPCCTier2;

            // AD
            this.DefaultBiodegradableFractionDairyManure = 0.025;
            this.DefaultBiodegradableFractionGreenWaste = 0.024;
            this.DefaultBiodegradableFractionSwineManure = 0.024;
            this.DefaultBiodegradableFractionOtherManure = 0.550;
        }


        #endregion

        #region Properties

        /// <summary>
        /// Indicates if the system should display/output run in period items when displaying final results
        /// </summary>
        public bool OutputRunInPeriodItems { get; set; }

        public CarbonModellingStrategies CarbonModellingStrategy
        {
            get => _carbonModellingStrategy;
            set => SetProperty(ref _carbonModellingStrategy, value, () => 
            {
                base.RaisePropertyChanged(nameof(this.UseICBMStrategy));
            }
            );
        }

        public bool UseICBMStrategy
        {
            get
            {
                return this.CarbonModellingStrategy == CarbonModellingStrategies.ICBM;
            }
        }

        /// <summary>
        /// The period of time in which climate normals are calculated from a set of daily climate data values
        /// </summary>
        public TimeFrame TimeFrame
        {
            get { return _timeFrame; }
            set { SetProperty(ref _timeFrame, value); }
        }

        public bool UseClimateParameterInsteadOfManagementFactor
        {
            get { return _useClimateParameterInsteadOfManagementFactor; }
            set { SetProperty(ref _useClimateParameterInsteadOfManagementFactor, value); }
        }

        /// <summary>
        /// (kg kg^-1)
        /// </summary>
        public double CarbonConcentration
        {
            get { return _carbonConcentration; }
            set { this.SetProperty(ref _carbonConcentration, value); }
        }
        public double Latitude
        {
            get { return _latitude; }
            set { this.SetProperty(ref _latitude, value); }
        }
        public double Longitude
        {
            get { return _longitude; }
            set { this.SetProperty(ref _longitude, value); }
        }

        public int EmergenceDay
        {
            get { return _emergenceDay; }
            set { this.SetProperty(ref _emergenceDay, value); }
        }

        public int RipeningDay
        {
            get { return _ripeningDay; }
            set { this.SetProperty(ref _ripeningDay, value); }
        }

        public double Variance
        {
            get { return _variance; }
            set { this.SetProperty(ref _variance, value); }
        }

        public double Alfa
        {
            get { return _alfa; }
            set { this.SetProperty(ref _alfa, value); }
        }

        public double DecompositionMinimumTemperature
        {
            get { return _decompositionMinimumTemperature; }
            set { this.SetProperty(ref _decompositionMinimumTemperature, value); }
        }

        public double DecompositionMaximumTemperature
        {
            get { return _decompositionMaximumTemperature; }
            set { this.SetProperty(ref _decompositionMaximumTemperature, value); }
        }

        public double MoistureResponseFunctionAtSaturation
        {
            get { return _moistureResponseFunctionAtSaturation; }
            set { this.SetProperty(ref _moistureResponseFunctionAtSaturation, value); }
        }

        public double MoistureResponseFunctionAtWiltingPoint
        {
            get { return _moistureResponseFunctionAtWiltingPoint; }
            set { this.SetProperty(ref _moistureResponseFunctionAtWiltingPoint, value); }
        }

        public double PercentageOfProductReturnedToSoilForAnnuals
        {
            get { return _percentageOfProductReturnedToSoilForAnnuals; }
            set { this.SetProperty(ref _percentageOfProductReturnedToSoilForAnnuals, value); }
        }

        public double PercentageOfStrawReturnedToSoilForAnnuals
        {
            get { return _percentageOfStrawReturnedToSoilForAnnuals; }
            set { this.SetProperty(ref _percentageOfStrawReturnedToSoilForAnnuals, value); }
        }

        public double PercentageOfRootsReturnedToSoilForAnnuals
        {
            get { return _percentageOfRootsReturnedToSoilForAnnuals; }
            set { this.SetProperty(ref _percentageOfRootsReturnedToSoilForAnnuals, value); }
        }

        public double PercentageOfProductReturnedToSoilForPerennials
        {
            get { return _percentageOfProductReturnedToSoilForPerennials; }
            set { this.SetProperty(ref _percentageOfProductReturnedToSoilForPerennials, value); }
        }

        public double PercentageOfRootsReturnedToSoilForPerennials
        {
            get { return _percentageOfRootsReturnedToSoilForPerennials; }
            set { this.SetProperty(ref _percentageOfRootsReturnedToSoilForPerennials, value); }
        }

        public double PercentageOfProductReturnedToSoilForFodderCorn
        {
            get { return _percentageOfProductReturnedToSoilForFodderCorn; }
            set { this.SetProperty(ref _percentageOfProductReturnedToSoilForFodderCorn, value); }
        }

        public double PercentageOfRootsReturnedToSoilForFodderCorn
        {
            get { return _percentageOfRootsReturnedToSoilForFodderCorn; }
            set { this.SetProperty(ref _percentageOfRootsReturnedToSoilForFodderCorn, value); }
        }

        public double PercentageOfProductReturnedToSoilForRootCrops
        {
            get { return _percentageOfProductReturnedToSoilForRootCrops; }
            set { this.SetProperty(ref _percentageOfProductReturnedToSoilForRootCrops, value); }
        }

        public double PercentageOfStrawReturnedToSoilForRootCrops
        {
            get { return _percentageOfStrawReturnedToSoilForRootCrops; }
            set { this.SetProperty(ref _percentageOfStrawReturnedToSoilForRootCrops, value); }
        }

        public double HumificationCoefficientAboveGround
        {
            get { return _humificationCoefficientAboveGround; }
            set { this.SetProperty(ref _humificationCoefficientAboveGround, value); }
        }

        public double HumificationCoefficientBelowGround
        {
            get { return _humificationCoefficientBelowGround; }
            set { this.SetProperty(ref _humificationCoefficientBelowGround, value); }
        }

        public double DecompositionRateConstantYoungPool
        {
            get { return _decompositionRateConstantYoungPool; }
            set { this.SetProperty(ref _decompositionRateConstantYoungPool, value); }
        }

        public double DecompositionRateConstantOldPool
        {
            get { return _decompositionRateConstantOldPool; }
            set { this.SetProperty(ref _decompositionRateConstantOldPool, value); }
        }

        public double HumificationCoefficientManure
        {
            get { return _humificationCoefficientManure; }
            set { this.SetProperty(ref _humificationCoefficientManure, value); }
        }

        public double PercentageOfProductYieldReturnedToSoilForSilageCrops
        {
            get { return _percentageOfProductYieldReturnedToSoilForSilageCrops; }
            set { this.SetProperty(ref _percentageOfProductYieldReturnedToSoilForSilageCrops, value); }
        }

        public double PercentageOfRootsReturnedToSoilForSilageCrops
        {
            get { return _percentageOfRootsReturnedToSoilForSilageCrops; }
            set { this.SetProperty(ref _percentageOfRootsReturnedToSoilForSilageCrops, value); }
        }

        public double PercentageOfProductYieldReturnedToSoilForCoverCrops
        {
            get { return _percentageOfProductYieldReturnedToSoilForCoverCrops; }
            set { this.SetProperty(ref _percentageOfProductYieldReturnedToSoilForCoverCrops, value); }
        }

        public double PercentageOfProductYieldReturnedToSoilForCoverCropsForage
        {
            get { return _percentageOfProductYieldReturnedToSoilForCoverCropsForage; }
            set { this.SetProperty(ref _percentageOfProductYieldReturnedToSoilForCoverCropsForage, value); }
        }

        public double PercentageOfProductYieldReturnedToSoilForCoverCropsProduce
        {
            get { return _percentageOfProductYieldReturnedToSoilForCoverCropsProduce; }
            set { this.SetProperty(ref _percentageOfProductYieldReturnedToSoilForCoverCropsProduce, value); }
        }

        public double PercentageOfStrawReturnedToSoilForCoverCrops
        {
            get { return _percentageOfStrawReturnedToSoilForCoverCrops; }
            set { this.SetProperty(ref _percentageOfStrawReturnedToSoilForCoverCrops, value); }
        }

        public double PercetageOfRootsReturnedToSoilForCoverCrops
        {
            get { return _percetageOfRootsReturnedToSoilForCoverCrops; }
            set { this.SetProperty(ref _percetageOfRootsReturnedToSoilForCoverCrops, value); }
        }

        public double PercentageOfProductReturnedToSoilForRangelandDueToHarvestLoss
        {
            get { return _percentageOfProductReturnedToSoilForRangelandDueToHarvestLoss; }
            set { this.SetProperty(ref _percentageOfProductReturnedToSoilForRangelandDueToHarvestLoss, value); }
        }

        public double PercentageOfProductReturnedToSoilForRangelandDueToGrazingLoss
        {
            get { return _percentageOfProductReturnedToSoilForRangelandDueToGrazingLoss; }
            set { this.SetProperty(ref _percentageOfProductReturnedToSoilForRangelandDueToGrazingLoss, value); }
        }

        public double PercentageOfRootsReturnedToSoilForRangeland
        {
            get { return _percentageOfRootsReturnedToSoilForRangeland; }
            set { this.SetProperty(ref _percentageOfRootsReturnedToSoilForRangeland, value); }
        }

        /// <summary>
        /// Section 2.3.3
        /// </summary>
        public double OldPoolCarbonN
        {
            get { return _oldPoolCarbonN; }
            set { this.SetProperty(ref _oldPoolCarbonN, value); }
        }

        /// <summary>
        /// Section 2.3.4.1
        /// </summary>
        public double NORatio
        {
            get { return _NORatio; }
            set { this.SetProperty(ref _NORatio, value); }
        }

        /// <summary>
        /// EF_leach
        ///
        /// (kg N2O-N (kg N)^-1)
        /// </summary>
        public double EmissionFactorForLeachingAndRunoff
        {
            get { return _emissionFactorForLeachingAndRunoff; }
            set { this.SetProperty(ref _emissionFactorForLeachingAndRunoff, value); }
        }

        /// <summary>
        /// EF_volatilization (IPCC 2006)
        /// Section 2.3.5.3
        /// </summary>
        public double EmissionFactorForVolatilization
        {
            get { return _emissionFactorForVolatilization; }
            set { this.SetProperty(ref _emissionFactorForVolatilization, value); }
        }

        /// <summary>
        /// Frac_volatilizationsoil
        ///
        /// (IPCC 2019)
        /// </summary>
        public double FractionOfNLostByVolatilization
        {
            get { return _fractionOfNLostByVolatilization; }
            set { this.SetProperty(ref _fractionOfNLostByVolatilization, value); }
        }

        /// <summary>
        /// Section 2.3.6.3
        /// </summary>
        public double MicrobeDeath
        {
            get { return _microbeDeath; }
            set { this.SetProperty(ref _microbeDeath, value); }
        }

        /// <summary>
        /// Section 2.3.6.3
        /// </summary>
        public double Denitrification
        {
            get { return _denitrification; }
            set { this.SetProperty(ref _denitrification, value); }
        }

        public double FertilizerEfficiency
        {
            get { return _fertilizerEfficiency; }
            set { SetProperty(ref _fertilizerEfficiency, value); }
        }

        public double FTopo
        {
            get { return _fTopo; }
            set { SetProperty(ref _fTopo, value); }
        }

        public double OtherAnimalsVolitilizationFraction
        {
            get { return _otherAnimalsVolitilizationFraction; }
            set { SetProperty(ref _otherAnimalsVolitilizationFraction, value); }
        }

        public double PoultryVolitilizationFraction
        {
            get { return _poultryVolitilizationFraction; }
            set { SetProperty(ref _poultryVolitilizationFraction, value); }
        }

        public int EmergenceDayForPerennials
        {
            get { return _emergenceDayForPerennials; }
            set { SetProperty(ref _emergenceDayForPerennials, value); }
        }

        public int RipeningDayForPerennnials
        {
            get { return _ripeningDayForPerennnials; }
            set { SetProperty(ref _ripeningDayForPerennnials, value); }
        }

        public double VarianceForPerennials
        {
            get { return _varianceForPerennials; }
            set { SetProperty(ref _varianceForPerennials, value); }
        }

        public EquilibriumCalculationStrategies EquilibriumCalculationStrategy
        {
            get { return _equilibriumCalculationStrategy; }
            set { SetProperty(ref _equilibriumCalculationStrategy, value); }
        }

        public int NumberOfYearsInCarRegionAverage
        {
            get { return _numberOfYearsInCarRegionAverage; }
            set { SetProperty(ref _numberOfYearsInCarRegionAverage, value); }
        }

        /// <summary>
        /// (kg CO2 kWh^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsCO2PerKiloWattHour)]
        public double ConversionOfElectricityToCo2
        {
            get { return _conversionOfElectricityToCO2; }
            set { SetProperty(ref _conversionOfElectricityToCO2, value); }
        }

        /// <summary>
        /// (kg CO2 GJ^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsCO2PerGigaJoule)]
        public double ConversionOfGjOfDieselToCo2
        {
            get { return _conversionOfGJOfDieselToCO2; }
            set { SetProperty(ref _conversionOfGJOfDieselToCO2, value); }
        }

        /// <summary>
        /// (kg CO2 GJ^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsCO2PerGigaJoule)]
        public double ConversionOfGjForHerbicideProduction
        {
            get { return _conversionOfGJForHerbicideProduction; }
            set { SetProperty(ref _conversionOfGJForHerbicideProduction, value); }
        }

        /// <summary>
        /// (kg CO2 (kg N)^-1)
        /// </summary>
        public double NitrogenFertilizerConversionFactor
        {
            get { return _nitrogenFertilizerConversionFactor; }
            set { SetProperty(ref _nitrogenFertilizerConversionFactor, value); }
        }

        /// <summary>
        /// (kg CO2 (kg P2O5)^-1)
        /// </summary>
        public double PhosphorusFertilizerConversionFactor
        {
            get { return _phosphorusFertilizerConversionFactor; }
            set { SetProperty(ref _phosphorusFertilizerConversionFactor, value); }
        }

        /// <summary>
        /// (kg CO2 (kg P2O5)^-1)
        /// </summary>
        public double PotassiumConversionFactor
        {
            get { return _potassiumConversionFactor; }
            set { SetProperty(ref _potassiumConversionFactor, value); }
        }

        /// <summary>
        /// User can indicate if a custom nitrogen fertilizer conversion factor should be used.
        /// </summary>
        public bool UseCustomNitrogenFertilizerConversionFactor
        {
            get => _useCustomNitrogenFertilizerConversionFactor;
            set => SetProperty(ref _useCustomNitrogenFertilizerConversionFactor, value);
        }

        /// <summary>
        /// User can indicate if a custom phosphorus fertilizer conversion factor should be used.
        /// </summary>
        public bool UseCustomPhosphorusFertilizerConversionFactor
        {
            get => _useCustomPhosphorusFertilizerConversionFactor;
            set => SetProperty(ref _useCustomPhosphorusFertilizerConversionFactor, value);
        }

        /// <summary>
        /// User can indicate if a custom potassium conversion factor should be used.
        /// </summary>
        public bool UseCustomPotassiumConversionFactor
        {
            get => _useCustomPotassiumConversionFactor;
            set => SetProperty(ref _useCustomPotassiumConversionFactor, value);
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
        /// Custom user-specified N2O emission factor. Overrides calculated value from Eq. 2.5.1-8
        /// 
        /// (kg N2O-N kg^-1 N)
        /// </summary>
        public double CustomN2OEmissionFactor
        {
            get => _customN2OEmissionFactor;
            set => SetProperty(ref _customN2OEmissionFactor, value);
        }

        /// <summary>
        /// (kg CO2 ha^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsPerHectare)]
        public double ConversionOfAreaIrrigated
        {
            get { return _conversionOfAreaIrrigated; }
            set { SetProperty(ref _conversionOfAreaIrrigated, value); }
        }

        /// <summary>
        /// (kWh cattle^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KiloWattHourPerAnimal)]
        public double ElectricityHousedBeef
        {
            get { return _electricityHousedBeef; }
            set { SetProperty(ref _electricityHousedBeef, value); }
        }

        /// <summary>
        /// (kWh dairy^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KiloWattHourPerAnimal)]
        public double ElectricityDairy
        {
            get { return _electricityDairy; }
            set { SetProperty(ref _electricityDairy, value); }
        }

        /// <summary>
        /// (kWh pig^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KiloWattHourPerAnimal)]
        public double ElectricitySwine
        {
            get { return _electricitySwine; }
            set { SetProperty(ref _electricitySwine, value); }
        }

        /// <summary>
        /// (kWh poultry^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilowattHourPerPoultryPlacement)]
        public double ElectricityPoultry
        {
            get { return _electricityPoultry; }
            set { SetProperty(ref _electricityPoultry, value); }
        }

        /// <summary>
        /// (GJ 1000 litre^1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.GigaJoulesPer1000Litres)]
        public double LiquidManureSpreading
        {
            get { return _liquidManureSpreading; }
            set { SetProperty(ref _liquidManureSpreading, value); }
        }

        /// <summary>
        /// (GJ 1000 litre^1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.GigaJoulesPer1000Litres)]
        public double SolidManureSpreading
        {
            get { return _solidManureSpreading; }
            set { SetProperty(ref _solidManureSpreading, value); }
        }

        public double LiquidManureSwineConcentrationEnergy
        {
            get { return _liquidManureSwineConcentrationEnergy; }
            set { SetProperty(ref _liquidManureSwineConcentrationEnergy, value); }
        }

        public double LiquidManureDairyConcentrationEnergy
        {
            get { return _liquidManureDairyConcentrationEnergy; }
            set { SetProperty(ref _liquidManureDairyConcentrationEnergy, value); }
        }

        public double LiquidManurePoultryConcentrationEnergy
        {
            get { return _liquidManurePoultryConcentrationEnergy; }
            set { SetProperty(ref _liquidManurePoultryConcentrationEnergy, value); }
        }

        public double SolidManureSwineConcentrationEnergy
        {
            get { return _solidManureSwineConcentrationEnergy; }
            set { SetProperty(ref _solidManureSwineConcentrationEnergy, value); }
        }

        public double SolidManureDairyConcentrationEnergy
        {
            get { return _solidManureDairyConcentrationEnergy; }
            set { SetProperty(ref _solidManureDairyConcentrationEnergy, value); }
        }

        public double SolidManurePoultryConcentrationEnergy
        {
            get { return _solidManurePoultryConcentrationEnergy; }
            set { SetProperty(ref _solidManurePoultryConcentrationEnergy, value); }
        }

        public double SolidManureBeefConcentrationEnergy
        {
            get { return _solidManureBeefConcentrationEnergy; }
            set { SetProperty(ref _solidManureBeefConcentrationEnergy, value); }
        }

        public double SolidManureSheepConcentrationEnergy
        {
            get { return _solidManureSheepConcentrationEnergy; }
            set { SetProperty(ref _solidManureSheepConcentrationEnergy, value); }
        }

        /// <summary>
        /// Used when calculating above ground inputs for perennials
        ///
        /// (%)
        /// </summary>
        public double EstablishmentGrowthFactorPercentageForPerennials
        {
            get => _establishmentGrowthFactorPercentageForPerennials;
            set => SetProperty(ref _establishmentGrowthFactorPercentageForPerennials, value);
        }

        public double EstablishmentGrowthFactorFractionForPerennials
        {
            get
            {
                return this.EstablishmentGrowthFactorPercentageForPerennials / 100;
            }
        }

        public TillageType DefaultTillageTypeForFallow
        {
            get => _defaultTillageTypeForFallow;
            set => SetProperty(ref _defaultTillageTypeForFallow, value);
        }

        /// <summary>
        /// The amount of feed that is lost when grazing animals are fed a supplemental forage/feed while grazing on pasture
        ///
        /// (%)
        /// </summary>
        public double DefaultSupplementalFeedingLossPercentage
        {
            get => _defaultSupplementalFeedingLossPercentage;
            set => SetProperty(ref _defaultSupplementalFeedingLossPercentage, value);
        }

        /// <summary>
        /// Default run in period in years for IPCC Tier 2 carbon model
        /// </summary>
        public int DefaultRunInPeriod 
        { 
            get => _defaultRunInPeriod; 
            set => SetProperty (ref _defaultRunInPeriod, value); 
        }


        /// <summary>
        /// The type of pump used for irrigation in the farm. Can be changed by the user in the User Settings menu of the UI.
        /// </summary>
        public PumpType DefaultPumpType
        {
            get => _defaulPumpType;
            set => SetProperty (ref _defaulPumpType, value);
        }

        public double PumpEmissionFactor
        {
            get
            {
                if (DefaultPumpType == PumpType.ElectricPump)
                {
                    _pumpEmissionsFactor =  0.266;
                }
                else if (DefaultPumpType == PumpType.NaturalGasPump)
                {
                    _pumpEmissionsFactor = 1.145;
                }
                else
                {
                    _pumpEmissionsFactor = 1;
                }

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

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}