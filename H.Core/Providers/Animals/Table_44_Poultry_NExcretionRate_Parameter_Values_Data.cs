using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class Table_44_Poultry_NExcretionRate_Parameter_Values_Data
    {
        /// <summary>
        /// The type of poultry for which data is needed.
        /// </summary>
        public AnimalType AnimalType { get; set; }

        /// <summary>
        /// (kg DM head^-1 day^-1)
        /// </summary>
        public double DailyMeanIntake { get; set; }

        /// <summary>
        /// Crude protein content (CP % of DM)
        /// </summary>
        public double CrudeProtein { get; set; }

        /// <summary>
        /// PRlw (average protein content in live weight (kg kg^-1 live weight)
        /// </summary>
        public double ProteinLiveWeight { get; set; }

        /// <summary>
        /// WG (weight gain, kg head^-1 day^-1)
        /// </summary>
        public double WeightGain { get; set; }

        /// <summary>
        /// PRegg average protein content of eggs (kg protein kg^-1)
        /// </summary>
        public double ProteinContentEgg { get; set; }

        /// <summary>
        /// EGG (g egg head^-1 day^-1)
        /// </summary>
        public double EggProduction { get; set; }

        /// <summary>
        /// BW-Initial (kg)
        /// </summary>
        public double InitialWeight { get; set; }

        /// <summary>
        /// BW-Final (kg)
        /// </summary>
        public double FinalWeight { get; set; }

        /// <summary>
        /// (days)
        /// </summary>
        public double ProductionPeriod { get; set; }
    }
}