using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public void ResetAllTanks(Farm farm)
        {
            var years = new List<int>();

            foreach (var animalComponent in farm.AnimalComponents)
            {
                foreach (var animalGroup in animalComponent.Groups)
                {
                    foreach (var managementPeriod in animalGroup.ManagementPeriods.Where(x => x.ManureDetails.StateType == ManureStateType.AnaerobicDigester))
                    {
                        for (int i = managementPeriod.Start.Year; i < managementPeriod.End.Year; i++)
                        {
                            years.Add(i);
                        }
                    }
                }
            }

            var distinctYears = years.Distinct().ToList();
            var adResults = this.GetDailyResults(farm);

            foreach (var distinctYear in distinctYears)
            {
                foreach (var digestateState in new List<DigestateState>() {DigestateState.LiquidPhase, DigestateState.Raw, DigestateState.SolidPhase})
                {
                    var digestateTank = this.GetDigestateTankInternal(distinctYear, digestateState);
                    this.SetStartingStateOfTank(digestateTank, adResults, farm);
                }
            }
        }

        public DigestateTank GetDigestateTankInternal(int year, DigestateState state)
        {
            var tank = _digestateTanks.SingleOrDefault(x => x.Year == year && x.DigestateState == state);
            if (tank == null)
            {
                // If no tank exists for this year, create one now
                tank = new DigestateTank() {Year = year, DigestateState = state};

                _digestateTanks.Add(tank);
            }

            return tank;
        }

        public void ReduceTankByDigestateApplications(
            Farm farm, 
            DigestateTank tank)
        {
            var totalNitrogenUsedFromAllApplications = 0d;
            var totalCarbonUsedFromAllApplications = 0d;

            foreach (var fieldSystemComponent in farm.FieldSystemComponents)
            {
                foreach (var cropViewItem in fieldSystemComponent.CropViewItems)
                {
                    foreach (var digestateApplicationViewItem in cropViewItem.DigestateApplicationViewItems)
                    {
                        totalNitrogenUsedFromAllApplications += digestateApplicationViewItem.AmountOfNitrogenAppliedPerHectare * cropViewItem.Area;
                        totalCarbonUsedFromAllApplications += digestateApplicationViewItem.AmountOfCarbonAppliedPerHectare * cropViewItem.Area;
                    }
                }
            }

            tank.TotalNitrogenAvailableForLandApplication -= totalNitrogenUsedFromAllApplications;
            tank.TotalAmountOfCarbonInStoredManure -= totalCarbonUsedFromAllApplications;
            tank.NitrogenSumOfAllManureApplicationsMade = totalNitrogenUsedFromAllApplications;
        }

        public DigestateTank GetTank(Farm farm, DateTime dateTime, DigestateState state)
        {
            var adResults = this.GetDailyResults(farm);

            var tank = this.GetDigestateTankInternal(dateTime.Year, state);
            this.SetStartingStateOfTank(tank, adResults, farm);
            this.ReduceTankByDigestateApplications(farm, tank);

            return tank;
        }

        public double MaximumAmountOfNitrogenAvailablePerDay(DateTime dateTime, Farm farm)
        {
            var results = this.GetDailyResults(farm);

            return this.MaximumAmountOfNitrogenAvailablePerDay(dateTime, results);
        }

        public double MaximumAmountOfNitrogenAvailablePerDay(DateTime dateTime, List<DigestorDailyOutput> digestorDailyOutputs)
        {
            var resultsUpToThisDay = digestorDailyOutputs.Where(x => x.Date.Year == dateTime.Year && x.Date.Date < dateTime.Date).ToList();

            // This is the average amount of N produced by the digestor produced per day.
            var averageAvailableNitrogen = resultsUpToThisDay.Average(x => x.TotalNitrogenInDigestateAvailableForLandApplication);

            return averageAvailableNitrogen;
        }

        public void SetStartingStateOfTank(DigestateTank tank,
            List<DigestorDailyOutput> results, 
            Farm farm)
        {
            tank.ResetTank();

            foreach (var digestorDailyOutput in results)
            {
                tank.TotalNitrogenAvailableForLandApplication += digestorDailyOutput.TotalNitrogenInDigestateAvailableForLandApplication;
                tank.TotalAmountOfCarbonInStoredManure += digestorDailyOutput.TotalCarbonInDigestateAvailableForLandApplication;
            }
        }

        #endregion

        #region Private Methods

        private List<DigestorDailyOutput> GetDailyResults(Farm farm)
        {
            var animalComponentResults = _animalResultsService.GetAnimalResults(farm);
            var adResults = _adCalculator.CalculateResults(farm, animalComponentResults);

            return adResults;
        }

        #endregion
    }
}