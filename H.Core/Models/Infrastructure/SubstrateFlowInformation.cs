using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Core.Models.Infrastructure
{
    public enum SubstrateType
    {
        StoredManure,
        FreshManure,
        FarmResidues
    }

    public class SubstrateFlowInformation
    {
        public AnimalType AnimalType { get; set; }
        public FarmResidueType FarmResidueType { get; set; }
        public SubstrateType SubstrateType { get; set; }

        public double TotalMassFlow { get; set; }
        public double VolatileSolidsFlow { get; set; }
        public double TotalSolidsFlow { get; set; }
        public double NitrogenFlow { get; set; }
        public double CarbonFlow { get; set; }
        public double OrganicNitrogenFlow { get; set; }
        public double TotalAmmonicalNitrogenFlow { get; set; }
        public double BiodegradableSolidsFlow { get; set; }
        public double MethaneProduction { get; set; }
        public double DegradedVolatileSolids { get; set; }
        public double BiogasProduction { get; set; }
        public double CarbonDioxideProduction { get; set; }
        public double TanFlowInDigestate { get; set; }
        public double OrganicNitrogenFlowInDigestate { get; set; }
        public double CarbonFlowInDigestate { get; set; }
    }
}
