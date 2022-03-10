namespace H.Core.Models.LandManagement.Fields
{
    public partial class CropViewItem
    {
        #region Fields
        
        private double _totalCarbonLossFromBaleExports;

        #endregion

        #region Properties

        /// <summary>
        /// Equation 12.3.2-4
        ///
        /// (kg C)
        /// </summary>
        public double TotalCarbonLossFromBaleExports
        {
            get => _totalCarbonLossFromBaleExports;
            set => SetProperty(ref _totalCarbonLossFromBaleExports, value);
        }

        #endregion
    }
}