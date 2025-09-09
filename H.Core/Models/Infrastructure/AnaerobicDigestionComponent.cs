using System.Collections.ObjectModel;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Core.Properties;

namespace H.Core.Models.Infrastructure
{
    public class AnaerobicDigestionComponent : ComponentBase
    {
        #region Constructors

        public AnaerobicDigestionComponent()
        {
            ComponentNameDisplayString = Resources.TitleAnaerobicDigestionComponent;
            ComponentDescriptionString = Resources.ToolTipAnaerobicDigestionComponent;
            ComponentCategory = ComponentCategory.Infrastructure;
            ComponentType = ComponentType.AnaerobicDigestion;

            SeparatorType = AnaerobicDigestorSeparatorType.Centrifuge;

            _anaerobicDigestionViewItem = new AnaerobicDigestionViewItem();

            ManagementPeriodViewItems = new ObservableCollection<ADManagementPeriodViewItem>();

            IsLiquidSolidSeparated = true;
        }

        #endregion

        #region Public Methods

        public int GetHydrolicRetentionTime()
        {
            if (NumberOfReactors == 1) return 25;

            return 60;
        }

        #endregion

        #region Fields

        private AnaerobicDigestorSeparatorType _separatorType;

        private AnaerobicDigestionViewItem _anaerobicDigestionViewItem;

        // Methane potential based on hydraulic retention time and kinetic hydrolysis rate
        private double _hydrolysisRateOfManureDuringDigestion;
        private double _hydrolysisRateOfGreenWastesDuringDigestion;
        private double _hydraulicRetentionTimeInDays;

        // Biogas Production
        private double _fractionOfMethaneInBiogas;

        // Organic Loading Rate
        private double _organicLoadingRate;

        //Recoverable methane considering fugitive methane losses:
        private double _fractionOfFugitiveMethaneLosses;

        // Total primary energy production potential:
        private double _calorificValueOfMethane;
        private double _conversionCoefficientKilowattHoursToMegajules;

        // Electricity and heat production through a combined heat and power (CHP) system
        private double _fractionPrimaryEnergyConvertedToElectricity;
        private double _fractionPrimaryEnergyConvertedToHeat;

        // Direct injection into the gas grid:
        private double _fractionOfMethaneLostInUpgradingPlants;

        // Methane emissions upon storage of digestate
        private double _methaneEmissionFactorDigestateStorageWorstCase;
        private double _methaneEmissionFactorDigestateStorageOptimizedFall;
        private double _methaneEmissionFactorDigestateStorageOptimizedSpring;
        private double _methaneEmissionFactorDigestateStorageOptimizedSummer;
        private double _methaneEmissionFactorDigestateStorageOptimizedWinter;

        // Nitrous oxide emissions upon storage of digestate
        private double _nitrousOxideEmissionFactorForDigestateStorage;

        // Ammonia emissions upon storage of digestate
        private double _ammoniaEmissionFactorForDigestateStorage;

        private int _numberOfReactors;
        private bool _isLiquidSolidSeparated;
        private bool _useImportedManure;

        private ObservableCollection<ADManagementPeriodViewItem> _managementPeriodViewItems;

        #endregion

        #region Properties

        public AnaerobicDigestorOutputTypes SelectedOutputType { get; set; }

        public AnaerobicDigestionViewItem AnaerobicDigestionViewItem
        {
            get => _anaerobicDigestionViewItem;
            set => SetProperty(ref _anaerobicDigestionViewItem, value);
        }

        /// <summary>
        ///     day^-1
        /// </summary>
        [Units(MetricUnitsOfMeasurement.PerDay)]
        public double HydrolysisRateOfManureDuringDigestion
        {
            get => _hydrolysisRateOfManureDuringDigestion;
            set => SetProperty(ref _hydrolysisRateOfManureDuringDigestion, value);
        }

        /// <summary>
        ///     day^-1
        /// </summary>
        [Units(MetricUnitsOfMeasurement.PerDay)]
        public double HydrolysisRateOfGreenWastesDuringDigestion
        {
            get => _hydrolysisRateOfGreenWastesDuringDigestion;
            set => SetProperty(ref _hydrolysisRateOfGreenWastesDuringDigestion, value);
        }

        public double HydraulicRetentionTimeInDays
        {
            get => _hydraulicRetentionTimeInDays;
            set => SetProperty(ref _hydraulicRetentionTimeInDays, value);
        }

        public double FractionOfMethaneInBiogas
        {
            get => _fractionOfMethaneInBiogas;
            set => SetProperty(ref _fractionOfMethaneInBiogas, value);
        }

        /// <summary>
        ///     Metric Unit of Measurement: Kg VS m^-3 d^-1<br />
        ///     Imperial Unit of Measurement: lb VS yd^-3 day^-1<br />
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramVolatileSolidsPerCubicMeterPerDay)]
        public double OrganicLoadingRate
        {
            get => _organicLoadingRate;
            set => SetProperty(ref _organicLoadingRate, value);
        }

        public double FractionOfFugitiveMethaneLosses
        {
            get => _fractionOfFugitiveMethaneLosses;
            set => SetProperty(ref _fractionOfFugitiveMethaneLosses, value);
        }

        /// <summary>
        ///     Metric Unit of Measurement: MJ Nm^-3 <br />
        ///     Imperial Unit of Measurement: BTU Sf^-3 <br />
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaJulesPerNormalCubicMeters)]
        public double CalorificValueOfMethane
        {
            get => _calorificValueOfMethane;
            set => SetProperty(ref _calorificValueOfMethane, value);
        }

        public double ConversionCoefficientKilowattHoursToMegajules
        {
            get => _conversionCoefficientKilowattHoursToMegajules;
            set => SetProperty(ref _conversionCoefficientKilowattHoursToMegajules, value);
        }

        public double FractionPrimaryEnergyConvertedToElectricity
        {
            get => _fractionPrimaryEnergyConvertedToElectricity;
            set => SetProperty(ref _fractionPrimaryEnergyConvertedToElectricity, value);
        }

        public double FractionPrimaryEnergyConvertedToHeat
        {
            get => _fractionPrimaryEnergyConvertedToHeat;
            set => SetProperty(ref _ammoniaEmissionFactorForDigestateStorage, value);
        }

        public double FractionOfMethaneLostInUpgradingPlants
        {
            get => _fractionOfMethaneLostInUpgradingPlants;
            set => SetProperty(ref _fractionOfMethaneLostInUpgradingPlants, value);
        }

        /// <summary>
        ///     Metric Unit of Measurement: CH4 m^-3 d^-1<br />
        ///     Imperial Unit of Measurement: CH4 yd^-3 d^-1<br />
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MethanePerCubicMeterPerDay)]
        public double MethaneEmissionFactorDigestateStorageWorstCase
        {
            get => _methaneEmissionFactorDigestateStorageWorstCase;
            set => SetProperty(ref _methaneEmissionFactorDigestateStorageWorstCase, value);
        }

        /// <summary>
        ///     Metric Unit of Measurement: CH4 m^-3 d^-1<br />
        ///     Imperial Unit of Measurement: CH4 yd^-3 d^-1<br />
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MethanePerCubicMeterPerDay)]
        public double MethaneEmissionFactorDigestateStorageOptimizedFall
        {
            get => _methaneEmissionFactorDigestateStorageOptimizedFall;
            set => SetProperty(ref _methaneEmissionFactorDigestateStorageOptimizedFall, value);
        }

        /// <summary>
        ///     Metric Unit of Measurement: CH4 m^-3 d^-1<br />
        ///     Imperial Unit of Measurement: CH4 yd^-3 d^-1<br />
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MethanePerCubicMeterPerDay)]
        public double MethaneEmissionFactorDigestateStorageOptimizedSpring
        {
            get => _methaneEmissionFactorDigestateStorageOptimizedSpring;
            set => SetProperty(ref _methaneEmissionFactorDigestateStorageOptimizedSpring, value);
        }

        /// <summary>
        ///     Metric Unit of Measurement: CH4 m^-3 d^-1<br />
        ///     Imperial Unit of Measurement: CH4 yd^-3 d^-1<br />
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MethanePerCubicMeterPerDay)]
        public double MethaneEmissionFactorDigestateStorageOptimizedSummer
        {
            get => _methaneEmissionFactorDigestateStorageOptimizedSummer;
            set => SetProperty(ref _methaneEmissionFactorDigestateStorageOptimizedSummer, value);
        }

        /// <summary>
        ///     Metric Unit of Measurement: CH4 m^-3 d^-1<br />
        ///     Imperial Unit of Measurement: CH4 yd^-3 d^-1<br />
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MethanePerCubicMeterPerDay)]
        public double MethaneEmissionFactorDigestateStorageOptimizedWinter
        {
            get => _methaneEmissionFactorDigestateStorageOptimizedWinter;
            set => SetProperty(ref _methaneEmissionFactorDigestateStorageOptimizedWinter, value);
        }

        /// <summary>
        ///     Metric Unit of Measurement: N2O m^-3 d^-1<br />
        ///     Imperial Unit of Measurement: N2O yd^-3 d^-1<br />
        /// </summary>
        [Units(MetricUnitsOfMeasurement.NitrousOxidePerCubicMeterPerDay)]
        public double NitrousOxideEmissionFactorForDigestateStorage
        {
            get => _nitrousOxideEmissionFactorForDigestateStorage;
            set => SetProperty(ref _nitrousOxideEmissionFactorForDigestateStorage, value);
        }

        /// <summary>
        ///     Metric Unit of Measurement: NH3 m^-3 d^-1<br />
        ///     Imperial Unit of Measurement: NH3 yd^-3 d^-1<br />
        /// </summary>
        [Units(MetricUnitsOfMeasurement.AmmoniaPerCubicMeterPerDay)]
        public double AmmoniaEmissionFactorForDigestateStorage
        {
            get => _ammoniaEmissionFactorForDigestateStorage;
            set => SetProperty(ref _ammoniaEmissionFactorForDigestateStorage, value);
        }

        public int NumberOfReactors
        {
            get => _numberOfReactors;
            set => SetProperty(ref _numberOfReactors, value);
        }

        /// <summary>
        ///     If false, digester is a belt press type
        /// </summary>
        public bool IsCentrifugeType => SeparatorType == AnaerobicDigestorSeparatorType.Centrifuge;

        public bool IsLiquidSolidSeparated
        {
            get => _isLiquidSolidSeparated;
            set => SetProperty(ref _isLiquidSolidSeparated, value);
        }

        public double VolumeOfDigestateEnteringStorage { get; set; }

        public ObservableCollection<ADManagementPeriodViewItem> ManagementPeriodViewItems
        {
            get => _managementPeriodViewItems;
            set => SetProperty(ref _managementPeriodViewItems, value);
        }

        public AnaerobicDigestorSeparatorType SeparatorType
        {
            get => _separatorType;
            set => SetProperty(ref _separatorType, value);
        }

        public bool UseImportedManure
        {
            get => _useImportedManure;
            set => SetProperty(ref _useImportedManure, value);
        }

        #endregion
    }
}