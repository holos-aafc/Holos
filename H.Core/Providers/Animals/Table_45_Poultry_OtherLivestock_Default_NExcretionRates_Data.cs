namespace H.Core.Providers.Animals
{
    public class Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Data
    {
        /// <summary>
        /// N excretion_rate (kg head-1 day-1) (calc.from IPCC, 2019)

        /// </summary>
        public double NitrogenExcretionRatePerHead { get; set; }

        /// <summary>
        /// Average live animal weight (kg) (ECCC 2022)
        /// </summary>
        public double AverageLiveAnimalWeight { get; set; }

        /// <summary>
        /// Default values for N excretion rates (kg N (1000 kg animal mass)-1 day-1) (IPCC, 2019)
        /// </summary>
        public double NitrogenExcretionPerThousandKG { get; set; }
    }
}