using H.Core.Enumerations;
using H.Core.Tools;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 30. Enteric CH4 emission rates for swine, poultry and other livestock groups
    /// <para>Source: IPCC (2019), Table 10.10.</para>
    /// </summary>
    public class Table_30_Enteric_CH4_Swine_Poultry_OtherLivestock_Provider 
    {
        #region Properties
        private List<Table_39_Livestock_Emission_Conversion_Factors_Data> Data { get; set; } = new List<Table_39_Livestock_Emission_Conversion_Factors_Data>(); 
        #endregion

        #region Constructors

        public Table_30_Enteric_CH4_Swine_Poultry_OtherLivestock_Provider()
        {
            HTraceListener.AddTraceListener();
        }

        #endregion

        public double GetAnnualEntericMethaneEmissionRate(AnimalType animalType)
        {
            
            if (animalType.IsSwineType()) // Footnote 1
            {
                return 1.5;
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

            Trace.TraceError($"{nameof(Table_30_Enteric_CH4_Swine_Poultry_OtherLivestock_Provider)}.{nameof(GetAnnualEntericMethaneEmissionRate)}" +
                             $" unable to get data for animal type: {animalType}." +
                             $" Returning default value of 0.");

            return 0;
        }

        #region Footnotes

        // Footnote 1: High productivity systems
        // Footnote 2: Due to insufficient data, no default enteric CH4 emission factors are available for poultry (IPCC, 2019)
        // Footnote 3: Low productivity systems
        // Footnote 4: Elk were added to this category
        // Footnote 5: Value for “other (non-dairy) cattle” in North America (from IPCC (2019), Table 10.11) was used

        #endregion
    }
}
