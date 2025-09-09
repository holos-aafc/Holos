using System.Collections.ObjectModel;
using System.Collections.Specialized;
using H.Infrastructure;

namespace H.Core.Models.Infrastructure
{
    /// <summary>
    ///     The AD view item class helps us encapsulate methods and functions, separate from the view model, that relate to a
    ///     specific/select view that represents an AD component.
    ///     A <see cref="AnaerobicDigestionViewItem" /> is part of an AD ViewModel but is specific to each component.
    /// </summary>
    public class AnaerobicDigestionViewItem : ModelBase
    {
        #region Constructors

        public AnaerobicDigestionViewItem()
        {
            ManureSubstrateViewItems.CollectionChanged += ManureSubstrateViewItemsOnCollectionChanged;
            FarmResiduesSubstrateViewItems.CollectionChanged += FarmResiduesSubstrateViewItemsOnCollectionChanged;
            CropResiduesSubstrateViewItems.CollectionChanged += CropResiduesSubstrateViewItemsOnCollectionChanged;
        }

        #endregion

        #region Fields

        private bool _hasManureSubstrateViewItems;
        private bool _hasFarmResiduesSubstrateViewItems;
        private bool _hasCropResiduesSubstrateViewItems;

        #endregion

        #region Properties

        public bool HasCropSubstrateViewItems
        {
            get => _hasCropResiduesSubstrateViewItems;
            set => SetProperty(ref _hasCropResiduesSubstrateViewItems, value);
        }

        /// <summary>
        ///     A boolean value that represents whether a <see cref="ManureSubstrateViewItems" /> collection has any items in it.
        /// </summary>
        public bool HasManureSubstrateViewItems
        {
            get => _hasManureSubstrateViewItems;
            set => SetProperty(ref _hasManureSubstrateViewItems, value);
        }

        /// <summary>
        ///     A boolean value that represents whether a <see cref="FarmResiduesSubstrateViewItems" /> collection has any items in
        ///     it.
        /// </summary>
        public bool HasFarmResiduesSubstrateViewItems
        {
            get => _hasFarmResiduesSubstrateViewItems;
            set => SetProperty(ref _hasFarmResiduesSubstrateViewItems, value);
        }

        /// <summary>
        ///     A collection of <see cref="ManureSubstrateViewItem" />. This collection helps us bind to the view and display the
        ///     manure substrate table.
        /// </summary>
        public ObservableCollection<ManureSubstrateViewItem> ManureSubstrateViewItems { get; set; } =
            new ObservableCollection<ManureSubstrateViewItem>();

        /// <summary>
        ///     A collection of <see cref="FarmResiduesSubstrateViewItem" />. This collection helps us bind to the view and display
        ///     the manure substrate table.
        /// </summary>
        public ObservableCollection<FarmResiduesSubstrateViewItem> FarmResiduesSubstrateViewItems { get; set; } =
            new ObservableCollection<FarmResiduesSubstrateViewItem>();

        public ObservableCollection<CropResidueSubstrateViewItem> CropResiduesSubstrateViewItems { get; set; } =
            new ObservableCollection<CropResidueSubstrateViewItem>();

        #endregion

        #region Event Handlers

        private void CropResiduesSubstrateViewItemsOnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs e)
        {
            HasCropSubstrateViewItems = CropResiduesSubstrateViewItems.Count > 0;
        }

        /// <summary>
        ///     A method that is triggered when the <see cref="ManureSubstrateViewItems" /> collection changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManureSubstrateViewItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HasManureSubstrateViewItems = ManureSubstrateViewItems.Count > 0;
        }

        /// <summary>
        ///     A method that is triggered when the <see cref="FarmResiduesSubstrateViewItems" /> collection changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FarmResiduesSubstrateViewItemsOnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs e)
        {
            HasFarmResiduesSubstrateViewItems = FarmResiduesSubstrateViewItems.Count > 0;
        }

        #endregion
    }
}