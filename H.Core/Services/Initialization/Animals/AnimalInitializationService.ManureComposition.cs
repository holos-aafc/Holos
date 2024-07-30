using System.Collections.ObjectModel;
using H.Core.Models.Animals;
using H.Core.Models;
using H.Core.Providers.Animals;

namespace H.Core.Services.Initialization.Animals
{
    public partial class AnimalInitializationService
    {
        #region Public Methods

        /// <summary>
        /// Reinitialize the default <see cref="DefaultManureCompositionData"/> for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/>s to initialize</param>
        public void InitializeManureCompositionData(Farm farm)
        {
            if (farm != null)
            {
                var manureCompositionData = _defaultManureCompositionProvider.ManureCompositionData;

                farm.DefaultManureCompositionData.Clear();
                farm.DefaultManureCompositionData.AddRange(manureCompositionData);

                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    var defaults = _defaultManureCompositionProvider.GetManureCompositionDataByType(managementPeriod.AnimalType, managementPeriod.ManureDetails.StateType);

                    this.InitializeManureCompositionData(managementPeriod, defaults);
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
    }
}