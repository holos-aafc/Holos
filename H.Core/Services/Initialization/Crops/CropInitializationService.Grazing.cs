using H.Core.Enumerations;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        public void InitializeGrazingViewItem(GrazingViewItem grazingViewItem,
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
            grazingViewItem.Description = string.Format(H.Core.Properties.Resources.LabelGrazingAnimalsDescription,
                managementPeriod.NumberOfAnimals,
                animalGroup.AnimalTypeString.ToLowerInvariant(),
                managementPeriod.Start.ToString("m"),
                managementPeriod.End.ToString("m"),
                managementPeriod.NumberOfDays,
                managementPeriod.SelectedDiet.Name.ToLowerInvariant());

            // set utilization based on crop type
            var utilization = _utilizationRatesForLivestockGrazingProvider.GetUtilizationRate(cropViewItem.CropType);
            grazingViewItem.Utilization = utilization;
        }

        #endregion
    }
}