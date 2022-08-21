using System.Collections.Generic;
using H.Core.Enumerations;

namespace H.Core.Providers.Carbon
{
    public interface IResidueDataProvider
    {
        Table_10_Relative_Biomass_Data GetResidueData(IrrigationType irrigationType,
                                   double irrigationAmount,
                                   CropType cropType,
                                   SoilFunctionalCategory soilFunctionalCategory,
                                   Province province);

        IEnumerable<Table_10_Relative_Biomass_Data> GetData();
    }
}