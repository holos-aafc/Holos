using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Properties;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService
    {
        #region Public Methods

        public void AssignUndersownCropViewItemsDescription(
            IEnumerable<CropViewItem> viewItems)
        {
            foreach (var cropViewItem in viewItems)
                if (cropViewItem.CropType.IsPerennial() &&
                    cropViewItem.IsSecondaryCrop &&
                    cropViewItem.YearInPerennialStand == 1)
                    // Perennial description will be set at this point so append here
                    cropViewItem.Description += $" ({Resources.LabelUndersown})";
        }

        #endregion

        #region Private Methods

        protected void ProcessUndersownCrops(
            IEnumerable<CropViewItem> viewItems,
            FieldSystemComponent fieldSystemComponent)
        {
            AssignUndersownCropViewItemsDescription(viewItems);
        }

        #endregion
    }
}