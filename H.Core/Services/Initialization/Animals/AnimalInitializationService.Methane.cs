using System.Linq;
using H.Core.Enumerations;
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
        public void InitializeAnnualManureMethaneEmissionRate(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeAnnualManureMethaneEmissionRate(managementPeriod);
                }
            }
        }

        public void InitializeAnnualManureMethaneEmissionRate(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.ManureDetails != null)
            {
                if (managementPeriod.AnimalType.IsPoultryType() || managementPeriod.AnimalType.IsOtherAnimalType())
                {
                    managementPeriod.ManureDetails.DailyManureMethaneEmissionRate = _poultryAndOtherLivestockDefaultCh4EmissionFactorsProvider.GetDailyManureMethaneEmissionRate(managementPeriod.AnimalType);
                }
            }
        }

        /// <summary>
        /// Initialize the default annual enteric methane rate for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>Whi
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/>s to initialize</param>
        public void InitializeAnnualEntericMethaneEmissionRate(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeAnnualEntericMethaneEmissionRate(managementPeriod);
                }
            }
        }

        /// <summary>
        /// Initialize the default annual enteric methane rate for the <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> to initialize with a default <see cref="ManureDetails.YearlyEntericMethaneRate"/></param>
        public void InitializeAnnualEntericMethaneEmissionRate(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.ManureDetails != null)
            {
                if (managementPeriod.AnimalType.IsSwineType() || 
                    managementPeriod.AnimalType.IsPoultryType() || 
                    managementPeriod.AnimalType.IsOtherAnimalType())
                {
                    managementPeriod.ManureDetails.YearlyEntericMethaneRate = _entericMethaneProvider.GetAnnualEntericMethaneEmissionRate(managementPeriod);
                }
            }
        }

        /// <summary>
        /// Initialize the default <see cref="ManureDetails.MethaneProducingCapacityOfManure"/> for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/>s to initialize</param>
        public void InitializeMethaneProducingCapacityOfManure(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeMethaneProducingCapacityOfManure(managementPeriod);
                }
            }
        }

        /// <summary>
        /// Initialize the default <see cref="ManureDetails.MethaneProducingCapacityOfManure"/> for the <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> to initialize with a default <see cref="ManureDetails.MethaneProducingCapacityOfManure"/></param>
        public void InitializeMethaneProducingCapacityOfManure(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.ManureDetails != null && managementPeriod.HousingDetails != null)
            {
                if (managementPeriod.HousingDetails.HousingType.IsPasture())
                {
                    // When housed on pasture, this value should be set to a constant. See table 38 "Default values for maximum methane producing capacity (Bo)" footnote 3.
                    managementPeriod.ManureDetails.MethaneProducingCapacityOfManure = 0.19;
                }
                else
                {
                    var capacity = _defaultMethaneProducingCapacityProvider.GetMethaneProducingCapacityOfManure(managementPeriod.AnimalType);

                    managementPeriod.ManureDetails.MethaneProducingCapacityOfManure = capacity;
                }
            }
        }

        #endregion
    }
}