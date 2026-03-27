using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Models
{
    public partial class Farm
    {
        #region Public Methods

        public List<int> GetYearsWithGrazingAnimals()
        {
            var list = new List<int>();

            foreach (var fieldSystemComponent in this.FieldSystemComponents)
            {
                foreach (var cropViewItem in fieldSystemComponent.CropViewItems)
                {
                    foreach (var grazingViewItem in cropViewItem.GrazingViewItems)
                    {
                        if (list.Contains(grazingViewItem.Start.Year) == false)
                        {
                            list.Add(grazingViewItem.Start.Year);
                        }
                    }
                }
            }

            return list;
        }

        public bool CropHasGrazingAnimals(CropViewItem viewItem)
        {
            var field = this.GetFieldSystemComponent(viewItem.FieldSystemComponentGuid);
            if (field == null)
            {
                return false;
            }

            var crops = field.CropViewItems.Where(x => x.CropType == viewItem.CropType);
            foreach (var cropViewItem in crops)
            {
                if (cropViewItem.GrazingViewItems.Any(x => x.Start.Year == viewItem.Year))
                {
                    return true;
                }  
            }

            return false;
        }

        public bool HasGrazingAnimalsInYear(int year)
        {
            return this.GetYearsWithGrazingAnimals().Contains(year);
        }

        public bool IsNonSwathingGrazingScenario(CropViewItem viewItem)
        {
            return viewItem.TotalCarbonLossesByGrazingAnimals > 0 &&
                   this.CropHasGrazingAnimals(viewItem) &&
                   this.YieldAssignmentMethod != YieldAssignmentMethod.Custom &&
                   viewItem.HarvestMethod != HarvestMethods.StubbleGrazing &&
                   viewItem.HarvestMethod != HarvestMethods.Swathing;
        }

        #endregion
    }
}
