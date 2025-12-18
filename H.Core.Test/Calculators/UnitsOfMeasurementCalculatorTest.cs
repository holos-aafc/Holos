using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace H.Core.Test.Calculators
{
    [TestClass]
    public class UnitsOfMeasurementCalculatorTest
    {
        private UnitsOfMeasurementCalculator _calculator;

        [TestInitialize]
        public void TestInitialize()
        {
            _calculator = new UnitsOfMeasurementCalculator();
        }


        // 50 kg * 2.205 lbs/1 kg = 110.25 lbs
        [TestMethod]
        public void ConverterValueToImperialFromMetric_KilogramsToPounds()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.Kilograms, 50.0);
            Assert.AreEqual(110.25, convertedValue);
        }

        //50 grams * 15.432 grains/1 gram = 771.6 grains
        [TestMethod]
        public void ConverterValueToImperialFromMetric_GramsToGrains()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.Grams, 50.0);
            Assert.AreEqual(771.6, convertedValue);
        }

        //50 hectares * 2.4711 acres/1 ha = 123.555 acres
        [TestMethod]
        public void ConverterValueToImperialFromMetric_HectaresToAcres()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.Hectares, 50.0);
            Assert.AreEqual(123.555, Math.Round(convertedValue, 4));
        }

        //50 mm * 0.0394 inches/1 mm = 1.97 inches
        [TestMethod]
        public void ConverterValueToImperialFromMetric_MillimetersToInches()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.Millimeters, 50.0);
            Assert.AreEqual(1.97, convertedValue);
        }

        //50 kg/ha * 1 ha/2.4711 acres * 2.205 lbs/1 kg = 44.615758 lbs/acre
        [TestMethod]
        public void ConverterValueToImperialFromMetric_KilogramsPerHectareToPoundsPerAcre()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.KilogramsPerHectare, 50.0);
            Assert.AreEqual(44.6158, Math.Round(convertedValue, 4));
        }

        //50 mm/year * 0.0394 inches/1 mm = 1.97 inches per year
        [TestMethod]
        public void ConverterValueToImperialFromMetric_MillimetersPerYearToInchesPerYear()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.MillimetersPerYear, 50.0);
            Assert.AreEqual(1.97, convertedValue);
        }

        //50 C * 1.8 + 32.0 = 122 Fahrenheit
        [TestMethod]
        public void ConverterValueToImperialFromMetric_DegreesCelsiusToDegreesFahrenheit()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.DegreesCelsius, 50.0);
            Assert.AreEqual(122, convertedValue);
        }

        //50MJ/kg * kg/2.205lbs * 1000000J/MJ * BTU/1055.056  = 21,492.45 BTU/lb
        [TestMethod]
        public void ConverterValueToImperialFromMetric_MegaJoulesPerKilogramToBritishThermalUnitsPerPound()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.MegaJoulesPerKilogram, 50.0);
            Assert.AreEqual(21492.4487, Math.Round(convertedValue, 4));
        }

        //50 mg/kg * (15.432 grains)/1000/1 mg *  1 kg/2.205lbs = 0.3499319 grains/lbs
        [TestMethod]
        public void ConverterValueToImperialFromMetric_MilligramsPerKilogramToGrainsPerPound()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.MilligramsPerKilogram, 50.0);
            Assert.AreEqual(0.3499, Math.Round(convertedValue, 4));
        }

        //50 IU/gram * 1 gram/15.432 grains = 3.240 IU/grain
        [TestMethod]
        public void ConverterValueToImperialFromMetric_IUPerGramToIUPerGrain()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.InternationalUnitsPerGram, 50.0);
            Assert.AreEqual(3.240, Math.Round(convertedValue, 4));
        }

        //50 kg/kg * 2.205lbs = 110.25 lbs/lbs
        [TestMethod]
        public void ConverterValueToImperialFromMetric_KilogramPerKilogramToPoundPerPound()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.KilogramPerKilogram, 50.0);
            Assert.AreEqual(110.25, Math.Round(convertedValue, 4));
        }

        //50 kg/head/year * 2.205lbs = 110.25 lbs/head/year
        [TestMethod]
        public void ConverterValueToImperialFromMetric_KilogramPerHeadPerYearToPoundsPerHeadPerYear()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.KilogramPerHeadPerYear, 50.0);
            Assert.AreEqual(110.25, Math.Round(convertedValue, 4));
        }

        //50 kg/head/day * 2.205lbs = 110.25 lbs/head/day
        [TestMethod]
        public void ConverterValueToImperialFromMetric_KilogramPerHeadPerDayToPoundsPerHeadPerDay()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.KilogramPerHeadPerDay, 50.0);
            Assert.AreEqual(110.25, Math.Round(convertedValue, 4));
        }

        //50 kg N20/1 kg N * 2.205lbs = 110.25 kg N20/kg N
        [TestMethod]
        public void ConverterValueToImperialFromMetric_KilogramsN20PerKilogramNToPoundsN20PerPoundsN()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.KilogramsN2ONPerKilogramN, 50.0);
            Assert.AreEqual(110.25, Math.Round(convertedValue, 4));
        }

        //50 kg N/ha * 2.205 lbs N/1 kg N * 1 ha/2.4711 acres = 44.615758 lbs N/acre
        [TestMethod]
        public void ConverterValueToImperialFromMetric_KilogramsNitrogenPerHectareToPoundsNitrogenPerAcre()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare, 50.0);
            Assert.AreEqual(44.6158, Math.Round(convertedValue, 4));
        }

        //50 kg P/ha * 2.205 lbs P/1 kg P * 1 ha/2.4711 acres = 44.615758 lbs P/acre
        [TestMethod]
        public void ConverterValueToImperialFromMetric_KilogramsPhosphorousPerHectareToPoundsPhosphorousPerAcre()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.KilogramsPhosphorousPerHectare, 50.0);
            Assert.AreEqual(44.6158, Math.Round(convertedValue, 4));
        }

        //50 m^3 CH4/kg VS * 1 kg VS/2.205 lbs VS * 1.308 yards^3/1 m^3 = 29.659863 yards^3/lbs VS
        [TestMethod]
        public void ConverterValueToImperialFromMetric_CubicMetersMethanePerKilogramVSToCubicYardsPerPoundVS()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.CubicMetersMethanePerKilogramVolatileSolids, 50.0);
            Assert.AreEqual(29.6599, Math.Round(convertedValue, 4));
        }

        //50 kg CH4/kg CH4 * 2.205 lbs CH4 = 110.25 lbs CH4/lbs CH4
        [TestMethod]
        public void ConverterValueToImperialFromMetric_KilogramsMethanePerKilogramsMethaneToPoundsMethanePerPoundsMethane()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.KilogramsMethanePerKilogramMethane, 50.0);
            Assert.AreEqual(110.25, Math.Round(convertedValue, 4));
        }

        //50MJ/day/kg * kg/2.205lbs * 1000000J/MJ * BTU/1055.056 = 21492.4487
        [TestMethod]
        public void ConverterValueToImperialFromMetric_MegaJoulesPerDayPerKilogramToBritishThermalUnitPerDayPerPound()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.MegaJoulesPerDayPerKilogram, 50.0);
            Assert.AreEqual(21492.4487, Math.Round(convertedValue, 4));
        }

        //50 kg N/kg * 2.205 lbs N = 110.25 lbs N/lbs
        [TestMethod]
        public void ConverterValueToImperialFromMetric_KilogramsNitrogenPerKilogramToPoundsNitrogenPerPound()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.KilogramsNitrogenPerKilogram, 50.0);
            Assert.AreEqual(110.25, Math.Round(convertedValue, 4));
        }

        //50GJ/ha * 1 ha/2.4711 acres  * 1000MJ/GJ * 1000000J/MJ * BTU/1055.056J = 19178037.8761 BTU/ac
        [TestMethod]
        public void ConverterValueToImperialFromMetric_GigaJoulesPerHectareToBritishThermalUnitsPerAcre()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.GigajoulesPerHectare, 50.0);
            Assert.AreEqual(19178037.8761, Math.Round(convertedValue, 4));
        }

        //50 kg/day * 2.205 lbs/1 kg = 110.25 lbs/day
        [TestMethod]
        public void ConverterValueToImperialFromMetric_KilogramsPerDayToPoundsPerDay()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.KilogramsPerDay, 50.0);
            Assert.AreEqual(110.25, Math.Round(convertedValue, 4));
        }

        //50kg C/ha * 2.205 lbs C/1 kg C * 1 ha/2.4711 acres = 44.61578 lbs C/ac
        [TestMethod]
        public void ConverterValueToImperialFromMetric_KilogramsCarbonPerHectareToPoundsCarbonPerAcre()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, 50.0);
            Assert.AreEqual(44.6158, Math.Round(convertedValue, 4));
        }

        //50 kg C/tree * 2.205 lbs C/1 kg C = 110.25 lbs C/tree
        [TestMethod]
        public void ConverterValueToImperialFromMetric_KilogramsCarbonPerTreeToPoundsCarbonPerTree()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.KilogramsCarbonPerTree, 50.0);
            Assert.AreEqual(110.25, Math.Round(convertedValue, 4));
        }

        //50 kg C/planting * 2.205 lbs C/1kg C = 110.25 lbs C/planting
        [TestMethod]
        public void ConverterValueToImperialFromMetric_KilogramsCarbonPerPlantingToPoundsCarbonPerPlanting()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.KilogramsCarbonPerPlanting, 50.0);
            Assert.AreEqual(110.25, Math.Round(convertedValue, 4));
        }

        //50 kg CO2/shelterbelt * 2.205lbs CO2/1 kg CO2 = 110.25 lbs CO2/shelterbelt
        [TestMethod]
        public void ConverterValueToImperialFromMetric_KilogramsCarbonDioxidePerShelterbeltToPoundsCarbonDioxidePerShelterbelt()
        {
            var convertedValue = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.KilogramsCarbonDioxidePerShelterbelt, 50.0);
            Assert.AreEqual(110.25, Math.Round(convertedValue, 4));
        }

        //(50 kg/ha * 2.205 lbs/kg) / (2.4711 acres/hectare * BarleyCropFactor = 48) = 0.9294 bushels per acre
        [TestMethod]
        public void ConverterValueToMetricFromImperial_KilogramsPerHectareToBushelsPerAcre()
        {
            var convertedValue = _calculator.ConvertKilogramsPerHectareToImperialBushelPerAcresBasedOnCropType(CropType.Barley, 50);
            Assert.AreEqual(0.9295, Math.Round(convertedValue, 4));
        }


        ////////////Imperial To Metric Tests//////////////////////////////////////////////////////////////////////////////////////////
        //50 lbs / 2.205 kg/lbs = 22.675736 kg
        [TestMethod]
        public void ConverterValueToMetricFromImperial_PoundsToKilograms()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.Pounds, 50.0);
            Assert.AreEqual(22.6757, Math.Round(convertedValue, 4));
        }

        //50 grains / (15.432 grains/gram) = 3.240 grams
        [TestMethod]
        public void ConverterValueToMetricFromImperial_GrainsToGrams()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.Grains, 50.0);
            Assert.AreEqual(3.240, Math.Round(convertedValue, 4));
        }

        //50 acres / 2.4711 acres/1 ha = 20.2339 hectares
        [TestMethod]
        public void ConverterValueToMetricFromImperial_AcresToHectares()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.Acres, 50.0);
            Assert.AreEqual(20.2339, Math.Round(convertedValue, 4));
        }

        //50 inches / 0.0394 inches/1 mm = 1269.0355 mm
        [TestMethod]
        public void ConverterValueToMetricFromImperial_InchesToMm()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.InchesToMm, 50.0);
            Assert.AreEqual(1269.0355, Math.Round(convertedValue, 4));
        }


        //50 inches/year / 0.0394 inches/1 mm = 1269.0355 mm/year
        [TestMethod]
        public void ConverterValueToImperialFromMetric_InchesPerYearToMillimetersPerYear()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.InchesPerYear, 50.0);
            Assert.AreEqual(1269.0355, Math.Round(convertedValue, 4));
        }

        //50 F/ 1.8 -32 = -4.22 Celsius
        [TestMethod]
        public void ConverterValueToImperialFromMetric_DegreesFahrenheitToDegreesCelsiusTo()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.DegreesFahrenheit, 50.0);
            Assert.AreEqual(10, Math.Round(convertedValue, 4));
        }

        [TestMethod]
        public void ConverterValueToIMetricFromImperial_BTUPerPoundToMegaJoulesPerKilogram()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.BritishThermalUnitPerPound, 50.0, MetricUnitsOfMeasurement.MegaJoulesPerKilogram);
            Assert.AreEqual(0.1163, Math.Round(convertedValue, 4));
        }


        [TestMethod]
        public void ConverterValueToIMetricFromImperial_BTUPerPoundToKiloCaloriePerKilogram()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.BritishThermalUnitPerPound, 50.0, MetricUnitsOfMeasurement.KiloCaloriePerKilogram);
            Assert.AreEqual(27.8011, Math.Round(convertedValue, 4));
        }

        [TestMethod]
        public void ConvertValueTOMetricFromImperial_BTUPerPoundToMegaCaloriePerKilogram()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.BritishThermalUnitPerPound, 50.0, MetricUnitsOfMeasurement.MegaCaloriePerKilogram);
            Assert.AreEqual(0.0278, Math.Round(convertedValue, 4));
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void ConvertValueToMetricFromImperial_ThrowException()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(
                ImperialUnitsOfMeasurement.BritishThermalUnitPerPound, 50);
        }

        //(50 grains/lbs / (15.432 grains)/1000/1 mg) * (2.205lbs/kg) = 7144.2457 mg/kg
        [TestMethod]
        public void ConverterValueToImperialFromMetric_GrainsPerPoundToMilligramsPerKilogram()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.GrainsPerPound, 50.0);
            Assert.AreEqual(7144.2457, Math.Round(convertedValue, 4));
        }

        //50 IU/grain * 1 15.432 grains/gram = 771.6IU/gram
        [TestMethod]
        public void ConverterValueToMetricFromImperial_IUPerGrainToIUPerGram()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.InternationalUnitsPerGrain, 50.0);
            Assert.AreEqual(771.6, Math.Round(convertedValue, 4));
        }

        //50 lbs/lbs / 2.205lbs/kg = 22.675736 kg/kg
        [TestMethod]
        public void ConverterValueToMetricFromImperial_PoundPerPoundToKilogramPerKilogram()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.PoundsPerPound, 50.0);
            Assert.AreEqual(22.6757, Math.Round(convertedValue, 4));
        }

        //50 lbs/head/year / 2.205lbs/kg = 22.6757 kg/head/year
        [TestMethod]
        public void ConverterValueToMetricFromImperial_PoundsPerHeadPerYearToKilogramPerHeadPerYear()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.PoundPerHeadPerYear, 50.0);
            Assert.AreEqual(22.6757, Math.Round(convertedValue, 4));
        }

        //50 lbs/head/Day / 2.205lbs/kg = 22.6757 kg/head/Day
        [TestMethod]
        public void ConverterValueToMetricFromImperial_PoundsPerHeadPerDayToKilogramPerHeadPerDay()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.PoundPerHeadPerDay, 50.0);
            Assert.AreEqual(22.6757, Math.Round(convertedValue, 4));
        }

        //50 lbs N20/1 lbs N / 2.205lbs/kg = 22.6757 kg N20/kg N
        [TestMethod]
        public void ConverterValueToImperialFromMetric_PoundsN20PerPoundsNToKilogramsN20PerKilogramN()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.PoundsN2ONPerPoundN, 50.0);
            Assert.AreEqual(22.6757, Math.Round(convertedValue, 4));
        }

        //50 lbs N/acre / 2.205 lbs N/1 kg N * 2.4711 acres/ha = 56.0340 kg N/hectare
        [TestMethod]
        public void ConverterValueToMetricFromImperialKPoundsNitrogenPerAcreToKilogramsNitrogenPerHectare()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.PoundsNitrogenPerAcre, 50.0);
            Assert.AreEqual(56.0340, Math.Round(convertedValue, 4));
        }
        //50 lbs P/acre / 2.205 lbs P/1 kg P / * 2.4711 acres/ha = 56.0340 kg P/hectare
        [TestMethod]
        public void ConverterValueToMetricFromImperialKPoundsPhosphorousPerAcreTKoilogramsPhosphorousPerHectare()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.PoundsNitrogenPerAcre, 50.0);
            Assert.AreEqual(56.0340, Math.Round(convertedValue, 4));
        }

        //50 yards^3 CH4/lbs VS * 2.205 lbs VS/kg / 1.308 yards^3/1 m^3 = 84.289 m^3/kg VS
        [TestMethod]
        public void ConverterValueToMetricFromImperial_CubicYardsPerPoundVSToCubicMetersMethanePerKilogramVS()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.CubicYardsMethanePerPoundVolatileSolids, 50.0);
            Assert.AreEqual(84.289, Math.Round(convertedValue, 4));
        }

        //50 lbs CH4/kg CH4 * 2.205 lbs CH4/kg =22.6757 lbs CH4/lbs CH4
        [TestMethod]
        public void ConverterValueToZMetricFromImperial_PoundsMethanePerPoundsMethaneToKilogramsMethanePerKilogramsMethane()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.PoundsMethanePerPoundMethane, 50.0);
            Assert.AreEqual(22.6757, Math.Round(convertedValue, 4));
        }

        //50BTU/day/lbs * 2.205lbs/kg * 1055.056J/BTU * MJ/1000000J = 0.1161
        [TestMethod]
        public void ConverterValueToMetricFromImperial_BrithishThermalUnitsPerDayPerPoundToMegaJoulesPerDayPerKilogram()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.BritishThermalUnitPerDayPerPound, 50.0);
            Assert.AreEqual(0.1163, Math.Round(convertedValue, 4));
        }

        //50 lbs N/lbs / 2.205 kg N/lbs = 22.6757 kg N/kg
        [TestMethod]
        public void ConverterValueToMetricFromImperial_PoundsNitrogenPerPoundKilogramsNitrogenPerKilogram()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.PoundsNitrogenPerPound, 50.0);
            Assert.AreEqual(22.6757, Math.Round(convertedValue, 4));
        }

        //50BTU/acre * 2.4711 acres/1 hectare * 1055.056J/BTU * MJ/1000000J = 0.1304 MJ/ha
        [TestMethod]
        public void ConverterValueToMetricFromImperial_BritishThermalUnitsPerAcreToMegaJoulesPerHectare()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.BritishThermalUnitsPerAcre, 50.0);
            Assert.AreEqual(0.1304, Math.Round(convertedValue, 4));
        }

        //50 lbs/day * 2.205 kg/1 lbs = 22.6757 kg/day
        [TestMethod]
        public void ConverterValueToMetricFromImperial_PoundsPerDayToKilogramsPerDay()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.PoundsPerDay, 50.0);
            Assert.AreEqual(22.6757, Math.Round(convertedValue, 4));
        }

        //50lbs C/acre / 2.205 lbs C/1 kg C * 2.4711 acres/hectare = 56.034011 kg C/hectare
        [TestMethod]
        public void ConverterValueToImperialFromMetric_PoundsCarbonPerAcreToKilogramsCarbonPerHectare()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.PoundsCarbonPerAcre, 50.0);
            Assert.AreEqual(56.0340, Math.Round(convertedValue, 4));
        }

        //50 lbs C/tree * 2.205 lbs C/1 kg C = 22.6757 kg C/tree
        [TestMethod]
        public void ConverterValueToMetricFromImperial_PoundsCarbonPerTreeToKilogramsCarbonPerTree()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.PoundsCarbonPerTree, 50.0);
            Assert.AreEqual(22.6757, Math.Round(convertedValue, 4));
        }


        //50 lbs CO2/Shelterbelt * 2.205 lbs C/1 kg C = 22.6757 kg C/Shelterbelt
        [TestMethod]
        public void ConverterValueToMetricFromImperial_PoundsCarbonDioxidePerTreeToKilogramsCarbonDioxidePerShelterbelt()
        {
            var convertedValue = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.PoundsCarbonPerPlanting, 50.0);
            Assert.AreEqual(22.6757, Math.Round(convertedValue, 4));
        }

        //(50 kg/ha * 2.205 lbs/kg) / (2.4711 acres/hectare * BarleyCropFactor = 48) = 0.9294 bushels per acre
        [TestMethod]
        public void ConverterValueToImperialFromMetric_BushelsPerAcreToKilogramsPerHectare()
        {
            var convertedValue = _calculator.ConvertBushelsPerAcreToMetricKilogramsPerHectareBasedOnCropType(CropType.Barley, 0.9295);
            Assert.AreEqual(50.0003, Math.Round(convertedValue, 4));
        }

        [TestMethod]
        public void GetMetricValueFromViewModels()
        {
            //when in imperial mode we want metric values to hand to all the calculations
            //handing in 12 lb
            var metricValue = _calculator.GetMetricValueFromViewModels(MeasurementSystemType.Imperial,
                MetricUnitsOfMeasurement.Kilograms, 12);
            Assert.AreEqual(12 / 2.205, metricValue, 1);

            //handing in 12 kg
            metricValue = _calculator.GetMetricValueFromViewModels(MeasurementSystemType.Metric,
                MetricUnitsOfMeasurement.Kilograms, 12);
            Assert.AreEqual(12, metricValue);
        }

        [TestMethod]
        public void GetUnitValueFromHolos()
        {
            //when in imperial mode we want metric values to hand to all the calculations
            //handing in 12 kg
            //expect 26.46 lb back 
            var imperialValue = _calculator.GetUnitValueFromHolos(MeasurementSystemType.Imperial,
                MetricUnitsOfMeasurement.Kilograms, 12);
            Assert.AreEqual(26.46, imperialValue, 1);

            //handing in 12 kg
            //expect 12 kg back 
            var metricValue = _calculator.GetUnitValueFromHolos(MeasurementSystemType.Metric,
                MetricUnitsOfMeasurement.Kilograms, 12);
            Assert.AreEqual(12, metricValue);
        }

        [TestMethod]
        public void CalculateShortTonValueFromMetricTon()
        {
            var shortTon = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.MetricTon, 366);
            Assert.AreEqual(403.446, Math.Round(shortTon, 3));
        }

        [TestMethod]
        public void CalculateMetricTonValueFromShortTon()
        {
            var tonne = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.Ton, 421);
            Assert.AreEqual(381.925, Math.Round(tonne, 3));
        }

        [TestMethod]
        public void GetEnumMetricTonFromShortTon()
        {
            var tonne = _calculator.GetMetricUnitsOfMeasurement(ImperialUnitsOfMeasurement.Ton);
            Assert.AreEqual(MetricUnitsOfMeasurement.Tonne, tonne);
        }

        [TestMethod]
        public void GetEnumShortTonFromMetricTon()
        {
            var shortTon = _calculator.GetImperialUnitsOfMeasurement(MetricUnitsOfMeasurement.Tonne);
            Assert.AreEqual(ImperialUnitsOfMeasurement.Ton, shortTon);
        }

        [TestMethod]
        public void CalculateVSCubicMeterToCubicYard()
        {
            var value = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.KilogramVolatileSolidsPerCubicMeterPerDay, 1.6);
            Assert.AreEqual(2.70, Math.Round(value, 2));
        }

        [TestMethod]
        public void CalculateVSCubicYardToCubicMeter()
        {
            var value = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.PoundVolatileSolidsPerCubicYardPerDay, 25);
            Assert.AreEqual(14.83, Math.Round(value, 2));
        }

        [TestMethod]
        public void CalculateMethanePerCubicYardToCubicMeter()
        {
            var value = _calculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.MethanePerCubicYardPerDay, 12);
            Assert.AreEqual(9.17, Math.Round(value, 2));
        }

        [TestMethod]
        public void CalculateMethanePerCubicMeterToCubicYard()
        {
            var value = _calculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.MethanePerCubicMeterPerDay, 3);
            Assert.AreEqual(3.92, Math.Round(value, 2));
        }
    }
}
