using System.Collections.Generic;
using H.Core.Enumerations;

namespace H.Core.Providers.Carbon
{
    public interface IResidueDataProvider
    {
        ResidueData GetResidueData(IrrigationType irrigationType,
                                   double irrigationAmount,
                                   CropType cropType,
                                   SoilFunctionalCategory soilFunctionalCategory,
                                   Province province);

        IEnumerable<ResidueData> GetData();
    }
}