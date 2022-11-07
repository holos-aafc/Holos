using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum SoilReductionFactors
    {
        [LocalizedDescription("EnumControlledRelease", typeof(Resources))]
        ControlledRelease,

        [LocalizedDescription("EnumNitrificationInhibitor", typeof(Resources))]
        NitrificationInhibitor,

        [LocalizedDescription("EnumUreaseInhibitor", typeof(Resources))]
        UreaseInhibitor,

        [LocalizedDescription("EnumNitrificationAndUreaseInhibitor", typeof(Resources))]
        NitrificationAndUreaseInhibitor,

        [LocalizedDescription("EnumNone", typeof(Resources))]
        None,
    }
}
