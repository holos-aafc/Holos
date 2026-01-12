using System.Collections.Generic;
using H.Core.Models;
using H.Core.Services.Initialization.Animals;
using H.Core.Services.Initialization.Crops;

namespace H.Core.Services.Initialization
{
    public interface IInitializationService : ICropInitializationService, IAnimalInitializationService
    {
        void ReInitializeFarms(IEnumerable<Farm> farms);
    }
}