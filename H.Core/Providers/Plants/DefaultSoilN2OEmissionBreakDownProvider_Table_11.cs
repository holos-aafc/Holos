namespace H.Core.Providers.Plants
{
    /// <summary>
    /// Table 11
    /// </summary>
    public class DefaultSoilN2OEmissionBreakDownProvider_Table_11
    {
        private DefaultSoilN2OEmissionBreakDownData Data { get; set; }
        public DefaultSoilN2OEmissionBreakDownProvider_Table_11()
        {
            Data = new DefaultSoilN2OEmissionBreakDownData()
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
