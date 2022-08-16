namespace H.Core.Providers.Plants
{
    /// <summary>
    /// Table 18: Default soil N2O emission breakdown.
    /// </summary>
    public class DUPLICATE_Table_18_Default_Soil_N2O_Emission_BreakDown_Provider
    {
        private DUPLICATE_Table_18_Default_Soil_N2O_Emission_BreakDown_Data Data { get; set; }
        public DUPLICATE_Table_18_Default_Soil_N2O_Emission_BreakDown_Provider()
        {
            Data = new DUPLICATE_Table_18_Default_Soil_N2O_Emission_BreakDown_Data()
            {
                JanurarySoilN2OEmission = 0,
                FebruarySoilN2OEmission = 0,
                MarchSoilN2OEmission = 5,
                AprilSoilN2OEmission = 30,
                MaySoilN2OEmission = 20,
                JuneN2OEmission = 15,
                JulySoilN2OEmission = 5,
                AugustN2OEmission = 5,
                SeptemberSoilN2OEmission = 15,
                OctoberSoilN2OEmission = 5,
                NovemberSoilN2OEmission = 0,
                DecemberSoilN2OEmission = 0
            };
        }

    }
}
