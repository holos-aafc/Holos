using H.Core.Properties;
using H.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Core.Enumerations
{
    public enum CarbonModellingStrategies
    {
        [LocalizedDescription("EnumIPCCTier2", typeof(Resources))]
        IPCCTier2,

        [LocalizedDescription("EnumICBM", typeof(Resources))]
        ICBM,
    }
}
