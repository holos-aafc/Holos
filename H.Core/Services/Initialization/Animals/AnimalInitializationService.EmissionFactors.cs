using System.Diagnostics;
using H.Core.Enumerations;
using H.Core.Models.Animals;
using H.Core.Models;

namespace H.Core.Services.Initialization.Animals
{
    public partial class AnimalInitializationService
    {
		#region Public Methods

		/// <summary>
		/// Initialize the default emission factors for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
		/// </summary>
		/// <param name="farm">The <see cref="Farm"/> that will be reinitialized to new default values</param>
		public void InitializeDefaultEmissionFactors(Farm farm)
		{
			if (farm != null)
			{
				foreach (var managementPeriod in farm.GetAllManagementPeriods())
				{
					this.InitializeDefaultEmissionFactors(farm, managementPeriod);
				}
			}
		}

		/// <summary>
		/// Initialize the default emission factors for the <see cref="ManagementPeriod"/>.
		/// </summary>
		/// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/></param>
		/// <param name="managementPeriod">The <see cref="ManagementPeriod"/> that will be reinitialized to new default values</param>
		public void InitializeDefaultEmissionFactors(
			Farm farm,
			ManagementPeriod managementPeriod)
		{
			if (farm != null &&
				managementPeriod != null)
            {
                var year = managementPeriod.Start.Date.Year;

                var precipitation = farm.GetAnnualPrecipitation(year);
                var evapotranspiration = farm.GetAnnualEvapotranspiration(year);
                
				var emissionData = _livestockEmissionConversionFactorsProvider.GetFactors(
                    manureStateType: managementPeriod.ManureDetails.StateType,
					meanAnnualPrecipitation: precipitation,
					meanAnnualTemperature: farm.ClimateData.TemperatureData.GetMeanAnnualTemperature(),
					meanAnnualEvapotranspiration: evapotranspiration,
					beddingRate: managementPeriod.HousingDetails.UserDefinedBeddingRate,
					animalType: managementPeriod.AnimalType,
					farm: farm,
					year: year);

				managementPeriod.ManureDetails.MethaneConversionFactor = emissionData.MethaneConversionFactor;
				managementPeriod.ManureDetails.N2ODirectEmissionFactor = emissionData.N20DirectEmissionFactor;
				managementPeriod.ManureDetails.VolatilizationFraction = emissionData.VolatilizationFraction;
				managementPeriod.ManureDetails.EmissionFactorVolatilization = emissionData.EmissionFactorVolatilization;
				managementPeriod.ManureDetails.EmissionFactorLeaching = emissionData.EmissionFactorLeach;
			}
		}

        /// <summary>
        /// Reinitialize the Beef_Dairy_Cattle_Feeding_Activity_Coefficient object
        /// </summary>
        /// <param name="farm"> Contains the ActivityCoefficientFeedingSituation of each HousingDetails of each ManagementPeriod of each <see cref="Farm"/></param>
        public void InitializeFeedingActivityCoefficient(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeFeedingActivityCoefficient(managementPeriod);
                }
            }
        }

        public void InitializeFeedingActivityCoefficient(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.HousingDetails != null)
            {
                var defaultValue = 0d;

                if (managementPeriod.AnimalType.IsBeefCattleType() || managementPeriod.AnimalType.IsDairyCattleType())
                {
                    managementPeriod.HousingDetails.ActivityCeofficientOfFeedingSituation = _beefDairyCattleFeedingActivityCoefficientProvider.GetByHousing(managementPeriod.HousingDetails.HousingType).FeedingActivityCoefficient;
                }
                else if (managementPeriod.AnimalType.IsSheepType())
                {
                    managementPeriod.HousingDetails.ActivityCeofficientOfFeedingSituation = _feedingActivityCoefficientSheepProvider.GetByHousing(managementPeriod.HousingDetails.HousingType).FeedingActivityCoefficient;
                }
                else
                {
                    Trace.TraceError($"{nameof(InitializeFeedingActivityCoefficient)}" + $" unable to get data for housing type: {managementPeriod.HousingDetails.HousingType}." + $" Returning default value of '{defaultValue}'.");

                    managementPeriod.HousingDetails.ActivityCeofficientOfFeedingSituation = defaultValue;
                }
            }
        }

        public void InitializeBaselineCoefficient(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeBaselineCoefficient(managementPeriod);
                }
            }
        }

        public void InitializeBaselineCoefficient(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.HousingDetails != null)
            {
                if (managementPeriod.AnimalType.IsBeefCattleType() || managementPeriod.AnimalType.IsDairyCattleType())
                {
                    managementPeriod.HousingDetails.BaselineMaintenanceCoefficient = _livestockCoefficientsBeefAndDairyCattleProvider.GetCoefficientsByAnimalType(managementPeriod.AnimalType).BaselineMaintenanceCoefficient;
                }
                else if (managementPeriod.AnimalType.IsSheepType())
                {
                    managementPeriod.HousingDetails.BaselineMaintenanceCoefficient = _sheepProvider.GetCoefficientsByAnimalType(managementPeriod.AnimalType).BaselineMaintenanceCoefficient;
                }
            }
        }

        public void InitializeGainCoefficient(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeGainCoefficient(managementPeriod);
                }
            }
        }

        public void InitializeGainCoefficient(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null )
            {
                if (managementPeriod.AnimalType.IsBeefCattleType() || managementPeriod.AnimalType.IsDairyCattleType())
                {
                    managementPeriod.GainCoefficient = _livestockCoefficientsBeefAndDairyCattleProvider.GetCoefficientsByAnimalType(managementPeriod.AnimalType).GainCoefficient;
                }
                else if (managementPeriod.AnimalType.IsSheepType())
                {
                    managementPeriod.GainCoefficient = _sheepProvider.GetCoefficientsByAnimalType(managementPeriod.AnimalType).GainCoefficient;
                }
            }
        }

        #endregion
    }
}