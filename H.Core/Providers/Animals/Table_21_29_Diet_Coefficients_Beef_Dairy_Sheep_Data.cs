using H.Core.CustomAttributes;
using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class Table_21_29_Diet_Coefficients_Beef_Dairy_Sheep_Data
    {
        /// <summary>
        /// The livestock group of the beef cattle, dairy cattle and sheep.
        /// </summary>
        public AnimalType AnimalType { get; set; }
        
        /// <summary>
        /// The type of diet of the animal
        /// </summary>
        public DietType DietType { get; set; }

        /// <summary>
        /// TDN = Percent total digestible nutrients in feed
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Percentage)] 
        public double TotalDigestibleNutrients { get; set; }


        /// <summary>
        /// Cp = Crude protein content (kg kg-1) 
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramPerKilogram)] 
        public double CrudeProteinContent { get; set; }

        /// <summary>
        /// Ym = Methane conversion factor 
        /// </summary>
        public double MethaneConversionFactor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramPerKilogram)] 
        public double StarchContent { get; set; }

        /// <summary>
        /// Forage (% DM)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.PercentageDryMatter)]
        public double ForageContent { get; set; }

        /// <summary>
        /// NDF = Neutral Detergent Fiber (kg kg^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramPerKilogram)]
        public double NeutralDetergentFiber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramPerKilogram)] 
        public double AcidDetergentFiber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaCaloriePerKilogram)] 
        public double MetabolizableEnergy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Percentage)] 
        public double EtherExtract { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaCaloriePerKilogram)] 
        public double NetEnergyLactation { get; set; }
    }
}