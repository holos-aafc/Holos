using System;
using H.Core.Enumerations;
using H.Core.Tools;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Core.Models;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 29
    ///
    /// Default bedding application rates and composition of bedding materials.
    /// </summary>
    public class DefaultBeddingMaterialCompositionProvider_Table_29
    {
        public List<DefaultsCompositionOfBeddingMaterialsData> Data { get; } = new List<DefaultsCompositionOfBeddingMaterialsData>();
        
        public DefaultBeddingMaterialCompositionProvider_Table_29()
        {
            HTraceListener.AddTraceListener();

            /*
             * Beef
             */

            this.Data.Add(new DefaultsCompositionOfBeddingMaterialsData()
            {
                AnimalType = AnimalType.Beef,
                BeddingMaterial = BeddingMaterialType.Straw,
                TotalNitrogenKilogramsDryMatter = 0.057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5,
            });

            this.Data.Add(new DefaultsCompositionOfBeddingMaterialsData()
            {
                AnimalType = AnimalType.Beef,
                BeddingMaterial = BeddingMaterialType.WoodChip,                
                TotalNitrogenKilogramsDryMatter = 0.00185,
                TotalCarbonKilogramsDryMatter = 0.506,
                TotalPhosphorusKilogramsDryMatter = 0.00025,
                CarbonToNitrogenRatio = 329.5,
            });

            /*
             * Dairy
             */

            this.Data.Add(new DefaultsCompositionOfBeddingMaterialsData()
            {
                AnimalType = AnimalType.Dairy,
                BeddingMaterial = BeddingMaterialType.Sand,
            });

            this.Data.Add(new DefaultsCompositionOfBeddingMaterialsData()
            {
                AnimalType = AnimalType.Dairy,
                BeddingMaterial = BeddingMaterialType.SeparatedManureSolid,
                TotalNitrogenKilogramsDryMatter = 0.033,
                TotalCarbonKilogramsDryMatter = 0.395,
                TotalPhosphorusKilogramsDryMatter = 0,
                CarbonToNitrogenRatio = 12,
            });

            this.Data.Add(new DefaultsCompositionOfBeddingMaterialsData()
            {
                AnimalType = AnimalType.Dairy,
                BeddingMaterial = BeddingMaterialType.StrawLong,
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5,
            });

            this.Data.Add(new DefaultsCompositionOfBeddingMaterialsData()
            {
                AnimalType = AnimalType.Dairy,
                BeddingMaterial = BeddingMaterialType.StrawChopped,
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5,
            });

            this.Data.Add(new DefaultsCompositionOfBeddingMaterialsData()
            {
                AnimalType = AnimalType.Dairy,
                BeddingMaterial = BeddingMaterialType.Shavings,
                TotalNitrogenKilogramsDryMatter = 0.00185,
                TotalCarbonKilogramsDryMatter = 0.506,
                TotalPhosphorusKilogramsDryMatter = 0.000275,
                CarbonToNitrogenRatio = 329.5,
            });

            this.Data.Add(new DefaultsCompositionOfBeddingMaterialsData()
            {
                AnimalType = AnimalType.Dairy,
                BeddingMaterial = BeddingMaterialType.Sawdust,
                TotalNitrogenKilogramsDryMatter = 0.00185,
                TotalCarbonKilogramsDryMatter = 0.506,
                TotalPhosphorusKilogramsDryMatter = 0.000275,
                CarbonToNitrogenRatio = 329.5,
            });

            /*
             * Swine
             */

            this.Data.Add(new DefaultsCompositionOfBeddingMaterialsData()
            {
                AnimalType = AnimalType.Swine,
                BeddingMaterial = BeddingMaterialType.StrawLong,
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5,
            });

            this.Data.Add(new DefaultsCompositionOfBeddingMaterialsData()
            {
                AnimalType = AnimalType.Swine,
                BeddingMaterial = BeddingMaterialType.StrawChopped,
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5,
            });

            this.Data.Add(new DefaultsCompositionOfBeddingMaterialsData()
            {
                AnimalType = AnimalType.Swine,
                BeddingMaterial = BeddingMaterialType.Shavings,
                TotalNitrogenKilogramsDryMatter = 0.00185,
                TotalCarbonKilogramsDryMatter = 0.506,
                TotalPhosphorusKilogramsDryMatter = 0.000275,
                CarbonToNitrogenRatio = 329.5,
            });

            this.Data.Add(new DefaultsCompositionOfBeddingMaterialsData()
            {
                AnimalType = AnimalType.Swine,
                BeddingMaterial = BeddingMaterialType.Sawdust,
                TotalNitrogenKilogramsDryMatter = 0.00185,
                TotalCarbonKilogramsDryMatter = 0.506,
                TotalPhosphorusKilogramsDryMatter = 0.000275,
                CarbonToNitrogenRatio = 329.5,
            });

            /*
             * Sheep
             */

            this.Data.Add(new DefaultsCompositionOfBeddingMaterialsData()
            {
                AnimalType = AnimalType.Sheep,
                BeddingMaterial = BeddingMaterialType.Straw,
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5,
            });

            this.Data.Add(new DefaultsCompositionOfBeddingMaterialsData()
            {
                AnimalType = AnimalType.Sheep,
                BeddingMaterial = BeddingMaterialType.Shavings,
                TotalNitrogenKilogramsDryMatter = 0.00185,
                TotalCarbonKilogramsDryMatter = 0.506,
                TotalPhosphorusKilogramsDryMatter = 0.000275,
                CarbonToNitrogenRatio = 329.5,
            });

            /*
             * Poultry
             */

            this.Data.Add(new DefaultsCompositionOfBeddingMaterialsData()
            {
                AnimalType = AnimalType.Poultry,
                BeddingMaterial = BeddingMaterialType.Straw,
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5,
            });

            this.Data.Add(new DefaultsCompositionOfBeddingMaterialsData()
            {
                AnimalType = AnimalType.Poultry,
                BeddingMaterial = BeddingMaterialType.Shavings,
                TotalNitrogenKilogramsDryMatter = 0.00185,
                TotalCarbonKilogramsDryMatter = 0.506,
                TotalPhosphorusKilogramsDryMatter = 0.000275,
                CarbonToNitrogenRatio = 329.5,
            });

            this.Data.Add(new DefaultsCompositionOfBeddingMaterialsData()
            {
                AnimalType = AnimalType.Poultry,
                BeddingMaterial = BeddingMaterialType.Sawdust,
                TotalNitrogenKilogramsDryMatter = 0.00185,
                TotalCarbonKilogramsDryMatter = 0.506,
                TotalPhosphorusKilogramsDryMatter = 0.000275,
                CarbonToNitrogenRatio = 329.5,
            });
        }

        #region Public Methods

        /// <summary>
        /// Returns the default bedding application rate for a particular housing type and bedding material
        /// </summary>
        /// <returns>Bedding rate (kg head^-1 day^1)</returns>
        public double GetDefaultBeddingRate(
            HousingType housingType, 
            BeddingMaterialType beddingMaterialType, 
            AnimalType animalType)
        {
            if (housingType.IsPasture())
            {
                return 0;
            }

            if (animalType.IsBeefCattleType())
            {
                if (beddingMaterialType == BeddingMaterialType.Straw)
                {
                    if (housingType.IsFeedlot())
                    {
                        return 1.5;
                    }

                    if (housingType.IsBarn())
                    {
                        return 3.5;
                    }
                }

                if (beddingMaterialType == BeddingMaterialType.WoodChip)
                {
                    if (housingType.IsFeedlot())
                    {
                        return 3.6;
                    }

                    if (housingType.IsBarn())
                    {
                        return 5.0;
                    }
                }
            }

            if (animalType.IsDairyCattleType())
            {
                if (housingType.IsTieStall() || housingType.IsFreeStall()) // Currently both housing types have same rates for bedding types
                {
                    if (beddingMaterialType == BeddingMaterialType.Sand)
                    {
                        return 21.6;
                    }

                    if (beddingMaterialType == BeddingMaterialType.SeparatedManureSolid)
                    {
                        return 0;
                    }

                    if (beddingMaterialType == BeddingMaterialType.StrawLong)
                    {
                        return 0.7;
                    }

                    if (beddingMaterialType == BeddingMaterialType.StrawChopped)
                    {
                        return 0.7;
                    }

                    if (beddingMaterialType == BeddingMaterialType.Shavings)
                    {
                        return 1.9;
                    }

                    if (beddingMaterialType == BeddingMaterialType.Sawdust)
                    {
                        return 1.9;
                    }
                }
            }

            if (animalType.IsSheepType())
            {
                return 0.57;
            }

            if (animalType.IsSwineType())
            {
                if (beddingMaterialType == BeddingMaterialType.StrawLong)
                {
                    return 0.44;
                }
                else
                {
                    return 0.5;
                }
            }

            if (animalType.IsPoultryType())
            {
                if (beddingMaterialType == BeddingMaterialType.Straw || beddingMaterialType == BeddingMaterialType.Shavings)
                {
                    return 0;
                }

                if (beddingMaterialType == BeddingMaterialType.Sawdust)
                {
                    if (animalType.IsChickenType())
                    {
                        return 0.0016;
                    }
                    else
                    {
                        return 0.011;
                    }
                }

                return 0;
            }

            Trace.TraceError($"Unknown default bedding rate for {animalType.GetDescription()}, {housingType.GetDescription()}, and {beddingMaterialType.GetHashCode()}. Returning default value of 1.");

            return 1;
        } 

        #endregion
    }
}
