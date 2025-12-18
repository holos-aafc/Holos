using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{    
    public enum CoverCropTypes
    {
        [LocalizedDescription("EnumOptionA", typeof(Resources))]
        OptionA,

        [LocalizedDescription("EnumOptionB", typeof(Resources))]
        OptionB,

        [LocalizedDescription("EnumOptionC", typeof(Resources))]
        OptionC
    }
}