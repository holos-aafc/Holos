using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        public void InitializeGrazingViewItem(
            Farm farm,
            GrazingViewItem grazingViewItem,
            ManagementPeriod managementPeriod,
            AnimalComponentBase animalComponent,
            AnimalGroup animalGroup, CropViewItem cropViewItem)
        {
            grazingViewItem.Start = managementPeriod.Start;
            grazingViewItem.End = managementPeriod.End;
            grazingViewItem.ForageActivity = ForageActivities.Grazed;
            grazingViewItem.AnimalComponentGuid = animalComponent.Guid;
            grazingViewItem.ManagementPeriodGuid = managementPeriod.Guid;
            grazingViewItem.AnimalGroupGuid = animalGroup.Guid;
            grazingViewItem.ManagementPeriodName = managementPeriod.Name;

            // See table 9 for MC of grazed fields
            grazingViewItem.MoistureContentAsPercentage = 80;

            // Create a string that will be used on the field view to list details of this view item
            grazingViewItem.Description = this.InitializeDescription(managementPeriod, animalGroup);

            grazingViewItem.Utilization = this.GetUtilizationRate(farm, cropViewItem);
        }

        /// <summary>
        /// Resets the grazing utilization rate on every grazing item on the farm to the Table 60 default for the crop.
        ///
        /// Utilization is a user input - it is editable on the Grazing tab - so nothing resets it automatically, and
        /// <see cref="InitializeGrazingViewItems"/> deliberately preserves it when it rebuilds the items from the animal
        /// components. That protects a deliberate entry, but leaves no way back from a mistaken one, and a low rate does
        /// not fail quietly: grazing carbon is grossed up by dividing by it, so a rate near zero inflates the field's
        /// production enormously. This is the way back, offered through the reset defaults window so the user asks for
        /// it rather than having their entry overwritten.
        /// </summary>
        public void InitializeUtilization(Farm farm)
        {
            foreach (var cropViewItem in farm.GetAllCropViewItems())
            {
                foreach (var grazingViewItem in cropViewItem.GrazingViewItems)
                {
                    grazingViewItem.Utilization = this.GetUtilizationRate(farm, cropViewItem);
                }
            }
        }

        /// <summary>
        /// The utilization rate to apply to a grazing item: the farm's own rate when the user has set one, otherwise
        /// the Table 60 rate for the crop type. Resolved here so item creation and the reset defaults window cannot
        /// disagree about which rate applies.
        /// </summary>
        private double GetUtilizationRate(Farm farm, CropViewItem cropViewItem)
        {
            if (farm?.Defaults != null &&
                farm.Defaults.UseCustomGrazingUtilizationRate &&
                farm.Defaults.CustomGrazingUtilizationRate > 0)
            {
                return farm.Defaults.CustomGrazingUtilizationRate;
            }

            return _utilizationRatesForLivestockGrazingProvider.GetUtilizationRate(cropViewItem.CropType);
        }

        public string InitializeDescription(ManagementPeriod managementPeriod, AnimalGroup animalGroup)
        {
            // Create a string that will be used on the field view to list details of this view item
            return  string.Format(H.Core.Properties.Resources.LabelGrazingAnimalsDescription,
                managementPeriod.NumberOfAnimals,
                animalGroup.AnimalTypeString.ToLowerInvariant(),
                managementPeriod.Start.ToString("MM/dd/yyyy"),
                managementPeriod.End.ToString("MM/dd/yyyy"),
                managementPeriod.NumberOfDays,
                managementPeriod.SelectedDiet?.Name?.ToLowerInvariant());
        }

        public void InitializeGrazingViewItems(Farm farm, CropViewItem viewItem,
            FieldSystemComponent fieldSystemComponent)
        {
            var managementPeriodsThatAlreadyExistAsGrazingItems = new List<GrazingViewItem>();
            var managementPeriodsThatNeedToBeAddedAsGrazingItems = new List<GrazingViewItem>();

            foreach (var animalComponent in farm.AnimalComponents)
            {
                foreach (var animalGroup in animalComponent.Groups)
                {
                    foreach (var managementPeriod in animalGroup.ManagementPeriods)
                    {
                        GrazingViewItem thisManagementPeriodExistsAsAGrazingItem = null;
                        GrazingViewItem thisManagementPeriodNeedsToBeAddedAsANewItem = null;

                        // Find all the management periods where animals are on pasture/grazing
                        if (managementPeriod.HousingDetails.PastureLocation != null && managementPeriod.HousingDetails.HousingType == HousingType.Pasture)
                        {
                            var fieldComponent = managementPeriod.HousingDetails.PastureLocation;

                            // This event could have come from any animal component associated with any field, ensure the animals are grazing on this field
                            if (fieldComponent.Guid.Equals(fieldSystemComponent.Guid))
                            {
                                // Create a grazing view item that specifies when the animals started grazing and when they completed the grazing
                                var grazingViewItem = new GrazingViewItem();

                                this.InitializeGrazingViewItem(farm, grazingViewItem, managementPeriod, animalComponent, animalGroup, viewItem);

                                /*
                                 * Check which items exist in list, and keep the ones that exist according to the management period
                                 */

                                thisManagementPeriodExistsAsAGrazingItem =
                                    viewItem.GrazingViewItems.SingleOrDefault(x =>
                                        x.AnimalComponentGuid == animalComponent.Guid &&
                                        x.ManagementPeriodGuid == managementPeriod.Guid &&
                                        x.AnimalGroupGuid == animalGroup.Guid &&
                                        x.Start.Date.Year == managementPeriod.Start.Year &&
                                        x.Start.Date.Month == managementPeriod.Start.Month &&
                                        x.Start.Date.Day == managementPeriod.Start.Day &&
                                        x.End.Date.Year == managementPeriod.End.Date.Year &&
                                        x.End.Date.Month == managementPeriod.End.Month &&
                                        x.End.Date.Day == managementPeriod.End.Day);

                                if (thisManagementPeriodExistsAsAGrazingItem != null)
                                {
                                    thisManagementPeriodExistsAsAGrazingItem.Description = this.InitializeDescription(managementPeriod, animalGroup);
                                    managementPeriodsThatAlreadyExistAsGrazingItems.Add(thisManagementPeriodExistsAsAGrazingItem);
                                }
                                else
                                {
                                    grazingViewItem.Description = this.InitializeDescription(managementPeriod, animalGroup);
                                    managementPeriodsThatNeedToBeAddedAsGrazingItems.Add(grazingViewItem);
                                }
                            }
                        }
                    }
                }
            }

            viewItem.GrazingViewItems.Clear();

            viewItem.GrazingViewItems.AddRange(managementPeriodsThatAlreadyExistAsGrazingItems);
            viewItem.GrazingViewItems.AddRange(managementPeriodsThatNeedToBeAddedAsGrazingItems);
        }

        #endregion
    }
}