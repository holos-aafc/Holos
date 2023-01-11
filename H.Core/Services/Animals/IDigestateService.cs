using System;
using H.Core.Enumerations;
using H.Core.Models;

namespace H.Core.Services.Animals
{
    public interface IDigestateService
    {
        void Initialize(Farm farm);
        double MaximumAmountOfNitrogenAvailablePerDay(DateTime dateTime, Farm farm);
        DigestateTank GetTank(Farm farm, DateTime dateTime, DigestateState state);
    }
}