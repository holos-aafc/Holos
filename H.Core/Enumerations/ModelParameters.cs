using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum ModelParameters
    {
        /// <summary>
        /// TillFac = Tillage disturbance modifier for decay rates
        /// </summary>
        [LocalizedDescription("EnumTillageModifier", typeof(Resources))]
        TillageModifier,

        /// <summary>
        /// Ws = Slope parameter for mappet term to estimate wfac
        /// </summary>
        [LocalizedDescription("EnumSlopeParameter", typeof(Resources))]
        SlopeParameter,

        /// <summary>
        /// Kfaca = Decay rate constant under optimal conditions for decomposition of the active sub-pool
        /// </summary>
        [LocalizedDescription("EnumDecayRateActive", typeof(Resources))]
        DecayRateActive,

        /// <summary>
        /// Kfacs = Decay rate constant under optimal conditions for decomposition of the slow sub-pool
        /// </summary>
        [LocalizedDescription("EnumDecayRateSlow", typeof(Resources))]
        DecayRateSlow,

        /// <summary>
        /// Kfacp = Decay rate constant under optimal conditions for decomposition of the passive sub-pool
        /// </summary>
        [LocalizedDescription("EnumDecayRatePassive", typeof(Resources))]
        DecayRatePassive,

        /// <summary>
        /// f1 = Fraction of metabolic dead organic matter decay products transferred to the active sub-pool
        /// </summary>
        [LocalizedDescription("EnumFractionMetabolicDMActivePool", typeof(Resources))]
        FractionMetabolicDMActivePool,

        /// <summary>
        /// f2 = Fraction of structural dead organic matter decay products transferred the active sub-pool
        /// </summary>
        [LocalizedDescription("EnumFractionStructuralDMActivePool", typeof(Resources))]
        FractionStructuralDMActivePool,

        /// <summary>
        /// f3 = Fraction of structural dead organic matter decay products transferred to the slow sub-pool
        /// </summary>
        [LocalizedDescription("EnumFractionStructuralDMSlowPool", typeof(Resources))]
        FractionStructuralDMSlowPool,

        /// <summary>
        /// f5 = Fraction of active sub-pool decay products transferred to the passive sub-pool
        /// </summary>
        [LocalizedDescription("EnumFractionActiveDecayToPassive", typeof(Resources))]
        FractionActiveDecayToPassive,

        /// <summary>
        /// f6 = Fraction of slow sub-pool decay products transferred to the passive sub-pool
        /// </summary>
        [LocalizedDescription("EnumFractionSlowDecayToPassive", typeof(Resources))]
        FractionSlowDecayToPassive,

        /// <summary>
        /// f7 = Fraction of slow sub-pool decay products transferred to the active sub-pool
        /// </summary>
        [LocalizedDescription("EnumFractionSlowDecayToActive", typeof(Resources))]
        FractionSlowDecayToActive,

        /// <summary>
        /// f8 = Fraction of passive sub-pool decay products transferred to the active sub-pool
        /// </summary>
        [LocalizedDescription("EnumFractionPassiveDecayToActive", typeof(Resources))]
        FractionPassiveDecayToActive,

        /// <summary>
        /// Topt = Optimum temperature to estimate temperature modifier on decomposition
        /// </summary>
        [LocalizedDescription("EnumOptimumTemperature", typeof(Resources))]
        OptimumTemperature,

        /// <summary>
        /// Tmax = Maximum monthly average temperature for decomposition.
        /// </summary>
        [LocalizedDescription("EnumMaximumAvgTemperature", typeof(Resources))]
        MaximumAvgTemperature,
    }
}
