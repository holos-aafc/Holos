using System.ComponentModel;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Economics;

namespace H.Core.Models.Results
{
    public class EconomicsResultsViewItem : EstimatesOfProductionResultsViewItem
    {
        #region Fields

        private double _totalVariableCost;
        private double _totalFixedCost;
        private Farm _farm;
        private CropEconomicData _cropEconomicData;
        private CropViewItem _cropViewItem;

        private double _profit;
        private double _revenues;

        #endregion

        #region Properties

        public CropEconomicData CropEconomicData
        {
            get => _cropEconomicData;
            set
            {
                SetProperty(ref _cropEconomicData, value, () =>
                {
                    value.PropertyChanged -= ValueOnPropertyChanged;
                    value.PropertyChanged += ValueOnPropertyChanged;
                });
            }
        }

        private void ValueOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.RaisePropertyChanged(e.PropertyName);
        }

        public double Revenues
        {
            get => _revenues;
            set => SetProperty(ref _revenues, value);
        }

        public CropViewItem CropViewItem
        {
            get => _cropViewItem;
            set => SetProperty(ref _cropViewItem, value);
        }

        public double TotalVariableCost
        {
            get => _totalVariableCost;
            set => SetProperty(ref _totalVariableCost, value);
        }

        public double TotalFixedCost
        {
            get => _totalFixedCost;
            set => SetProperty(ref _totalFixedCost, value);
        }

        private double _totalCost;

        public double TotalCost
        {
            get => _totalCost;
            set => SetProperty(ref _totalCost, value);
        }
        public double Profit
        {
            get => _profit;
            set => SetProperty(ref _profit, value);
        }

        public Farm Farm
        {
            get => _farm;
            set => SetProperty(ref _farm, value);
        }

        #endregion 
    }
}