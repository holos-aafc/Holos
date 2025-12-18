using H.Core.Enumerations;
using H.Core.Models.Animals;
using H.Core.Providers.Animals;

namespace H.Core.Models.Infrastructure
{
    public class SubstrateFlowInformation : SubstrateViewItemBase
    {
        public AnimalType AnimalType { get; set; }
        public SubstrateType SubstrateType { get; set; }
        public ManagementPeriod ManagementPeriod { get; set; }
        public AnaerobicDigestionComponent Component { get; set; }
        public DefaultManureCompositionData ManureCompositionData { get; set; }

        /// <summary>
        ///     (kg day^-1)
        /// </summary>
        public double TotalMassFlowOfSubstrate { get; set; }

        /// <summary>
        ///     (kg day^-1)
        /// </summary>
        public double VolatileSolidsFlowOfSubstrate { get; set; }

        /// <summary>
        ///     (kg DM day^-1)
        /// </summary>
        public double TotalSolidsFlowOfSubstrate { get; set; }

        /// <summary>
        ///     (kg day^-1)
        /// </summary>
        public double NitrogenFlowOfSubstrate { get; set; }

        /// <summary>
        ///     (kg day^-1)
        /// </summary>
        public double CarbonFlowOfSubstrate { get; set; }

        public double OrganicNitrogenFlowOfSubstrate { get; set; }

        /// <summary>
        ///     This is the TAN in substrate only
        /// </summary>
        public double ExcretedTanInSubstrate { get; set; }

        /// <summary>
        ///     (kg day^-1)
        /// </summary>
        public double BiodegradableSolidsFlow { get; set; }

        /// <summary>
        ///     (Nm^3 day^-1)
        /// </summary>
        public double MethaneProduction { get; set; }

        /// <summary>
        ///     (kg VS day^-1)
        /// </summary>
        public double DegradedVolatileSolids { get; set; }

        /// <summary>
        ///     (Nm^3 day^-1)
        /// </summary>
        public double BiogasProduction { get; set; }

        /// <summary>
        ///     (Nm^3 day^-1)
        /// </summary>
        public double CarbonDioxideProduction { get; set; }

        /// <summary>
        ///     The TAN from excretion plus the added nitrogen from degraded volatile solids
        /// </summary>
        public double TanFlowInDigestate { get; set; }

        public double OrganicNitrogenFlowInDigestate { get; set; }

        public double CarbonFlowInDigestate { get; set; }


        #region Public Methods

        public bool IsLiquidManure()
        {
            return ManagementPeriod != null && ManagementPeriod.ManureDetails != null &&
                   ManagementPeriod.ManureDetails.StateType.IsLiquidManure();
        }

        #endregion
    }
}