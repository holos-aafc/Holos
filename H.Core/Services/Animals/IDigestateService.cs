using System;
using System.Collections.Generic;
using H.Core.Calculators.Infrastructure;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.Animals
{
    public interface IDigestateService
    {
        DateTime GetDateOfMaximumAvailableDigestate(Farm farm, DigestateState state, int year,
            List<DigestorDailyOutput> digestorDailyOutputs, bool subtractFieldAppliedAmounts);
        DigestateTank GetTank(Farm farm, DateTime targetDate, List<DigestorDailyOutput> dailyOutputs);
        List<DigestorDailyOutput> GetDailyResults(Farm farm);

        /// <summary>
        /// Returns the total amount of N applied (to the entire field) from a digestate field application.
        /// 
        /// (kg N)
        /// </summary>
        double CalculateTotalNitrogenFromDigestateApplication(
            CropViewItem cropViewItem,
            DigestateApplicationViewItem digestateApplicationViewItem,
            DigestateTank tank);

        /// <summary>
        /// (kg C)
        /// </summary>
        double CalculateTotalCarbonFromDigestateApplication(
            CropViewItem cropViewItem,
            DigestateApplicationViewItem digestateApplicationViewItem,
            DigestateTank tank);

        /// <summary>
        /// Equation 4.6.1-4
        /// 
        /// Stored nitrogen available for application to land minus digestate applied to fields or exported
        ///
        /// (kg N)
        /// </summary>
        double GetTotalNitrogenRemainingAtEndOfYear(int year, Farm farm);

        double GetTotalNitrogenExported(int year, Farm farm);

        /// <summary>
        /// Equation 4.9.7-3
        /// </summary>
        double GetTotalCarbonRemainingAtEndOfYear(int year, Farm farm, AnaerobicDigestionComponent component);
        double GetTotalAmountOfDigestateAppliedOnDay(DateTime dateTime, Farm farm, DigestateState state);

        /// <summary>
        /// Equation 4.9.7-5
        /// </summary>
        double GetTotalCarbonForField(CropViewItem cropViewItem, int year, Farm farm, AnaerobicDigestionComponent component);

        List<AnimalComponentEmissionsResults> AnimalResults { get; set; }
    }
}