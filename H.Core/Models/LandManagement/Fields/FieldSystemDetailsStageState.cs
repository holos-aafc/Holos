using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace H.Core.Models.LandManagement.Fields
{
    /// <summary>
    /// A class to hold all of the annual crop view items that are used for multiyear carbon modelling.
    /// </summary>
    public class FieldSystemDetailsStageState : StageStateBase
    {
        #region Properties

        /// <summary>
        /// This is a collection to hold the main crops grown in the year. This is the collection of view items that can be edited (year by year) by the user if needed. It is this collection that will then
        /// be merged into a another collection that will be used by ICBM. These items have to be merged since it may contain multiple items for the same year but ICBM needs to have a single view item per year
        /// to perform calculations correctly.
        /// </summary>
        public ObservableCollection<CropViewItem> DetailsScreenViewCropViewItems { get; set; } = new ObservableCollection<CropViewItem>();

        #endregion

        #region Public Methods

        public override void ClearState()
        {
            this.DetailsScreenViewCropViewItems.Clear();
        }

        public IList<CropViewItem> GetMainCropsByField(FieldSystemComponent fieldSystemComponent)
        {
            return this.DetailsScreenViewCropViewItems.Where(viewItem => viewItem.FieldSystemComponentGuid == fieldSystemComponent.Guid).ToList();
        }

        #endregion
    }
}
