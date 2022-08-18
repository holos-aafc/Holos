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
    /// Table 33. Default bedding application rates and composition of bedding materials for all livestock groups.
    /// </summary>
    public class Table_33_Default_Bedding_Material_Composition_Provider
    {
        public List<Table_33_Default_Bedding_Material_Composition_Data> Data { get; } = new List<Table_33_Default_Bedding_Material_Composition_Data>();
        
        public Table_33_Default_Bedding_Material_Composition_Provider()
        {
            HTraceListener.AddTraceListener();

            /*
             * Beef
             */

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data()
            {
                AnimalType = AnimalType.Beef,
                BeddingMaterial = BeddingMaterialType.Straw, // Footnote 1
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5,
                MoistureContent = 9.57, // Footnote 12
            });

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data()
            {
                AnimalType = AnimalType.Beef,
                BeddingMaterial = BeddingMaterialType.WoodChip, // Footnotes 1, 2                
                TotalNitrogenKilogramsDryMatter = 0.00185,
                TotalCarbonKilogramsDryMatter = 0.506,
                TotalPhosphorusKilogramsDryMatter = 0.000275,
                CarbonToNitrogenRatio = 329.5,
                MoistureContent = 12.82, // Footnote 12
            });

            /*
             * Dairy
             */

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data()
            {
                AnimalType = AnimalType.Dairy,
                BeddingMaterial = BeddingMaterialType.Sand, // Footnote 4
            });

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data()
            {
                AnimalType = AnimalType.Dairy,
                BeddingMaterial = BeddingMaterialType.SeparatedManureSolid, // Footnote 5
                TotalNitrogenKilogramsDryMatter = 0.033,
                TotalCarbonKilogramsDryMatter = 0.395,
                TotalPhosphorusKilogramsDryMatter = 0,
                CarbonToNitrogenRatio = 12,
                MoistureContent = 0,
            });

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data()
            {
                AnimalType = AnimalType.Dairy,
                BeddingMaterial = BeddingMaterialType.StrawLong, // Footnote 6
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5,
                MoistureContent = 9.57, // Footnote 12
            });

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data()
            {
                AnimalType = AnimalType.Dairy,
                BeddingMaterial = BeddingMaterialType.StrawChopped, // Footnote 6
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5,
                MoistureContent = 9.57, // Footnote 12
            });

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data()
            {
                AnimalType = AnimalType.Dairy,
                BeddingMaterial = BeddingMaterialType.Shavings, // Footnotes 4, 7
                TotalNitrogenKilogramsDryMatter = 0.00185,
                TotalCarbonKilogramsDryMatter = 0.506,
                TotalPhosphorusKilogramsDryMatter = 0.000275,
                CarbonToNitrogenRatio = 329.5,
                MoistureContent = 10.09, // Footnote 12
            });

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data()
            {
                AnimalType = AnimalType.Dairy,
                BeddingMaterial = BeddingMaterialType.Sawdust, // Footnotes 4, 7
                TotalNitrogenKilogramsDryMatter = 0.00185,
                TotalCarbonKilogramsDryMatter = 0.506,
                TotalPhosphorusKilogramsDryMatter = 0.000275,
                CarbonToNitrogenRatio = 329.5,
                MoistureContent = 10.99, // Footnote 12
            });

            /*
             * Swine
             */

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data()
            {
                AnimalType = AnimalType.Swine,
                BeddingMaterial = BeddingMaterialType.StrawLong, // Footnotes 4, 9
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5,
                MoistureContent = 9.57, // Footnote 12
            });

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data()
            {
                AnimalType = AnimalType.Swine,
                BeddingMaterial = BeddingMaterialType.StrawChopped, // Footnotes 4, 9
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5,
                MoistureContent = 9.57, // Footnote 12
            });

            /*
             * Sheep
             */

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data()
            {
                AnimalType = AnimalType.Sheep,
                BeddingMaterial = BeddingMaterialType.Straw, // Footnote 7
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5,
                MoistureContent = 9.57, // Footnote 12
            });

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data()
            {
                AnimalType = AnimalType.Sheep,
                BeddingMaterial = BeddingMaterialType.Shavings, // Footnote 7
                TotalNitrogenKilogramsDryMatter = 0.00185,
                TotalCarbonKilogramsDryMatter = 0.506,
                TotalPhosphorusKilogramsDryMatter = 0.000275,
                CarbonToNitrogenRatio = 329.5,
                MoistureContent = 10.09, // Footnote 12
            });

            /*
             * Poultry
             */

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data()
            {
                AnimalType = AnimalType.Poultry,
                BeddingMaterial = BeddingMaterialType.Straw, // Footnote 9
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5,
                MoistureContent = 9.57, // Footnote 12
            });

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data()
            {
                AnimalType = AnimalType.Poultry,
                BeddingMaterial = BeddingMaterialType.Shavings, // Footnote 9
                TotalNitrogenKilogramsDryMatter = 0.00185,
                TotalCarbonKilogramsDryMatter = 0.506,
                TotalPhosphorusKilogramsDryMatter = 0.000275,
                CarbonToNitrogenRatio = 329.5,
                MoistureContent = 10.09, // Footnote 12
            });

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data()
            {
                AnimalType = AnimalType.Poultry,
                BeddingMaterial = BeddingMaterialType.Sawdust, // Footnotes 4, 7
                TotalNitrogenKilogramsDryMatter = 0.00185,
                TotalCarbonKilogramsDryMatter = 0.506,
                TotalPhosphorusKilogramsDryMatter = 0.000275,
                CarbonToNitrogenRatio = 329.5,
                MoistureContent = 10.99 // Footnote 12
            });


            // Other Livestock

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data
            {
                AnimalType = AnimalType.Llamas,
                BeddingMaterial = BeddingMaterialType.Straw,
                MoistureContent = 9.57,
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5, // Footnote 12
            });

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data
            {
                AnimalType = AnimalType.Alpacas,
                BeddingMaterial = BeddingMaterialType.Straw,
                MoistureContent = 9.57,
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5, // Footnote 12
            });

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data
            {
                AnimalType = AnimalType.Deer,
                BeddingMaterial = BeddingMaterialType.Straw,
                MoistureContent = 9.57,
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5, // Footnote 12
            });

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data
            {
                AnimalType = AnimalType.Elk,
                BeddingMaterial = BeddingMaterialType.Straw,
                MoistureContent = 9.57,
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5, // Footnote 12
            });

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data
            {
                AnimalType = AnimalType.Goats,
                BeddingMaterial = BeddingMaterialType.Straw,
                MoistureContent = 9.57,
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5, // Footnote 12
            });

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data
            {
                AnimalType = AnimalType.Horses,
                BeddingMaterial = BeddingMaterialType.Straw,
                MoistureContent = 9.57,
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5, // Footnote 12
            });

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data
            {
                AnimalType = AnimalType.Mules,
                BeddingMaterial = BeddingMaterialType.Straw,
                MoistureContent = 9.57,
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5, // Footnote 12
            });

            this.Data.Add(new Table_33_Default_Bedding_Material_Composition_Data
            {
                AnimalType = AnimalType.Bison,
                BeddingMaterial = BeddingMaterialType.Straw,
                MoistureContent = 9.57,
                TotalNitrogenKilogramsDryMatter = 0.0057,
                TotalCarbonKilogramsDryMatter = 0.447,
                TotalPhosphorusKilogramsDryMatter = 0.000635,
                CarbonToNitrogenRatio = 90.5, // Footnote 12
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
                if (housingType.IsTieStall() || housingType.IsFreeStall() || housingType == HousingType.DryLot) // Currently, all housing types have same rates for bedding types
                {
                    if (beddingMaterialType == BeddingMaterialType.Sand)
                    {
                        return 24.3;
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
                        return 2.1;
                    }

                    if (beddingMaterialType == BeddingMaterialType.Sawdust)
                    {
                        return 2.1;
                    }
                }
            }
            // Footnote 8 for sheep value reference.
            if (animalType.IsSheepType())
            {
                return 0.57;
            }

            if (animalType.IsSwineType())
            {
                if (beddingMaterialType == BeddingMaterialType.StrawLong)
                {
                    return 0.70;
                }

                return 0.79;
            }

            if (animalType.IsPoultryType())
            {
                if (beddingMaterialType == BeddingMaterialType.Straw || beddingMaterialType == BeddingMaterialType.Shavings)
                {
                    return 0;
                }

                if (beddingMaterialType == BeddingMaterialType.Sawdust)
                {
                    if (animalType == AnimalType.Broilers)
                    {
                        return 0.0014;
                    }

                    if (animalType == AnimalType.Layers)
                    {
                        return 0.0028;
                    }

                    if (animalType.IsTurkeyType())
                    {
                        return 0.011;
                    }

                    return 0;
                }

                return 0;
            }

            if (animalType.IsOtherAnimalType())
            {
                // Footnote 11 for Other livestock value reference
                switch (animalType)
                {
                    case AnimalType.Llamas:
                        return 0.57;

                    case AnimalType.Alpacas:
                        return 0.57;

                    case AnimalType.Deer:
                        return 1.5;

                    case AnimalType.Elk:
                        return 1.5;

                    case AnimalType.Goats:
                        return 0.57;

                    case AnimalType.Horses:
                        return 1.5;

                    case AnimalType.Mules:
                        return 1.5;

                    case AnimalType.Bison:
                        return 1.5;
                }
            }

            Trace.TraceError($"Unknown default bedding rate for {animalType.GetDescription()}, {housingType.GetDescription()}, and {beddingMaterialType.GetHashCode()}. Returning default value of 1.");

            return 1;
        }

        #endregion

        #region Footnotes

        /*
        *    Footnote 1: C, N and P composition values for straw (barley) and wood-chip bedding are averages of data recorded in 1998 and 1999 in Larney et al. (2008); 
                straw bedding application rates for beef cattle are from Chai et al. (2014), originally based on Larney et al. (2008) and Gilhespy et al. (2009)
        *    Footnote 2: Wood-chip bedding is a mixture of sawdust and bark peelings derived from 80% lodgepole pine (Pinus contorta var. latifolia  Engelm.) and 20% white
                spruce [Picea glauca (Moench)Voss]. 
        *    Footnote 3: “Drylot” refers to milking parlours, yards and exercise lots.
        *    Footnote 4: Values for bedding amounts are based on minimum values for recommended bedding per 450 kg animal weight from Lorimor et al. (2004, Table 13). 
                We assumed an average dairy cow weight of 687 kg (Lactanet, 2020) (sand for freestall barn: 15.9 kg/450 kg * 687 kg = 24.3 kg head-1 day-1; 
                shavings and sawdust for tie-stall barn: 1.4 kg/450 kg * 687 kg = 1.9 kg head-1 day-1) – due to a lack of available data, bedding application 
                rates for sand for freestall barns and shavings and sawdust for freestall barns were also applied to other dairy housing types. For swine, calculations 
                assumed an average weight for sows and boars of 198 kg (ECCC, 2022) (chopped straw: 1.8 kg/450 kg * 198 kg = 0.79 kg head-1 day-1; 
                long straw: 1.6 kg/450 kg * 198 kg = 0.7 kg head-1 day-1). For poultry we assumed an average weight of 0.9 kg for broilers, 1.8 kg for layers, 
                and 6.8 kg for turkeys (ECCC, 2022). For sawdust bedding this gave application rates of: broilers – 0.7 kg/450 kg * 0.9 kg =  = 0.0014 kg head-1 day-1; 
                for layers – 0.7 kg/450 kg * 1.8 kg = 0.0028 kg head-1 day-1; for turkeys – 0.7 kg/450 kg * 6.8 kg = 0.011 kg head-1 day-1.
        *    Footnote 5: Dairy manure soilds separated from manure liquid using a screw press and composted and dried; also sometimes referred to as recycled manure solids, 
                dried manure solids, undigested feedstuffs or more commonly compost bedding (OMAFRA, 2015). Values from Misselbrook and Powell (2005, Table 1).
        *    Footnote 6: Values for long and chopped straw follow Chai et al. (2016) and are originally from Rotz et al. (2013). 
        *    Footnote 7: Due to a lack of data, total C, total N, total P and C:N ratio values for wood-chip bedding from Larney et al. (2008) are used for wood shavings 
                and sawdust bedding for dairy cattle, sheep, swine and poultry.
        *    Footnote 8: Bedding application rates for sheep were obtained from the Canadian Sheep Foundation (2021), and applied to all bedding options. The application
                rate of 0.57 was calculated as the midpoint of the recommended 0.45-0.68 kg head-1 day-1 range provided.
        *    Footnote 9: Following Chai et al. (2016), a total N content value of 0.0057 (derived from Larney et al., 2008) was used for chopped straw bedding for dairy 
                cattle; due to a lack of data, the total C, total N, total P and C:N ratio values for straw bedding from Larney et al. (2008) were applied to 
                long- and chopped-straw bedding for dairy cows, swine and poultry. 
        *    Footnote 10: Bedding options for sheep were identified from the Canadian Sheep Foundation (2021). Nutrient concentration values for straw and wood shavings 
                for beef cattle were used for sheep in a drylot/corral and barn.
        *    Footnote 11: For Other livestock, straw was assumed to be the main bedding type and total C, total N, total P and C:N ratio values for straw bedding from 
                Larney et al. (2008) were applied. For llamas and alpacas and goats, the bedding application rate for sheep was used; for deer and elk, horses,
                mules and bison, the bedding application rate for beef cattle (feedlot) was used as no data for these animals groups was available.
        *   Footnote  12: Values for moisture content of bedding from Ferraz et al. (2020). The moisture content for straw is the mean of values for barley straw (9.8%)
                and wheat straw (9.33%); the moisture content for sawdust is the value for dried sawdust.

         */

        #endregion
    }
}
