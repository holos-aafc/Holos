using H.Core.CustomAttributes;
using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class Table_39_Livestock_Emission_Conversion_Factors_Data : IEmissionData
    {
        public AnimalType AnimalType { get; set; }
        public ManureStateType HandlingSystem { get; set; }

        /// <summary>
        /// Methane conversion factor of manure
        ///
        /// MCF
        ///
        /// (kg kg^-1)
        /// </summary>
        public double MethaneConversionFactor { get; set; }

        /// <summary>
        /// Direct N2O emission factor
        /// 
        /// EF_direct
        ///
        /// [kg N2O-N (kg N)^-1]
        /// </summary>
        public double N20DirectEmissionFactor { get; set; }

        /// <summary>
        /// Fraction of volatilization
        ///
        /// Frac_volatilization
        /// 
        /// [kg NH3-N (kg N)^-1]
        /// </summary>
        public double VolatilizationFraction { get; set; }

        /// <summary>
        /// Emission factor for volatilization
        ///
        /// EF_volatilization
        /// 
        /// [kg n2O-N (kg N)^-1] 
        /// </summary>
        public double EmissionFactorVolatilization { get; set; }

        /// <summary>
        /// Fraction of leaching
        ///
        /// Frac_leach
        ///
        /// [kg N (kg N)^-1]
        /// </summary>
        public double LeachingFraction { get; set; }

        /// <summary>
        /// Emission factor for leaching
        ///
        /// EF_leach
        ///
        /// [kg N2O-N (kg N)^-1]
        /// </summary>
        public double EmissionFactorLeach { get; set; }

        [Units(MetricUnitsOfMeasurement.KilogramPerHeadPerDay)]
        public double MethaneEntericRate { get; set; }

        [Units(MetricUnitsOfMeasurement.KilogramPerHeadPerDay)]
        public double MethaneManureRate { get; set; }

        [Units(MetricUnitsOfMeasurement.KilogramPerHeadPerDay)]
        public double NitrogenExcretionRate { get; set; }
    }
}