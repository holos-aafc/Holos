using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Calculators.Infrastructure;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;

namespace H.Core.Services.Animals
{
    public class DigestateService
    {
        #region Fields

        private readonly List<DigestateTank> _digestateTanks;
        private readonly ADCalculator _adCalculator;
        private readonly AnimalResultsService _animalResultsService;

        #endregion

        #region Constructors

        public DigestateService()
        {
            _digestateTanks = new List<DigestateTank>();
            _adCalculator = new ADCalculator();
            _animalResultsService = new AnimalResultsService();
        }

        #endregion

        #region Public Methods

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

        public void ResetTank(DigestateTank tank, Farm farm)
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

            var animalComponetResults = _animalResultsService.GetAnimalResults(farm);
            var adResults = _adCalculator.CalculateResults(farm, animalComponetResults);

            foreach (var distinctYear in distinctYears)
            {
                var digestateTank = this.GetDigestateTankInternal(distinctYear, tank.DigestateState);
                this.SetStartingStateOfTank(digestateTank, adResults);
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

        public void SetStartingStateOfTank(
            DigestateTank tank,
            List<DigestorDailyOutput> results)
        {
            tank.ResetTank();

            foreach (var digestorDailyOutput in results)
            {
                tank.TotalAvailableManureNitrogenAvailableForLandApplication += digestorDailyOutput.TotalNitrogenInDigestateAvailableForLandApplication;
                tank.TotalAmountOfCarbonInStoredManure += digestorDailyOutput.TotalCarbonInDigestateAvailableForLandApplication;
            }
        }

        #endregion

        #region Private Methods

        #endregion
    }
}