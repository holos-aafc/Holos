#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Climate;

#endregion

namespace H.Core.Calculators.Climate
{
    public class ClimateParameterCalculator : IClimateParameterCalculator
    {
        #region Fields

        private readonly Table_1_Growing_Degree_Crop_Coefficients_Provider _growingDegreeCropCoefficientsProvider;
        private readonly EvapotranspirationCalculator _evapotranspirationCalculator;

        #endregion

        #region Constructors

        public ClimateParameterCalculator()
        {
            _growingDegreeCropCoefficientsProvider = new Table_1_Growing_Degree_Crop_Coefficients_Provider();
            _evapotranspirationCalculator = new EvapotranspirationCalculator();
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        /// <summary>
        /// Calculates the annual climate parameter for a given set of daily temperature data points. The value returned is the average
        /// of all daily values calculated.
        /// </summary>
        public double CalculateClimateParameterForYear(
            Farm farm,
            CropViewItem cropViewItem,
            List<double> evapotranspirations,
            List<double> precipitations,
            List<double> temperatures)
        {
            var dailyResults = this.CalculateDailyClimateParameters(
                farm: farm,
                cropViewItem: cropViewItem,
                dailyEvapotranspiration: evapotranspirations,
                dailyPrecipitation: precipitations,
                dailyTemperature: temperatures);

            // Adjust precip values so that the irrigation is included

            var climateParameter = dailyResults.Average(dailyResult => dailyResult.ClimateParameter);

            return climateParameter;
        }

        /// <summary>
        /// Calculate the daily climate parameter for each given temperature data point (julian day). Returns a list of results
        /// that contain the resultant climate parameter calculations for each julian day in the year
        /// </summary>
        public List<ClimateParameterDailyResult> CalculateDailyClimateParameters(
            Farm farm,
            CropViewItem cropViewItem,
            List<double> dailyEvapotranspiration,
            List<double> dailyPrecipitation,
            List<double> dailyTemperature)
        {
            var defaults = farm.Defaults;
            var geographicData = farm.GeographicData;
            var yield = cropViewItem.Yield;

            var ripeningDay = cropViewItem.CropType.IsPerennial() ? defaults.RipeningDayForPerennnials : defaults.RipeningDay;
            var variance = cropViewItem.CropType.IsPerennial() ? defaults.VarianceForPerennials : defaults.Variance;
            var emergenceDay = cropViewItem.CropType.IsPerennial() ? defaults.EmergenceDayForPerennials : defaults.EmergenceDay;

            return this.CalculateDailyClimateParameterResults(
                    emergenceDay: emergenceDay,
                    ripeningDay: ripeningDay,
                    yield: yield,
                    clay: geographicData.DefaultSoilData.ProportionOfClayInSoil,
                    sand: geographicData.DefaultSoilData.ProportionOfSandInSoil,
                    layerThicknessInMillimeters: geographicData.DefaultSoilData.TopLayerThickness,
                    percentageSoilOrganicCarbon: geographicData.DefaultSoilData.ProportionOfSoilOrganicCarbon,
                    variance: variance,
                    alfa: defaults.Alfa,
                    decompositionMinimumTemperature: defaults.DecompositionMinimumTemperature,
                    decompositionMaximumTemperature: defaults.DecompositionMaximumTemperature,
                    moistureResponseFunctionAtWiltingPoint: defaults.MoistureResponseFunctionAtWiltingPoint,
                    moistureResponseFunctionAtSaturation: defaults.MoistureResponseFunctionAtSaturation,
                    evapotranspirations: dailyEvapotranspiration,
                    precipitations: dailyPrecipitation,
                    temperatures: dailyTemperature);
        }

        /// <summary>
        /// Overload that takes in a user defined tillage factor.
        /// </summary>
        public double CalculateManagementFactor(double climateParameter, double tillageFactor)
        {
            return this.CalculateClimateManagementFactor(climateParameter, tillageFactor);
        }

        public List<double> CalculateDailyClimateParameters(
            int emergenceDay,
            int ripeningDay,
            double yield,
            double clay,
            double sand,
            double layerThicknessInMillimeters,
            double percentageSoilOrganicCarbon,
            double variance,
            double alfa,
            double decompositionMinimumTemperature,
            double decompositionMaximumTemperature,
            double moistureResponseFunctionAtWiltingPoint,
            double moistureResponseFunctionAtSaturation,
            List<double> evapotranspirations,
            List<double> precipitations,
            List<double> temperatures)
        {
            double soilTemperaturePrevious = 0;
            double waterTemperaturePrevious = 0;

            var dailyClimateParameterList = new List<double>();
            var julianDays = this.GetJulianDays().ToList();

            for (var index = 0; index < julianDays.Count(); index++)
            {
                var julianDay = julianDays.ElementAt(index);
                var temperature = temperatures.ElementAt(index);
                var precipitation = precipitations.ElementAt(index);
                var evapotranspiration = evapotranspirations.ElementAt(index);

                var dailyClimateParameter = this.CalculateDailyClimateParameter(emergenceDay,
                                                                                ripeningDay,
                                                                                julianDay,
                                                                                temperature,
                                                                                precipitation,
                                                                                evapotranspiration,
                                                                                percentageSoilOrganicCarbon,
                                                                                variance,
                                                                                clay,
                                                                                sand,
                                                                                layerThicknessInMillimeters,
                                                                                yield,
                                                                                alfa,
                                                                                decompositionMinimumTemperature,
                                                                                decompositionMaximumTemperature,
                                                                                moistureResponseFunctionAtSaturation,
                                                                                moistureResponseFunctionAtWiltingPoint,
                                                                                ref soilTemperaturePrevious,
                                                                                ref waterTemperaturePrevious);

                dailyClimateParameterList.Add(dailyClimateParameter.ClimateParameter);
            }

            return dailyClimateParameterList;
        }

        public List<ClimateParameterDailyResult> CalculateDailyClimateParameterResults(
            int emergenceDay,
            int ripeningDay,
            double yield,
            double clay,
            double sand,
            double layerThicknessInMillimeters,
            double percentageSoilOrganicCarbon,
            double variance,
            double alfa,
            double decompositionMinimumTemperature,
            double decompositionMaximumTemperature,
            double moistureResponseFunctionAtWiltingPoint,
            double moistureResponseFunctionAtSaturation,
            List<double> evapotranspirations,
            List<double> precipitations,
            List<double> temperatures)
        {
            double soilTemperaturePrevious = 0;
            double waterTemperaturePrevious = 0;

            var dailyClimateParameterList = new List<ClimateParameterDailyResult>();
            var julianDays = this.GetJulianDays().ToList();

            for (var index = 0; index < temperatures.Count() && index < julianDays.Count(); index++)
            {
                var julianDay = julianDays.ElementAt(index);
                var temperature = temperatures.ElementAt(index);
                var precipitation = precipitations.ElementAt(index);
                var evapotranspiration = evapotranspirations.ElementAt(index);

                var dailyClimateParameter = this.CalculateDailyClimateParameter(emergenceDay,
                                                                                ripeningDay,
                                                                                julianDay,
                                                                                temperature,
                                                                                precipitation,
                                                                                evapotranspiration,
                                                                                percentageSoilOrganicCarbon,
                                                                                variance,
                                                                                clay,
                                                                                sand,
                                                                                layerThicknessInMillimeters,
                                                                                yield,
                                                                                alfa,
                                                                                decompositionMinimumTemperature,
                                                                                decompositionMaximumTemperature,
                                                                                moistureResponseFunctionAtSaturation,
                                                                                moistureResponseFunctionAtWiltingPoint,
                                                                                ref soilTemperaturePrevious,
                                                                                ref waterTemperaturePrevious);

                dailyClimateParameterList.Add(dailyClimateParameter);
            }

            return dailyClimateParameterList;
        }

        public double CalculateClimateParameter(
            int emergenceDay,
            int ripeningDay,
            double yield,
            double clay,
            double sand,
            double layerThicknessInMillimeters,
            double percentageSoilOrganicCarbon,
            double variance,
            double alfa,
            double decompositionMinimumTemperature,
            double decompositionMaximumTemperature,
            double moistureResponseFunctionAtWiltingPoint,
            double moistureResponseFunctionAtSaturation,
            List<double> evapotranspirations,
            List<double> precipitations,
            List<double> temperatures)
        {
            var dailyClimateParameterList = this.CalculateDailyClimateParameters(
                    emergenceDay,
                    ripeningDay,
                    yield,
                    clay,
                    sand,
                    layerThicknessInMillimeters,
                    percentageSoilOrganicCarbon,
                    variance,
                    alfa,
                    decompositionMinimumTemperature,
                    decompositionMaximumTemperature,
                    moistureResponseFunctionAtWiltingPoint,
                    moistureResponseFunctionAtSaturation,
                    evapotranspirations,
                    precipitations,
                    temperatures
                    );

            return dailyClimateParameterList.Average();
        }

        /// <summary>
        /// Equation 2.1.1-47
        /// </summary>
        public double CalculateClimateManagementFactor(double climateParameter, double tillageFactor)
        {
            var result = climateParameter * tillageFactor;

            return result;
        }

        /// <summary>
        /// Eq. 1.6.2-3
        /// </summary>
        /// <param name="maxTemperature">Maximum temperature of all temperatures considered</param>
        /// <param name="minTemperature">Minimum temperature of all temperatures considered</param>
        /// <param name="cropType">Crop you wish to calculate</param>
        /// <returns>Crop Coefficient</returns>
        public double CalculateCropCoefficient(double maxTemperature, double minTemperature, CropType cropType)
        {
            var cropCoefficient = _growingDegreeCropCoefficientsProvider.GetByCropType(cropType);

            var a = cropCoefficient.A;
            var b = cropCoefficient.B;
            var c = cropCoefficient.C;
            var d = cropCoefficient.D;
            var e = cropCoefficient.E;
            var Tbase = 5;

            // Calculation in the algorithm document assumes separate values (parameters) for maximum temperature and minimum temperature, if the values that the users passes in
            // are the same then the caller only has the mean value, in this case, don't recalculate the mean temperature
            var meanTemperature = (maxTemperature - minTemperature) / 2;
            if (Math.Abs(maxTemperature - minTemperature) < double.Epsilon)
            {
                meanTemperature = maxTemperature; // Use the max (min is the same at this point)
            }

            var innerTerm = meanTemperature - Tbase;
            var term2 = b * (innerTerm);
            var term3 = c * Math.Pow(innerTerm, 2);
            var term4 = d * Math.Pow(innerTerm, 3);
            var term5 = e * Math.Pow(innerTerm, 4);

            return a + term2 + term3 + term4 + term5;
        }


        /// <summary>
        /// Eq. 1.6.2-4
        /// </summary>
        /// <param name="maxTemp">Maximum temperature of all temps considered</param>
        /// <param name="minTemp">Minimum tempearture of all temps considered</param>
        /// <param name="cropType">The crop you wish to measure</param>
        /// <param name="meanDailyTemp">Mean daily temperature (C)</param>
        /// <param name="solarRadiaton">Solar radiation (MJ m^-2 day^-1)</param>
        /// <param name="relativeHumidity">Relative humidity(%)</param>
        /// <returns></returns>
        public double CalculateCropSpecificEvapotranspirationNoWaterAvailability(double maxTemp,
                                                                                 double minTemp,
                                                                                 CropType cropType,
                                                                                 double meanDailyTemp,
                                                                                 double solarRadiaton,
                                                                                 double relativeHumidity)

        {
            var cropCoefficient = CalculateCropCoefficient(maxTemp, minTemp, cropType);
            var evapotranspiration = _evapotranspirationCalculator.CalculateReferenceEvapotranspiration(meanDailyTemp, solarRadiaton, relativeHumidity);

            return cropCoefficient * evapotranspiration;
        }

        #endregion

        #region Private Methods

        private IEnumerable<int> GetJulianDays()
        {
            for (var i = 1; i <= CoreConstants.DaysInYear; i++)
            {
                yield return i;
            }
        }

        private ClimateParameterDailyResult CalculateDailyClimateParameter(int emergenceDay,
                                                                            int ripeningDay,
                                                                            int julianDay,
                                                                            double temperature,
                                                                            double precipitation,
                                                                            double evapotranspiration,
                                                                            double organicCarbon,
                                                                            double variance,
                                                                            double clay,
                                                                            double sand,
                                                                            double layerThickness,
                                                                            double yield,
                                                                            double alfa,
                                                                            double decompositionMinimumTemperature,
                                                                            double decompositionMaximumTemperature,
                                                                            double moistureResponseFunctionAtSaturation,
                                                                            double moistureResponseFunctionAtWiltingPoint,
                                                                            ref double soilTemperaturePrevious,
                                                                            ref double waterTemperaturePrevious)
        {
            var dailyClimateParameterResult = new ClimateParameterDailyResult();
            dailyClimateParameterResult.JulianDay = julianDay;
            dailyClimateParameterResult.InputPrecipitation = precipitation;
            dailyClimateParameterResult.InputEvapotranspiration = evapotranspiration;
            dailyClimateParameterResult.InputTemperature = temperature;

            var greenAreaIndexMax = this.CalculateGreenAreaIndexMax(yield);
            var midSeason = this.CalculateMidSeason(emergenceDay, ripeningDay);
            var greenAreaIndex = this.CalculateGreenAreaIndex(greenAreaIndexMax, julianDay, midSeason, variance);
            dailyClimateParameterResult.GreenAreaIndex = greenAreaIndex;

            var organicCarbonFactor = this.CalculateOrganicCarbonFactor(organicCarbon);
            var clayFactor = this.CalculateClayFactor(clay);
            var sandFactor = this.CalculateSandFactor(sand);

            var wiltingPointPercent = this.CalculateWiltingPointPercent(organicCarbonFactor, clayFactor, sandFactor);
            var wiltingPoint = this.CalculateWiltingPoint(wiltingPointPercent);
            dailyClimateParameterResult.WiltingPoint = wiltingPoint;

            var fieldCapacityPercent = this.CalculateFieldCapacityPercent(organicCarbonFactor, clayFactor, sandFactor);
            var fieldCapacity = this.CalculateFieldCapacity(fieldCapacityPercent);
            dailyClimateParameterResult.FieldCapacity = fieldCapacity;

            var soilMeanDepth = this.CalculateSoilMeanDepth(layerThickness);
            var leafAreaIndex = this.CalculateLeafAreaIndex(greenAreaIndex);

            var surfaceTemperature = this.CalculateSurfaceTemperature(temperature, leafAreaIndex);
            dailyClimateParameterResult.SurfaceTemperature = surfaceTemperature;

            var soilTemperatureCurrent = this.CalculateSoilTemperatures(julianDay, surfaceTemperature, soilMeanDepth, greenAreaIndex, ref soilTemperaturePrevious);
            dailyClimateParameterResult.SoilTemperature = soilTemperatureCurrent;

            var cropCoefficient = this.CalculateCropCoefficient(greenAreaIndex);
            dailyClimateParameterResult.CropCoefficient = cropCoefficient;

            var cropEvapotranspiration = this.CalculateCropEvapotranspiration(evapotranspiration, cropCoefficient);
            dailyClimateParameterResult.ReferenceEvapotranspiration = cropEvapotranspiration;

            var cropInterception = this.CalculateCropInterception(precipitation, greenAreaIndex, cropEvapotranspiration);
            dailyClimateParameterResult.CropInterception = cropInterception;

            var soilAvailableWater = this.CalculateSoilAvailableWater(precipitation, greenAreaIndex, cropEvapotranspiration);
            dailyClimateParameterResult.SoilAvailableWater = soilAvailableWater;

            var volumetricSoilWaterContent = this.CalculateVolumetricSoilWaterContent(ref waterTemperaturePrevious, layerThickness, fieldCapacity);
            dailyClimateParameterResult.VolumetricSoilWaterContent = volumetricSoilWaterContent;

            var soilCoefficient = this.CalculateSoilCoefficient(fieldCapacity, volumetricSoilWaterContent, wiltingPoint, alfa);
            var actualEvapotranspiration = this.CalculateActualEvapotranspiration(cropEvapotranspiration, soilCoefficient);
            dailyClimateParameterResult.ActualEvapotranspiration = actualEvapotranspiration;

            var deepPercolation = this.CalculateDeepPercolation(fieldCapacity, layerThickness, ref waterTemperaturePrevious);
            dailyClimateParameterResult.DeepPercolation = deepPercolation;

            var currentWaterStorage = this.CalculateJulianDayWaterStorage(fieldCapacity, layerThickness, julianDay, ref waterTemperaturePrevious, soilAvailableWater, actualEvapotranspiration);
            dailyClimateParameterResult.WaterStorage = currentWaterStorage;

            var temperatureResponseFactor = this.CalculateTemperatureResponseFactor(ref soilTemperaturePrevious, decompositionMinimumTemperature, decompositionMaximumTemperature);
            dailyClimateParameterResult.ClimateParamterTemperature = temperatureResponseFactor;

            var moistureResponseFactor = this.CalculateMoistureResponseFactor(volumetricSoilWaterContent, fieldCapacity, wiltingPoint, moistureResponseFunctionAtSaturation, moistureResponseFunctionAtWiltingPoint);
            dailyClimateParameterResult.ClimateParameterWater = moistureResponseFactor;

            var climateFactor = this.CalculateClimateFactor(moistureResponseFactor, temperatureResponseFactor);
            dailyClimateParameterResult.ClimateParameter = climateFactor;

            soilTemperaturePrevious = soilTemperatureCurrent;
            waterTemperaturePrevious = currentWaterStorage;

            return dailyClimateParameterResult;
        }

        /// <summary>
        /// Equation 2.1.1-1
        /// </summary>
        private double CalculateGreenAreaIndexMax(double yield)
        {
            var greenAreaIndexMaximum = 0.0731 * Math.Pow(yield / 1000, 2) + 0.408 * yield / 1000;

            return greenAreaIndexMaximum;
        }

        /// <summary>
        /// Equation 2.1.1-2
        /// </summary>
        private double CalculateMidSeason(int emergenceDay, int ripeningDay)
        {
            var midSeason = emergenceDay + (ripeningDay - emergenceDay) / 2;

            return midSeason;
        }

        /// <summary>
        /// Equation 2.1.1-3
        /// </summary>
        private double CalculateGreenAreaIndex(double greenAreaIndexMax, int julianDay, double midSeason,
                                               double variance)
        {
            var greenAreaIndex = greenAreaIndexMax * Math.Exp(-1 * Math.Pow(julianDay - midSeason, 2) / (2 * variance));

            return greenAreaIndex;
        }

        /// <summary>
        /// Equation 2.1.1-4
        /// </summary>
        private double CalculateOrganicCarbonFactor(double organicCarbon)
        {
            var organicCarbonFactor = -0.837531 + 0.430183 * organicCarbon;

            return organicCarbonFactor;
        }

        /// <summary>
        /// Equation 2.1.1-5
        /// </summary>
        private double CalculateClayFactor(double clay)
        {
            var clayFactor = -1.40744 + 0.0661969 * clay * 100;

            return clayFactor;
        }

        /// <summary>
        /// Equation 2.1.1-6
        /// </summary>
        private double CalculateSandFactor(double sand)
        {
            var sandFactor = -1.51866 + 0.0393284 * sand * 100;

            return sandFactor;
        }

        /// <summary>
        /// Equation 2.1.1-7
        /// </summary>
        private double CalculateWiltingPointPercent(double organicCarbon, double clayFactor, double sandFactor)
        {
            var wiltingPointPercent = 14.2568 + 7.36318 *
                                      (0.06865 + 0.108713 * organicCarbon -
                                       0.0157225 * Math.Pow(organicCarbon, 2) +
                                       0.00102805 * Math.Pow(organicCarbon, 3) +
                                       0.886569 * clayFactor -
                                       0.223581 * organicCarbon * clayFactor +
                                       0.0126379 * Math.Pow(organicCarbon, 2) * clayFactor -
                                       0.017059 * Math.Pow(clayFactor, 2) +
                                       0.0135266 * organicCarbon * Math.Pow(clayFactor, 2) -
                                       0.0334434 * Math.Pow(clayFactor, 3) -
                                       0.0535182 * sandFactor -
                                       0.0354271 * organicCarbon * sandFactor -
                                       0.00261313 * Math.Pow(organicCarbon, 2) * sandFactor -
                                       0.154563 * clayFactor * sandFactor -
                                       0.0160219 * organicCarbon * clayFactor * sandFactor -
                                       0.0400606 * Math.Pow(clayFactor, 2) * sandFactor -
                                       0.104875 * Math.Pow(sandFactor, 2) +
                                       0.0159857 * organicCarbon * Math.Pow(sandFactor, 2) -
                                       0.0671656 * clayFactor * Math.Pow(sandFactor, 2) -
                                       0.0260699 * Math.Pow(sandFactor, 3));

            return wiltingPointPercent;
        }

        /// <summary>
        /// Equation 2.1.1-8
        /// </summary>
        private double CalculateWiltingPoint(double wiltingPointPercent)
        {
            var wiltingPoint = wiltingPointPercent / 100;

            return wiltingPoint;
        }

        /// <summary>
        /// Equation 2.1.1-9
        /// </summary>
        private double CalculateFieldCapacityPercent(double organicCarbonFactor, double clayFactor, double sandFactor)
        {
            var fieldCapacityPercent = 29.7528 + 10.3544 * (
                                           0.0461615 + 0.290955 * organicCarbonFactor -
                                           0.0496845 * organicCarbonFactor * organicCarbonFactor +
                                           0.00704802 * organicCarbonFactor * organicCarbonFactor *
                                           organicCarbonFactor +
                                           0.269101 * clayFactor -
                                           0.176528 * organicCarbonFactor * clayFactor +
                                           0.0543138 * organicCarbonFactor * organicCarbonFactor * clayFactor +
                                           0.1982 * clayFactor * clayFactor -
                                           0.060699 * clayFactor * clayFactor * clayFactor -
                                           0.320249 * sandFactor -
                                           0.0111693 * organicCarbonFactor * organicCarbonFactor * sandFactor +
                                           0.14104 * clayFactor * sandFactor +
                                           0.0657345 * organicCarbonFactor * clayFactor * sandFactor -
                                           0.102026 * clayFactor * clayFactor * sandFactor -
                                           0.04012 * sandFactor * sandFactor +
                                           0.160838 * organicCarbonFactor * sandFactor * sandFactor -
                                           0.121392 * clayFactor * sandFactor * sandFactor -
                                           0.061667 * sandFactor * sandFactor * sandFactor);

            return fieldCapacityPercent;
        }

        /// <summary>
        /// Equation 2.1.1-10
        /// </summary>
        private double CalculateFieldCapacity(double fieldCapacityPercent)
        {
            var fieldCapacity = fieldCapacityPercent / 100;

            return fieldCapacity;
        }

        /// <summary>
        /// Equation 2.1.1-11
        /// </summary>
        private double CalculateSoilMeanDepth(double layerThickness)
        {
            var soilMeanDepth = layerThickness / 20;

            return soilMeanDepth;
        }

        /// <summary>
        /// Equation 2.1.1-12
        /// </summary>
        private double CalculateLeafAreaIndex(double greenAreaIndex)
        {
            var leafAreaIndex = 0.8 * greenAreaIndex;

            return leafAreaIndex;
        }

        /// <summary>
        /// Equation 2.1.1-13
        /// Equation 2.1.1-14
        /// </summary>
        private double CalculateSurfaceTemperature(double temperature, double leafAreaIndex)
        {
            var surfaceTemperature = 0D;
            if (temperature < 0)
            {
                surfaceTemperature = 0.20 * temperature;
            }
            else
            {
                surfaceTemperature = temperature * (0.95 + 0.05 * Math.Exp(-0.4 * (leafAreaIndex - 3)));
            }

            return surfaceTemperature;
        }

        /// <summary>
        /// Equation 2.1.1-15
        /// Equation 2.1.1-16
        /// </summary>
        private double CalculateSoilTemperatures(
            int julianDay,
            double surfaceTemperature,
            double soilMeanDepth,
            double greenAreaIndex,
            ref double soilTemperaturePrevious)
        {
            var currentSoilTemperature = 0d;

            if (julianDay == 1)
            {
                currentSoilTemperature = 0;
            }
            else
            {
                currentSoilTemperature = soilTemperaturePrevious + (surfaceTemperature - soilTemperaturePrevious) *
                                          0.24 * Math.Exp(-soilMeanDepth * 0.017) * Math.Exp(-0.15 * greenAreaIndex);
            }

            return currentSoilTemperature;
        }

        /// <summary>
        /// Equation 2.1.1-19
        /// </summary>
        private double CalculateCropCoefficient(double greenAreaIndex)
        {
            var cropCoefficient = 1.3 - (1.3 - 0.8) * Math.Exp(-0.17 * greenAreaIndex);

            return cropCoefficient;
        }

        /// <summary>
        /// Equation 2.1.1-20
        /// </summary>
        private double CalculateCropEvapotranspiration(double evapotranspiration, double cropCoefficient)
        {
            var cropEvapotranspiration = evapotranspiration * cropCoefficient;

            return cropEvapotranspiration;
        }

        /// <summary>
        /// Equation 2.1.1-21
        /// Equation 2.1.1-22
        /// Equation 2.1.1-23
        /// </summary>
        private double CalculateCropInterception(
            double totalDailyPrecipitation,
            double greenAreaIndex,
            double cropEvapotranspiration)
        {
            var cropInterception = 0d;
            if (totalDailyPrecipitation < 0.2 * greenAreaIndex)
            {
                cropInterception = totalDailyPrecipitation;
            }
            else
            {
                cropInterception = 0.2 * greenAreaIndex;
            }

            if (cropInterception > cropEvapotranspiration)
            {
                cropInterception = cropEvapotranspiration;
            }

            return cropInterception;
        }

        /// <summary>
        /// Equation 2.1.1-24
        /// </summary>
        private double CalculateSoilAvailableWater(
            double totalDailyPrecipitation, 
            double greenAreaIndex, 
            double cropEvapotranspiration)
        {
            var cropInterception = this.CalculateCropInterception(
                totalDailyPrecipitation: totalDailyPrecipitation,
                greenAreaIndex: greenAreaIndex,
                cropEvapotranspiration: cropEvapotranspiration);

            var soilAvailableWater = totalDailyPrecipitation - cropInterception;

            return soilAvailableWater;
        }

        /// <summary>
        /// Equation 2.1.1-25
        /// Equation 2.1.1-26
        /// </summary>
        private double CalculateVolumetricSoilWaterContent(ref double waterStoragePrevious,
                                                           double layerThickness,
                                                           double wiltingPoint)
        {
            var volumetricSoilWaterContent = waterStoragePrevious / layerThickness;

            if (Math.Abs(volumetricSoilWaterContent) < double.Epsilon)
            {
                volumetricSoilWaterContent = wiltingPoint;
            }

            return volumetricSoilWaterContent;
        }

        /// <summary>
        /// Equation 2.1.1-27
        /// Equation 2.1.1-28
        /// Equation 2.1.1-29
        /// </summary>
        private double CalculateSoilCoefficient(
            double fieldCapacity,
            double volumetricSoilWaterContent,
            double wiltingPoint,
            double alfa)
        {
            var soilCoefficient = 0d;

            var calculation =
                Math.Pow(
                         1 - (0.95 * fieldCapacity - volumetricSoilWaterContent) /
                         (0.95 * fieldCapacity - alfa * wiltingPoint), 2);
            soilCoefficient = calculation > 0 ? calculation : 0;

            if (soilCoefficient > 1)
            {
                return 1;
            }

            if (volumetricSoilWaterContent < alfa / 100 * wiltingPoint)
            {
                return 0;
            }

            return soilCoefficient;
        }

        /// <summary>
        /// Equation 2.1.1-30
        /// </summary>
        private double CalculateActualEvapotranspiration(double cropEvapotranspiration, double soilCoefficient)
        {
            var actualEvapotranspiration = soilCoefficient * cropEvapotranspiration;

            return actualEvapotranspiration;
        }

        /// <summary>
        /// Equation 2.1.1-33
        /// Equation 2.1.1-34
        /// </summary>
        private double CalculateDeepPercolation(
            double fieldCapacity,
            double layerThickness,
            ref double previousWaterStorage)
        {
            var deepPercolation = 0d;
            if (previousWaterStorage - fieldCapacity * layerThickness > 0)
            {
                deepPercolation = previousWaterStorage - fieldCapacity * layerThickness;
            }
            else
            {
                deepPercolation = 0;
            }

            return deepPercolation;
        }

        /// <summary>
        /// Equation 2.1.1-31
        /// Equation 2.1.1-32
        /// Equation 2.1.1-35
        /// </summary>
        private double CalculateJulianDayWaterStorage(
            double fieldCapacity,
            double layerThickness,
            int julianDay,
            ref double previousWaterStorage,
            double soilAvailableWater,
            double actualEvapotranspiration)
        {
            var currentWaterStorage = 0d;

            if (julianDay == 1)
            {
                currentWaterStorage = fieldCapacity * layerThickness;
                previousWaterStorage = fieldCapacity * layerThickness;
            }

            var deepPercolation = this.CalculateDeepPercolation(fieldCapacity, layerThickness, ref previousWaterStorage);

            currentWaterStorage = previousWaterStorage +
                                  soilAvailableWater -
                                  actualEvapotranspiration -
                                  deepPercolation;

            return currentWaterStorage;
        }

        /// <summary>
        /// Equation 2.1.1-36
        /// Equation 2.1.1-37
        /// </summary>
        private double CalculateTemperatureResponseFactor(
            ref double soilTemperaturePrevious,
            double decompositionMinimumTemperature,
            double decompositionMaximumTemperature)
        {
            var temperatureResponseFactor = Math.Pow(soilTemperaturePrevious - decompositionMinimumTemperature, 2) /
                                            Math.Pow(decompositionMaximumTemperature - decompositionMinimumTemperature,
                                                     2);

            if (soilTemperaturePrevious < -3.78)
            {
                temperatureResponseFactor = 0;
            }

            return temperatureResponseFactor;
        }

        /// <summary>
        /// Equation 2.1.1-38
        /// Equation 2.1.1-39
        /// Equation 2.1.1-40
        /// Equation 2.1.1-41
        /// Equation 2.1.1-42
        /// Equation 2.1.1-43
        /// </summary>
        private double CalculateMoistureResponseFactor(
            double volumetricWaterContent,
            double fieldCapacity,
            double wiltingPoint,
            double referenceSaturationPoint,
            double referenceWiltingPoint)
        {
            var saturationPoint = 1.2 * fieldCapacity;
            var optimumWaterContent = 0.9 * fieldCapacity;

            var moistureResponseFactor = 0d;
            if (volumetricWaterContent > optimumWaterContent)
            {
                moistureResponseFactor = (1 - referenceSaturationPoint) *
                                         ((volumetricWaterContent - optimumWaterContent) /
                                          (optimumWaterContent - saturationPoint)) + 1;
            }

            if (volumetricWaterContent < wiltingPoint)
            {
                moistureResponseFactor = referenceWiltingPoint * volumetricWaterContent /
                                         wiltingPoint;
            }

            if (wiltingPoint <= volumetricWaterContent && volumetricWaterContent <= optimumWaterContent)
            {
                moistureResponseFactor = (1 - referenceWiltingPoint) *
                                         ((volumetricWaterContent - wiltingPoint) /
                                          (optimumWaterContent - wiltingPoint)) +
                                         referenceWiltingPoint;
            }

            if (moistureResponseFactor > 1)
            {
                moistureResponseFactor = 1;
            }

            if (moistureResponseFactor < 0)
            {
                moistureResponseFactor = 0;
            }

            return moistureResponseFactor;
        }

        /// <summary>
        /// Equation 2.1.1-44
        /// Equation 2.1.1-45
        /// Equation 2.1.1-46
        /// </summary>
        private double CalculateClimateFactor(double moistureResponseFactor, double temperatureResponseFactor)
        {
            var climateParameterAlpha = moistureResponseFactor * temperatureResponseFactor;
            var climateParameterDaily = climateParameterAlpha / 0.10516;

            return climateParameterDaily;
        }

        #endregion

        #region Event Handlers

        #endregion
    }
}