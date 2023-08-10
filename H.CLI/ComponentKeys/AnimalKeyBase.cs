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
        }

        #endregion

        #region Public Methods

        public bool IsHeaderOptional(string s)
        {
            return false;
        }

        #endregion

        #region Private Methods

        #endregion
    }
}