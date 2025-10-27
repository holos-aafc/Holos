﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using H.Content;
using H.Core.Calculators.Climate;
using H.Core.Enumerations;
using H.Core.Providers.Temperature;
using H.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Calculators
{
    [TestClass]
    public class ClimateParameterCalculatorTest
    {
        private ClimateParameterCalculator _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            _sut = new ClimateParameterCalculator();
        }

        #region Tests

        [TestMethod]
        public void PerennialTest()
        {
            const int emergenceDay = 141;
            const int ripeningDay = 197;
            const double yield = 3795;
            const double clay = 0.39;
            const double sand = 0.08;
            const double layerThickness = 250;
            const double percentageSoilOrganicCarbon = 2.7;
            const double variance = 300;
            const double wiltingPointFraction = 0.7;
            const double decompositionMinimumTemperature = -3.78;
            const double decompositionMaximumTemperature = 30;
            const double moistureResponseFunctionAtSaturation = 0.4;
            const double moistureResponseFunctionAtWiltingPoint = 0.18;
            const CropType cropType = CropType.None;

            var kapuskasingTestFileLines = CsvResourceReader.GetFileLines(CsvResourceNames.Kapuskasing);
            var temperatures = new List<double>();
            var precipitations = new List<double>();
            var evapotranspirations = new List<double>();
            var dailyMinimumTemperatures = new List<double>();
            var dailyMaximumTemperatures = new List<double>();

            for (var yearIndex = 1970; yearIndex < 1971; yearIndex++)
            {
                foreach (var line in kapuskasingTestFileLines)
                {
                    var year = int.Parse(line.ElementAt(1));
                    if (year != yearIndex)
                    {
                        continue;
                    }

                    temperatures.Add(double.Parse(line.ElementAt(5), InfrastructureConstants.EnglishCultureInfo));
                    precipitations.Add(double.Parse(line.ElementAt(6), InfrastructureConstants.EnglishCultureInfo));
                    evapotranspirations.Add(double.Parse(line.ElementAt(7), InfrastructureConstants.EnglishCultureInfo));
                    dailyMinimumTemperatures.Add(double.Parse(line.ElementAt(5), InfrastructureConstants.EnglishCultureInfo));
                    dailyMaximumTemperatures.Add(double.Parse(line.ElementAt(5), InfrastructureConstants.EnglishCultureInfo));
                }

                var result = _sut.CalculateClimateParameter(emergenceDay,
                                                            ripeningDay,
                                                            yield,
                                                            clay,
                                                            sand,
                                                            layerThickness,
                                                            percentageSoilOrganicCarbon,
                                                            variance,
                                                            wiltingPointFraction,
                                                            decompositionMinimumTemperature,
                                                            decompositionMaximumTemperature,
                                                            moistureResponseFunctionAtWiltingPoint,
                                                            moistureResponseFunctionAtSaturation,
                                                            evapotranspirations,
                                                            precipitations,
                                                            temperatures, 
                                                            dailyMinimumTemperatures, 
                                                            dailyMaximumTemperatures, 
                                                            cropType);
            }
        }

        [TestMethod]
        public void DailyClimateInputTest()
        {
            const int emergenceDay = 135;
            const int ripeningDay = 227;
            const double yield = 980;
            const double clay = 0.31;
            const double sand = 0.46;
            const double layerThickness = 150;
            const double percentageSoilOrganicCarbon = 2;
            const double variance = 300;
            const double wiltingPointFraction = 0.7;
            const double decompositionMinimumTemperature = -3.78;
            const double decompositionMaximumTemperature = 30;
            const double moistureResponseFunctionAtSaturation = 0.4;
            const double moistureResponseFunctionAtWiltingPoint = 0.18;
            const CropType cropType = CropType.None;

            var kapuskasingTestFileLines = CsvResourceReader.GetFileLines(CsvResourceNames.Kapuskasing);
            var temperatures = new List<double>();
            var precipitations = new List<double>();
            var evapotranspirations = new List<double>();
            var dailyMinimumTemperatures = new List<double>();
            var dailyMaximumTemperatures = new List<double>();

            var file = H.Core.Test.Resource1.climate_parameter_tool_input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var line in file)
            {
                var tokens = line.Split(',');

                var year = int.Parse(tokens[0]);
                var day = int.Parse(tokens[1]);
                var temp = double.Parse(tokens[2]);
                var prec = double.Parse(tokens[3]);
                var evap = double.Parse(tokens[4]);

                temperatures.Add(temp);
                precipitations.Add(prec);
                evapotranspirations.Add(evap);
                dailyMinimumTemperatures.Add(temp);
                dailyMaximumTemperatures.Add(temp);
            }

            var result = _sut.CalculateClimateParameter(emergenceDay,
                                                        ripeningDay,
                                                        yield,
                                                        clay,
                                                        sand,
                                                        layerThickness,
                                                        percentageSoilOrganicCarbon,
                                                        variance,
                                                        wiltingPointFraction,
                                                        decompositionMinimumTemperature,
                                                        decompositionMaximumTemperature,
                                                        moistureResponseFunctionAtWiltingPoint,
                                                        moistureResponseFunctionAtSaturation,
                                                        evapotranspirations,
                                                        precipitations,
                                                        temperatures, 
                                                        dailyMinimumTemperatures, 
                                                        dailyMaximumTemperatures, 
                                                        cropType);
        }

        [TestMethod]
        public void ChangLethbridgeTest()
        {
            const int emergenceDay = 135;
            const int ripeningDay = 227;
            const double yield = 980;
            const double clay = 0.31;
            const double sand = 0.46;
            const double layerThickness = 250;
            const double percentageSoilOrganicCarbon = 2;
            const double variance = 300;
            const double wiltingPointFraction = 0.7;
            const double decompositionMinimumTemperature = -3.78;
            const double decompositionMaximumTemperature = 30;
            const double moistureResponseFunctionAtSaturation = 0.4;
            const double moistureResponseFunctionAtWiltingPoint = 0.18;
            const CropType cropType = CropType.None;

            var kapuskasingTestFileLines = CsvResourceReader.GetFileLines(CsvResourceNames.Kapuskasing);
            var temperatures = new List<double>();
            var precipitations = new List<double>();
            var evapotranspirations = new List<double>();
            var dailyMinimumTemperatures = new List<double>();
            var dailyMaximumTemperatures = new List<double>();

            double result = 0;
            for (var yearIndex = 1970; yearIndex < 1971; yearIndex++)
            {
                foreach (var line in kapuskasingTestFileLines)
                {
                    var year = int.Parse(line.ElementAt(1));
                    if (year != yearIndex)
                    {
                        continue;
                    }

                    temperatures.Add(double.Parse(line.ElementAt(5), InfrastructureConstants.EnglishCultureInfo));
                    dailyMinimumTemperatures.Add(double.Parse(line.ElementAt(5), InfrastructureConstants.EnglishCultureInfo));
                    dailyMaximumTemperatures.Add(double.Parse(line.ElementAt(5), InfrastructureConstants.EnglishCultureInfo));
                    precipitations.Add(double.Parse(line.ElementAt(6), InfrastructureConstants.EnglishCultureInfo));
                    evapotranspirations.Add(double.Parse(line.ElementAt(7), InfrastructureConstants.EnglishCultureInfo));
                }

                result = _sut.CalculateClimateParameter(emergenceDay,
                                                        ripeningDay,
                                                        yield,
                                                        clay,
                                                        sand,
                                                        layerThickness,
                                                        percentageSoilOrganicCarbon,
                                                        variance,
                                                        wiltingPointFraction,
                                                        decompositionMinimumTemperature,
                                                        decompositionMaximumTemperature,
                                                        moistureResponseFunctionAtWiltingPoint,
                                                        moistureResponseFunctionAtSaturation,
                                                        evapotranspirations,
                                                        precipitations,
                                                        temperatures, 
                                                        dailyMinimumTemperatures, 
                                                        dailyMaximumTemperatures, 
                                                        cropType);
            }
        }

        /// <summary>
        /// Equation 2.2.1-1 to 2.2.1-44
        /// Tested against results from Z:\SAS MARTIN ICBM STUFF\SAS Output_htm#IDX3.htm
        /// </summary>
        [TestMethod]
        public void CalculateClimateParameterReturnsCorrectValue()
        {
            const int emergenceDay = 141;
            const int ripeningDay = 197;
            const double yield = 3795;
            const double clay = 0.39;
            const double sand = 0.08;
            const double layerThickness = 250;
            const double percentageSoilOrganicCarbon = 2.7;
            const double variance = 300;
            const double wiltingPointFraction = 0.7;
            const double decompositionMinimumTemperature = -3.78;
            const double decompositionMaximumTemperature = 30;
            const double moistureResponseFunctionAtSaturation = 0.4;
            const double moistureResponseFunctionAtWiltingPoint = 0.18;
            const CropType cropType = CropType.None;

            var kapuskasingTestFileLines = CsvResourceReader.GetFileLines(CsvResourceNames.Kapuskasing);
            var results = new Dictionary<int, double>();
            var temperatures = new List<double>();
            var precipitations = new List<double>();
            var evapotranspirations = new List<double>();
            var dailyMinimumTemperatures = new List<double>();
            var dailyMaximumTemperatures = new List<double>();

            for (var yearIndex = 1970; yearIndex <= 1999; yearIndex++)
            {
                foreach (var line in kapuskasingTestFileLines)
                {
                    var year = int.Parse(line.ElementAt(1));
                    if (year != yearIndex)
                    {
                        continue;
                    }

                    temperatures.Add(double.Parse(line.ElementAt(5), InfrastructureConstants.EnglishCultureInfo));
                    dailyMinimumTemperatures.Add(double.Parse(line.ElementAt(5), InfrastructureConstants.EnglishCultureInfo));
                    dailyMaximumTemperatures.Add(double.Parse(line.ElementAt(5), InfrastructureConstants.EnglishCultureInfo));
                    precipitations.Add(double.Parse(line.ElementAt(6), InfrastructureConstants.EnglishCultureInfo));
                    evapotranspirations.Add(double.Parse(line.ElementAt(7), InfrastructureConstants.EnglishCultureInfo));
                }

                var result = _sut.CalculateClimateParameter(emergenceDay,
                                                            ripeningDay,
                                                            yield,
                                                            clay,
                                                            sand,
                                                            layerThickness,
                                                            percentageSoilOrganicCarbon,
                                                            variance,
                                                            wiltingPointFraction,
                                                            decompositionMinimumTemperature,
                                                            decompositionMaximumTemperature,
                                                            moistureResponseFunctionAtWiltingPoint,
                                                            moistureResponseFunctionAtSaturation,
                                                            evapotranspirations,
                                                            precipitations,
                                                            temperatures,
                                                            dailyMinimumTemperatures,
                                                            dailyMaximumTemperatures, 
                                                            cropType);

                results.Add(yearIndex, Math.Round(result, 2));

                temperatures.Clear();
                precipitations.Clear();
                evapotranspirations.Clear();
            }

            Assert.AreEqual(results[1970], 1.04);
            Assert.AreEqual(results[1971], 0.98);
            Assert.AreEqual(results[1972], 0.87);
            Assert.AreEqual(results[1973], 1.14);
            Assert.AreEqual(results[1974], 0.91);
            Assert.AreEqual(results[1975], 1.10);
            Assert.AreEqual(results[1976], 0.88);
            Assert.AreEqual(results[1977], 0.98);
            Assert.AreEqual(results[1978], 0.99);
            Assert.AreEqual(results[1979], 0.95);
            Assert.AreEqual(results[1980], 0.99);
            Assert.AreEqual(results[1981], 0.84);
            Assert.AreEqual(results[1982], 0.98);
            Assert.AreEqual(results[1983], 1.08);
            Assert.AreEqual(results[1984], 1.07);
            Assert.AreEqual(results[1985], 1.04);
            Assert.AreEqual(results[1986], 0.92);
            Assert.AreEqual(results[1987], 1.17);
            Assert.AreEqual(results[1988], 0.95);
            Assert.AreEqual(results[1989], 1);
            Assert.AreEqual(results[1990], 0.99);
            Assert.AreEqual(results[1991], 1.10);
            Assert.AreEqual(results[1992], 0.88);
            Assert.AreEqual(results[1993], 0.95);
            Assert.AreEqual(results[1994], 1.07);
            Assert.AreEqual(results[1995], 0.99);
            Assert.AreEqual(results[1996], 1.04);
            Assert.AreEqual(results[1997], 0.90);
            Assert.AreEqual(results[1998], 1.09);
            Assert.AreEqual(results[1999], 1.18);
        }

        /// <summary>
        /// Equation 2.2.1-45
        /// </summary>
        [TestMethod]
        public void CalculateClimateManagementFactorReturnsCorrectValue()
        {
            var result = _sut.CalculateClimateManagementFactor(32.34253, 4.63365);
            Assert.AreEqual(149.8639641345, result, 0.0000000001);
        }

        [TestMethod]
        public void CalculateCropCoefficientCorrectValue()
        {
            const double maxTemperatureForDay = 30.3;
            const double minTemperatureForDay = 21.5;
            var meanTemperature = (maxTemperatureForDay + minTemperatureForDay) / 2.0;

            var cropCoefficient = _sut.CalculateCropCoefficient(30.3, 21.5, Enumerations.CropType.AlfalfaSeed, meanTemperature);

            Assert.AreEqual(-0.00024093600718921304, cropCoefficient);
        }

        [TestMethod]
        public void CalculateCropCoefficient_ReturnsExpected_ForTypicalValues()
        {
            // Arrange
            double maxTemp = 25.0;
            double minTemp = 15.0;
            double meanTemp = (maxTemp + minTemp) / 2.0;
            var cropType = CropType.Wheat;

            // Act
            double result = _sut.CalculateCropCoefficient(maxTemp, minTemp, cropType, meanTemp);

            // Assert
            Assert.AreEqual(-0.0192, result);
        }

        [TestMethod]
        public void CalculateCropCoefficient_ReturnsExpected_ForZeroMinimumAndMaximumTemperatures()
        {
            // Arrange
            double maxTemp = 0;
            double minTemp = 0;
            double meanTemp = 15;
            var cropType = CropType.Wheat;

            // Act
            double result = _sut.CalculateCropCoefficient(maxTemp, minTemp, cropType, meanTemp);

            // Assert
            Assert.AreEqual(0.0072082735700000015, result);
        }

        [TestMethod]
        public void CalculateCropCoefficient_ReturnsSame_ForEqualMinMaxTemp()
        {
            // Arrange
            double temp = 20.0;
            var cropType = CropType.Barley;

            // Act
            double result = _sut.CalculateCropCoefficient(temp, temp, cropType, temp);

            // Assert
            Assert.AreEqual(0.06592104730625, result);
        }

        [TestMethod]
        public void CalculateCropCoefficient_HandlesNegativeTemperatures()
        {
            // Arrange
            double maxTemp = -2.0;
            double minTemp = -5.0;
            double meanTemp = (maxTemp + minTemp) / 2.0;

            // Use a crop type that Holos supports. Not all types in table 1 are presented to the user for selection.
            var cropType = CropType.FallRye;

            // Act
            double result = _sut.CalculateCropCoefficient(maxTemp, minTemp, cropType, meanTemp);

            // Assert
            Assert.AreEqual(-0.058580722557568757, result);
        }

        [TestMethod]
        public void CalculateCropCoefficient_ForPerennialCropType()
        {
            // Arrange
            double maxTemp = 30.0;
            double minTemp = 10.0;
            double meanTemp = 15.0;
            var cropType = CropType.AlfalfaSeed;

            // Act
            double result = _sut.CalculateCropCoefficient(maxTemp, minTemp, cropType, meanTemp);

            // Assert
            Assert.AreEqual(0.0019864688327069263, result);
        }

        #endregion
    }
}