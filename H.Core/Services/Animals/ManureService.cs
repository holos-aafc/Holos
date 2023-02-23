using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Services.Animals
{
    /// <summary>
    /// Keep manure calculations separate from <see cref="FarmResultsService"/>
    /// </summary>
    public class ManureService : IManureService
    {
        #region Fields

        private readonly IAnimalService _animalService;
        private readonly List<ManureTank> _manureTanks;

        #endregion

        #region Constructors

        public ManureService(IAnimalService animalService)
        {
            if (animalService != null)
            {
                _animalService = animalService;
            }
            else
            {
                throw new ArgumentNullException(nameof(animalService));
            }

            _manureTanks = new List<ManureTank>();
        }

        #endregion

        #region Public Methods

        public void CalculateResults(Farm farm)
        {
            foreach (var manureTank in _manureTanks.ToList())
            {
                this.ResetTank(manureTank, farm);
                this.UpdateAmountsUsed(manureTank, farm);
            }
        }

        public ManureTank GetTank(AnimalType animalType, int year, Farm farm)
        {
            var tank = this.GetManureTankInternal(animalType, year);
            this.ResetTank(tank, farm);
            this.UpdateAmountsUsed(tank, farm);

            return tank;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the amounts of manure (and amounts of applied nitrogen) whenever a manure application is added, removed, or updated.
        /// </summary>
        /// <param name="manureTank">The <see cref="ManureTank"/> that will have volume and nitrogen amounts subtracted from</param>
        /// <param name="farm">The <see cref="Farm"/> where the application was made</param>
        private void UpdateAmountsUsed(ManureTank manureTank, Farm farm)
        {
            // Iterate over each field and total the land applied manure
            foreach (var farmFieldSystemComponent in farm.FieldSystemComponents)
            {
                foreach (var cropViewItem in farmFieldSystemComponent.CropViewItems)
                {
                    foreach (var manureApplicationViewItem in cropViewItem.ManureApplicationViewItems)
                    {
                        // If the manure was imported from off-farm, we don't update/reduce the amounts in the storage tanks
                        if (manureApplicationViewItem.IsImportedManure())
                        {
                            continue;
                        }

                        // Account for the total nitrogen that was applied and removed from the tank
                        var amountOfNitrogenAppliedPerHectare = manureApplicationViewItem.AmountOfNitrogenAppliedPerHectare;
                        var totalAmountOfNitrogen = amountOfNitrogenAppliedPerHectare * cropViewItem.Area;
                        manureTank.NitrogenSumOfAllManureApplicationsMade += totalAmountOfNitrogen;

                        // Account for the total volume that was applied and removed from the tank
                        var amountOfManureAppliedPerHectare = manureApplicationViewItem.AmountOfManureAppliedPerHectare;
                        var totalVolume = amountOfManureAppliedPerHectare * cropViewItem.Area;
                        manureTank.VolumeSumOfAllManureApplicationsMade += totalVolume;
                    }
                }
            }
        }

        /// <summary>
        /// Set the state of the manure tank as if there had been no field applications made on the farm.
        /// </summary>
        /// <param name="manureTank">The <see cref="ManureTank"/> that should have all properties reset</param>
        /// <param name="animalComponentResults">The results that will be used to set the starting state of the tank</param>
        private void SetStartingStateOfManureTank(
           ManureTank manureTank,
           List<AnimalComponentEmissionsResults> animalComponentResults)
        {
            manureTank.ResetTank();

            var targetGroupEmissions = this.GetTargetEmissions(animalComponentResults, manureTank.Year);
            foreach (var groupEmissionsByMonth in targetGroupEmissions)
            {
                manureTank.TotalOrganicNitrogenAvailableForLandApplication += groupEmissionsByMonth.MonthlyOrganicNitrogenAvailableForLandApplication;
                manureTank.TotalTanAvailableForLandApplication += groupEmissionsByMonth.MonthlyTanAvailableForLandApplication;
                manureTank.TotalAmountOfCarbonInStoredManure += groupEmissionsByMonth.TotalAmountOfCarbonInStoredManure;

                // Before any nitrogen from any manure applications have been subtracted from the tank, these two values will be the same
                manureTank.TotalNitrogenAvailableForLandApplication += groupEmissionsByMonth.MonthlyNitrogenAvailableForLandApplication;
                manureTank.TotalNitrogenAvailableAfterAllLandApplications += groupEmissionsByMonth.MonthlyNitrogenAvailableForLandApplication;

                // Before any volume of manure from field applications have been subtracted from the tank, these two values will be the same
                manureTank.VolumeOfManureAvailableForLandApplication += groupEmissionsByMonth.TotalVolumeOfManureAvailableForLandApplication * 1000;
                manureTank.VolumeRemainingInTank += groupEmissionsByMonth.TotalVolumeOfManureAvailableForLandApplication * 1000;
            }
        }

        private void ResetTank(ManureTank manureTank, Farm farm)
        {
            var years = new List<int>();
            foreach (var animalComponent in farm.AnimalComponents)
            {
                foreach (var animalComponentGroup in animalComponent.Groups)
                {
                    foreach (var managementPeriod in animalComponentGroup.ManagementPeriods)
                    {
                        for (int i = managementPeriod.Start.Year; i <= managementPeriod.End.Year; i++)
                        {
                            years.Add(i);
                        }
                    }
                }
            }

            var animalType = manureTank.AnimalType;
            var distinctYears = years.Distinct().ToList();

            var resultsForType = _animalService.GetAnimalResults(animalType, farm);

            foreach (var year in distinctYears)
            {
                var tank = this.GetManureTankInternal(animalType, year);
                this.SetStartingStateOfManureTank(tank, resultsForType);
            }
        }

        /// <summary>
        /// Get the tank for the year that the application was made.
        /// </summary>
        /// <param name="animalType">The tank associated with this type of animal</param>
        /// <param name="year">The year of the tank</param>
        /// <returns>The manure tank associated with animal type and the year</returns>
        private ManureTank GetManureTankInternal(AnimalType animalType, int year)
        {
            var tank = _manureTanks.SingleOrDefault(x => x.AnimalType.GetCategory() == animalType.GetCategory() && x.Year == year);
            if (tank == null)
            {
                // If no tank exists for this year, create one now
                tank = new ManureTank() { AnimalType = animalType, Year = year };

                _manureTanks.Add(tank);
            }

            return tank;
        }

        /// <summary>
        /// Returns all <see cref="GroupEmissionsByMonth"/> that are not housing animals on pasture and the result of management periods that have the same year as the <see cref="ManureTank"/>.
        /// </summary>
        /// <param name="animalComponentEmissionsResults">The emission results for all components with the same livestock type</param>
        /// <param name="yearOfTank">The year of <see cref="ManureTank"/></param>
        /// <returns>A list of <see cref="GroupEmissionsByMonth"/> that match the above criteria</returns>
        private List<GroupEmissionsByMonth> GetTargetEmissions(List<AnimalComponentEmissionsResults> animalComponentEmissionsResults, int yearOfTank)
        {
            var result = new List<GroupEmissionsByMonth>();

            foreach (var animalComponentEmissionsResult in animalComponentEmissionsResults)
            {
                foreach (var allGroupEmissions in animalComponentEmissionsResult.EmissionResultsForAllAnimalGroupsInComponent)
                {
                    foreach (var groupEmissionsByMonth in allGroupEmissions.GroupEmissionsByMonths)
                    {
                        if (groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.HousingDetails.HousingType.IsPasture() == false && groupEmissionsByMonth.MonthsAndDaysData.Year == yearOfTank)
                        {
                            result.Add(groupEmissionsByMonth);
                        }
                    }
                }
            }

            return result;
        }

        #endregion
    }
}