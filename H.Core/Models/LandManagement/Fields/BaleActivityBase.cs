namespace H.Core.Models.LandManagement.Fields
{
    public abstract class BaleActivityBase : FieldActivityBase
    {
        #region Fields

        private double _baleWeight;

        #endregion

        #region Properties

        /// <summary>
        ///     The total weight of each bale. This is the wet bale weight.
        ///     (kg)
        /// </summary>
        public double BaleWeight
        {
            get => _baleWeight;
            set => SetProperty(ref _baleWeight, value);
        }

        #endregion
    }
}