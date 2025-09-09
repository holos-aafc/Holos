﻿using System;
using System.Diagnostics;
using System.Linq;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.LandManagement;

namespace H.Core.Calculators.Climate
{
    public class ClimateService : IClimateService
    {
        #region Constructors

        public ClimateService()
        {
            _irrigationService = new IrrigationService();
            _climateParameterCalculator = new ClimateParameterCalculator();
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Calculate climate parameter. Will use custom climate data if it exists for the farm, otherwise will use SLC normals
        ///     for climate data.
        /// </summary>
        public double CalculateClimateParameter(CropViewItem viewItem, Farm farm)
        {
            if (farm.ClimateData.DailyClimateData.Any())
            {
                var climateDataGroupedByYear =
                    farm.ClimateData.DailyClimateData.GroupBy(userClimateData => userClimateData.Year);
                var climateDataForYear =
                    climateDataGroupedByYear.SingleOrDefault(groupingByYear => groupingByYear.Key == viewItem.Year);
                var climateParameter = 0d;

                if (climateDataForYear != null)
                {
                    // Use daily climate data
                    var precipitationList = climateDataForYear.OrderBy(climateData => climateData.JulianDay)
                        .Select(climateData => climateData.MeanDailyPrecipitation).ToList();

                    // Add irrigation amounts to daily precipitations
                    var totalPrecipitationList =
                        _irrigationService.AddIrrigationToDailyPrecipitations(precipitationList, farm, viewItem);

                    var temperatureList = climateDataForYear.OrderBy(climateData => climateData.JulianDay)
                        .Select(climateData => climateData.MeanDailyAirTemperature).ToList();
                    var evapotranspirationList = climateDataForYear.OrderBy(climateData => climateData.JulianDay)
                        .Select(climateData => climateData.MeanDailyPET).ToList();

                    climateParameter = _climateParameterCalculator.CalculateClimateParameterForYear(
                        farm,
                        viewItem,
                        evapotranspirationList,
                        totalPrecipitationList,
                        temperatureList);
                }
                else
                {
                    // If user has entered custom climate data but their input file has no data for a particular year, then use normals for that particular year

                    // Add irrigation amounts to daily precipitations
                    var totalPrecipitationList =
                        _irrigationService.AddIrrigationToDailyPrecipitations(
                            farm.ClimateData.PrecipitationData.GetAveragedYearlyValues(), farm, viewItem);

                    climateParameter = _climateParameterCalculator.CalculateClimateParameterForYear(
                        farm,
                        viewItem,
                        farm.ClimateData.EvapotranspirationData.GetAveragedYearlyValues(),
                        totalPrecipitationList,
                        farm.ClimateData.TemperatureData.GetAveragedYearlyValues());
                }

                return Math.Round(climateParameter, CoreConstants.DefaultNumberOfDecimalPlaces);
            }

            {
                // Add irrigation amounts to daily precipitations
                var totalPrecipitationList =
                    _irrigationService.AddIrrigationToDailyPrecipitations(
                        farm.ClimateData.PrecipitationData.GetAveragedYearlyValues(), farm, viewItem);

                // Use SLC normals when there is no custom user climate data
                Trace.TraceWarning(
                    $"{nameof(FieldResultsService)}: No custom daily climate data exists for this farm. Defaulting to SLC climate normals (and averaged daily values)");

                var result = _climateParameterCalculator.CalculateClimateParameterForYear(
                    farm,
                    viewItem,
                    farm.ClimateData.EvapotranspirationData.GetAveragedYearlyValues(),
                    totalPrecipitationList,
                    farm.ClimateData.TemperatureData.GetAveragedYearlyValues());

                return Math.Round(result, CoreConstants.DefaultNumberOfDecimalPlaces);
            }
        }

        #endregion

        #region Fields

        private readonly IrrigationService _irrigationService;
        private readonly IClimateParameterCalculator _climateParameterCalculator;

        #endregion
    }
}