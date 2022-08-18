using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Converters;
using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 45. Default nitrogen excretion rates for poultry and other livestock.
    /// </summary>
    public class Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Provider
    {
        #region Public Methods
        public double GetNitrogenExcretionRateValue(AnimalType animalType)
        {
            var result = this.GetNExcretionRateByAnimalType(animalType);
            if (result != null)
            {
                return result.NitrogenExcretionRatePerHead;
            }
            else
            {
                return 0;
            }
        }

        public Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Data GetNExcretionRateByAnimalType(AnimalType animalType)
        {
            if (animalType == AnimalType.ChickenPullets || animalType == AnimalType.ChickenCockerels)
            {
                return new Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Data()
                {
                    AverageLiveAnimalWeight = 0.725,
                    NitrogenExcretionPerThousandKG = 0,
                    NitrogenExcretionRatePerHead = 0.0009,
                };
            }

            if (animalType == AnimalType.ChickenHens)
            {
                return new Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Data()
                {
                    AverageLiveAnimalWeight = 1.8,
                    NitrogenExcretionPerThousandKG = 0,
                    NitrogenExcretionRatePerHead = 0.0017,
                };
            }

            if (animalType == AnimalType.ChickenRoosters)
            {
                return new Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Data()
                {
                    AverageLiveAnimalWeight = 0.9,
                    NitrogenExcretionPerThousandKG = 0,
                    NitrogenExcretionRatePerHead = 0.0022,
                };
            }

            if (animalType.IsTurkeyType())
            {
                return new Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Data()
                {
                    AverageLiveAnimalWeight = 6.8,
                    NitrogenExcretionPerThousandKG = 0.74,
                    NitrogenExcretionRatePerHead = 0.005,
                };
            }

            if (animalType == AnimalType.Ducks)
            {
                return new Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Data()
                {
                    AverageLiveAnimalWeight = 2.7,
                    NitrogenExcretionPerThousandKG = 0.83,
                    NitrogenExcretionRatePerHead = 0.0022,
                };
            }

            // Footnote 1
            if (animalType == AnimalType.Geese)
            {
                return new Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Data()
                {
                    AverageLiveAnimalWeight = 4,
                    NitrogenExcretionPerThousandKG = 0.83,
                    NitrogenExcretionRatePerHead = 0.0033,
                };
            }

            if (animalType == AnimalType.Goats)
            {
                return new Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Data()
                {
                    AverageLiveAnimalWeight = 64,
                    NitrogenExcretionPerThousandKG = 0.46,
                    NitrogenExcretionRatePerHead = 0.0294,
                };
            }

            // Footnote 2
            if (animalType == AnimalType.Llamas || animalType == AnimalType.Alpacas)
            {
                return new Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Data()
                {
                    AverageLiveAnimalWeight = 112,
                    NitrogenExcretionPerThousandKG = 0.35,
                    NitrogenExcretionRatePerHead = 0.0392,
                };
            }


            // Footnote 3
            if (animalType == AnimalType.Deer || animalType == AnimalType.Elk)
            {
                return new Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Data()
                {
                    AverageLiveAnimalWeight = 120,
                    NitrogenExcretionPerThousandKG = 0.67,
                    NitrogenExcretionRatePerHead = 0.0804,
                };
            }

            if (animalType == AnimalType.Horses)
            {
                return new Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Data()
                {
                    AverageLiveAnimalWeight = 450,
                    NitrogenExcretionPerThousandKG = 0.3,
                    NitrogenExcretionRatePerHead = 0.1350,
                };
            }

            if (animalType == AnimalType.Mules)
            {
                return new Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Data()
                {
                    AverageLiveAnimalWeight = 245,
                    NitrogenExcretionPerThousandKG = 0.3,
                    NitrogenExcretionRatePerHead = 0.0735,
                };
            }

            // Footnote 4
            if (animalType == AnimalType.Bison)
            {
                return new Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Data()
                {
                    AverageLiveAnimalWeight = 580,
                    NitrogenExcretionPerThousandKG = 0.4,
                    NitrogenExcretionRatePerHead = 0.2320,
                };
            }

            Trace.TraceError($"{nameof(Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Provider)}.{nameof(this.GetNExcretionRateByAnimalType)}" +
                             $" unable to get data for animal type: {animalType}." +
                             $" Returning default value of 0.");

            return new Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Data();
        }

        #endregion

        #region Footnotes

        /*
         * Source: Calculated based on ECCC (2022) default values for live animal weight (kg) for turkeys, goats, llamas and alpacas, deer and elk, horses and mules (Table A3.4-25) and the IPCC (2019) default live animal weight (kg) for North America for ducks (Table 10A.5) and default values for N excretion rates (kg N (1000 kg animal mass)-1 day-1 from iPCC (2019), Table 10.19).
            Footnote 1: Average weight of geese based on value for broiler geese for meat production from FAO (2002); N excretion rate (1000 kg animal mass)-1 day-1) is from IPCC (2019) – value for ducks used.
            Footnote 2: N excretion rate for sheep (from IPCC (2019), Table 10.19) was used for llamas and alpacas.
            Footnote 3: Elk was added to this group.
            Footnote 4: N excretion rate value for ‘other cattle’ was used.

         */
        #endregion
    }
}
