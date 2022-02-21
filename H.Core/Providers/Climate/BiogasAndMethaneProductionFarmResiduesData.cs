using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Core.Providers.Climate
{
    public class BiogasAndMethaneProductionFarmResiduesData : BiogasAndMethaneProductionParametersData
    {
        public FarmResidueType ResidueType { get; set; }
    }
}
