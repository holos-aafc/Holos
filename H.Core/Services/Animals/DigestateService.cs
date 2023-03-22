using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using H.Core.Calculators.Infrastructure;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;

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

        public void UpdateAmountsUsed(DigestateTank tank, Farm farm)
        {
            foreach (var fieldSystemComponent in farm.FieldSystemComponents)
            {
                foreach (var cropViewItem in fieldSystemComponent.CropViewItems)
                {
                    foreach (var digestateApplicationViewItem in cropViewItem.DigestateApplicationViewItems)
                    {
                        var amountAppliedPerHectare = digestateApplicationViewItem.AmountAppliedPerHectare;
                        var totalVolume = amountAppliedPerHectare * cropViewItem.Area;
                        tank.VolumeSumOfAllManureApplicationsMade += totalVolume;
                    }
                }
            }
        }

        public DigestateTank GetDigestateTankInternal(int year, DigestateState state)
        {
            var tank = _digestateTanks.SingleOrDefault(x => x.Year == year && x.DigestateState == state);
            if (tank == null)
            {
                // If no tank exists for this year, create one now
                tank = new DigestateTank() { Year = year, DigestateState = state };

                _digestateTanks.Add(tank);
            }

            return tank;
        }

        public void ReduceTankByDigestateApplications(
            Farm farm,
            DigestateTank tank)
        {
            var totalAmountUsedFromAllApplications = 0d;

            foreach (var fieldSystemComponent in farm.FieldSystemComponents)
            {
                foreach (var cropViewItem in fieldSystemComponent.CropViewItems)
                {
                    if (cropViewItem.Year != tank.Year)
                    {
                        continue;
                    }

                    foreach (var digestateApplicationViewItem in cropViewItem.DigestateApplicationViewItems.Where(x => x.DigestateState == tank.DigestateState))
                    {
                        var totalAmount = digestateApplicationViewItem.AmountAppliedPerHectare * cropViewItem.Area;

                        totalAmountUsedFromAllApplications += totalAmount;
                    }
                }
            }

            tank.TotalDigestateAfterAllApplications -= totalAmountUsedFromAllApplications;
            // set date of digestate application to maximum by default
        }

        public DigestateTank GetTank(
            Farm farm, 
            int year, 
            DigestateState state,
            List<DigestorDailyOutput> dailyDigestorResults)
        {
            var tank = this.GetDigestateTankInternal(year, state);

            this.SetStartingStateOfTank(tank, dailyDigestorResults, farm);
            this.ReduceTankByDigestateApplications(farm, tank);

            return tank;
        }

        public double MaximumAmountOfDigestateAvailableForLandApplication(DateTime dateTime, Farm farm, DigestateState state)
        {
            var dailyResults = this.GetDailyResults(farm);
            var tank = this.GetTank(farm, dateTime.Year, state, dailyResults);

            return tank.TotalDigestateAfterAllApplications;
        }

        /// <summary>
        /// Set the state of all results of the digestate tank to 0 and recalculates the results again. This method does not consider any field applications of digestate
        /// made by the user.
        /// </summary>
        /// <param name="tank">The <see cref="DigestateTank"/> that will be reset.</param>
        /// <param name="results">The daily results from anaerobic digestor.</param>
        /// <param name="farm">The farm containing the <see cref="H.Core.Models.Infrastructure.AnaerobicDigestionComponent"/></param>
        public void SetStartingStateOfTank(
            DigestateTank tank,
            List<DigestorDailyOutput> results,
            Farm farm)
        {
            tank.ResetTank();

            var resultsByYear = results.Where(x => x.Date.Year == tank.Year);

            var flowRateOfAllSubstrates = resultsByYear.Sum(x => x.FlowRateOfAllSubstratesInDigestate);
            var flowRateOfLiquidFraction = results.Sum(x => x.FlowRateLiquidFraction);
            var flowRateOfSolidFraction = results.Sum(x => x.FlowRateSolidFraction);

            switch (tank.DigestateState)
            {
                case DigestateState.SolidPhase:
                    {
                        tank.TotalDigestateAfterAllApplications = flowRateOfSolidFraction;
                    }
                    break;

                case DigestateState.LiquidPhase:
                    {
                        tank.TotalDigestateAfterAllApplications = flowRateOfLiquidFraction;
                    }
                    break;

                default:
                    {
                        tank.TotalDigestateAfterAllApplications = flowRateOfAllSubstrates;
                    }
                    break;
            }

            // This property to the gauge view now (maximum value of gauge) // Before digestate applications have been considered, these two will be equal
            tank.TotalDigestateProducedBySystem = tank.TotalDigestateAfterAllApplications;
        }

        public List<DigestorDailyOutput> GetDailyResults(Farm farm)
        {
            var animalComponentResults = _animalResultsService.GetAnimalResults(farm);
            var adResults = _adCalculator.CalculateResults(farm, animalComponentResults);

            return adResults;
        }

        #endregion

        #region Private Methods

        #endregion
    }
}