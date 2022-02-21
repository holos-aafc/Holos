using System.Collections.Generic;
using H.Core.Enumerations;
using System.Diagnostics;
using System.Linq;
using H.Core.Tools;

namespace H.Core.Providers.Animals
{
    /// <summary>         
    /// Table 9
    /// 
    /// Manure types and default composition included in the Holos model
    /// </summary>
    public class DefaultManureCompositionProvider_Table_9
    {
        #region Fields

        #endregion

        #region Constructors

        public DefaultManureCompositionProvider_Table_9()
        {
            HTraceListener.AddTraceListener();

            #region Beef

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.Pasture,
                AnimalType = AnimalType.Beef,
                MoistureContent = 50,
                NitrogenFraction = 1,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = 0.24,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.DeepBedding,
                AnimalType = AnimalType.Beef,
                MoistureContent = 60.08,
                NitrogenFraction = 0.715,
                CarbonFraction = 12.63,
                PhosphorusFraction = 0.236,
                CarbonToNitrogenRatio = 18.79,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.SolidStorage,
                AnimalType = AnimalType.Beef,
                MoistureContent = 60.43,
                NitrogenFraction = 0.722,
                CarbonFraction = 8.58,
                PhosphorusFraction = 0.254,
                CarbonToNitrogenRatio = 16.9,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.CompostPassive,
                AnimalType = AnimalType.Beef,
                MoistureContent = 62.35,
                NitrogenFraction = 0.659,
                CarbonFraction = 9.16,
                PhosphorusFraction = 0.255,
                CarbonToNitrogenRatio = 14.52,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.CompostIntensive,
                AnimalType = AnimalType.Beef,
                MoistureContent = 36.54,
                NitrogenFraction = 1.041,
                CarbonFraction = 14.48,
                PhosphorusFraction = 0.398,
                CarbonToNitrogenRatio = 14.23,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.Liquid,
                AnimalType = AnimalType.Beef,
                MoistureContent = 89.9,
                NitrogenFraction = 0.37,
                CarbonFraction = 0,
                PhosphorusFraction = 0.080,
                CarbonToNitrogenRatio = 21.5,
            });

            #endregion

            #region Dairy

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.Pasture,
                AnimalType = AnimalType.Dairy,
                MoistureContent = 50,
                NitrogenFraction = 1,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = 0.24,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.DeepBedding,
                AnimalType = AnimalType.Dairy,
                MoistureContent = 60.08,
                NitrogenFraction = 0.715,
                CarbonFraction = 12.63,
                PhosphorusFraction = 0.236,
                CarbonToNitrogenRatio = 18.79,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.SolidStorage,
                AnimalType = AnimalType.Dairy,
                MoistureContent = 77.3,
                NitrogenFraction = 0.392,
                CarbonFraction = 2.99,
                PhosphorusFraction = 0.118,
                CarbonToNitrogenRatio = 21.95,
                NitrogenConcentrationOfManure = 5,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.Composted,
                AnimalType = AnimalType.Dairy,
                MoistureContent = 78.11,
                NitrogenFraction = 0.265,
                CarbonFraction = 4.72,
                PhosphorusFraction = CoreConstants.ValueNotDetermined,
                CarbonToNitrogenRatio = 17.11,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.Slurry,
                AnimalType = AnimalType.Dairy,
                MoistureContent = 94.41,
                NitrogenFraction = 0.209,
                CarbonFraction = 2.19,
                PhosphorusFraction = 0.06,
                CarbonToNitrogenRatio = 10.58,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.LiquidSeparated,
                AnimalType = AnimalType.Dairy,
                MoistureContent = 95.5,
                NitrogenFraction = 0.013,
                CarbonFraction = 0.125,
                PhosphorusFraction = 0.002,
                CarbonToNitrogenRatio = 6.53,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.AnaerobicDigester,
                AnimalType = AnimalType.Dairy,
                MoistureContent = 95,
                NitrogenFraction = 0.233,
                CarbonFraction = 2.39,
                PhosphorusFraction = CoreConstants.ValueNotDetermined,
                CarbonToNitrogenRatio = 10.42,
            });

            #endregion

            #region Swine

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.CompostIntensive,
                AnimalType = AnimalType.Swine,
                MoistureContent = 79.23,
                NitrogenFraction = 0.279,
                CarbonFraction = 9.13,
                PhosphorusFraction = CoreConstants.ValueNotDetermined,
                CarbonToNitrogenRatio = 30.88,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {

                ManureStateType = ManureStateType.CompostPassive,
                AnimalType = AnimalType.Swine,
                MoistureContent = 79.23,
                NitrogenFraction = 0.279,
                CarbonFraction = 9.13,
                PhosphorusFraction = CoreConstants.ValueNotDetermined,
                CarbonToNitrogenRatio = 30.88,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.DeepPit,
                AnimalType = AnimalType.Swine,
                MoistureContent = CoreConstants.ValueNotDetermined,
                NitrogenFraction = CoreConstants.ValueNotDetermined,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = CoreConstants.ValueNotDetermined,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.LiquidWithNaturalCrust,
                AnimalType = AnimalType.Swine,
                MoistureContent = 95.16,
                NitrogenFraction = 0.325,
                CarbonFraction = 1.29,
                PhosphorusFraction = 0.118,
                CarbonToNitrogenRatio = 3.25,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {

                ManureStateType = ManureStateType.LiquidNoCrust,
                AnimalType = AnimalType.Swine,
                MoistureContent = 95.16,
                NitrogenFraction = 0.325,
                CarbonFraction = 1.29,
                PhosphorusFraction = 0.118,
                CarbonToNitrogenRatio = 3.25,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {

                ManureStateType = ManureStateType.LiquidWithSolidCover,
                AnimalType = AnimalType.Swine,
                MoistureContent = 95.16,
                NitrogenFraction = 0.325,
                CarbonFraction = 1.29,
                PhosphorusFraction = 0.118,
                CarbonToNitrogenRatio = 3.25,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.AnaerobicDigester,
                AnimalType = AnimalType.Swine,
                MoistureContent = CoreConstants.ValueNotDetermined,
                NitrogenFraction = CoreConstants.ValueNotDetermined,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = CoreConstants.ValueNotDetermined,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            #endregion

            #region Poultry

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.SolidStorage,
                AnimalType = AnimalType.Poultry,
                MoistureContent = 44.83,
                NitrogenFraction = 2.427,
                CarbonFraction = 10.12,
                PhosphorusFraction = 1.06,
                CarbonToNitrogenRatio = 4.36,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.Liquid,
                AnimalType = AnimalType.Poultry,
                MoistureContent = 89,
                NitrogenFraction = 8.95,
                CarbonFraction = 2.92,
                PhosphorusFraction = 0.26,
                CarbonToNitrogenRatio = 3.27,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.Slurry,
                AnimalType = AnimalType.Poultry,
                MoistureContent = 89,
                NitrogenFraction = 0.895,
                CarbonFraction = 2.92,
                PhosphorusFraction = 0.26,
                CarbonToNitrogenRatio = 3.57,
            });

            #endregion

            #region Sheep

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.Pasture,
                AnimalType = AnimalType.Sheep,
                MoistureContent = 25,
                NitrogenFraction = 1,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = 0.231,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {

                ManureStateType = ManureStateType.SolidStorage,
                AnimalType = AnimalType.Sheep,
                MoistureContent = 67.8,
                NitrogenFraction = 0.87,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = 0.34,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
                NitrogenConcentrationOfManure = 10,
            });

            #endregion

            #region Other Animals

            // Pasture
            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.Pasture,
                AnimalType = AnimalType.Llamas,
                MoistureContent = 25,
                NitrogenFraction = CoreConstants.ValueNotDetermined,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = 0.231,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.Pasture,
                AnimalType = AnimalType.Alpacas,
                MoistureContent = 25,
                NitrogenFraction = CoreConstants.ValueNotDetermined,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = 0.231,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.Pasture,
                AnimalType = AnimalType.Deer,
                MoistureContent = CoreConstants.ValueNotDetermined,
                NitrogenFraction = CoreConstants.ValueNotDetermined,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = CoreConstants.ValueNotDetermined,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.Pasture,
                AnimalType = AnimalType.Elk,
                MoistureContent = CoreConstants.ValueNotDetermined,
                NitrogenFraction = CoreConstants.ValueNotDetermined,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = CoreConstants.ValueNotDetermined,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.Pasture,
                AnimalType = AnimalType.Goats,
                MoistureContent = CoreConstants.ValueNotDetermined,
                NitrogenFraction = 0.63,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = CoreConstants.ValueNotDetermined,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.Pasture,
                AnimalType = AnimalType.Horses,
                MoistureContent = 75,
                NitrogenFraction = 0.6,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = 0.131,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.Pasture,
                AnimalType = AnimalType.Mules,
                MoistureContent = 75,
                NitrogenFraction = 0.6,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = 0.131,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {
                
                ManureStateType = ManureStateType.Pasture,
                AnimalType = AnimalType.Bison,
                MoistureContent = CoreConstants.ValueNotDetermined,
                NitrogenFraction = CoreConstants.ValueNotDetermined,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = CoreConstants.ValueNotDetermined,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            // Solid storage
            this.Data.Add(new DefaultManureCompositionData()
            {
                ManureStateType = ManureStateType.SolidStorage,
                AnimalType = AnimalType.Llamas,
                MoistureContent = 67.8,
                NitrogenFraction = 0.87,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = 0.34,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {

                ManureStateType = ManureStateType.SolidStorage,
                AnimalType = AnimalType.Alpacas,
                MoistureContent = 67.8,
                NitrogenFraction = 0.87,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = 0.34,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {

                ManureStateType = ManureStateType.SolidStorage,
                AnimalType = AnimalType.Deer,
                MoistureContent = 35,
                NitrogenFraction = 0.65,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = 0.21,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {

                ManureStateType = ManureStateType.SolidStorage,
                AnimalType = AnimalType.Elk,
                MoistureContent = 35,
                NitrogenFraction = 0.65,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = 0.21,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {

                ManureStateType = ManureStateType.SolidStorage,
                AnimalType = AnimalType.Goats,
                MoistureContent = 64.3,
                NitrogenFraction = 0.104,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = 0.28,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {

                ManureStateType = ManureStateType.SolidStorage,
                AnimalType = AnimalType.Horses,
                MoistureContent = 62.6,
                NitrogenFraction = 0.5,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = 0.15,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {

                ManureStateType = ManureStateType.SolidStorage,
                AnimalType = AnimalType.Mules,
                MoistureContent = 62.6,
                NitrogenFraction = 0.5,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = 0.15,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            this.Data.Add(new DefaultManureCompositionData()
            {

                ManureStateType = ManureStateType.SolidStorage,
                AnimalType = AnimalType.Bison,
                MoistureContent = 35,
                NitrogenFraction = 0.65,
                CarbonFraction = CoreConstants.ValueNotDetermined,
                PhosphorusFraction = 0.21,
                CarbonToNitrogenRatio = CoreConstants.ValueNotDetermined,
            });

            #endregion
        }

        public List<DefaultManureCompositionData> Data { get; } = new List<DefaultManureCompositionData>();

        #endregion

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
                    ManureStateType.SolidStorage,
                    ManureStateType.CompostPassive,
                    ManureStateType.CompostIntensive,
                    ManureStateType.DeepBedding,
                    ManureStateType.AnaerobicDigester,
                };
            }

            if (animalType.IsDairyCattleType())
            {
                return new List<ManureStateType>()
                {
                    ManureStateType.Pasture,
                    ManureStateType.DailySpread,
                    ManureStateType.SolidStorage,
                    ManureStateType.CompostIntensive,
                    ManureStateType.CompostPassive,
                    ManureStateType.DeepBedding,
                    ManureStateType.LiquidWithNaturalCrust,
                    ManureStateType.LiquidNoCrust,
                    ManureStateType.LiquidWithSolidCover,
                    ManureStateType.AnaerobicDigester,
                };
            }

            if (animalType.IsSwineType())
            {
                return new List<ManureStateType>()
                {
                    ManureStateType.SolidStorage,
                    ManureStateType.LiquidWithNaturalCrust,
                    ManureStateType.LiquidNoCrust,
                    ManureStateType.LiquidWithSolidCover,
                    ManureStateType.AnaerobicDigester,
                    ManureStateType.DeepPit,
                };
            }

            if (animalType.IsSheepType())
            {
                return new List<ManureStateType>()
                {
                    ManureStateType.Pasture,
                    ManureStateType.SolidStorage,
                    ManureStateType.CompostIntensive,
                    ManureStateType.CompostPassive,
                    ManureStateType.DeepBedding,
                    ManureStateType.AnaerobicDigester,
                };
            }

            if (animalType.IsPoultryType())
            {
                return new List<ManureStateType>()
                {
                    ManureStateType.SolidStorage,
                    ManureStateType.AnaerobicDigester,
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