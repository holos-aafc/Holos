using System.Collections.Generic;
using H.Core.Enumerations;

namespace H.Core.Providers.Carbon
{
    public interface IResidueDataProvider
    {
        Table_7_Relative_Biomass_Information_Data GetResidueData(IrrigationType irrigationType,
                                   double irrigationAmount,
                                   CropType cropType,
                                   SoilFunctionalCategory soilFunctionalCategory,
                                   Province province);

        IEnumerable<Table_7_Relative_Biomass_Information_Data> GetData();
    }
}