#region Imports

#endregion

using H.Core.Enumerations;
using H.Core.Properties;
using H.Infrastructure;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Runtime.Remoting.Channels;
using System.Windows.Navigation;

namespace H.Core.Calculators.UnitsOfMeasurement
{
    /// <summary>
    /// Unit Constants are coming from: https://www.flexaust.com/wp-content/uploads/2012/08/chart-metric-imp-conv.pdf
    /// Bushel Constants are coming from the following:
    /// https://farmlead.com/wp-content/uploads/2017/08/Conversion-Chart.pdf
    /// https://www.rayglen.com/crop-bushel-weights/
    /// </summary>
    public class UnitsOfMeasurementCalculator : BindableBase, IUnitsOfMeasurementCalculator
    {
        #region Constructors

        public UnitsOfMeasurementCalculator()
        {
            _isMetric = Settings.Default.MeasurementSystem == MeasurementSystemType.Metric;

            this.SetUnits();
        }

        #endregion

        #region Properties

        public bool IsMetric
        {
            get { return _isMetric; }
        }
        public string KilogramsPerHectareString
        {
            get { return _kilogramsPerHectareString; }
            set { this.SetProperty(ref _kilogramsPerHectareString, value); }
        }

        #endregion

        #region Fields
        private int roundingDigits = 4;
        private readonly bool _isMetric;
        private string _kilogramsPerHectareString;
        private const double YardsToMetersFactor = 0.9144;
        private const double KgToLbsFactor = 2.205;
        private const double GramsToGrainsFactor = 15.432;
        private const double HectaresToAcresFactor = 2.4711;
        private const double MmToInchesFactor = 0.0394;
        private const double CmToInchesFactor = 0.3937;
        private const double CubicMetersToCubicYardsFactor = 1.308;
        private const double JoulesPerBTU = 1055.056;
        private const double JoulesPerMJ = 1000000;
        private const double BTUperMCal = 3968.32072;
        private const double BTUperkWh = 3412.1416;
        private const double BTUperGJ = 947817.0777;
        private const double BTUperKcal = 0.2521644;
        private const double MegaJulesToBTUFactor = 947.817;

        private const double QuartsPerLitre = 0.8799;

        // reference -> https://www.nrcan.gc.ca/energy/international/nacei/18057
        private const double NormalCubicMeterToStandardCubicMeterFactor = 1.055;
        private const double StandardCubicMeterToStandardCubicFootFactor = 35.3147;
        private const double NormalCubicMeterToStandardCubicFootFactor = 37.257;

        //Crop Constants
        private const double AlfalfaBushelToPoundsFactor = 60;
        private const double BarleyBushelToPoundsFactor = 48;
        private const double CanolaBushelToPoundsFactor = 50;
        //need chickpeas, grouped with lentiles and peas
        private const double ChickPeasBushelToPoundsFactor = 60;

        private const double CornBushelToPoundsFactor = 56;
        private const double DryPeasBushelToPoundsFactor = 60;
        private const double PeasBushelToPoundsFactor = 60;
        //no fallow - field? 

        private const double FlaxBushelToPoundsFactor = 56;
        //no fodder corn, use normal corn
        private const double FodderCornBushelToPoundsFactor = 56;
        //no forage

        private const double SorghumBushelToPoundsFactor = 56;
        //no hay grass - use brome grass which is used for hay, silage, pasture or stockpiling
        private const double HayGrassBushelToPoundsFactor = 14;
        //no grain corn use normal corn
        private const double GrainCornBushelToPoundsFactor = 56;

        //no legume - these are beans, maybe use soybean
        private const double LegumeBushelToPoundsFactor = 60;

        //no hay mixed - use brome grass
        private const double HayMixedBushelToPoundsFactor = 14;

        private const double LentilsBushelToPoundsFactor = 60;
        private const double MustardBushelToPoundsFactor = 50;
        private const double OatsBushelToPoundsFactor = 34;
        //no oil seeds

        //no pasture use brome grass which is used for hay, silage, pasture or stockpiling
        private const double PastureBushelToPoundsFactor = 14;
        //no pulse crops,  Dried beans, chickpeas, lentils and peas are the most commonly known and consumed types of pulses.
        private const double PulseCropsBushelToPoundsFactor = 60;
        //no small grain cereals, Small grain cereals—wheat, barley, oat, rye, and triticale

        private const double SoyBeansBushelToPoundsFactor = 60;
        //triticale - grouped with corn,flax,rye
        private const double TriticaleBushelToPoundsFactor = 56;
        //no undersown barley,use normal barley
        private const double UnderSownBarleyBushelToPoundsFactor = 48;
        //no wheat bolinder, use normal wheat
        private const double WheatBolinderBushelToPoundsFactor = 60;
        //no wheat gan, use normal wheat
        private const double WheatGanBushelToPoundsFactor = 60;
        private const double WheatBushelToPoundsFactor = 60;

        // Tonne -> Ton factors
        private const double TonneToShortTonFactor = 1.102311;

        protected const double LbsPerKg = 2.205;
        protected const double KgPerTonne = 1000;
        protected const double KgPerHundredWeight = 45.359;
        protected const double AcresPerHectare = 2.4711;

        protected const double AlfalfaBushelPerTonne = 36.744;
        protected const double BarelyBushelPerTonne = 45.930;
        protected const double CanolaBushelPerTonne = 44.092;
        protected const double CornBushelPerTonne = 39.368;
        protected const double DryPeasBushelPerTonne = 36.74;
        protected const double FlaxBushelPerTonne = 39.368;
        protected const double FodderCornBushelPerTonne = 39.368;
        protected const double GrainCornBushelPerTonne = 39.368;
        protected const double HayGrassBushelPerTonne = 157.5;
        protected const double HayMixedBushelPerTonne = 157.5;
        protected const double LegumeBushelPerTonne = 36.744;
        protected const double LentilsBushelPerTonne = 36.744;
        protected const double MustardBushelPerTonne = 44.092;
        protected const double OatsBushelPerTonne = 64.842;
        protected const double PastureBushelPerTonne = 157.5;
        protected const double PeasBushelPerTonne = 36.74;
        protected const double PulseCropBushelPerTonne = 36.744;
        protected const double SorghumBushelPerTonne = 39.368;
        protected const double SoyBeansBushelPerTonne = 36.744;
        protected const double TriticaleBushelPerTonne = 39.368;
        protected const double UnderSownBarleyBushelPerTonne = 45.930;
        protected const double WheatBolinderBushelPerTonne = 36.744;
        protected const double WheatBushelPerTonne = 36.744;
        protected const double WheatGanBushelPerTonne = 36.744;

        #endregion

        #region Public Methods
        /// <summary>
        /// Based on the measurement type, returns the proper units as a string
        /// </summary>
        public string GetUnitsOfMeasurementString(MeasurementSystemType measurementSystemType,
                                                  MetricUnitsOfMeasurement unitsOfMeasurement)
        {
            switch (measurementSystemType)
            {
                case MeasurementSystemType.Imperial:
                    return this.WrapString(this.GetImperialUnitsOfMeasurementString(unitsOfMeasurement));

                default:
                    return this.WrapString(unitsOfMeasurement.GetDescription());
            }
        }

        /// <summary>
        /// Based on the measurement type, returns the proper value (metric or imperial) rounded to 4 decimal points
        /// Takes in Metric Units
        /// First argument corresponds to the user inputed measurement system (Metric or Imperial).
        /// The second argument is set to MetricUnitsOfMeasurement, you can change it to Imperial if needed, but there is no need
        /// 
        /// </summary>
        public double GetUnitsOfMeasurementValue(MeasurementSystemType measurementSystemType,
            MetricUnitsOfMeasurement unitsOfMeasurement, double value, bool exportedFromFarm)
        {
            switch (measurementSystemType)
            {
                case MeasurementSystemType.Imperial when exportedFromFarm:
                    {
                        //the view model has already converted everything we just need a rounded value now
                        return Math.Round(value, roundingDigits);
                    }
                case MeasurementSystemType.Imperial:
                    {
                        //we are being called from the CLI and therefore nee to be converted appropriatedly
                        return Math.Round(ConvertValueToImperialFromMetric(unitsOfMeasurement, value), roundingDigits);
                    }
                default:
                    return Math.Round(value, roundingDigits);
            }
        }

        /// <summary>
        /// Used to convert a GUI bound property into metric value 
        ///
        /// While in imperial mode Holos equations are always expecting metric regardless of what the GUI unit strings show.
        /// The user might think that he/she is entering the imperial values but Holos assumes metric all of the time.
        /// This method will convert from an imperial input into the metric equivalent so that we can hand that converted value
        /// to the equations.
        /// 
        /// </summary>
        /// <param name="measurementSystemType">the farms selected measurement system</param>
        /// <param name="unitsOfMeasurement">the metric units of the value</param>
        /// <param name="value">the value to conver</param>
        /// <returns>metric equivalent value</returns>
        public double GetMetricValueFromViewModels(MeasurementSystemType measurementSystemType,
            MetricUnitsOfMeasurement unitsOfMeasurement, double value)
        {
            switch (measurementSystemType)
            {
                case MeasurementSystemType.Imperial:
                    var imperialUnit = this.GetImperialUnitsOfMeasurement(unitsOfMeasurement);
                    return ConvertValueToMetricFromImperial(imperialUnit, value, unitsOfMeasurement);
                case MeasurementSystemType.Metric:
                    return value;
                default:
                    return value;
            }
        }

        /// <summary>
        /// This does the opposite of <see cref="GetMetricValueFromViewModels"/>.  When the Holos equation inputs change (if at all) we want to make sure that
        /// the GUI will also see those changes made. For use just with the VM's.
        /// </summary>
        /// <param name="measurementSystemType">the units of measurement that the farm is using</param>
        /// <param name="unitsOfMeasurement">metric units associated with the value</param>
        /// <param name="value">the value to convert</param>
        /// <returns>An imperial value if under imperial rule metric otherwise</returns>
        public double GetUnitValueFromHolos(MeasurementSystemType measurementSystemType,
            MetricUnitsOfMeasurement unitsOfMeasurement, double value)
        {
            switch (measurementSystemType)
            {
                case MeasurementSystemType.Imperial:
                    return this.ConvertValueToImperialFromMetric(unitsOfMeasurement, value);
                case MeasurementSystemType.Metric:
                    return value;
                default:
                    return value;
            }
        }

        public double ConvertCropTypeValue(MeasurementSystemType measurementSystemType, CropType crop, double value)
        {
            switch (measurementSystemType)
            {
                case MeasurementSystemType.Imperial:
                    return ConvertKilogramsPerHectareToImperialBushelPerAcresBasedOnCropType(crop, value);
                default:
                    return Math.Round(value, roundingDigits);

            }
        }

        public double ConvertKilogramsPerHectareToImperialBushelPerAcresBasedOnCropType(CropType crop, double value)
        {
            switch (crop)
            {
                case CropType.Barley:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * BarleyBushelToPoundsFactor);
                case CropType.Canola:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * CanolaBushelToPoundsFactor);
                case CropType.Chickpeas:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * ChickPeasBushelToPoundsFactor);
                case CropType.Corn:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * CornBushelToPoundsFactor);
                case CropType.DryPeas:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * DryPeasBushelToPoundsFactor);
                case CropType.Flax:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * FlaxBushelToPoundsFactor);
                case CropType.FodderCorn:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * FodderCornBushelToPoundsFactor);
                case CropType.GrainCorn:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * GrainCornBushelToPoundsFactor);
                case CropType.GrainSorghum:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * SorghumBushelToPoundsFactor);
                case CropType.TameGrass:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * HayGrassBushelToPoundsFactor);
                case CropType.TameLegume:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * LegumeBushelToPoundsFactor);
                case CropType.TameMixed:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * HayMixedBushelToPoundsFactor);
                case CropType.Lentils:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * LentilsBushelToPoundsFactor);
                case CropType.Mustard:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * MustardBushelToPoundsFactor);
                case CropType.Oats:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * OatsBushelToPoundsFactor);
                case CropType.Soybeans:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * SoyBeansBushelToPoundsFactor);
                case CropType.Triticale:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * TriticaleBushelToPoundsFactor);
                case CropType.UndersownBarley:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * UnderSownBarleyBushelToPoundsFactor);
                case CropType.WheatGan:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * WheatGanBushelToPoundsFactor);
                case CropType.WheatBolinder:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * WheatBolinderBushelToPoundsFactor);
                case CropType.Wheat:
                    return value * KgToLbsFactor / (HectaresToAcresFactor * WheatBushelToPoundsFactor);
                default:
                    return 0;
            }
        }
        public double ConvertBushelsPerAcreToMetricKilogramsPerHectareBasedOnCropType(CropType crop, double value)
        {
            switch (crop)
            {
                case CropType.Barley:
                case CropType.FeedBarley:
                case CropType.MaltBarley:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * BarleyBushelToPoundsFactor);
                case CropType.Canola:
                case CropType.Camelina:
                case CropType.PolishCanola:
                case CropType.ArgentineHTCanola:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * CanolaBushelToPoundsFactor);
                case CropType.Chickpeas:
                case CropType.Peas:
                case CropType.FieldPeas:
                case CropType.RedLentils:
                case CropType.DesiChickpeas:
                case CropType.KabuliChickpea:
                case CropType.EdibleGreenPeas:
                case CropType.EdibleYellowPeas:
                case CropType.LargeGreenLentils:
                case CropType.SmallKabuliChickpea:
                case CropType.LargeKabuliChickpea:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * ChickPeasBushelToPoundsFactor);
                case CropType.Corn:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * CornBushelToPoundsFactor);
                case CropType.DryPeas:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * DryPeasBushelToPoundsFactor);
                case CropType.Flax:
                case CropType.FlaxSeed:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * FlaxBushelToPoundsFactor);
                case CropType.FodderCorn:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * FodderCornBushelToPoundsFactor);
                case CropType.GrainCorn:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * GrainCornBushelToPoundsFactor);
                case CropType.GrainSorghum:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * SorghumBushelToPoundsFactor);
                case CropType.TameGrass:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * HayGrassBushelToPoundsFactor);
                case CropType.TameLegume:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * LegumeBushelToPoundsFactor);
                case CropType.TameMixed:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * HayMixedBushelToPoundsFactor);
                case CropType.Lentils:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * LentilsBushelToPoundsFactor);
                case CropType.Mustard:
                case CropType.BrownMustard:
                case CropType.YellowMustard:
                case CropType.OrientalMustard:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * MustardBushelToPoundsFactor);
                case CropType.Oats:
                case CropType.MillingOats:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * OatsBushelToPoundsFactor);
                case CropType.Soybeans:
                case CropType.DryBean:
                case CropType.BeansPinto:
                case CropType.BeansWhite:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * SoyBeansBushelToPoundsFactor);
                case CropType.Triticale:
                case CropType.HybridFallRye:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * TriticaleBushelToPoundsFactor);
                case CropType.UndersownBarley:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * UnderSownBarleyBushelToPoundsFactor);
                case CropType.WheatGan:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * WheatGanBushelToPoundsFactor);
                case CropType.WheatBolinder:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * WheatBolinderBushelToPoundsFactor);
                case CropType.Wheat:
                case CropType.Durum:
                case CropType.CPSWheat:
                case CropType.SoftWheat:
                case CropType.SpringWheat:
                case CropType.WinterWheat:
                case CropType.WheatOtherSpring:
                case CropType.WheatHardRedSpring:
                case CropType.HardRedSpringWheat:
                case CropType.WheatPrairieSpring:
                case CropType.WheatNorthernHardRed:
                    return value / KgToLbsFactor * (HectaresToAcresFactor * WheatBushelToPoundsFactor);
                default:
                    return 0;
            }

        }

        public double ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement metricUnit, double value)
        {
            switch (metricUnit)
            {
                case MetricUnitsOfMeasurement.Kilograms:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.Grams:
                    return value * GramsToGrainsFactor;

                case MetricUnitsOfMeasurement.Hectares:
                    return value * HectaresToAcresFactor;

                case MetricUnitsOfMeasurement.Millimeters:
                    return value * MmToInchesFactor;

                case MetricUnitsOfMeasurement.Centimeters:
                    return value * CmToInchesFactor;

                case MetricUnitsOfMeasurement.Meters:
                    return value / YardsToMetersFactor;

                case MetricUnitsOfMeasurement.MillimetersPerYear:
                    return value * MmToInchesFactor;

                case MetricUnitsOfMeasurement.DegreesCelsius:
                    return (value * 9.0 / 5.0) + 32.0;

                case MetricUnitsOfMeasurement.MegaJoulesPerKilogram:
                    return value / KgToLbsFactor * JoulesPerMJ / JoulesPerBTU;

                case MetricUnitsOfMeasurement.MilligramsPerKilogram:
                    return value * (GramsToGrainsFactor / 1000.0) / KgToLbsFactor;

                case MetricUnitsOfMeasurement.InternationalUnitsPerGram:
                    return value / GramsToGrainsFactor;

                //This is unit-less, we only include these units so the reader can understand that
                //we are representing the value as a percent weight rather than percent volume
                case MetricUnitsOfMeasurement.KilogramPerKilogram:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.KilogramsPerHectare:
                    return value * KgToLbsFactor / HectaresToAcresFactor;

                case MetricUnitsOfMeasurement.KilogramPerHeadPerYear:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.KilogramPerHeadPerDay:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.KilogramsN2ONPerKilogramN:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.KilogramsN2ONPerYear:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.KilogramsN2OPerYear:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.KilogramsN2ON:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare:
                    return value * KgToLbsFactor / HectaresToAcresFactor;

                case MetricUnitsOfMeasurement.KilogramsPhosphorousPerHectare:
                    return value * KgToLbsFactor / HectaresToAcresFactor;

                case MetricUnitsOfMeasurement.KilogramsPotassiumPerHectare:
                    return value * KgToLbsFactor / HectaresToAcresFactor;

                case MetricUnitsOfMeasurement.KilogramsSulphurPerHectare:
                    return value * KgToLbsFactor / HectaresToAcresFactor;

                case MetricUnitsOfMeasurement.CubicMetersMethanePerKilogramVolatileSolids:
                    return value * CubicMetersToCubicYardsFactor / KgToLbsFactor;

                case MetricUnitsOfMeasurement.KilogramsMethanePerKilogramMethane:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.KilogramsMethanePerYear:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.KilogramsMethane:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.MegaJoulesPerDayPerKilogram:
                    return value / KgToLbsFactor * JoulesPerMJ / JoulesPerBTU;

                case MetricUnitsOfMeasurement.MegaJoulesPerKilogramSquared:
                    return value / Math.Pow(KgToLbsFactor, 2) * JoulesPerMJ / JoulesPerBTU;

                case MetricUnitsOfMeasurement.KilogramsNitrogenPerKilogram:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.KilogramsNitrogen:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.KilogramsPerKilogramProteinIntake:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.KilogramsVolatileSolidsPerKilogramFeed:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.GigajoulesPerHectare:
                    return value / HectaresToAcresFactor * 1000 * JoulesPerMJ / JoulesPerBTU;

                case MetricUnitsOfMeasurement.KilogramsPerDay:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.KilogramsPerYear:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.KilogramsCarbonPerHectare:
                    return value * KgToLbsFactor / HectaresToAcresFactor;

                case MetricUnitsOfMeasurement.KilogramsCarbonPerTree:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.KilogramsCarbonPerPlanting:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.KilogramsCarbonDioxidePerShelterbelt:
                    return value * KgToLbsFactor;

                case MetricUnitsOfMeasurement.MegaCalorie:
                    return value * BTUperMCal;

                case MetricUnitsOfMeasurement.KilogramsCO2PerKiloWattHour:
                    return value * KgToLbsFactor / BTUperkWh;

                case MetricUnitsOfMeasurement.GigaJoulesPer1000Litres:
                    return value * BTUperGJ / QuartsPerLitre;

                case MetricUnitsOfMeasurement.KilogramsCO2PerGigaJoule:
                    return value / BTUperGJ * LbsPerKg;

                case MetricUnitsOfMeasurement.KiloWattHourPerAnimal:
                case MetricUnitsOfMeasurement.KilowattHourPerPoultryPlacement:
                    return value * BTUperkWh;

                case MetricUnitsOfMeasurement.MegaCaloriePerKilogram:
                    return value * BTUperMCal / KgToLbsFactor;

                case MetricUnitsOfMeasurement.KiloCaloriePerKilogram:
                    return value / 1000 * BTUperMCal / KgToLbsFactor;

                case MetricUnitsOfMeasurement.Tonne:
                case MetricUnitsOfMeasurement.MetricTon:
                    // Reference for tonne conversions -> https://www.onlineconversion.com/faq_09.htm
                    return value * TonneToShortTonFactor;

                case MetricUnitsOfMeasurement.MegaJulesPerNormalCubicMeters:
                    // Returning value is BTUPerStandardCubicFoot. Conversion is done in two steps.
                    // Convert Megajules -> BTU
                    // Convert Normal Cubic Meter -> Standard Cubic Foot
                    return (value * MegaJulesToBTUFactor) / NormalCubicMeterToStandardCubicFootFactor;

                case MetricUnitsOfMeasurement.KilogramVolatileSolidsPerCubicMeterPerDay:
                    // Returning value is PoundVolatileSolidsPerCubicYardPerDay
                    // Convert KG -> pounds
                    // Convert Cubic meter -> Cubic yard
                    return (value * KgToLbsFactor) / CubicMetersToCubicYardsFactor;

                case MetricUnitsOfMeasurement.MethanePerCubicMeterPerDay:
                    // Returning value is MethanePerCubicYardPerDay
                    return value * CubicMetersToCubicYardsFactor;

                case MetricUnitsOfMeasurement.NitrousOxidePerCubicMeterPerDay:
                    // Returning value is NitrousOxidePerCubicYardPerDay
                    return value * CubicMetersToCubicYardsFactor;

                case MetricUnitsOfMeasurement.AmmoniaPerCubicMeterPerDay:
                    return value * CubicMetersToCubicYardsFactor;

                case MetricUnitsOfMeasurement.DollarsPerTonne:
                    return value * TonneToShortTonFactor;

                case MetricUnitsOfMeasurement.DollarsPerKilogram:
                    // returns DollarPerLB
                    return value * KgToLbsFactor;
                default:
                    //days, months, years, percentages, Co2Equivalents, perday unit
                    return value;
            }
        }

        /// <summary>
        /// Convert imperial values to metric
        /// </summary>
        /// <param name="imperialUnit">the unit of the value</param>
        /// <param name="value">the value</param>
        /// <param name="originalUnit">the expected unit. Used for cases that like kcal/kg and Mcal/kg where they both use BTU/lb for imperial conversions</param>
        /// <returns>metric value</returns>
        public double ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement? imperialUnit, double value, MetricUnitsOfMeasurement originalUnit = MetricUnitsOfMeasurement.None)
        {
            switch (imperialUnit)
            {
                case ImperialUnitsOfMeasurement.Pounds:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.Grains:
                    return value / GramsToGrainsFactor;

                case ImperialUnitsOfMeasurement.Acres:
                    return value / HectaresToAcresFactor;

                case ImperialUnitsOfMeasurement.Yards:
                    return value / (1 / YardsToMetersFactor);

                case ImperialUnitsOfMeasurement.InchesToMm:
                    return value / MmToInchesFactor;

                case ImperialUnitsOfMeasurement.InchesToCm:
                    return value / CmToInchesFactor;

                case ImperialUnitsOfMeasurement.InchesPerYear:
                    return value / MmToInchesFactor;

                case ImperialUnitsOfMeasurement.DegreesFahrenheit:
                    return (value - 32.0) * 5.0 / 9.0;

                case ImperialUnitsOfMeasurement.BritishThermalUnitPerPound:
                    switch (originalUnit)
                    {
                        //metric has multiple units for energy Mcal/kg, kcal/kg, MJ/kg, etc. but they all get converted to BTU/lb
                        //need to convert them from BTU/lb to the expected unit for that value
                        case MetricUnitsOfMeasurement.KiloCaloriePerKilogram:
                            return value * KgToLbsFactor * BTUperKcal;
                        case MetricUnitsOfMeasurement.MegaJoulesPerKilogram:
                            return value * KgToLbsFactor * JoulesPerBTU / JoulesPerMJ;
                        case MetricUnitsOfMeasurement.MegaCaloriePerKilogram:
                            return value * KgToLbsFactor / BTUperMCal;
                        default:
                            throw new ArgumentException(
                                $"{originalUnit} was not handled correctly. Need specific units when dealing with conversion of {ImperialUnitsOfMeasurement.BritishThermalUnitPerPound}");
                    }

                case ImperialUnitsOfMeasurement.GrainsPerPound:
                    return value / (GramsToGrainsFactor / 1000.0) * KgToLbsFactor;

                case ImperialUnitsOfMeasurement.InternationalUnitsPerGrain:
                    return value * GramsToGrainsFactor;

                //This is unit-less, we only include these units so the reader can understand that
                //we are representing the value as a percent weight rather than percent volume
                case ImperialUnitsOfMeasurement.PoundsPerPound:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.PoundsPerAcre:
                    return value / KgToLbsFactor * HectaresToAcresFactor;

                case ImperialUnitsOfMeasurement.PoundPerHeadPerYear:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.PoundPerHeadPerDay:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.PoundsN2ONPerPoundN:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.PoundsN2ONPerYear:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.PoundsN2OPerYear:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.PoundsN2ON:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.PoundsNitrogenPerAcre:
                    return value / KgToLbsFactor * HectaresToAcresFactor;

                case ImperialUnitsOfMeasurement.PoundsPhosphorousPerAcre:
                    return value / KgToLbsFactor * HectaresToAcresFactor;

                case ImperialUnitsOfMeasurement.CubicYardsMethanePerPoundVolatileSolids:
                    return value / CubicMetersToCubicYardsFactor * KgToLbsFactor;

                case ImperialUnitsOfMeasurement.PoundsMethanePerPoundMethane:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.PoundsMethanePerYear:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.PoundsMethane:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.BritishThermalUnitPerDayPerPound:
                    return value * KgToLbsFactor * JoulesPerBTU / JoulesPerMJ;

                case ImperialUnitsOfMeasurement.BritishThermalUnitPerPoundSquared:
                    return value * Math.Pow(KgToLbsFactor, 2) * JoulesPerBTU / JoulesPerMJ;

                case ImperialUnitsOfMeasurement.PoundsNitrogenPerPound:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.PoundsNitrogen:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.PoundsPerPoundProteinIntake:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.PoundsVolatileSolidsPerPoundFeed:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.BritishThermalUnitsPerAcre:
                    return value * HectaresToAcresFactor * JoulesPerBTU / JoulesPerMJ;

                case ImperialUnitsOfMeasurement.PoundsPerDay:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.PoundsPerYear:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.PoundsCarbonPerAcre:
                    return value / KgToLbsFactor * HectaresToAcresFactor;

                case ImperialUnitsOfMeasurement.PoundsCarbonPerTree:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.PoundsCarbonPerPlanting:
                    return value / KgToLbsFactor;

                case ImperialUnitsOfMeasurement.PoundsCarbonDioxidePerShelterbelt:
                    return value / KgToLbsFactor;
                
                case ImperialUnitsOfMeasurement.Ton:
                case ImperialUnitsOfMeasurement.ShortTon:
                    // https://www.onlineconversion.com/faq_09.htm
                    return value / TonneToShortTonFactor;

                case ImperialUnitsOfMeasurement.BTUPerStandardCubicFoot:
                    // Returning value is MegaJulesPerNormalCubicMeters. Conversion is done in two steps.
                    // Convert BTU -> MegaJules
                    // Convert Standard Cubic Foot -> Normal Cubic Meter
                    return (value / MegaJulesToBTUFactor) * StandardCubicMeterToStandardCubicFootFactor;

                case ImperialUnitsOfMeasurement.PoundVolatileSolidsPerCubicYardPerDay:
                    return (value / KgToLbsFactor) * CubicMetersToCubicYardsFactor;

                case ImperialUnitsOfMeasurement.MethanePerCubicYardPerDay:
                    // Returning value is MethanePerCubicMetersPerDay
                    return value / CubicMetersToCubicYardsFactor;

                case ImperialUnitsOfMeasurement.NitrousOxidePerCubicYardPerDay:
                    // Return NitrousOxidePerCubicMeterPerDay
                    return value / CubicMetersToCubicYardsFactor;

                case ImperialUnitsOfMeasurement.AmmoniaPerCubicYardPerDay:
                    // Return AmmoniaPerCubicMeterPerDay
                    return value / CubicMetersToCubicYardsFactor;

                case ImperialUnitsOfMeasurement.DollarsPerTon:
                    // returns DollarsPerTonne
                    return value / TonneToShortTonFactor;

                case ImperialUnitsOfMeasurement.DollarsPerPound:
                    // returns DollarPerKG
                    return value / KgToLbsFactor;

                default:
                    //days, months, years, percentages, Co2Equivalents, perday unit
                    return value;
            }
        }

        public string GetImperialUnitsOfMeasurementString(MetricUnitsOfMeasurement unitsOfMeasurement)
        {
            return this.GetImperialUnitsOfMeasurement(unitsOfMeasurement).GetDescription();
        }

        public string GetMetricUnitsOfMeasurementString(ImperialUnitsOfMeasurement unitsOfMeasurement)
        {
            return this.GetMetricUnitsOfMeasurement(unitsOfMeasurement).GetDescription();
        }

        public ImperialUnitsOfMeasurement GetImperialUnitsOfMeasurement(MetricUnitsOfMeasurement unitsOfMeasurement)
        {
            switch (unitsOfMeasurement)
            {
                case MetricUnitsOfMeasurement.Millimeters:
                    return ImperialUnitsOfMeasurement.InchesToMm;

                case MetricUnitsOfMeasurement.Kilograms:
                    return ImperialUnitsOfMeasurement.Pounds;

                case MetricUnitsOfMeasurement.Grams:
                    return ImperialUnitsOfMeasurement.Grains;

                case MetricUnitsOfMeasurement.Hectares:
                    return ImperialUnitsOfMeasurement.Acres;

                case MetricUnitsOfMeasurement.Meters:
                    return ImperialUnitsOfMeasurement.Yards;

                case MetricUnitsOfMeasurement.Centimeters:
                    return ImperialUnitsOfMeasurement.InchesToCm;

                case MetricUnitsOfMeasurement.MillimetersPerYear:
                    return ImperialUnitsOfMeasurement.InchesPerYear;

                case MetricUnitsOfMeasurement.DegreesCelsius:
                    return ImperialUnitsOfMeasurement.DegreesFahrenheit;

                case MetricUnitsOfMeasurement.MegaJoulesPerKilogram:
                    return ImperialUnitsOfMeasurement.BritishThermalUnitPerPound;

                case MetricUnitsOfMeasurement.GramsPerKilogram:
                    return ImperialUnitsOfMeasurement.GrainsPerPound;

                case MetricUnitsOfMeasurement.KilogramsPerHectareCrop:
                    return ImperialUnitsOfMeasurement.BushelsPerAcre;

                case MetricUnitsOfMeasurement.KilogramsPerHectare:
                    return ImperialUnitsOfMeasurement.PoundsPerAcre;

                case MetricUnitsOfMeasurement.InternationalUnitsPerGram:
                    return ImperialUnitsOfMeasurement.InternationalUnitsPerGrain;

                case MetricUnitsOfMeasurement.KilogramPerKilogram:
                    return ImperialUnitsOfMeasurement.PoundsPerPound;

                case MetricUnitsOfMeasurement.KilogramPerHeadPerYear:
                    return ImperialUnitsOfMeasurement.PoundPerHeadPerYear;

                case MetricUnitsOfMeasurement.KilogramPerHeadPerDay:
                    return ImperialUnitsOfMeasurement.PoundPerHeadPerDay;

                case MetricUnitsOfMeasurement.KilogramsN2ONPerKilogramN:
                    return ImperialUnitsOfMeasurement.PoundsN2ONPerPoundN;

                case MetricUnitsOfMeasurement.KilogramsN2ONPerYear:
                    return ImperialUnitsOfMeasurement.PoundsN2ONPerYear;

                case MetricUnitsOfMeasurement.KilogramsN2OPerYear:
                    return ImperialUnitsOfMeasurement.PoundsN2OPerYear;

                case MetricUnitsOfMeasurement.KilogramsN2ON:
                    return ImperialUnitsOfMeasurement.PoundsN2ON;

                case MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare:
                    return ImperialUnitsOfMeasurement.PoundsNitrogenPerAcre;

                case MetricUnitsOfMeasurement.KilogramsNitrogenPerHectarePerYear:
                    return ImperialUnitsOfMeasurement.PoundsNitrogenPerAcrePerYear;

                case MetricUnitsOfMeasurement.KilogramsPhosphorousPerHectare:
                    return ImperialUnitsOfMeasurement.PoundsPhosphorousPerAcre;

                case MetricUnitsOfMeasurement.CubicMetersMethanePerKilogramVolatileSolids:
                    return ImperialUnitsOfMeasurement.CubicYardsMethanePerPoundVolatileSolids;

                case MetricUnitsOfMeasurement.KilogramsMethanePerKilogramMethane:
                    return ImperialUnitsOfMeasurement.PoundsMethanePerPoundMethane;

                case MetricUnitsOfMeasurement.KilogramsMethanePerYear:
                    return ImperialUnitsOfMeasurement.PoundsMethanePerYear;

                case MetricUnitsOfMeasurement.KilogramsMethane:
                    return ImperialUnitsOfMeasurement.PoundsMethane;

                case MetricUnitsOfMeasurement.MegaJoulesPerDayPerKilogram:
                    return ImperialUnitsOfMeasurement.BritishThermalUnitPerDayPerPound;

                case MetricUnitsOfMeasurement.MegaJoulesPerKilogramSquared:
                    return ImperialUnitsOfMeasurement.BritishThermalUnitPerPoundSquared;

                case MetricUnitsOfMeasurement.KilogramsNitrogenPerKilogram:
                    return ImperialUnitsOfMeasurement.PoundsNitrogenPerPound;

                case MetricUnitsOfMeasurement.KilogramsNitrogen:
                    return ImperialUnitsOfMeasurement.PoundsNitrogen;

                case MetricUnitsOfMeasurement.KilogramsPerKilogramProteinIntake:
                    return ImperialUnitsOfMeasurement.PoundsPerPoundProteinIntake;

                case MetricUnitsOfMeasurement.KilogramsVolatileSolidsPerKilogramFeed:
                    return ImperialUnitsOfMeasurement.PoundsVolatileSolidsPerPoundFeed;

                case MetricUnitsOfMeasurement.GigajoulesPerHectare:
                    return ImperialUnitsOfMeasurement.BritishThermalUnitsPerAcre;

                case MetricUnitsOfMeasurement.KilogramsPerDay:
                    return ImperialUnitsOfMeasurement.PoundsPerDay;

                case MetricUnitsOfMeasurement.KilogramsPerYear:
                    return ImperialUnitsOfMeasurement.PoundsPerYear;

                case MetricUnitsOfMeasurement.KilogramsCarbonPerHectare:
                    return ImperialUnitsOfMeasurement.PoundsCarbonPerAcre;

                case MetricUnitsOfMeasurement.KilogramsCarbonPerTree:
                    return ImperialUnitsOfMeasurement.PoundsCarbonPerTree;

                case MetricUnitsOfMeasurement.KilogramsCarbonPerPlanting:
                    return ImperialUnitsOfMeasurement.PoundsCarbonPerPlanting;

                case MetricUnitsOfMeasurement.KilogramsCarbonDioxidePerShelterbelt:
                    return ImperialUnitsOfMeasurement.PoundsCarbonDioxidePerShelterbelt;

                case MetricUnitsOfMeasurement.PercentageAF:
                    return ImperialUnitsOfMeasurement.PercentageAF;

                case MetricUnitsOfMeasurement.PercentageCrudeProtein:
                    return ImperialUnitsOfMeasurement.PercentageCrudeProtein;

                case MetricUnitsOfMeasurement.PercentageDryMatter:
                    return ImperialUnitsOfMeasurement.PercentageDryMatter;

                case MetricUnitsOfMeasurement.PercentageH:
                    return ImperialUnitsOfMeasurement.PercentageH;

                case MetricUnitsOfMeasurement.PercentageNdf:
                    return ImperialUnitsOfMeasurement.PercentageNdf;

                case MetricUnitsOfMeasurement.Percentage:
                    return ImperialUnitsOfMeasurement.Percentage;

                case MetricUnitsOfMeasurement.Days:
                    return ImperialUnitsOfMeasurement.Days;

                case MetricUnitsOfMeasurement.Months:
                    return ImperialUnitsOfMeasurement.Months;

                case MetricUnitsOfMeasurement.Years:
                    return ImperialUnitsOfMeasurement.Years;

                case MetricUnitsOfMeasurement.KilogramsN2ONPerHectare:
                    return ImperialUnitsOfMeasurement.PoundsN2ONPerAcre;

                case MetricUnitsOfMeasurement.KilogramsNONPerHectare:
                    return ImperialUnitsOfMeasurement.PoundsNONPerAcre;

                case MetricUnitsOfMeasurement.KilogramsNO3NPerHectare:
                    return ImperialUnitsOfMeasurement.PoundsNO3NPerAcre;

                case MetricUnitsOfMeasurement.KilogramsNH4NPerHectare:
                    return ImperialUnitsOfMeasurement.PoundsNH4NPerAcre;

                case MetricUnitsOfMeasurement.KilogramsN2NPerHectare:
                    return ImperialUnitsOfMeasurement.PoundsN2NPerAcre;

                case MetricUnitsOfMeasurement.KilogramsCO2PerKiloWattHour:
                case MetricUnitsOfMeasurement.KilogramsCO2PerGigaJoule:
                    return ImperialUnitsOfMeasurement.PoundsCO2PerBTU;

                case MetricUnitsOfMeasurement.KiloWattHourPerAnimal:
                    return ImperialUnitsOfMeasurement.BTUPerAnimal;

                case MetricUnitsOfMeasurement.KilowattHourPerPoultryPlacement:
                    return ImperialUnitsOfMeasurement.BTUPerPoultryPlacement;

                case MetricUnitsOfMeasurement.GigaJoulesPer1000Litres:
                    return ImperialUnitsOfMeasurement.BTUPer1000Quarts;

                case MetricUnitsOfMeasurement.KiloCaloriePerKilogram:
                    return ImperialUnitsOfMeasurement.BritishThermalUnitPerPound;

                case MetricUnitsOfMeasurement.MegaCaloriePerKilogram:
                    return ImperialUnitsOfMeasurement.BritishThermalUnitPerPound;

                case MetricUnitsOfMeasurement.Tonne:
                case MetricUnitsOfMeasurement.MetricTon:
                    return ImperialUnitsOfMeasurement.Ton;

                case MetricUnitsOfMeasurement.MegaJulesPerNormalCubicMeters:
                    return ImperialUnitsOfMeasurement.BTUPerStandardCubicFoot;

                case MetricUnitsOfMeasurement.KilogramVolatileSolidsPerCubicMeterPerDay:
                    return ImperialUnitsOfMeasurement.PoundVolatileSolidsPerCubicYardPerDay;

                case MetricUnitsOfMeasurement.MethanePerCubicMeterPerDay:
                    return ImperialUnitsOfMeasurement.MethanePerCubicYardPerDay;

                case MetricUnitsOfMeasurement.NitrousOxidePerCubicMeterPerDay:
                    return ImperialUnitsOfMeasurement.NitrousOxidePerCubicYardPerDay;

                case MetricUnitsOfMeasurement.AmmoniaPerCubicMeterPerDay:
                    return ImperialUnitsOfMeasurement.AmmoniaPerCubicYardPerDay;

                default:
                    throw new Exception($"{unitsOfMeasurement} is an Invalid Unit Of Measurement");
            }
        }

        public MetricUnitsOfMeasurement GetMetricUnitsOfMeasurement(ImperialUnitsOfMeasurement unitsOfMeasurement)
        {
            switch (unitsOfMeasurement)
            {
                case ImperialUnitsOfMeasurement.Pounds:
                    return MetricUnitsOfMeasurement.Kilograms;

                case ImperialUnitsOfMeasurement.Grains:
                    return MetricUnitsOfMeasurement.Grams;

                case ImperialUnitsOfMeasurement.Acres:
                    return MetricUnitsOfMeasurement.Hectares;

                case ImperialUnitsOfMeasurement.Yards:
                    return MetricUnitsOfMeasurement.Meters;

                case ImperialUnitsOfMeasurement.InchesToMm:
                    return MetricUnitsOfMeasurement.Millimeters;

                case ImperialUnitsOfMeasurement.InchesToCm:
                    return MetricUnitsOfMeasurement.Centimeters;

                case ImperialUnitsOfMeasurement.InchesPerYear:
                    return MetricUnitsOfMeasurement.MillimetersPerYear;

                case ImperialUnitsOfMeasurement.DegreesFahrenheit:
                    return MetricUnitsOfMeasurement.DegreesCelsius;

                case ImperialUnitsOfMeasurement.BritishThermalUnitPerPound:
                    return MetricUnitsOfMeasurement.MegaJoulesPerKilogram;

                case ImperialUnitsOfMeasurement.GrainsPerPound:
                    return MetricUnitsOfMeasurement.GramsPerKilogram;

                case ImperialUnitsOfMeasurement.BushelsPerAcre:
                    return MetricUnitsOfMeasurement.KilogramsPerHectareCrop;

                case ImperialUnitsOfMeasurement.PoundsPerAcre:
                    return MetricUnitsOfMeasurement.KilogramsPerHectare;

                case ImperialUnitsOfMeasurement.InternationalUnitsPerGrain:
                    return MetricUnitsOfMeasurement.InternationalUnitsPerGram;

                //This is unit-less, we only include these units so the reader can understand that
                //we are representing the value as a percent weight rather than percent volume
                case ImperialUnitsOfMeasurement.PoundsPerPound:
                    return MetricUnitsOfMeasurement.KilogramPerKilogram;

                case ImperialUnitsOfMeasurement.PoundPerHeadPerYear:
                    return MetricUnitsOfMeasurement.KilogramPerHeadPerYear;

                case ImperialUnitsOfMeasurement.PoundPerHeadPerDay:
                    return MetricUnitsOfMeasurement.KilogramPerHeadPerDay;

                case ImperialUnitsOfMeasurement.PoundsN2ONPerPoundN:
                    return MetricUnitsOfMeasurement.KilogramsN2ONPerKilogramN;

                case ImperialUnitsOfMeasurement.PoundsN2ONPerYear:
                    return MetricUnitsOfMeasurement.KilogramsN2ONPerYear;

                case ImperialUnitsOfMeasurement.PoundsN2OPerYear:
                    return MetricUnitsOfMeasurement.KilogramsN2OPerYear;

                case ImperialUnitsOfMeasurement.PoundsN2ON:
                    return MetricUnitsOfMeasurement.KilogramsN2ON;

                case ImperialUnitsOfMeasurement.PoundsNitrogenPerAcre:
                    return MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare;

                case ImperialUnitsOfMeasurement.PoundsNitrogenPerAcrePerYear:
                    return MetricUnitsOfMeasurement.KilogramsNitrogenPerHectarePerYear;

                case ImperialUnitsOfMeasurement.PoundsNitrogen:
                    return MetricUnitsOfMeasurement.KilogramsNitrogen;

                case ImperialUnitsOfMeasurement.PoundsPhosphorousPerAcre:
                    return MetricUnitsOfMeasurement.KilogramsPhosphorousPerHectare;

                case ImperialUnitsOfMeasurement.CubicYardsMethanePerPoundVolatileSolids:
                    return MetricUnitsOfMeasurement.CubicMetersMethanePerKilogramVolatileSolids;

                case ImperialUnitsOfMeasurement.PoundsMethanePerPoundMethane:
                    return MetricUnitsOfMeasurement.KilogramsMethanePerKilogramMethane;

                case ImperialUnitsOfMeasurement.PoundsMethanePerYear:
                    return MetricUnitsOfMeasurement.KilogramsMethanePerYear;

                case ImperialUnitsOfMeasurement.PoundsMethane:
                    return MetricUnitsOfMeasurement.KilogramsMethane;

                case ImperialUnitsOfMeasurement.BritishThermalUnitPerDayPerPound:
                    return MetricUnitsOfMeasurement.MegaJoulesPerDayPerKilogram;

                case ImperialUnitsOfMeasurement.BritishThermalUnitPerPoundSquared:
                    return MetricUnitsOfMeasurement.MegaJoulesPerKilogramSquared;

                case ImperialUnitsOfMeasurement.PoundsNitrogenPerPound:
                    return MetricUnitsOfMeasurement.KilogramsNitrogenPerKilogram;

                case ImperialUnitsOfMeasurement.PoundsPerPoundProteinIntake:
                    return MetricUnitsOfMeasurement.KilogramsPerKilogramProteinIntake;

                case ImperialUnitsOfMeasurement.PoundsVolatileSolidsPerPoundFeed:
                    return MetricUnitsOfMeasurement.KilogramsVolatileSolidsPerKilogramFeed;

                case ImperialUnitsOfMeasurement.BritishThermalUnitsPerAcre:
                    return MetricUnitsOfMeasurement.GigajoulesPerHectare;

                case ImperialUnitsOfMeasurement.PoundsPerDay:
                    return MetricUnitsOfMeasurement.KilogramsPerDay;

                case ImperialUnitsOfMeasurement.PoundsPerYear:
                    return MetricUnitsOfMeasurement.KilogramsPerYear;

                case ImperialUnitsOfMeasurement.PoundsCarbonPerAcre:
                    return MetricUnitsOfMeasurement.KilogramsCarbonPerHectare;

                case ImperialUnitsOfMeasurement.PoundsCarbonPerTree:
                    return MetricUnitsOfMeasurement.KilogramsCarbonPerTree;

                case ImperialUnitsOfMeasurement.PoundsCarbonPerPlanting:
                    return MetricUnitsOfMeasurement.KilogramsCarbonPerPlanting;

                case ImperialUnitsOfMeasurement.PoundsCarbonDioxidePerShelterbelt:
                    return MetricUnitsOfMeasurement.KilogramsCarbonDioxidePerShelterbelt;

                case ImperialUnitsOfMeasurement.PercentageAF:
                    return MetricUnitsOfMeasurement.PercentageAF;

                case ImperialUnitsOfMeasurement.PercentageCrudeProtein:
                    return MetricUnitsOfMeasurement.PercentageCrudeProtein;

                case ImperialUnitsOfMeasurement.PercentageDryMatter:
                    return MetricUnitsOfMeasurement.PercentageDryMatter;

                case ImperialUnitsOfMeasurement.PercentageH:
                    return MetricUnitsOfMeasurement.PercentageH;

                case ImperialUnitsOfMeasurement.PercentageNdf:
                    return MetricUnitsOfMeasurement.PercentageNdf;

                case ImperialUnitsOfMeasurement.Percentage:
                    return MetricUnitsOfMeasurement.Percentage;

                case ImperialUnitsOfMeasurement.Days:
                    return MetricUnitsOfMeasurement.Days;

                case ImperialUnitsOfMeasurement.Months:
                    return MetricUnitsOfMeasurement.Months;

                case ImperialUnitsOfMeasurement.Years:
                    return MetricUnitsOfMeasurement.Years;

                case ImperialUnitsOfMeasurement.PoundsCO2PerBTU:
                    return MetricUnitsOfMeasurement.KilogramsCO2PerKiloWattHour;

                case ImperialUnitsOfMeasurement.BTUPerAnimal:
                    return MetricUnitsOfMeasurement.KiloWattHourPerAnimal;

                case ImperialUnitsOfMeasurement.BTUPerPoultryPlacement:
                    return MetricUnitsOfMeasurement.KilowattHourPerPoultryPlacement;

                case ImperialUnitsOfMeasurement.BTUPer1000Quarts:
                    return MetricUnitsOfMeasurement.GigaJoulesPer1000Litres;

                case ImperialUnitsOfMeasurement.Ton:
                case ImperialUnitsOfMeasurement.ShortTon:
                    return MetricUnitsOfMeasurement.Tonne;

                case ImperialUnitsOfMeasurement.BTUPerStandardCubicFoot:
                    return MetricUnitsOfMeasurement.MegaJulesPerNormalCubicMeters;

                case ImperialUnitsOfMeasurement.PoundVolatileSolidsPerCubicYardPerDay:
                    return MetricUnitsOfMeasurement.KilogramVolatileSolidsPerCubicMeterPerDay;

                case ImperialUnitsOfMeasurement.MethanePerCubicYardPerDay:
                    return MetricUnitsOfMeasurement.MethanePerCubicMeterPerDay;

                case ImperialUnitsOfMeasurement.NitrousOxidePerCubicYardPerDay:
                    return MetricUnitsOfMeasurement.NitrousOxidePerCubicMeterPerDay;

                case ImperialUnitsOfMeasurement.AmmoniaPerCubicYardPerDay:
                    return MetricUnitsOfMeasurement.AmmoniaPerCubicMeterPerDay;

                default:
                    throw new Exception($"{unitsOfMeasurement} is an Invalid Unit Of Measurement");
            }

        }
        #endregion

        #region Private Methods

        private void SetUnits()
        {
            if (_isMetric)
            {
                this.KilogramsPerHectareString = " kg ha⁻¹";
            }
            else
            {
                this.KilogramsPerHectareString = " bu ac⁻¹";
            }
        }

        private string WrapString(string unitsOfMeasurement)
        {
            return " (" + unitsOfMeasurement + ")";
        }

        #endregion

        #region Event Handlers

        #endregion
    }
}