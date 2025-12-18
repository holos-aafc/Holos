namespace H.Core.Models.LandManagement.Fields
{
    public partial class CropViewItem
    {
        #region Public Methods

        public bool IsFinalYearInPerennialStand()
        {
            return YearInPerennialStand == PerennialStandLength;
        }

        #endregion
    }
}