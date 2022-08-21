using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 41. Default CH4 emission factors for solid manure for other livestock groups.
    /// </summary>
    public class Table_41_OtherLivestock_Default_CH4_Emission_Factors_Provider
    {
        public double GetDailyManureMethaneEmissionRate(AnimalType animalType)
        {
            // Refer to Footnote 1 for CH4 manure rate value reference.

            // Footnote 2
            if (animalType.IsTurkeyType())
            {
                return 0.00036;
            }

            // Footnote 2
            if (animalType == AnimalType.Ducks)
            {
                return 0.00010;
            }

            // Footnote 2
            if (animalType == AnimalType.Geese)
            {
                return 0.00015;
            }

            if (animalType == AnimalType.Llamas || animalType == AnimalType.Alpacas)
            {
                return 0.000521;
            }

            // Footnote 3
            if (animalType == AnimalType.Deer || animalType == AnimalType.Elk)
            {
                return 0.000603;
            }

            System.Diagnostics.Trace.TraceError(
                $"{nameof(Table_41_OtherLivestock_Default_CH4_Emission_Factors_Provider)}.{nameof(GetDailyManureMethaneEmissionRate)}" +
                $": Unable to get data for animal type: {animalType.GetDescription()}." +
                $" Returning default value of 0.");

            return 0;


            #region Footnotes

            /*
               Footnote 1: In IPCC (2019) Table 10.14 (for turkeys and ducks/geese), default manure CH4 emission rates are based on VS excretion rates, i.e., g 
                CH4 per kg VS; in Table 10.15 (for deer/elk), default manure CH4 emission rates are per head per year and are divided by 365 to get the daily rate.

               Footnote 2: For turkeys and ducks/geese, the IPCC (2019) default for poultry of 5.2 g CH4 kg-1 VS excreted was used. For turkeys, a daily 
                VS excretion rate of 0.07 kg was estimated based on the default IPCC rate of 10.3 kg VS kg-1 body weight (IPCC, 2019, Table 10.13a) and an average 
                body weight of 6.8 kg (ECCC, 2022), to give a daily CH4 emission rate of 0.0052 kg * 0.07 kg  = 0.00036 kg CH4 head-1 day-1. For ducks, a daily VS 
                excretion rate of 0.02 kg was estimated based on the default IPCC rate of 7.4 kg VS kg-1 body weight (value for ducks, IPCC, 2019, Table 10.13a) and 
                an average body weight of 2.7 kg (IPCC, 2019, Table 10A.5), to give a daily CH4 emission rate of 0.0052 kg * 0.02 kg = 0.0001 kg CH4 head-1 day-1.
                For geese, a daily VS excretion rate of 0.03 kg was estimated based on the default IPCC rate of 10.3 kg VS kg-1 body weight (IPCC, 2019, Table 10.13a)
                and an average body weight of 4.0 kg (based on value for broiler geese for meat production from FAO, 2002), to give a daily 
                CH4 emission rate of 0.0052 kg * 0.03 kg = 0.00016 kg CH4 head-1 day-1.

               Footnote 3: As no value is available for elk, the value for deer is used from IPCC (2019), Table 10.15
             */

            #endregion
        }
    }
}