using System.Collections.Generic;
using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class HousingTypeProvider
    {
        public List<HousingType> GetValidHousingTypes(AnimalType animalType)
        {
            if (animalType.IsPoultryType())
                return new List<HousingType>
                {
                    HousingType.HousedInBarn
                };

            if (animalType.IsSwineType())
                return new List<HousingType>
                {
                    HousingType.HousedInBarn,
                    HousingType.Pasture
                };

            if (animalType.IsSheepType())
                return new List<HousingType>
                {
                    HousingType.Confined,
                    HousingType.Pasture,
                    HousingType.HousedEwes
                };

            if (animalType.IsOtherAnimalType())
                return new List<HousingType>
                {
                    HousingType.HousedInBarn,
                    HousingType.ConfinedNoBarn,
                    HousingType.Pasture
                };

            if (animalType.IsDairyCattleType())
                return new List<HousingType>
                {
                    HousingType.FreeStallBarnSolidLitter,
                    HousingType.FreeStallBarnSlurryScraping,
                    HousingType.FreeStallBarnFlushing,

                    HousingType.FreeStallBarnMilkParlourSlurryFlushing,

                    HousingType.TieStallSolidLitter,
                    HousingType.TieStallSlurry,

                    HousingType.DryLot,
                    HousingType.Pasture
                };

            if (animalType.IsBeefCattleType())
                return new List<HousingType>
                {
                    HousingType.ConfinedNoBarn,
                    HousingType.HousedInBarnSolid,
                    HousingType.Pasture
                };

            return new List<HousingType>();
        }
    }
}