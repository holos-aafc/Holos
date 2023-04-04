using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using H.Core.Calculators.Infrastructure;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals.Sheep;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.Animals
{
    public class DigestateService : IDigestateService
    {
        #region Fields

        private readonly ADCalculator _adCalculator = new ADCalculator();
        private readonly IAnimalService _animalService = new AnimalResultsService();

        #endregion

        #region Public Methods

        public List<DigestorDailyOutput> GetDailyResults(Farm farm)
        {
            var animalResults = _animalService.GetAnimalResults(farm);
            var dailyResults = _adCalculator.CalculateResults(farm, animalResults);

            return dailyResults;
        }

        public DateTime GetDateOfMaximumAvailableDigestate(Farm farm, DigestateState state, int year, List<DigestorDailyOutput> digestorDailyOutputs)
        {
            var tankStates = this.GetDailyTankStates(farm, digestorDailyOutputs);
            if (tankStates.Any() == false)
            {
                // No management periods selected for input into AD system
                return DateTime.Now;
            }

            switch (state)
            {
                case DigestateState.Raw:
                    return tankStates.Where(x => x.DateCreated.Year == year).OrderBy(x => x.TotalRawDigestateAvailable).Last().DateCreated;

                case DigestateState.LiquidPhase:
                    return tankStates.Where(x => x.DateCreated.Year == year).OrderBy(x => x.TotalLiquidDigestateAvailable).Last().DateCreated;

                default:
                    return tankStates.Where(x => x.DateCreated.Year == year).OrderBy(x => x.TotalSolidDigestateAvailable).Last().DateCreated;
            }
        }

        public List<DigestateTank> GetDailyTankStates(Farm farm, List<DigestorDailyOutput> dailyOutputs)
        {
            var component = farm.AnaerobicDigestionComponents.SingleOrDefault();
            if (component == null)
            {
                return new List<DigestateTank>();
            }

            return this.GetDailyTankStates(dailyOutputs, farm, component);
        }

        public DigestateTank GetTank(Farm farm, DateTime targetDate, List<DigestorDailyOutput> dailyOutputs)
        {
            var tanks = this.GetDailyTankStates(farm, dailyOutputs);
            var result = tanks.SingleOrDefault(x => x.DateCreated.Date.Equals(targetDate.Date));
            if (result != null)
            {
                return result;
            }
            else
            {
                return new DigestateTank()
                {
                    DateCreated = targetDate,
                };
            }
        }

        public List<DigestateTank> GetDailyTankStates(
            List<DigestorDailyOutput> digestorDailyOutputs, 
            Farm farm, 
            AnaerobicDigestionComponent component)
        {
            var result = new List<DigestateTank>();

            for (int i = 0; i < digestorDailyOutputs.Count; i++)
            {
                var outputs = digestorDailyOutputs.ElementAt(i);
                var outputDate = outputs.Date;

                var tank = new DigestateTank
                {
                    DateCreated = outputDate,
                };

                result.Add(tank);

                /*
                 * Calculate raw amounts available
                 */

                // Raw digestate
                var totalRawDigestateOnThisDay = outputs.FlowRateOfAllSubstratesInDigestate;
                var totalRawDigesteUsedForFieldApplications = this.GetTotalAmountOfDigesateAppliedOnDay(outputDate, farm, DigestateState.Raw);
                var totalRawDigestateFromPreviousDay = i == 0 ? 0 : result.ElementAt(i - 1).TotalRawDigestateAvailable;
                var totalRawProduced = totalRawDigestateOnThisDay + totalRawDigestateFromPreviousDay;
                var totalRawDigestateAvailableAfterFieldApplications = totalRawProduced - totalRawDigesteUsedForFieldApplications;

                // Nitrogen from raw digestate
                var totalNitrogenFromRawDigestateOnThisDay = outputs.TotalAmountOfNitrogenFromRawDigestateAvailableForLandApplication;
                var totalNitrogenFromRawDigestateFromPreviousDay = i == 0 ? 0 : result.ElementAt(i - 1).NitrogenFromRawDigestate;
                var totalNitrogenFromRawDigestateAvailable = totalNitrogenFromRawDigestateOnThisDay + totalNitrogenFromRawDigestateFromPreviousDay;

                // Carbon from raw digestate
                var totalCarbonFromRawDigestateOnThisDay = outputs.TotalAmountOfCarbonInRawDigestateAvailableForLandApplication;
                var totalCarbonFromRawDigestateFromPreviousDay = i == 0 ? 0 : result.ElementAt(i - 1).CarbonFromRawDigestate;
                var totalCarbonFromRawDigestateAvailable = totalCarbonFromRawDigestateOnThisDay + totalCarbonFromRawDigestateFromPreviousDay;

                // There should only be raw amounts if there is no separation performed
                if (component.IsLiquidSolidSeparated == false)
                {
                    tank.TotalRawDigestateAvailable = totalRawDigestateAvailableAfterFieldApplications;
                    tank.TotalRawDigestateProduced = totalRawProduced;
                    tank.NitrogenFromRawDigestate = totalNitrogenFromRawDigestateAvailable;
                    tank.CarbonFromRawDigestate = totalCarbonFromRawDigestateAvailable;
                }

                /*
                 * Calculate liquid amounts available
                 */

                var totalLiquidFractionOnThisDay = outputs.FlowRateLiquidFraction;
                var totalLiquidDigestateUsedForFieldApplications = this.GetTotalAmountOfDigesateAppliedOnDay(outputDate, farm, DigestateState.LiquidPhase);
                var totalLiquidDigestateFromPreviousDay = i == 0 ? 0 : result.ElementAt(i - 1).TotalLiquidDigestateAvailable;
                var totalLiquidProduced = totalLiquidFractionOnThisDay + totalLiquidDigestateFromPreviousDay;
                var totalLiquidDigestateAvailalbleAfterFieldApplications = totalLiquidProduced - totalLiquidDigestateUsedForFieldApplications;

                // Nitrogen from liquid digestate
                var totalNitrogenFromLiquidDigestateOnThisDay = outputs.TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromLiquidFraction;
                var totalNitrogenFromLiquidDigestateFromPreviousDay = i == 0 ? 0 : result.ElementAt(i - 1).NitrogenFromLiquidDigestate;
                var totalNitrogenFromLiquidDigestateAvailable = totalNitrogenFromLiquidDigestateOnThisDay + totalNitrogenFromLiquidDigestateFromPreviousDay;

                // Carbon from liquid digestate
                var totalCarbonFromLiquidDigestateOnThisDay = outputs.TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromLiquidFraction;
                var totalCarbonFromLiquidDigestateFromPreviousDay = i == 0 ? 0 : result.ElementAt(i - 1).CarbonFromLiquidDigestate;
                var totalCarbonFromLiquidDigestateAvailable = totalCarbonFromLiquidDigestateOnThisDay + totalCarbonFromLiquidDigestateFromPreviousDay;

                // There should only be liquid amounts if separation is performed
                if (component.IsLiquidSolidSeparated)
                {
                    tank.TotalLiquidDigestateAvailable = totalLiquidDigestateAvailalbleAfterFieldApplications;
                    tank.TotalLiquidDigestateProduced = totalLiquidProduced;
                    tank.NitrogenFromLiquidDigestate = totalNitrogenFromLiquidDigestateAvailable;
                    tank.CarbonFromLiquidDigestate = totalCarbonFromLiquidDigestateAvailable;
                }

                /*
                 * Calculate solid amounts available
                 */

                var totalSolidFractionOnThisDay = outputs.FlowRateSolidFraction;
                var totalSolidDigestateUsedForFieldApplications = this.GetTotalAmountOfDigesateAppliedOnDay(outputDate, farm, DigestateState.SolidPhase);
                var totalSolidDigestateFromPreviousDay = i == 0 ? 0 : result.ElementAt(i - 1).TotalSolidDigestateAvailable;
                var totalSolidProduced = totalSolidFractionOnThisDay + totalSolidDigestateFromPreviousDay;
                var totalSolidDigestateAvailalbleAfterFieldApplications = totalSolidProduced - totalSolidDigestateUsedForFieldApplications;

                // Nitrogen from solid digestate
                var totalNitrogenFromSolidDigestateOnThisDay = outputs.TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromSolidFraction;
                var totalNitrogenFromSolidDigestateFromPreviousDay = i == 0 ? 0 : result.ElementAt(i - 1).NitrogenFromSolidDigestate;
                var totalNitrogenFromSolidDigestateAvailable = totalNitrogenFromSolidDigestateOnThisDay + totalNitrogenFromSolidDigestateFromPreviousDay;

                // Carbon from solid digestate
                var totalCarbonFromSolidDigestateOnThisDay = outputs.TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromSolidFraction;
                var totalCarbonFromSolidDigestateFromPreviousDay = i == 0 ? 0 : result.ElementAt(i - 1).CarbonFromSolidDigestate;
                var totalCarbonFromSolidDigestateAvailable = totalCarbonFromSolidDigestateOnThisDay + totalCarbonFromSolidDigestateFromPreviousDay;

                // There should only be liquid amounts if separation is performed
                if (component.IsLiquidSolidSeparated)
                {
                    tank.TotalSolidDigestateAvailable = totalSolidDigestateAvailalbleAfterFieldApplications;
                    tank.TotalSolidDigestateProduced = totalSolidProduced;
                    tank.NitrogenFromSolidDigestate = totalNitrogenFromSolidDigestateAvailable;
                    tank.CarbonFromSolidDigestate = totalCarbonFromSolidDigestateAvailable;
                }
            }

            return result;
        }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double CalculateTotalNitrogenFromDigestateApplication(
            CropViewItem cropViewItem,
            DigestateApplicationViewItem digestateApplicationViewItem,
            DigestateTank tank)
        {
            var amountApplied = digestateApplicationViewItem.AmountAppliedPerHectare;
            var totalAmountApplied = amountApplied * cropViewItem.Area;

            var totalDigestateCreatedOnDay = 0d;
            var totalNitrogenAvailableOnDay = 0d;
            switch (digestateApplicationViewItem.DigestateState)
            {
                case DigestateState.Raw:
                    totalDigestateCreatedOnDay = tank.TotalRawDigestateProduced;
                    totalNitrogenAvailableOnDay = tank.NitrogenFromRawDigestate;
                    break;

                case DigestateState.SolidPhase:
                    totalDigestateCreatedOnDay = tank.TotalSolidDigestateProduced;
                    totalNitrogenAvailableOnDay = tank.NitrogenFromSolidDigestate;
                    break;

                default:
                    totalDigestateCreatedOnDay = tank.TotalLiquidDigestateProduced;
                    totalNitrogenAvailableOnDay = tank.NitrogenFromLiquidDigestate;
                    break;
            }

            var fraction = totalAmountApplied / totalDigestateCreatedOnDay;

            var amountOfNitrogen = fraction * totalNitrogenAvailableOnDay;

            return amountOfNitrogen;
        }

        /// <summary>
        /// (kg C)
        /// </summary>
        public double CalculateTotalCarbonFromDigestateApplication(
            CropViewItem cropViewItem,
            DigestateApplicationViewItem digestateApplicationViewItem,
            DigestateTank tank)
        {
            var amountApplied = digestateApplicationViewItem.AmountAppliedPerHectare;
            var totalAmountApplied = amountApplied * cropViewItem.Area;

            var totalDigestateCreatedOnDay = 0d;
            var totalCarbonAvailableOnDay = 0d;
            switch (digestateApplicationViewItem.DigestateState)
            {
                case DigestateState.Raw:
                    totalDigestateCreatedOnDay = tank.TotalRawDigestateProduced;
                    totalCarbonAvailableOnDay = tank.CarbonFromRawDigestate;
                    break;

                case DigestateState.SolidPhase:
                    totalDigestateCreatedOnDay = tank.TotalSolidDigestateProduced;
                    totalCarbonAvailableOnDay = tank.CarbonFromSolidDigestate;
                    break;

                default:
                    totalDigestateCreatedOnDay = tank.TotalLiquidDigestateProduced;
                    totalCarbonAvailableOnDay = tank.CarbonFromLiquidDigestate;
                    break;
            }

            var fraction = totalAmountApplied / totalDigestateCreatedOnDay;

            var amountOfCarbon = fraction * totalCarbonAvailableOnDay;

            return amountOfCarbon;
        }

        public double GetTotalNitrogenRemainingAtEndOfYear(int year, Farm farm, AnaerobicDigestionComponent component)
        {
            var dailyResults = this.GetDailyResults(farm);
            if (dailyResults.Any() == false)
            {
                return 0;
            }

            var dateOfLastOutput = dailyResults.Max(x => x.Date);
            var tank = this.GetTank(farm, dateOfLastOutput, dailyResults);

            var totalNitrogen = 0d;
            if (component.IsLiquidSolidSeparated)
            {
                // Total remaining N from liquid and solid fractions
                totalNitrogen = tank.NitrogenFromLiquidDigestate + tank.NitrogenFromSolidDigestate;
            }
            else
            {
                totalNitrogen = tank.NitrogenFromRawDigestate;
            }

            return totalNitrogen;
        }

        #endregion

        #region Private Methods

        private double GetTotalAmountOfDigesateAppliedOnDay(DateTime dateTime, Farm farm, DigestateState state)
        {
            var result = 0d;

            foreach (var farmFieldSystemComponent in farm.FieldSystemComponents)
            {
                foreach (var cropViewItem in farmFieldSystemComponent.CropViewItems)
                {
                    foreach (var digestateApplicationViewItem in cropViewItem.DigestateApplicationViewItems)
                    {
                        if (digestateApplicationViewItem.DateCreated.Date == dateTime.Date && digestateApplicationViewItem.DigestateState == state)
                        {
                            result += digestateApplicationViewItem.AmountAppliedPerHectare * cropViewItem.Area;
                        }
                    }
                }
            }

            return result;
        }

        #endregion
    }
}