namespace H.Core.Providers.Animals
{
    public class PoultryDietInformationProviderData
    {
        /// <summary>
        /// (kg DM head^-1 day^-1)
        /// </summary>
        public double DailyMeanIntake { get; set; }

        /// <summary>
        /// (CP % of DM)
        /// </summary>
        public double CrudeProtein { get; set; }

        /// <summary>
        /// Average protein in live weight
        ///
        /// (kg kg^-1)
        /// </summary>
        public double ProteinLiveWeight { get; set; }

        /// <summary>
        /// (kg head^-1 day^-1)
        /// </summary>
        public double WeightGain { get; set; }

        /// <summary>
        /// (kg kg^-1)
        /// </summary>
        public double ProteinContentEgg { get; set; }

        /// <summary>
        /// (g egg head^-1 day^-1)
        /// </summary>
        public double EggProduction { get; set; }

        /// <summary>
        /// (kg)
        /// </summary>
        public double InitialWeight { get; set; }

        /// <summary>
        /// (kg)
        /// </summary>
        public double FinalWeight { get; set; }

        /// <summary>
        /// (days)
        /// </summary>
        public double ProductionPeriod { get; set; }
    }
}