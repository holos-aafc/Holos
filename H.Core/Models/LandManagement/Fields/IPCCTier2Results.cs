using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Fields
{
    public class IPCCTier2Results : ModelBase
    {
        #region Fields

        private double _activePool;
        private double _passivePool;
        private double _slowPool;

        #endregion

        /// <summary>
        /// Active sub-pool SOC stock in year y
        /// 
        /// (tonnes C ha^-1)
        /// </summary>
        public double ActivePool {
            get
            {
                return _activePool;
            }
            set
            {
                SetProperty(ref _activePool, value);
            } }

        public double Beta { get; set; }
        public double Alpha { get; set; }
        public double ActivePoolDecayRate { get; set; }
        public double SlowPoolDecayRate { get; set; }
        public double PassivePoolDecayRate { get; set; }
        public double ActivePoolSteadyState { get; set; }
        public double SlowPoolSteadyState { get; set; }

        public double SlowPool
        {
            get
            {
                return _slowPool;
            }
            set
            {
                SetProperty(ref _slowPool, value);
            }
        }


        public double PassivePoolSteadyState { get; set; }

        public double PassivePool
        {
            get
            {
                return _passivePool;
            }
            set
            {
                SetProperty(ref _passivePool, value);
            }
        }
        public double ActivePoolDiff { get; set; }
        public double SlowPoolDiff { get; set; }
        public double PassivePoolDiff { get; set; }
    }
}
