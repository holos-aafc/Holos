using System;
using H.Core.Enumerations;
using H.Core.Models;

namespace H.Core.Services.Animals
{
    public interface IDigestateService
    {
        void Initialize(Farm farm);
        double MaximumAmountOfDigestateAvailableForLandApplication(DateTime dateTime, Farm farm, DigestateState digestateState);
        DigestateTank GetTank(Farm farm, DateTime dateTime, DigestateState state);
    }
}