using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Core.Providers.Soil
{
    public class SOCData
    {
        public SOCData()
        {
            this.SocDataByRotationName = new Dictionary<string, double>();
        }

        public double MeasuredCarbonAtYear { get; set; }
        public int Year { get; set; }

        public double W_N0P0 { get; set; }
        public double W_N45P0 { get; set; }
        public double W_N0P20 { get; set; }
        public double W_N45P20 { get; set; }
        public double FW_N0P0 { get; set; }
        public double FW_N45P0 { get; set; }
        public double FW_N0P20 { get; set; }
        public double FW_N45P20 { get; set; }
        public double FWW_N0P0 { get; set; }
        public double FWW_N45P0 { get; set; }
        public double FWW_N0P20 { get; set; }
        public double FWW_N45P20 { get; set; }

        public Dictionary<string, double> SocDataByRotationName { get; set; }
    }
}
