namespace H.Core.Models.LandManagement.Fields
{
    public partial class CropViewItem
    {
        #region Public Methods

        public bool IsFinalYearInPerennialStand()
        {
            return this.YearInPerennialStand == this.PerennialStandLength;
        }

        #endregion
    }
}