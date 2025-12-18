using H.Core.Models.Animals;
using H.Core.Models;
using System.ComponentModel;

namespace H.Core.Services.Animals
{
    public partial class ManagementPeriodService
    {
        #region Public Methods

        public void MulesManagementPeriod(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            AddOtherManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
        }

        #endregion
    }
}