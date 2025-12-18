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
            List<double> temperatures, List<double> dailyMinimumTemperatures, List<double> dailyMaximumTemperatures,
            CropType cropType);

        double CalculateClimateManagementFactor(double climateParameter, double tillageFactor);

        List<ClimateParameterDailyResult> CalculateDailyClimateParameterResults(int emergenceDay,
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
            List<double> temperatures, List<double> dailyMinimumTemperatures, List<double> dailyMaximumTemperatures,
            CropType cropType);

        double CalculateManagementFactor(
            double climateParameter,
            double tillageFactor);

        double CalculateClimateParameterForYear(Farm farm,
            CropViewItem cropViewItem,
            List<double> evapotranspirations,
            List<double> precipitations,
            List<double> temperatures, List<double> dailyMinimumTemperatures, List<double> dailyMaximumTemperatures);

        List<ClimateParameterDailyResult> CalculateDailyClimateParameters(Farm farm,
            CropViewItem cropViewItem,
            List<double> dailyEvapotranspiration,
            List<double> dailyPrecipitation,
            List<double> dailyTemperature, List<double> dailyMinimumTemperature, List<double> dailyMaximumTemperature);

        /// <summary>
        /// Equation 1.6.2-5
        /// </summary>
        /// <param name="cropType"></param>
        /// <param name="greenAreaIndex"></param>
        /// <param name="maximumTemperatureForDay"></param>
        /// <param name="minimumTemperatureForDay"></param>
        /// <param name="meanTemperatureForDay"></param>
        /// <returns></returns>
        double CalculateCropCoefficient(
            CropType cropType,
            double greenAreaIndex,
            double maximumTemperatureForDay,
            double minimumTemperatureForDay, 
            double meanTemperatureForDay);

        /// <summary>
        /// Equation 1.6.2-3
        /// </summary>
        double CalculateKcForAnnualCrops(double a, double b, double c, double d, double e, double temperatureTerm, double Tbase);

        /// <summary>
        /// Equation 1.6.2-4
        /// </summary>
        double CalculateKcForPerennialCrops(double a, double b, double temperatureTerm, double Tbase);
    }
}