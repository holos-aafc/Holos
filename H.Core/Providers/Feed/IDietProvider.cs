using System.Collections.Generic;
using H.Core.Enumerations;

namespace H.Core.Providers.Feed
{
    public interface IDietProvider
    {
        List<Diet> GetDiets();
        Diet GetNoDiet();
        List<AnimalType> GetValidAnimalDietTypes(AnimalType animalType);
    }
}