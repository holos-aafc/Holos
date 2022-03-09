namespace H.Core.Models.LandManagement.Fields
{
    public partial class CropViewItem
    {
        #region Fields

        private double _totalCarbonExportedFromBales;

        #endregion

        #region Properties

        public double TotalCarbonExportedFromBales
        {
            get => _totalCarbonExportedFromBales;
            set => SetProperty(ref _totalCarbonExportedFromBales, value);
        }

        #endregion
    }
}