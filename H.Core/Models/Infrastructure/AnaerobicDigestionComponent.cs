using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.Infrastructure
{
    public class AnaerobicDigestionComponent : ComponentBase
    {
        #region Fields

        private AnaerobicDigestionViewItem _anaerobicDigestionViewItem;

        // Flow of biodegradable volatile solids
        private double _biodegradableFractionOfVSForDairyManure;
        private double _biodegradableFractionOfVSForSwineManure;
        private double _biodegradableFractionOfVSForGreenWaste;
        private double _biodegradableFractionOfVSForSOtherManure;

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

        private double _proportionTotalManureAddedToAD;
        private double _volumeOfDigestateEnteringStorage;

        private int _numberOfReactors;
        private bool _isCentrifugeType;
        private bool _isLiquidSolidSeparated;

        #endregion

        #region Constructors
        public AnaerobicDigestionComponent()
        {
            this.ComponentNameDisplayString = Properties.Resources.TitleAnaerobicDigestionComponent;
            this.ComponentDescriptionString = Properties.Resources.ToolTipAnaerobicDigestionComponent;
            this.ComponentCategory = ComponentCategory.Infrastructure;
            this.ComponentType = ComponentType.AnaerobicDigestion;

            _anaerobicDigestionViewItem = new AnaerobicDigestionViewItem();

        }
        #endregion

        #region Properties


        public AnaerobicDigestionViewItem AnaerobicDigestionViewItem
        {
            get => _anaerobicDigestionViewItem;
            set => this.SetProperty(ref _anaerobicDigestionViewItem, value);
        }

        public double BiodegradableFractionOfVSForDairyManure
        {
            get { return _biodegradableFractionOfVSForDairyManure; }
            set { this.SetProperty(ref _biodegradableFractionOfVSForDairyManure, value); }
        }

        public double BiodegradableFractionOfVSForSwineManure
        {
            get { return _biodegradableFractionOfVSForSwineManure; }
            set { this.SetProperty(ref _biodegradableFractionOfVSForSwineManure, value); }
        }

        public double BiodegradableFractionOfVSForGreenWaste
        {
            get { return _biodegradableFractionOfVSForGreenWaste; }
            set { this.SetProperty(ref _biodegradableFractionOfVSForGreenWaste, value); }
        }

        public double BiodegradableFractionOfVSForOtherManure
        {
            get { return _biodegradableFractionOfVSForSOtherManure; }
            set { this.SetProperty(ref _biodegradableFractionOfVSForSOtherManure, value); }
        }

        /// <summary>
        /// day^-1
        /// </summary>
        [Units(MetricUnitsOfMeasurement.PerDay)]
        public double HydrolysisRateOfManureDuringDigestion
        {
            get { return _hydrolysisRateOfManureDuringDigestion; }
            set { this.SetProperty(ref _hydrolysisRateOfManureDuringDigestion, value); }
        }

        /// <summary>
        /// day^-1
        /// </summary>
        [Units(MetricUnitsOfMeasurement.PerDay)]
        public double HydrolysisRateOfGreenWastesDuringDigestion
        {
            get { return _hydrolysisRateOfGreenWastesDuringDigestion; }
            set { this.SetProperty(ref _hydrolysisRateOfGreenWastesDuringDigestion, value); }
        }

        public double HydraulicRetentionTimeInDays
        {
            get { return _hydraulicRetentionTimeInDays; }
            set { this.SetProperty(ref _hydraulicRetentionTimeInDays, value); }
        }

        public double FractionOfMethaneInBiogas
        {
            get { return _fractionOfMethaneInBiogas; }
            set { this.SetProperty(ref _fractionOfMethaneInBiogas, value); }
        }

        /// <summary>
        /// Metric Unit of Measurement: Kg VS m^-3 d^-1<br/>
        /// Imperial Unit of Measurement: lb VS yd^-3 day^-1<br/>
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramVolatileSolidsPerCubicMeterPerDay)]
        public double OrganicLoadingRate
        {
            get { return _organicLoadingRate; }
            set { this.SetProperty(ref _organicLoadingRate, value); }
        }

        public double FractionOfFugitiveMethaneLosses
        {
            get { return _fractionOfFugitiveMethaneLosses; }
            set { this.SetProperty(ref _fractionOfFugitiveMethaneLosses, value); }
        }

        /// <summary>
        /// Metric Unit of Measurement: MJ Nm^-3 <br/>
        /// Imperial Unit of Measurement: BTU Sf^-3 <br/>
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaJulesPerNormalCubicMeters)]
        public double CalorificValueOfMethane
        {
            get { return _calorificValueOfMethane; }
            set { this.SetProperty(ref _calorificValueOfMethane, value); }
        }

        public double ConversionCoefficientKilowattHoursToMegajules
        {
            get { return _conversionCoefficientKilowattHoursToMegajules; }
            set { this.SetProperty(ref _conversionCoefficientKilowattHoursToMegajules, value); }
        }

        public double FractionPrimaryEnergyConvertedToElectricity
        {
            get { return _fractionPrimaryEnergyConvertedToElectricity; }
            set { this.SetProperty(ref _fractionPrimaryEnergyConvertedToElectricity, value); }
        }

        public double FractionPrimaryEnergyConvertedToHeat
        {
            get { return _fractionPrimaryEnergyConvertedToHeat; }
            set { this.SetProperty(ref _ammoniaEmissionFactorForDigestateStorage, value); }
        }

        public double FractionOfMethaneLostInUpgradingPlants
        {
            get { return _fractionOfMethaneLostInUpgradingPlants; }
            set { this.SetProperty(ref _fractionOfMethaneLostInUpgradingPlants, value); }
        }

        /// <summary>
        /// Metric Unit of Measurement: CH4 m^-3 d^-1<br/>
        /// Imperial Unit of Measurement: CH4 yd^-3 d^-1<br/>
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MethanePerCubicMeterPerDay)]
        public double MethaneEmissionFactorDigestateStorageWorstCase
        {
            get { return _methaneEmissionFactorDigestateStorageWorstCase; }
            set { this.SetProperty(ref _methaneEmissionFactorDigestateStorageWorstCase, value); }
        }

        /// <summary>
        /// Metric Unit of Measurement: CH4 m^-3 d^-1<br/>
        /// Imperial Unit of Measurement: CH4 yd^-3 d^-1<br/>
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MethanePerCubicMeterPerDay)]
        public double MethaneEmissionFactorDigestateStorageOptimizedFall
        {
            get { return _methaneEmissionFactorDigestateStorageOptimizedFall; }
            set { this.SetProperty(ref _methaneEmissionFactorDigestateStorageOptimizedFall, value); }
        }

        /// <summary>
        /// Metric Unit of Measurement: CH4 m^-3 d^-1<br/>
        /// Imperial Unit of Measurement: CH4 yd^-3 d^-1<br/>
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MethanePerCubicMeterPerDay)]
        public double MethaneEmissionFactorDigestateStorageOptimizedSpring
        {
            get { return _methaneEmissionFactorDigestateStorageOptimizedSpring; }
            set { this.SetProperty(ref _methaneEmissionFactorDigestateStorageOptimizedSpring, value); }
        }

        /// <summary>
        /// Metric Unit of Measurement: CH4 m^-3 d^-1<br/>
        /// Imperial Unit of Measurement: CH4 yd^-3 d^-1<br/>
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MethanePerCubicMeterPerDay)]
        public double MethaneEmissionFactorDigestateStorageOptimizedSummer
        {
            get { return _methaneEmissionFactorDigestateStorageOptimizedSummer; }
            set { this.SetProperty(ref _methaneEmissionFactorDigestateStorageOptimizedSummer, value); }
        }

        /// <summary>
        /// Metric Unit of Measurement: CH4 m^-3 d^-1<br/>
        /// Imperial Unit of Measurement: CH4 yd^-3 d^-1<br/>
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MethanePerCubicMeterPerDay)]
        public double MethaneEmissionFactorDigestateStorageOptimizedWinter
        {
            get { return _methaneEmissionFactorDigestateStorageOptimizedWinter; }
            set { this.SetProperty(ref _methaneEmissionFactorDigestateStorageOptimizedWinter, value); }
        }

        /// <summary>
        /// Metric Unit of Measurement: N2O m^-3 d^-1<br/>
        /// Imperial Unit of Measurement: N2O yd^-3 d^-1<br/>
        /// </summary>
        [Units(MetricUnitsOfMeasurement.NitrousOxidePerCubicMeterPerDay)]
        public double NitrousOxideEmissionFactorForDigestateStorage
        {
            get { return _nitrousOxideEmissionFactorForDigestateStorage; }
            set { this.SetProperty(ref _nitrousOxideEmissionFactorForDigestateStorage, value); }
        }

        /// <summary>
        /// Metric Unit of Measurement: NH3 m^-3 d^-1<br/>
        /// Imperial Unit of Measurement: NH3 yd^-3 d^-1<br/>
        /// </summary>
        [Units(MetricUnitsOfMeasurement.AmmoniaPerCubicMeterPerDay)]
        public double AmmoniaEmissionFactorForDigestateStorage
        {
            get { return _ammoniaEmissionFactorForDigestateStorage; }
            set { this.SetProperty(ref _ammoniaEmissionFactorForDigestateStorage, value); }
        }


        public double ProportionTotalManureAddedToAD
        {
            get { return _proportionTotalManureAddedToAD; }
            set { this.SetProperty(ref _proportionTotalManureAddedToAD, value); }
        }

        public int NumberOfReactors
        {
            get => _numberOfReactors;
            set => SetProperty(ref _numberOfReactors, value);
        }

        /// <summary>
        /// If false, digester is a belt press type
        /// </summary>
        public bool IsCentrifugeType
        {
            get => _isCentrifugeType;
            set => SetProperty(ref _isCentrifugeType, value);
        }

        public bool IsLiquidSolidSeparated
        {
            get => _isLiquidSolidSeparated;
            set => SetProperty(ref _isLiquidSolidSeparated, value);
        }

        public double VolumeOfDigestateEnteringStorage
        {
            get => _volumeOfDigestateEnteringStorage;
            set => SetProperty(ref _volumeOfDigestateEnteringStorage, value);
        }

        #endregion

        #region Public Methods

        public int GetHydrolicRetentionTime()
        {
            if (this.NumberOfReactors == 1)
            {
                return 25;
            }
            else
            {
                return 60;
            }
        }

        #endregion

        #region Private Methods
        #endregion
    }
}
