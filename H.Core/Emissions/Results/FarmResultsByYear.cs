namespace H.Core.Emissions.Results
{
    public class FarmResultsByYear
    {
        #region Constructors

        public FarmResultsByYear()
        {
            this.FarmEnergyResults = new FarmEnergyResults();
        }

        #endregion

        public int Year { get; set; }
        public FarmEnergyResults FarmEnergyResults { get; set; }
    }
}