using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Infrastructure;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService
    {
        protected void AssignPerennialViewItemsDescription(IEnumerable<CropViewItem> viewItems)
        {
            foreach (var cropViewItem in viewItems)
            {
                if (cropViewItem.CropType.IsPerennial())
                {
                    cropViewItem.Description = string.Format(H.Core.Properties.Resources.LabelYearXInYYearPerennialStand, cropViewItem.YearInPerennialStand, cropViewItem.PerennialStandLength);
                }
            }
        }

        protected void AssignPerennialStandPositionalYears(
            IEnumerable<CropViewItem> viewItems,
            FieldSystemComponent fieldSystemComponent)
        {
            var groups = viewItems.Where(x => x.CropType.IsPerennial()).GroupBy(x => x.PerennialStandGroupId);
            foreach (var group in groups)
            {
                var totalLength = group.Count();

                for (int i = 0; i < group.Count(); i++)
                {
                    var viewItem = group.OrderBy(x => x.Year).ElementAt(i);
                    viewItem.YearInPerennialStand = i + 1;
                    viewItem.PerennialStandLength = totalLength;
                }
            }
        }

        /// <summary>
        /// It is assumed that 30% of the root biomass is turned over (input) annually before harvest, and 100% on harvest for perennials
        /// </summary>
        protected void AssignPerennialRootsReturned(IEnumerable<CropViewItem> list)
        {
            var perennialStands = list.Where(x => x.CropType.IsPerennial()).GroupBy(x => x.PerennialStandGroupId);
            foreach (var currentPerennialStand in perennialStands)
            {
                for (var index = 0; index < currentPerennialStand.OrderBy(x => x.Year).Count(); index++)
                {
                    var cropViewItem = currentPerennialStand.ElementAt(index);

                    if (cropViewItem.YearInPerennialStand == cropViewItem.PerennialStandLength)
                    {
                        // Last year of stand is when 100% of roots are returned
                        cropViewItem.PercentageOfRootsReturnedToSoil = 100;
                    }
                    else
                    {
                        // In years leading up to the last, only 30% of roots are returned
                        cropViewItem.PercentageOfRootsReturnedToSoil = 30;
                    }
                }
            }
        }

        public IEnumerable<IGrouping<Guid, CropViewItem>> AssignPerennialStandIds(
            IEnumerable<CropViewItem> viewItems, FieldSystemComponent fieldSystemComponent)
        {
            var years = viewItems.Select(x => x.Year).ToList();
            var distinctYears = years.Distinct().OrderBy(x => x);

            var currentGuid = Guid.NewGuid();
            foreach (var year in distinctYears)
            {
                var previousYear = year - 1;
                var previousYearPerennial = viewItems.SingleOrDefault(x => x.Year == previousYear && x.CropType.IsPerennial() && x.IsSecondaryCrop == false);
                var currentYearPerennial = viewItems.SingleOrDefault(x => x.Year == year && x.CropType.IsPerennial() && x.IsSecondaryCrop == false);
                var mainCropForNextYear = this.GetMainCropForYear(
                    viewItems: viewItems,
                    year: year + 1, fieldSystemComponent: fieldSystemComponent);

                if (currentYearPerennial != null)
                {
                    if (previousYearPerennial == null)
                    {
                        currentYearPerennial.PerennialStandGroupId = currentGuid;
                    }
                    else if (currentYearPerennial.CropType == previousYearPerennial.CropType)
                    {
                        currentYearPerennial.PerennialStandGroupId = currentGuid;
                    }
                    else
                    {
                        currentGuid = Guid.NewGuid();
                        currentYearPerennial.PerennialStandGroupId = currentGuid;
                    }

                    // Check if the perennial grown in this year concludes the stand (is harvest year)
                    if (mainCropForNextYear != null && mainCropForNextYear.CropType.IsPerennial() == false)
                    {
                        // This is the last item in the stand
                        currentGuid = Guid.NewGuid();
                    }
                }
                else
                {
                    currentGuid = Guid.NewGuid();
                }
            }

            var groupsOfStands = viewItems.Where(x => x.PerennialStandGroupId != Guid.Empty).GroupBy(x => x.PerennialStandGroupId);

            return groupsOfStands;
        }

        public void AssignTillageToFinalYearOfPerennialStands(IEnumerable<CropViewItem> viewItems)
        {
            foreach (var cropViewItem in viewItems)
            {
                if (cropViewItem.CropType.IsPerennial())
                {
                    if (cropViewItem.YearInPerennialStand == cropViewItem.PerennialStandLength)
                    {
                        cropViewItem.TillageType = TillageType.Reduced;
                    }
                    else
                    {
                        cropViewItem.TillageType = TillageType.NoTill;
                    }
                }
            }
        }

        protected void ProcessPerennials(IEnumerable<CropViewItem> viewItems, FieldSystemComponent fieldSystemComponent)
        {
            this.AssignPerennialStandIds(viewItems, fieldSystemComponent);
            this.AssignPerennialStandPositionalYears(viewItems, fieldSystemComponent);
            this.AssignPerennialViewItemsDescription(viewItems);
            this.AssignPerennialRootsReturned(viewItems);
            this.AssignTillageToFinalYearOfPerennialStands(viewItems);
        }
    }
}