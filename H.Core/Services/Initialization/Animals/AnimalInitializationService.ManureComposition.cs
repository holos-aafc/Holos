using System.Collections.ObjectModel;
using H.Core.Models.Animals;
using H.Core.Models;
using H.Core.Providers.Animals;
using System.Collections.Generic;

namespace H.Core.Services.Initialization.Animals
{
    public partial class AnimalInitializationService
    {
        #region Public Methods

        /// <summary>
        /// Initialize the default <see cref="DefaultManureCompositionData"/> for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/>s to initialize</param>
        public void InitializeManureCompositionData(Farm farm)
        {
            if (farm != null)
            {
                var manureCompositionData = _defaultManureCompositionProvider.ManureCompositionData;
                farm.DefaultManureCompositionData.Clear();
                farm.DefaultManureCompositionData = new ObservableCollection<DefaultManureCompositionData>(SeedDefaultManureCompositionData(manureCompositionData));

                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    var defaults = farm.GetManureCompositionData(managementPeriod.ManureDetails.StateType, managementPeriod.AnimalType);

                    this.InitializeManureCompositionData(managementPeriod, defaults);
                }
            }
        }

        public void ReinitializeManureCompositionData(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    var manureCompositionData = farm.GetManureCompositionData(managementPeriod.ManureDetails.StateType, managementPeriod.AnimalType);

                    this.InitializeManureCompositionData(managementPeriod, manureCompositionData);
                }
            }
        }

        /// <summary>
        /// Initialize the manure <see cref="DefaultManureCompositionData"/> for the selected <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> that will have it's values reset to system defaults</param>
        /// <param name="manureCompositionData">The <see cref="DefaultManureCompositionData"/> containing the new default values to use</param>
        public void InitializeManureCompositionData(ManagementPeriod managementPeriod, DefaultManureCompositionData manureCompositionData)
        {
            if (managementPeriod != null &&
                managementPeriod.ManureDetails != null &&
                manureCompositionData != null)
            {
                managementPeriod.ManureDetails.FractionOfPhosphorusInManure = manureCompositionData.PhosphorusFraction;
                managementPeriod.ManureDetails.FractionOfCarbonInManure = manureCompositionData.CarbonFraction;
                managementPeriod.ManureDetails.FractionOfNitrogenInManure = manureCompositionData.NitrogenFraction;
            }
        }

        #endregion

        #region Private Methods

        private List<DefaultManureCompositionData> SeedDefaultManureCompositionData(List<DefaultManureCompositionData> source)
        {
            var manureCompositionData = new List<DefaultManureCompositionData>();

            foreach (var item in source)
            {
                var copiedInstance = _defaultManureCompositionDataMapper.Map<DefaultManureCompositionData, DefaultManureCompositionData>(item);
                manureCompositionData.Add(copiedInstance);
            }
            return manureCompositionData;
        }

        #endregion
    }
}