using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Climate
{
    public interface IClimateParameterCalculator
    {
        double CalculateClimateParameter(int emergenceDay,
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
                                         List<double> temperatures);

        double CalculateClimateManagementFactor(double climateParameter, double tillageFactor);

        List<ClimateParameterDailyResult> CalculateDailyClimateParameterResults(
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
            List<double> temperatures);

        double CalculateManagementFactor(
            double climateParameter,
            double tillageFactor);

        double CalculateClimateParameterForYear(
            Farm farm,
            CropViewItem cropViewItem,
            List<double> evapotranspirations,
            List<double> precipitations, 
            List<double> temperatures);

        List<ClimateParameterDailyResult> CalculateDailyClimateParameters(
            Farm farm,
            CropViewItem cropViewItem,
            List<double> dailyEvapotranspiration,
            List<double> dailyPrecipitation,
            List<double> dailyTemperature);

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
        double CalculateCropSpecificEvapotranspirationNoWaterAvailability(double maxTemp,
                                                                                          double minTemp,
                                                                                          CropType cropType,
                                                                                          double meanDailyTemp,
                                                                                          double solarRadiaton,
                                                                                          double relativeHumidity);
    }
}