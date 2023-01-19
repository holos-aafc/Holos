using H.Core.Enumerations;
using H.Core.Models;

namespace H.Core.Services.Animals
{
    public interface IManureService
    {
        void CalculateResults(Farm farm);
        ManureTank GetTank(AnimalType animalType, int year, Farm farm);
    }
}