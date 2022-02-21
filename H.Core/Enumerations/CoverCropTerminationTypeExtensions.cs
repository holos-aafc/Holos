using System.Collections.Generic;
using System.Linq;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public static class CoverCropTerminationTypeExtensions
    {
        public static IEnumerable<CoverCropTerminationType> GetValidCoverCropTerminationTypes()
        {
            return new List<CoverCropTerminationType>()
            {
                CoverCropTerminationType.Chemical, CoverCropTerminationType.Mechanical
            }.OrderBy(terminationType => terminationType.GetDescription());
        }
    }
}