using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Services.Initialization.Animals
{
    public partial class AnimalInitializationService
    {
        #region Public Methods

        /// <summary>
        ///     Initialize each <see cref="ManagementPeriod" />'s volatile solid excretion manure detail within a
        ///     <see cref="Farm" />
        /// </summary>
        /// <param name="farm">The <see cref="Farm" /> containing <see cref="ManagementPeriod" />'s to be reinitialized</param>
        public void InitializeVolatileSolidsExcretion(Farm farm)
        {
            if (farm != null && farm.DefaultSoilData != null)
            {
                var province = farm.DefaultSoilData.Province;

                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                    InitializeVolatileSolidsExcretion(managementPeriod, province);
            }
        }

        /// <summary>
        ///     Initialize the <see cref="ManagementPeriod" /> volatile solid excretion manure detail
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod" /> to be reinitialized</param>
        /// <param name="province">The <see cref="Province" /> used to get correct values from lookup table</param>
        public void InitializeVolatileSolidsExcretion(ManagementPeriod managementPeriod, Province province)
        {
            if (managementPeriod != null && managementPeriod.ManureDetails != null)
                if (managementPeriod.AnimalType.IsSwineType())
                    managementPeriod.ManureDetails.VolatileSolidExcretion =
                        _volatileExcretionForDietsProvider.Get(province, managementPeriod.AnimalType);
        }

        public void InitializeVolatileSolids(Farm farm)
        {
            if (farm != null)
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                    InitializeVolatileSolids(managementPeriod);
        }

        public void InitializeVolatileSolids(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.ManureDetails != null)
                managementPeriod.ManureDetails.VolatileSolids =
                    _livestockDailyVolatileExcretionFactorsProvider.GetVolatileSolids(managementPeriod.AnimalType);
        }

        #endregion
    }
}