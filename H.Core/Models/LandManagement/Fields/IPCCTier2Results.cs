using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Core.Models.LandManagement.Fields
{
    public class IPCCTier2Results
    {
        /// <summary>
        /// Active sub-pool SOC stock in year y
        /// 
        /// (tonnes C ha^-1)
        /// </summary>
        public double ActivePool { get; set; }

        public double Beta { get; set; }
        public double Alpha { get; set; }
        public double ActivePoolDecayRate { get; set; }
        public double SlowPoolDecayRate { get; set; }
        public double PassivePoolDecayRate { get; set; }
        public double ActivePoolSteadyState { get; set; }
        public double SlowPoolSteadyState { get; set; }
        public double SlowPool { get; set; }
        public double PassivePoolSteadyState { get; set; }
        public double PassivePool { get; set; }
        public double ActivePoolDiff { get; set; }
        public double SlowPoolDiff { get; set; }
        public double PassivePoolDiff { get; set; }
    }
}
