using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace H.Core.Models.LandManagement.Fields
{
    public partial class CropViewItem
    {
        #region Fields

        private ObservableCollection<DigestateApplicationViewItem> _digestateApplicationViewItems;

        #endregion

        #region Properties
        
        public ObservableCollection<DigestateApplicationViewItem> DigestateApplicationViewItems
        {
            get => _digestateApplicationViewItems;
            set => SetProperty(ref _digestateApplicationViewItems, value);
        }

        public bool HasDigestateApplications { get; set; }

        #endregion

        #region Event Handlers

        private void DigestateApplicationViewItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.HasDigestateApplications = this.DigestateApplicationViewItems.Count > 0;
            this.RaisePropertyChanged(nameof(this.HasDigestateApplications));
        }

        #endregion
    }
}