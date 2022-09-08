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
        private readonly OtherLivestockResultsService _otherLivestockResultsService = new OtherLivestockResultsService();
        private readonly SwineResultsService _swineResultsService = new SwineResultsService();
        private readonly PoultryResultsService _poultryResultsService = new PoultryResultsService();
        private readonly BeefCattleResultsService _beefCattleResultsService = new BeefCattleResultsService();
        private readonly DairyCattleResultsService _dairyCattleResultsService = new DairyCattleResultsService();
        private readonly SheepResultsService _sheepResultsService = new SheepResultsService();

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
    }
}