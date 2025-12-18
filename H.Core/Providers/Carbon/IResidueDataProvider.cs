using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Providers.AnaerobicDigestion;

namespace H.Core.Providers.Carbon
{
    public interface IResidueDataProvider
    {
        Table_7_Relative_Biomass_Information_Data GetResidueData(IrrigationType irrigationType,
                                   double totalWaterInputs,
                                   CropType cropType,
                                   SoilFunctionalCategory soilFunctionalCategory,
                                   Province province);

        IEnumerable<Table_7_Relative_Biomass_Information_Data> GetData();
        Table_46_Biogas_Methane_Production_CropResidue_Data GetBiomethaneData(CropType cropType);
    }
}