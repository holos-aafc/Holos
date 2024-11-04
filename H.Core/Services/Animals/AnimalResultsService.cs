using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.Initialization;

namespace H.Core.Services.Animals
{

    public class AnimalResultsService : IAnimalService
    {
        #region Fields

        private readonly OtherLivestockResultsService _otherLivestockResultsService = new OtherLivestockResultsService();
        private readonly SwineResultsService _swineResultsService = new SwineResultsService();
        private readonly PoultryResultsService _poultryResultsService = new PoultryResultsService();
        private readonly BeefCattleResultsService _beefCattleResultsService = new BeefCattleResultsService();
        private readonly DairyCattleResultsService _dairyCattleResultsService = new DairyCattleResultsService();
        private readonly SheepResultsService _sheepResultsService = new SheepResultsService();

        #endregion

        #region Constructors

        public AnimalResultsService()
        {
        }

        #endregion

        #region Public Methods

        public List<AnimalComponentEmissionsResults> GetAnimalResults(Farm farm)
        {
            var results = new List<AnimalComponentEmissionsResults>();

            results.AddRange(_otherLivestockResultsService.CalculateResultsForAnimalComponents(farm.OtherLivestockComponents.Cast<AnimalComponentBase>(), farm));
            results.AddRange(_swineResultsService.CalculateResultsForAnimalComponents(farm.SwineComponents.Cast<AnimalComponentBase>(), farm));
            results.AddRange(_poultryResultsService.CalculateResultsForAnimalComponents(farm.PoultryComponents.Cast<AnimalComponentBase>(), farm));
            results.AddRange(_sheepResultsService.CalculateResultsForAnimalComponents(farm.SheepComponents.Cast<AnimalComponentBase>(), farm));
            results.AddRange(_dairyCattleResultsService.CalculateResultsForAnimalComponents(farm.DairyComponents.Cast<AnimalComponentBase>(), farm));
            results.AddRange(_beefCattleResultsService.CalculateResultsForAnimalComponents(farm.BeefCattleComponents.Cast<AnimalComponentBase>(), farm));

            return results;
        }

        public List<AnimalComponentEmissionsResults> GetAnimalResults(AnimalType animalType, Farm farm)
        {
            var results = new List<AnimalComponentEmissionsResults>();

            if (animalType.GetCategory() == AnimalType.Beef)
            {
                results.AddRange(_beefCattleResultsService.CalculateResultsForAnimalComponents(farm.BeefCattleComponents.Cast<AnimalComponentBase>(), farm));
            }
            else if (animalType.GetCategory() == AnimalType.OtherLivestock)
            {
                results.AddRange(_otherLivestockResultsService.CalculateResultsForAnimalComponents(farm.OtherLivestockComponents.Cast<AnimalComponentBase>(), farm));
            }
            else if (animalType.GetCategory() == AnimalType.Swine)
            {
                results.AddRange(_swineResultsService.CalculateResultsForAnimalComponents(farm.SwineComponents.Cast<AnimalComponentBase>(), farm));
            }
            else if (animalType.GetCategory() == AnimalType.Poultry)
            {
                results.AddRange(_poultryResultsService.CalculateResultsForAnimalComponents(farm.PoultryComponents.Cast<AnimalComponentBase>(), farm));
            }
            else if (animalType.GetCategory() == AnimalType.Sheep)
            {
                results.AddRange(_sheepResultsService.CalculateResultsForAnimalComponents(farm.SheepComponents.Cast<AnimalComponentBase>(), farm));
            }
            else
            {
                results.AddRange(_dairyCattleResultsService.CalculateResultsForAnimalComponents(farm.DairyComponents.Cast<AnimalComponentBase>(), farm));
            }

            return results;
        }

        public AnimalGroupEmissionResults GetResultsForGroup(AnimalGroup animalGroup, Farm farm, AnimalComponentBase animalComponent)
        {
            var animalType = animalGroup.GroupType;

            if (animalType.GetCategory() == AnimalType.Beef)
            {
                return _beefCattleResultsService.GetResultsForGroup(animalGroup, farm, animalComponent);
            }
            else if (animalType.GetCategory() == AnimalType.OtherLivestock)
            {
                return _otherLivestockResultsService.GetResultsForGroup(animalGroup, farm, animalComponent);
            }
            else if (animalType.GetCategory() == AnimalType.Swine)
            {
                return _swineResultsService.GetResultsForGroup(animalGroup, farm, animalComponent);
            }
            else if (animalType.GetCategory() == AnimalType.Poultry)
            {
                return _poultryResultsService.GetResultsForGroup(animalGroup, farm, animalComponent);
            }
            else if (animalType.GetCategory() == AnimalType.Sheep)
            {
                return _sheepResultsService.GetResultsForGroup(animalGroup, farm, animalComponent);
            }

            // Dairy
            return _dairyCattleResultsService.GetResultsForGroup(animalGroup, farm, animalComponent);
        }

        public AnimalGroupEmissionResults GetResultsForManagementPeriod(AnimalGroup animalGroup, Farm farm, AnimalComponentBase animalComponent, ManagementPeriod managementPeriod)
        {
            var animalType = animalGroup.GroupType;

            AnimalGroupEmissionResults result = null;
            if (animalType.GetCategory() == AnimalType.Beef)
            {
                result = _beefCattleResultsService.GetResultsForManagementPeriod(animalGroup, managementPeriod, animalComponent, farm);
            }
            else if (animalType.GetCategory() == AnimalType.OtherLivestock)
            {
                result = _otherLivestockResultsService.GetResultsForManagementPeriod(animalGroup, managementPeriod, animalComponent, farm);
            }
            else if (animalType.GetCategory() == AnimalType.Swine)
            {
                result = _swineResultsService.GetResultsForManagementPeriod(animalGroup, managementPeriod, animalComponent, farm);
            }
            else if (animalType.GetCategory() == AnimalType.Poultry)
            {
                result = _poultryResultsService.GetResultsForManagementPeriod(animalGroup, managementPeriod, animalComponent, farm);
            }
            else if (animalType.GetCategory() == AnimalType.Sheep)
            {
                result = _sheepResultsService.GetResultsForManagementPeriod(animalGroup, managementPeriod, animalComponent, farm);
            }
            else
            {
                result = _dairyCattleResultsService.GetResultsForManagementPeriod(animalGroup, managementPeriod, animalComponent, farm);
            }

            return result;
        }

        public List<GroupEmissionsByMonth> GetGroupEmissionsFromGrazingAnimals(
            List<AnimalComponentEmissionsResults> results,
            GrazingViewItem grazingViewItem)
        {
            var result = new List<GroupEmissionsByMonth>();

            // Get all animal components that have been placed on this field for grazing.
            var animalComponentEmissionsResults = results.SingleOrDefault(x => x.Component.Guid == grazingViewItem.AnimalComponentGuid);
            if (animalComponentEmissionsResults != null)
            {
                //Get all animal groups that have been placed on this field for grazing.
                var groupEmissionResults = animalComponentEmissionsResults.EmissionResultsForAllAnimalGroupsInComponent.SingleOrDefault(x => x.AnimalGroup.Guid == grazingViewItem.AnimalGroupGuid);
                if (groupEmissionResults != null)
                {
                    // Get emissions from the group when they are placed on pasture (housing type is pasture)
                    foreach (var groupEmissionsByMonth in groupEmissionResults.GroupEmissionsByMonths)
                    {
                        if (groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.HousingDetails.HousingType.IsPasture())
                        {
                            var start = groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.Start;
                            var end = groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.End;

                            if (start >= grazingViewItem.Start && end <= grazingViewItem.End)
                            {
                                result.Add(groupEmissionsByMonth);
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Selects the management periods and associated emissions for animals that are grazing on pasture according to Chapter 11/Appendix methodology
        /// </summary>
        public List<ManagementPeriod> GetGrazingManagementPeriods(
            AnimalGroup animalGroup,
            FieldSystemComponent fieldSystemComponent)
        {
            var managementPeriods = animalGroup.ManagementPeriods.ToList();
            var grazingPeriods = managementPeriods.Where(x => fieldSystemComponent.IsGrazingManagementPeriodFromPasture(x)).ToList();
            return grazingPeriods;

        }

        #endregion
    }
}