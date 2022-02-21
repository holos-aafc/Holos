using System.Collections.Generic;
using H.Core.Emissions.Results;
using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Services.Animals
{
    public interface IAnimalResultsService
    {
        IList<AnimalGroupEmissionResults> CalculateResultsForComponent(AnimalComponentBase animalComponent, Farm farm);
        List<AnimalComponentEmissionsResults> CalculateResultsForAnimalComponents(IEnumerable<AnimalComponentBase> components, Farm farm);
    }
}