using System;
using H.Core.Enumerations;
using H.Core.Models.Animals;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Fields
{
    /// <summary>
    /// Note: can't hold references to a management period because of self referencing exceptions in JSON serialization
    /// </summary>
    public class GrazingViewItem : FieldActivityBase
    {
        #region Fields

        private Guid _managementPeriodGuid;
        private Guid _animalComponentGuid;
        private Guid _animalGroupGuid;

        private string _managementPeriodName;

        #endregion

        #region Constructors
        
        public GrazingViewItem()
        {
            base.ForageActivity = ForageActivities.Grazed;
            base.ForageGrowthStage = ForageGrowthStages.StageTwo;

            this.Utilization = 60;
        }

        #endregion

        #region Properties

        /// <summary>
        /// When a group of animals have their housing type set to pasture during one of their management periods, assign this value to the GUID
        /// of management period so we can track which animals have grazed on this pasture. 
        /// </summary>
        public Guid ManagementPeriodGuid
        {
            get => _managementPeriodGuid;
            set => SetProperty(ref _managementPeriodGuid, value);
        }

        /// <summary>
        /// Identifies the <see cref="AnimalComponentBase"/> that contains the <see cref="AnimalGroup"/> grazing on the <see cref="CropViewItem"/>.
        /// </summary>
        public Guid AnimalComponentGuid
        {
            get => _animalComponentGuid;
            set => SetProperty(ref _animalComponentGuid, value);
        }

        public string ManagementPeriodName
        {
            get => _managementPeriodName;
            set => SetProperty(ref _managementPeriodName, value);
        }

        /// <summary>
        /// Indicates the ID of the animal group that is grazing on the field
        /// </summary>
        public Guid AnimalGroupGuid
        {
            get => _animalGroupGuid;
            set => SetProperty(ref _animalGroupGuid, value);
        }

        #endregion
    }
}