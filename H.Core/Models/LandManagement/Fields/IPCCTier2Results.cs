using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Fields
{

    /// <summary>
    /// A class to hold intermediate results when running the IPCC Tier 2 model. Results will hold values related to N or C depending on the <see cref="CalculationMode"/>
    /// </summary>
    public class IPCCTier2Results : ModelBase
    {
        #region Fields

        private double _activePool;
        private double _passivePool;
        private double _slowPool;

        #endregion

        #region Properties

        /// <summary>
        /// k_a
        ///
        /// Equation 2.2.3-9
        ///
        /// (kg year^-1)
        /// </summary>
        public double ActivePoolDecayRate { get; set; }

        /// <summary>
        /// k_s
        ///
        /// Equation 2.2.3-10
        ///
        /// (kg year^-1)
        /// </summary>
        public double SlowPoolDecayRate { get; set; }

        /// <summary>
        /// k_p
        ///
        /// Equation 2.2.3-11
        ///
        /// (kg year^-1)
        /// </summary>
        public double PassivePoolDecayRate { get; set; }

        /// <summary>
        /// B_N
        ///
        /// Equation 2.7.3-1
        ///
        /// (kg ha^-1)
        /// </summary>
        public double Beta { get; set; }

        /// <summary>
        /// a_N
        ///
        /// Equation 2.7.3-2
        ///
        /// (kg ha^-1)
        /// </summary>
        public double Alpha { get; set; }

        /// <summary>
        /// Active_y*
        ///
        /// Equation 2.7.3-3
        ///
        /// (kg ha^-1)
        /// </summary>
        public double ActivePoolSteadyState { get; set; }

        /// <summary>
        /// Active_y
        /// 
        /// Active sub-pool in year y
        ///
        /// Equation 2.7.3-4
        /// 
        /// (kg ha^-1)
        /// </summary>
        public double ActivePool
        {
            get => _activePool;
            set => SetProperty(ref _activePool, value);
        }

        /// <summary>
        /// Slow_y*
        ///
        /// Equation 2.7.3-5
        ///
        /// (kg ha^-1)
        /// </summary>
        public double SlowPoolSteadyState { get; set; }

        /// <summary>
        /// Slow_y
        ///
        /// Slow sub-pool in year y
        ///
        /// Equation 2.7.3-6
        ///
        /// (kg ha^-1)
        /// </summary>
        public double SlowPool
        {
            get => _slowPool;
            set => SetProperty(ref _slowPool, value);
        }

        /// <summary>
        /// Passive_y*
        ///
        /// Equation 2.7.3-7
        ///
        /// (kg ha^-1)
        /// </summary>
        public double PassivePoolSteadyState { get; set; }

        /// <summary>
        /// Passive_y
        ///
        /// Passive sub-pool in year y
        ///
        /// Equation 2.7.3-8
        ///
        /// (kg ha^-1)
        /// </summary>
        public double PassivePool
        {
            get => _passivePool;
            set => SetProperty(ref _passivePool, value);
        }

        /// <summary>
        /// Equation 2.2.10-5
        ///
        /// (kg ha^-1)
        /// </summary>
        public double ActivePoolDiff { get; set; }

        /// <summary>
        /// Equation 2.2.10-5
        ///
        /// (kg ha^-1)
        /// </summary>
        public double SlowPoolDiff { get; set; }

        /// <summary>
        /// Equation 2.2.10-5
        ///
        /// (kg ha^-1)
        /// </summary>
        public double PassivePoolDiff { get; set; }

        #endregion
    }
}
