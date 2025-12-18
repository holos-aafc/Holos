using System.ComponentModel;
using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Services.Animals
{
    public partial class ManagementPeriodService
    {
        #region Public Methods

        public void GoatsManagementPeriod(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod,
            PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            AddOtherManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
        }

        #endregion
    }
}