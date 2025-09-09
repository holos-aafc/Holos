using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Properties;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        public void InitializeGrazingViewItem(
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
            grazingViewItem.Description = InitializeDescription(managementPeriod, animalGroup);

            // set utilization based on crop type
            var utilization = _utilizationRatesForLivestockGrazingProvider.GetUtilizationRate(cropViewItem.CropType);
            grazingViewItem.Utilization = utilization;
        }

        public string InitializeDescription(ManagementPeriod managementPeriod, AnimalGroup animalGroup)
        {
            // Create a string that will be used on the field view to list details of this view item
            return string.Format(Resources.LabelGrazingAnimalsDescription,
                managementPeriod.NumberOfAnimals,
                animalGroup.AnimalTypeString.ToLowerInvariant(),
                managementPeriod.Start.ToString("MM/dd/yyyy"),
                managementPeriod.End.ToString("MM/dd/yyyy"),
                managementPeriod.NumberOfDays,
                managementPeriod.SelectedDiet.Name.ToLowerInvariant());
        }

        public void InitializeGrazingViewItems(Farm farm, CropViewItem viewItem,
            FieldSystemComponent fieldSystemComponent)
        {
            var managementPeriodsThatAlreadyExistAsGrazingItems = new List<GrazingViewItem>();
            var managementPeriodsThatNeedToBeAddedAsGrazingItems = new List<GrazingViewItem>();

            foreach (var animalComponent in farm.AnimalComponents)
            foreach (var animalGroup in animalComponent.Groups)
            foreach (var managementPeriod in animalGroup.ManagementPeriods)
            {
                GrazingViewItem thisManagementPeriodExistsAsAGrazingItem = null;
                GrazingViewItem thisManagementPeriodNeedsToBeAddedAsANewItem = null;

                // Find all the management periods where animals are on pasture/grazing
                if (managementPeriod.HousingDetails.PastureLocation != null &&
                    managementPeriod.HousingDetails.HousingType == HousingType.Pasture)
                {
                    var fieldComponent = managementPeriod.HousingDetails.PastureLocation;

                    // This event could have come from any animal component associated with any field, ensure the animals are grazing on this field
                    if (fieldComponent.Guid.Equals(fieldSystemComponent.Guid))
                    {
                        // Create a grazing view item that specifies when the animals started grazing and when they completed the grazing
                        var grazingViewItem = new GrazingViewItem();

                        InitializeGrazingViewItem(grazingViewItem, managementPeriod, animalComponent, animalGroup,
                            viewItem);

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
                            thisManagementPeriodExistsAsAGrazingItem.Description =
                                InitializeDescription(managementPeriod, animalGroup);
                            managementPeriodsThatAlreadyExistAsGrazingItems.Add(
                                thisManagementPeriodExistsAsAGrazingItem);
                        }
                        else
                        {
                            grazingViewItem.Description = InitializeDescription(managementPeriod, animalGroup);
                            managementPeriodsThatNeedToBeAddedAsGrazingItems.Add(grazingViewItem);
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