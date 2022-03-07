using System.Collections.ObjectModel;

namespace H.Core.Models.LandManagement.Fields
{
    public partial class CropViewItem
    {
        #region Fields

        private double _totalCarbonUptakeByGrazingAnimals;

        private ObservableCollection<GrazingViewItem> _grazingViewItems;

        #endregion

        #region Properties

        public ObservableCollection<GrazingViewItem> GrazingViewItems
        {
            get => _grazingViewItems;
            set => SetProperty(ref _grazingViewItems, value);
        }

        /// <summary>
        /// Equation 12.3.2-1
        /// Equation 12.3.2-2
        /// 
        /// (kg C)
        /// </summary>
        public double TotalCarbonUptakeByGrazingAnimals
        {
            get => _totalCarbonUptakeByGrazingAnimals;
            set => SetProperty(ref _totalCarbonUptakeByGrazingAnimals, value);
        }

        #endregion
    }
}