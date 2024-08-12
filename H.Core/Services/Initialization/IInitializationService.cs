using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using H.Core.Providers.Carbon;
using H.Core.Services.Initialization.Animals;
using H.Core.Services.Initialization.Crops;

namespace H.Core.Services.Initialization
{
    public interface IInitializationService : ICropInitializationService, IAnimalInitializationService
    {
        void CheckInitialization(Farm farm);
        void ReInitializeFarms(IEnumerable<Farm> farms);
    }
}