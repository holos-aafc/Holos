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

        /// <summary>
        /// Equation 12.3.2-4
        ///
        /// (kg C)
        /// </summary>
        public double TotalCarbonUtilizedByGrazingAnimals
        {
            get
            {
                if (this.ForageUtilizationRate <= 0)
                {
                    return 0;
                }

                return this.TotalCarbonUptakeByGrazingAnimals * (1.0 / (this.ForageUtilizationRate / 100.0));
            }
        }

        #endregion
    }
}