using System.Diagnostics;
using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 32. Default manure volume excreted per day for sheep, poultry and other livestock.
    /// </summary>
    public class Table_32_Default_Manure_Excreted_Poultry_OtherLivestock_Provider
    {
        public double GetManureExcretionRate(AnimalType animalType)
        {
            // Footnote 1 (in reference to Lambs, Ewes and Rams)
            if (animalType.IsSheepType())
            {
                return 1.8;
            }

            
            if (animalType == AnimalType.ChickenPullets ||
                animalType == AnimalType.Broilers) // Footnote 1
            {
                return 0.08;
            }

            
            if (animalType == AnimalType.LayersDryPoultry ||
                animalType == AnimalType.LayersWetPoultry) // Footnote 1
            {
                return 0.12;
            }

            if (animalType == AnimalType.TurkeyHen ||
                animalType == AnimalType.YoungTurkeyHen ||
                animalType == AnimalType.Turkeys)
            {
                return 0.32;
            }

           
            if (animalType == AnimalType.Ducks)  // Footnote 2
            {
                return 0.2;
            }

            
            if (animalType == AnimalType.Geese) // Footnote 3
            {
                return 0.2;
            }

            
            if (animalType == AnimalType.Llamas ||
                animalType == AnimalType.Alpacas) // Footnote 4
            {
                return 2;
            }

            if (animalType == AnimalType.Deer ||
                animalType == AnimalType.Elk)
            {
                return 0;
            }

            
            if (animalType == AnimalType.Goats) // Footnote 1
            {
                return 3;
            }

            
            if (animalType == AnimalType.Horses) // Footnote 1
            {
                return 23;
            }

            
            if (animalType == AnimalType.Mules) // Footnote 5
            {
                return 23;
            }

            // Footnote 6:
            if (animalType == AnimalType.Bison)
            {
                return 37;
            }

            Trace.TraceError($"{nameof(Table_32_Default_Manure_Excreted_Poultry_OtherLivestock_Provider)}.{nameof(GetManureExcretionRate)}" +
                             $" unable to get data for manure excretion rate for animal type: {animalType}." +
                             $" Returning default value of 0.");

            return 0;
        }

        #region Footnotes


        /*
         *
           Footnote 1: Calculated from Hofmann and Beaulieu (2006), Table A1 (for all values in Table A1, total manure production consists of feces and urine.
            Bedding and other types of material such as feather, unused feed, etc. are not included)
           Footnote 2: Lorimor et al. (2004), Table 6
           Footnote 3: Value for ducks used from Lorimor et al., 2004), Table 6
           Footnote 4: Value for sheep and lambs used from Hofmann and Beaulieu (2006), Table A1
           Footnote 5: Value for horses and ponies used from Hofmann and Beaulieu (2006), Table A1
           Footnote 6: Value for dairy cows used from Hofmann and Beaulieu (2006), Table A1
         */

        #endregion
    }
}