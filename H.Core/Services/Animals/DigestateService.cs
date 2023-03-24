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

        private readonly List<DigestateTank> _digestateTanks;
        private readonly IADCalculator _adCalculator;
        private readonly IAnimalService _animalResultsService;

        #endregion

        #region Constructors

        public DigestateService(IADCalculator adCalculator, IAnimalService animalService)
        {
            if (adCalculator != null)
            {
                _adCalculator = adCalculator;
            }
            else
            {
                throw new ArgumentNullException(nameof(adCalculator));
            }

            if (animalService != null)
            {
                _animalResultsService = animalService;
            }
            else
            {
                throw new ArgumentNullException(nameof(animalService));
            }

            _digestateTanks = new List<DigestateTank>();
        }

        #endregion

        #region Public Methods

        public void Initialize(Farm farm)
        {
        }

        public DateTime GetDateOfMaximumAvailableDigestate(Farm farm, DigestateState state, int year)
        {
            var tankStates = this.GetTankStates(farm);
            switch (state)
            {
                case DigestateState.Raw:
                    return tankStates.Where(x => x.AsOfDate.Year == year).OrderBy(x => x.TotalRawDigestateAvailable).Last().AsOfDate;

                case DigestateState.LiquidPhase:
                    return tankStates.Where(x => x.AsOfDate.Year == year).OrderBy(x => x.TotalLiquidDigestateAvailable).Last().AsOfDate;

                default:
                    return tankStates.Where(x => x.AsOfDate.Year == year).OrderBy(x => x.TotalSolidDigestateAvailable).Last().AsOfDate;
            }
        }

        public List<DigestorDailyOutput> GetDailyResults(Farm farm)
        {
            var animalComponentResults = _animalResultsService.GetAnimalResults(farm);
            var adResults = _adCalculator.CalculateResults(farm, animalComponentResults);

            return adResults;
        }

        public List<DigestateTank> GetTankStates(Farm farm)
        {
            var dailyOutputs = this.GetDailyResults(farm);

            var component = farm.AnaerobicDigestionComponents.SingleOrDefault();
            if (component == null)
            {
                return new List<DigestateTank>();
            }

            return this.GetTankStates(dailyOutputs, farm, component);
        }

        public DigestateTank GetTank(Farm farm, DateTime targetDate)
        {
            var tanks = this.GetTankStates(farm);

            return tanks.SingleOrDefault(x =>  x.AsOfDate.Date.Equals(targetDate.Date));
        }

        public List<DigestateTank> GetTankStates(
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
                    AsOfDate = outputDate,
                };

                result.Add(tank);

                /*
                 * Calculate raw amount available
                 */

                var totalRawDigestateOnThisDay = outputs.FlowRateOfAllSubstratesInDigestate;
                var totalRawDigesteUsedForFieldApplications = this.GetTotalAmountOfDigesateAppliedOnDay(outputDate, farm, DigestateState.Raw);
                var totalRawDigestateFromPreviousDay = i == 0 ? 0 : result.ElementAt(i - 1).TotalRawDigestateAvailable;
                var totalRawProduced = totalRawDigestateOnThisDay + totalRawDigestateFromPreviousDay;
                var totalRawDigestateAvailableAfterFieldApplications = totalRawProduced - totalRawDigesteUsedForFieldApplications;

                // There should only be raw amounts if there is no separation performed
                tank.TotalRawDigestateAvailable = component.IsLiquidSolidSeparated == false ? totalRawDigestateAvailableAfterFieldApplications : 0;
                tank.TotalRawDigestateProduced = component.IsLiquidSolidSeparated == false ? totalRawProduced : 0;

                /*
                 * Calculate liquid fraction amount available
                 */

                var totalLiquidFractionOnThisDay = outputs.FlowRateLiquidFraction;
                var totalLiquidDigestateUsedForFieldApplications = this.GetTotalAmountOfDigesateAppliedOnDay(outputDate, farm, DigestateState.LiquidPhase);
                var totalLiquidDigestateFromPreviousDay = i == 0 ? 0 : result.ElementAt(i - 1).TotalLiquidDigestateAvailable;
                var totalLiquidProduced = totalLiquidFractionOnThisDay + totalLiquidDigestateFromPreviousDay;
                var totalLiquidDigestateAvailalbleAfterFieldApplications = totalLiquidProduced - totalLiquidDigestateUsedForFieldApplications;

                // There should only be liquid amounts if separation is performed
                tank.TotalLiquidDigestateAvailable = component.IsLiquidSolidSeparated ? totalLiquidDigestateAvailalbleAfterFieldApplications : 0;
                tank.TotalLiquidDigestateProduced = component.IsLiquidSolidSeparated ? totalLiquidProduced : 0;

                /*
                 * Calculate solid fraction amount available
                 */

                var totalSolidFractionOnThisDay = outputs.FlowRateSolidFraction;
                var totalSolidDigestateUsedForFieldApplications = this.GetTotalAmountOfDigesateAppliedOnDay(outputDate, farm, DigestateState.SolidPhase);
                var totalSolidDigestateFromPreviousDay = i == 0 ? 0 : result.ElementAt(i - 1).TotalSolidDigestateAvailable;
                var totalSolidProduced = totalSolidFractionOnThisDay + totalSolidDigestateFromPreviousDay;
                var totalSolidDigestateAvailalbleAfterFieldApplications = totalSolidProduced - totalSolidDigestateUsedForFieldApplications;

                // There should only be liquid amounts if separation is performed
                tank.TotalSolidDigestateAvailable = component.IsLiquidSolidSeparated ? totalSolidDigestateAvailalbleAfterFieldApplications : 0;
                tank.TotalSolidDigestateProduced = component.IsLiquidSolidSeparated ? totalSolidProduced : 0;
            }

            return result;
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