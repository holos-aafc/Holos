using System.Collections.Generic;
using H.Core.Models;

namespace H.Core.Services
{
    public interface IInitializationService
    {
        void CheckInitialization(Farm farm);
        void ReInitializeFarms(IEnumerable<Farm> farms);
    }
}