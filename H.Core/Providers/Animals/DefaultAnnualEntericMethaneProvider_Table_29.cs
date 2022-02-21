using H.Core.Enumerations;
using H.Core.Tools;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 29.
    ///
    /// Enteric CH4 emission rates for swine, poultry and other livestock groups.
    /// </summary>
    public class DefaultAnnualEntericMethaneProvider_Table_29 
    {
        #region Properties
        private List<EmissionData> Data { get; set; } = new List<EmissionData>(); 
        #endregion

        #region Constructors

        public DefaultAnnualEntericMethaneProvider_Table_29()
        {
            HTraceListener.AddTraceListener();
        }

        #endregion

        public double GetAnnualEntericMethaneEmissionRate(AnimalType animalType)
        {
            if (animalType.IsSwineType())
            {
                return 1.5;
            }

            if (animalType.IsPoultryType())
            {
                return 0;
            }

            if (animalType == AnimalType.Llamas || animalType == AnimalType.Alpacas)
            {
                return 8;
            }

            if (animalType == AnimalType.Deer || animalType == AnimalType.Elk)
            {
                return 20;
            }

            if (animalType == AnimalType.Goats)
            {
                return 5;
            }

            if (animalType == AnimalType.Horses)
            {
                return 18;
            }

            if (animalType == AnimalType.Mules)
            {
                return 10;
            }

            if (animalType == AnimalType.Bison)
            {
                return 64;
            }

            Trace.TraceError($"{nameof(DefaultAnnualEntericMethaneProvider_Table_29)}.{nameof(GetAnnualEntericMethaneEmissionRate)}" +
                             $" unable to get data for animal type: {animalType}." +
                             $" Returning default value of 0.");

            return 0;
        }
    }
}
