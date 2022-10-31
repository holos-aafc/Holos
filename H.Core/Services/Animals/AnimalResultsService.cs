using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Services.Animals
{
    public class AnimalResultsService
    {
        #region Fields
        
        private readonly OtherLivestockResultsService _otherLivestockResultsService = new OtherLivestockResultsService();
        private readonly SwineResultsService _swineResultsService = new SwineResultsService();
        private readonly PoultryResultsService _poultryResultsService = new PoultryResultsService();
        private readonly BeefCattleResultsService _beefCattleResultsService = new BeefCattleResultsService();
        private readonly DairyCattleResultsService _dairyCattleResultsService = new DairyCattleResultsService();
        private readonly SheepResultsService _sheepResultsService = new SheepResultsService();

        #endregion

        #region MyRegion
        
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

        #endregion

        public AnimalGroupEmissionResults GetResultsForGroup(AnimalGroup selectedAnimalGroup, Farm farm, AnimalComponentBase animalComponentBase)
        {
            var animalType = selectedAnimalGroup.GroupType;
            
            if (animalType.GetCategory() == AnimalType.Beef)
            {
                return _beefCattleResultsService.GetResultsForGroup(selectedAnimalGroup, farm, animalComponentBase);
            }
            else if (animalType.GetCategory() == AnimalType.OtherLivestock)
            {
                return _otherLivestockResultsService.GetResultsForGroup(selectedAnimalGroup, farm, animalComponentBase);
            }
            else if (animalType.GetCategory() == AnimalType.Swine)
            {
                return _swineResultsService.GetResultsForGroup(selectedAnimalGroup, farm, animalComponentBase);
            }
            else if (animalType.GetCategory() == AnimalType.Poultry)
            {
                return _poultryResultsService.GetResultsForGroup(selectedAnimalGroup, farm, animalComponentBase);
            }
            else if (animalType.GetCategory() == AnimalType.Sheep)
            {
                return _sheepResultsService.GetResultsForGroup(selectedAnimalGroup, farm, animalComponentBase);
            }

            // Dairy
            return _dairyCattleResultsService.GetResultsForGroup(selectedAnimalGroup, farm, animalComponentBase);
        }
    }
}