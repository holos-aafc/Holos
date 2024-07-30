using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Services.Initialization.Animals
{
    public partial class AnimalInitializationService
    {
        #region Public Methods

        /// <summary>
        /// Reinitialize the DailyMethaneEmissionRate for each ManagementPeriod of each farm
        /// </summary>
        /// <param name="farm"> Contains the <see cref="ManureDetails.DailyManureMethaneEmissionRate"/> that needs to be reinitialized to default</param>
        public void InitializeOtherLivestockDefaultCH4EmissionFactor(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    managementPeriod.ManureDetails.DailyManureMethaneEmissionRate = _otherLivestockDefaultCh4EmissionFactorsProvider.GetDailyManureMethaneEmissionRate(managementPeriod.AnimalType);
                }
            }
        }

        /// <summary>
        /// Initialize the default annual enteric methane rate for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>Whi
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/>s to initialize</param>
        public void InitializeAnnualEntericMethaneRate(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeAnnualEntericMethaneRate(managementPeriod);
                }
            }
        }

        /// <summary>
        /// Initialize the default annual enteric methane rate for the <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> to initialize with a default <see cref="ManureDetails.YearlyEntericMethaneRate"/></param>
        public void InitializeAnnualEntericMethaneRate(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.ManureDetails != null)
            {
                managementPeriod.ManureDetails.YearlyEntericMethaneRate = _entericMethaneProvider.GetAnnualEntericMethaneEmissionRate(managementPeriod);
            }
        }

        /// <summary>
        /// Initialize the default <see cref="ManureDetails.MethaneProducingCapacityOfManure"/> for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/>s to initialize</param>
        public void InitializeMethaneProducingCapacity(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeMethaneProducingCapacity(managementPeriod);
                }
            }
        }

        /// <summary>
        /// Initialize the default <see cref="ManureDetails.MethaneProducingCapacityOfManure"/> for the <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> to initialize with a default <see cref="ManureDetails.MethaneProducingCapacityOfManure"/></param>
        public void InitializeMethaneProducingCapacity(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.ManureDetails != null)
            {
                var capacity = _defaultMethaneProducingCapacityProvider.GetMethaneProducingCapacityOfManure(managementPeriod.AnimalType);

                managementPeriod.ManureDetails.MethaneProducingCapacityOfManure = capacity;
            }
        }

        #endregion
    }
}