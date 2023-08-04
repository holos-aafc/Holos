using H.Core.Enumerations;
using System.Collections.Generic;

namespace H.CLI.ComponentKeys
{
    public abstract class AnimalKeyBase
    {
        #region Properties

        public Dictionary<string, ImperialUnitsOfMeasurement?> Keys { get; set; }
        public Dictionary<string, bool> MissingHeaders { get; set; } = new Dictionary<string, bool>();

        #endregion

        #region Constructors

        protected AnimalKeyBase()
        {
            this.Keys = new Dictionary<string, ImperialUnitsOfMeasurement?>();

            AddCommonKeys(Keys);
        }

        #endregion

        #region Public Methods

        public bool IsHeaderOptional(string s)
        {
            return false;
        }

        #endregion

        #region Private Methods

        private static void AddCommonKeys(IDictionary<string, ImperialUnitsOfMeasurement?> keys)
        {
            keys.Add( Properties.Resources.Key_Name, null );
            keys.Add(H.Core.Properties.Resources.ComponentType, null);
            keys.Add(Properties.Resources.GroupName, null);
            keys.Add(Properties.Resources.GroupType, null);
            keys.Add(Properties.Resources.ManagementPeriodName, null);
            keys.Add(Properties.Resources.ManagementPeriodStartDate, null);
            keys.Add(Properties.Resources.ManagementPeriodDays, null);
            keys.Add(Properties.Resources.NumberOfAnimals, null);
        }

        #endregion
    }
}