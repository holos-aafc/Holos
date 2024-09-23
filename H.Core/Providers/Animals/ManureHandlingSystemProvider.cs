using System.Collections.Generic;
using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class ManureHandlingSystemProvider
    {
        /// <summary>
        /// Set the available options for the user based on the information in the table. If there is no lookup value in the table, the manure state type shouldn't be available as 
        /// an option for the user. The options here should match what is listed in table 35 (Default emission factors).
        /// </summary>
        public List<ManureStateType> GetValidManureStateTypesByAnimalCategory(AnimalType animalType)
        {
            if (animalType.IsBeefCattleType())
            {
                return new List<ManureStateType>()
                {
                    ManureStateType.Pasture,
                    ManureStateType.DeepBedding,
                    ManureStateType.SolidStorage,
                    ManureStateType.CompostPassive,
                    ManureStateType.CompostIntensive,
                };
            }

            if (animalType.IsDairyCattleType())
            {
                return new List<ManureStateType>()
                {
                    /*
                     * Solid
                     */

                    ManureStateType.Pasture,
                    ManureStateType.DeepBedding,
                    ManureStateType.SolidStorage,
                    ManureStateType.CompostIntensive,
                    ManureStateType.CompostPassive,

                    /*
                     * Liquid
                     */

                    ManureStateType.DailySpread,
                    ManureStateType.LiquidWithNaturalCrust,
                    ManureStateType.LiquidNoCrust,
                    ManureStateType.LiquidWithSolidCover,
                    ManureStateType.DeepPit,
                };
            }

            if (animalType.IsSwineType())
            {
                return new List<ManureStateType>()
                {
                    ManureStateType.Pasture,
                    ManureStateType.CompostedInVessel,

                    /*
                     * Liquid
                     */

                    ManureStateType.LiquidWithNaturalCrust,
                    ManureStateType.LiquidNoCrust,
                    ManureStateType.LiquidWithSolidCover,
                    ManureStateType.DeepPit,
                };
            }

            if (animalType.IsSheepType())
            {
                return new List<ManureStateType>()
                {
                    ManureStateType.Pasture,
                    ManureStateType.SolidStorage,
                };
            }

            if (animalType.IsPoultryType())
            {
                return new List<ManureStateType>()
                {
                    ManureStateType.SolidStorageWithOrWithoutLitter,
                };
            }

            // Other animals or unknown animal type
            return new List<ManureStateType>()
            {
                ManureStateType.SolidStorage,
                ManureStateType.Pasture,
            };
        }
    }
}