using System.Diagnostics;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models.Animals;
using H.Core.Models;
using H.Core.Providers.Animals;

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
				var emissionData = _livestockEmissionConversionFactorsProvider.GetFactors(
                    manureStateType: managementPeriod.ManureDetails.StateType,
					meanAnnualPrecipitation: farm.ClimateData.PrecipitationData.GetTotalAnnualPrecipitation(),
					meanAnnualTemperature: farm.ClimateData.TemperatureData.GetMeanAnnualTemperature(),
					meanAnnualEvapotranspiration: farm.ClimateData.EvapotranspirationData.GetTotalAnnualEvapotranspiration(),
					beddingRate: managementPeriod.HousingDetails.UserDefinedBeddingRate,
					animalType: managementPeriod.AnimalType,
					farm: farm,
					year: managementPeriod.Start.Date.Year);

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
        public void InitializeCattleFeedingActivity(Farm farm)
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
                    Trace.TraceError($"{nameof(AnimalInitializationService.InitializeFeedingActivityCoefficient)}" + $" unable to get data for housing type: {managementPeriod.HousingDetails.HousingType}." + $" Returning default value of '{defaultValue}'.");

                    managementPeriod.HousingDetails.ActivityCeofficientOfFeedingSituation = defaultValue;
                }
            }
        }

		#endregion
	}
}