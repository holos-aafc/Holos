using System.Collections.Generic;
using System.Linq;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen.NitrogenService
{
    public partial class NitrogenService
    {
        #region Public Methods

        public void ProcessCommandLineItems(List<CropViewItem> viewItems, Farm farm)
        {
            if (farm.IsCommandLineMode == false) return;

            for (var i = 0; i < viewItems.Count; i++)
            {
                var itemAtIndex = viewItems.ElementAt(i);
                var year = itemAtIndex.Year;
                var tuple = _fieldComponentHelper.GetAdjoiningYears(viewItems, year);

                var currentYearViewItem = tuple.CurrentYearViewItem;
                var previousYearViewItem = tuple.PreviousYearViewItem;

                if (currentYearViewItem.CombinedAboveGroundResidueNitrogen == 0 &&
                    currentYearViewItem.CombinedBelowGroundResidueNitrogen == 0)
                {
                    // User has not provided these values, we need to assign the necessary prerequisite values before calculating N inputs
                    if (currentYearViewItem.NitrogenContentInProduct == 0)
                        _cropInitializationService.InitializeNitrogenContentInProduct(currentYearViewItem, farm);

                    if (currentYearViewItem.NitrogenContentInStraw == 0)
                        _cropInitializationService.InitializeNitrogenContentInStraw(currentYearViewItem, farm);

                    if (currentYearViewItem.NitrogenContentInRoots == 0)
                        _cropInitializationService.InitializeNitrogenContentInRoots(currentYearViewItem, farm);

                    if (currentYearViewItem.NitrogenContentInExtraroot == 0)
                        _cropInitializationService.InitializeNitrogenContentInExtraroots(currentYearViewItem, farm);

                    if (currentYearViewItem.NitrogenContent == 0)
                        _cropInitializationService.InitializeIPCCNitrogenContent(currentYearViewItem, farm);

                    _cropInitializationService.InitializeNitrogenFixation(currentYearViewItem);

                    var field = farm.GetFieldSystemComponent(currentYearViewItem.FieldSystemComponentGuid);
                    _cropInitializationService.InitializeGrazingViewItems(farm, currentYearViewItem, field);

                    AssignNitrogenInputs(currentYearViewItem, farm, previousYearViewItem);
                }
            }
        }

        #endregion
    }
}