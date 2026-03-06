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

        List<ManureLocationSourceType> GetValidDigestateLocationSourceTypes();

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
        double GetTotalNitrogenRemainingAtEndOfYearAfterFieldApplications(int year, Farm farm);

        double GetTotalNitrogenExported(int year, Farm farm);

        /// <summary>
        /// Equation 4.9.7-3
        /// </summary>
        double GetTotalCarbonRemainingAtEndOfYear(int year, Farm farm, AnaerobicDigestionComponent component);
        double GetTotalAmountOfDigestateAppliedOnDay(DateTime dateTime, Farm farm, DigestateState state,
            ManureLocationSourceType sourceLocation);

        /// <summary>
        /// Equation 4.9.7-5
        /// </summary>
        double GetTotalCarbonForField(CropViewItem cropViewItem, int year, Farm farm, AnaerobicDigestionComponent component);

        double GetTotalNitrogenCreatedNotIncludingFieldApplicationRemovals(int year, Farm farm);
        double GetTotalCarbonCreatedNotIncludingFieldApplicationRemovals(int year, Farm farm);
        double GetTotalDigestateCarbonInputsForField(Farm farm, int year, CropViewItem viewItem);
        List<DigestorDailyOutput> Initialize(Farm farm, List<AnimalComponentEmissionsResults> results);

        /// <summary>
        /// Recalculates the AmountOfNitrogenAppliedPerHectare and AmountOfCarbonAppliedPerHectare for all digestate
        /// applications on all fields of the given farm. This ensures that stored N/ha and C/ha values are consistent
        /// with the current AD substrate composition (e.g. after food waste or other substrates are added/removed).
        ///
        /// This method fixes the stale-value bug where digestate application N/ha and C/ha were only computed when
        /// the user interacted with the specific application item in the UI, and were never refreshed when AD
        /// substrates changed. Calling this from <see cref="Initialize"/> ensures existing farm files are corrected
        /// on the next model run.
        /// </summary>
        void RecalculateDigestateApplicationAmounts(Farm farm);

        /// <summary>
        /// Overload that accepts freshly-computed daily AD results. Use this when the cached _dailyResults
        /// in the service may be stale (e.g. when navigating back to a field after modifying AD substrates).
        /// </summary>
        void RecalculateDigestateApplicationAmounts(Farm farm, List<DigestorDailyOutput> dailyResults);
    }
}