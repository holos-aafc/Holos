using System;
using System.Collections.Generic;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.LandManagement;

namespace H.Core.Services
{
    public interface IFieldComponentHelper
    {
        void InitializeComponent(FieldSystemComponent component, Farm farm);
        void Replicate(ComponentBase copyFrom, ComponentBase copyTo);
        FieldSystemComponent Replicate(FieldSystemComponent component);
        string GetUniqueFieldName(IEnumerable<FieldSystemComponent> components);

        /// <summary>
        /// Determines which view item is the main crop for a particular year. Will use the boolean <see cref="H.Core.Models.LandManagement.Fields.CropViewItem.IsSecondaryCrop"/> to determine which view item
        /// is the main crop for the particular year.
        /// </summary>
        CropViewItem GetMainCropForYear(IEnumerable<CropViewItem> viewItems,
            int year);

        CropViewItem GetCoverCropForYear(
            IEnumerable<CropViewItem> viewItems,
            int year);

        AdjoiningYears GetAdjoiningYears(IEnumerable<CropViewItem> viewItems,
            int year);
    }
}