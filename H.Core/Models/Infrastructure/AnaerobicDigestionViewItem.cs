using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Infrastructure;

namespace H.Core.Models.Infrastructure
{
    /// <summary>
    /// The AD view item class helps us encapsulate methods and functions, separate from the view model, that relate to a specific/select view that represents an AD component.
    /// A <see cref="AnaerobicDigestionViewItem"/> is part of an AD ViewModel but is specific to each component.
    /// </summary>
    public class AnaerobicDigestionViewItem : ModelBase
    {
        #region Fields

        private bool _hasManureSubstrateViewItems;
        private bool _hasFarmResiduesSubstrateViewItems;

        #endregion

        #region Constructors

        public AnaerobicDigestionViewItem()
        {
            this.ManureSubstrateViewItems.CollectionChanged += ManureSubstrateViewItemsOnCollectionChanged;
            this.FarmResiduesSubstrateViewItems.CollectionChanged += FarmResiduesSubstrateViewItemsOnCollectionChanged;
        }

        #endregion

        #region Properties

        /// <summary>
        /// A boolean value that represents whether a <see cref="ManureSubstrateViewItems"/> collection has any items in it.
        /// </summary>
        public bool HasManureSubstrateViewItems
        {
            get => _hasManureSubstrateViewItems;
            set => this.SetProperty(ref _hasManureSubstrateViewItems, value);
        }

        /// <summary>
        /// A boolean value that represents whether a <see cref="FarmResiduesSubstrateViewItems"/> collection has any items in it.
        /// </summary>
        public bool HasFarmResiduesSubstrateViewItems
        {
            get => _hasFarmResiduesSubstrateViewItems;
            set => this.SetProperty(ref _hasFarmResiduesSubstrateViewItems, value);
        }

        /// <summary>
        /// A collection of <see cref="ManureSubstrateViewItem"/>. This collection helps us bind to the view and display the manure substrate table.
        /// </summary>
        public ObservableCollection<ManureSubstrateViewItem> ManureSubstrateViewItems { get; set; } = new ObservableCollection<ManureSubstrateViewItem>();

        /// <summary>
        /// A collection of <see cref="FarmResiduesSubstrateViewItem"/>. This collection helps us bind to the view and display the manure substrate table.
        /// </summary>
        public ObservableCollection<FarmResiduesSubstrateViewItem> FarmResiduesSubstrateViewItems { get; set; } = new ObservableCollection<FarmResiduesSubstrateViewItem>();

        #endregion


        #region Public Methods
        #endregion

        #region Private Methods
        #endregion


        #region Event Handlers

        /// <summary>
        /// A method that is triggered when the <see cref="ManureSubstrateViewItems"/> collection changes. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManureSubstrateViewItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.HasManureSubstrateViewItems = this.ManureSubstrateViewItems.Count > 0;
            
        }

        /// <summary>
        /// A method that is triggered when the <see cref="FarmResiduesSubstrateViewItems"/> collection changes. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FarmResiduesSubstrateViewItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.HasFarmResiduesSubstrateViewItems = this.FarmResiduesSubstrateViewItems.Count > 0;
        }
        #endregion

    }
}
