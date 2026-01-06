using H.Core.Enumerations;
using H.Core.Tools;
using System.Collections.Generic;
using System.Diagnostics;
using H.Core.Models.Animals;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 27. Enteric CH4 emission rates for swine, poultry and other livestock groups
    /// </summary>
    public class Table_27_Enteric_CH4_Swine_Poultry_OtherLivestock_Provider 
    {
        #region Properties
        private List<Table_36_Livestock_Emission_Conversion_Factors_Data> Data { get; set; } = new List<Table_36_Livestock_Emission_Conversion_Factors_Data>(); 
        #endregion

        #region Constructors

        public Table_27_Enteric_CH4_Swine_Poultry_OtherLivestock_Provider()
        {
            HTraceListener.AddTraceListener();
        }

        #endregion

        #region Public Methods

        public double GetAnnualEntericMethaneEmissionRate(ManagementPeriod managementPeriod)
        {
            var weightMidPoint = (managementPeriod.EndWeight + managementPeriod.StartWeight) / 2.0;
            var animalType = managementPeriod.AnimalType;

            if (animalType == AnimalType.SwineSows || animalType == AnimalType.SwineLactatingSow || animalType == AnimalType.SwineDrySow) // Footnote 1
            {
                return 2.42;
            }

            // Swine boars are also used to indicate hogs
            if (animalType == AnimalType.SwineBoar || animalType == AnimalType.SwineGrower) // Footnote 1
            {
                // Check if is young animal (hogs/growers). Boars will have a higher rate.
                if (managementPeriod.ProductionStage == ProductionStages.GrowingAndFinishing)
                {
                    if (weightMidPoint <= 65)
                    {
                        return 0.7;
                    }
                    else
                    {
                        return 1.5;
                    }
                }
                else
                {
                    return 2.64;
                }
            }

            if (animalType == AnimalType.SwineGilts) // Footnote 1
            {
                return 2.42;
            }

            if (animalType == AnimalType.SwineFinisher) // Footnote 1
            {
                return 1.5;
            }

            if (animalType == AnimalType.SwineStarter || animalType == AnimalType.SwinePiglets) // Footnote 1
            {
                return 0.23;
            }

            if (animalType.IsPoultryType()) // Footnote 2
            {
                return 0;
            }

            if (animalType == AnimalType.Llamas || animalType == AnimalType.Alpacas)
            {
                return 8;
            }

            if (animalType == AnimalType.Goats) // Footnote 3
            {
                return 5;
            }

            if (animalType == AnimalType.Deer || animalType == AnimalType.Elk) // Footnote 4
            {
                return 20;
            }

            if (animalType == AnimalType.Horses)
            {
                return 18;
            }

            if (animalType == AnimalType.Mules)
            {
                return 10;
            }

            if (animalType == AnimalType.Bison) // Footnote 5
            {
                return 64;
            }

            Trace.TraceError($"{nameof(Table_27_Enteric_CH4_Swine_Poultry_OtherLivestock_Provider)}.{nameof(GetAnnualEntericMethaneEmissionRate)}" +
                             $" unable to get data for animal type: {animalType}." +
                             $" Returning default value of 0.");

            return 0;
        } 

        #endregion

        #region Footnotes

        // Footnote 1: Source: Verge et al. (2009)
        // Footnote 2: Due to insufficient data, no default enteric CH4 emission factors are available for poultry (IPCC, 2019)
        // Footnote 3: Low productivity systems. Source: IPCC (2019), Table 10.10
        // Footnote 4: Elk were added to this category
        // Footnote 5: Value for “other (non-dairy) cattle” in North America (from IPCC (2019), Table 10.11) was used

        #endregion
    }
}
