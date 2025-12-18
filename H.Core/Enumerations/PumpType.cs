using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum PumpType
    {
        [LocalizedDescription("EnumElectricPump", typeof(Resources))]
        ElectricPump,

        [LocalizedDescription("EnumNaturalGasPump", typeof(Resources))]
        NaturalGasPump,
    }
}
