#region Imports

using H.Core.Enumerations;

#endregion

namespace H.Core.Calculators.Tillage
{
    /// <summary>
    /// </summary>
    public class TillageFactorTableRow
    {
        #region Properties

        public SoilFunctionalCategory SoilFunctionalCategory { get; set; }
        public TillageType TillageType { get; set; }
        public double TillageFactor { get; set; }

        #endregion
    }
}