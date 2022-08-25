using System;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService
    {
        /// <summary>
        /// Calculates C/CO2 changes based on land use changes for single year fields.
        /// </summary>
        public LandUseChangeResults CalculateLandUseChangeResults(
            FieldSystemComponent fieldSystemComponent, 
            Farm farm)
        {
            var results = new LandUseChangeResults();

            results.CarbonDioxideFromTillageChange = this.CalculateCarbonDioxideFromTillage(fieldSystemComponent, farm);
            results.CarbonDioxideFromFallowChange = this.CalculateCarbonDioxideChangeFromFallow(fieldSystemComponent, farm);
            results.CarbonDioxideFromPastPerennials = this.CalculateCarbonDioxideChangeFromPastPerennials(fieldSystemComponent, farm);
            results.CarbonDioxideFromCurrentPerennials = this.CalculateCarbonDioxideChangeFromCurrentPerennials(fieldSystemComponent, farm);

            return results;
        }

        /// <summary>
        /// 2.1.1 Tillage
        /// 
        /// Calculates soil carbon change when tillage type is changed after a period of time. If a user changes tillage from reduced/intensive tillage to no tillage, then carbon is sequestered (a negative
        /// value will be returned). If user changes tillage from no tillage to reduced/intensive, the carbon is emitted (a positive value will be returned.)
        ///
        /// NOTE: values calculated here will be different than from v3. In v3 the calculation uses the area of the entire farm (all annual area, fallow area, etc.). v4 uses the area of the specific field/crop only.
        /// </summary>
        public SoilCarbonEmissionResult CalculateCarbonDioxideFromTillage(
            FieldSystemComponent fieldSystemComponent, 
            Farm farm)
        {
            var result = new SoilCarbonEmissionResult()
            {
                CarbonChangeSource = LandUseCarbonChangeSource.Tillage,
                Name = fieldSystemComponent.Name,
            };

            var viewItem = fieldSystemComponent.GetSingleYearViewItem();
            if (viewItem == null || viewItem.CropType.IsFallow())
            {
                return result;
            }

            // If in multiyear mode, we calculate carbon change using ICBM instead and so there is no single year emissions.
            if (farm.EnableCarbonModelling == true)
            {
                return result;
            }

            // Rotation component not currently set up to ask for both current and past tillage
            if (fieldSystemComponent.IsPartOfRotationComponent == true)
            {
                return result;
            }

            // There was no change in tillage so there was no emission/sequestration.
            if (viewItem.TillageType == viewItem.PastTillageType)
            {
                return result;
            }

            var currentYear = fieldSystemComponent.YearOfObservation;
            var yearOfTillageChange = viewItem.YearOfTillageChange;
            var yearsSinceTillageChange = _soilEmissionsCalculator.CalculateTimeSinceManagementChangeInYears(currentYear, yearOfTillageChange);
            var tillageChangeType = _landManagementChangeHelper.GetTillagePracticeChangeType(viewItem.TillageType, viewItem.PastTillageType);

            var soilData = farm.GeographicData.DefaultSoilData;
            var ecozone = _ecodistrictDefaultsProvider.GetEcozone(soilData.EcodistrictId);
            var lumCMaxForTillage = _lumCMaxKValuesTillagePracticeChangeProvider.GetLumCMax(ecozone, soilData.SoilTexture, tillageChangeType);
            var kValueForTillage = _lumCMaxKValuesTillagePracticeChangeProvider.GetKValue(ecozone, soilData.SoilTexture, tillageChangeType);
            var carbonChangeRateForTillage = _soilEmissionsCalculator.CalculateCarbonChangeRate(lumCMaxForTillage, kValueForTillage, yearsSinceTillageChange);
            var carbonChangeForTillage = _soilEmissionsCalculator.CalculateCarbonChange(carbonChangeRateForTillage, viewItem.Area);
            var carbonDioxideFromTillage = _soilEmissionsCalculator.CalculateCarbonDioxideChange(carbonChangeForTillage);

            // Equation 2.1.5-1
            result.CarbonChangeForSoil = (-1) * carbonChangeForTillage;

            result.CarbonDioxideChangeForSoil = carbonDioxideFromTillage;

            return result;
        }

        /// <summary>
        /// 2.1.2 Fallow
        ///
        /// Calculates soil carbon change when area of current fallow area is different from past fallow area. Larger increases (compared to past area) will results in a larger emission. It is not possible to sequester carbon
        /// when changing fallow areas, the best that can be done is to have past fallow area equal to current fallow area.
        /// </summary>
        public SoilCarbonEmissionResult CalculateCarbonDioxideChangeFromFallow(
            FieldSystemComponent fieldSystemComponent, 
            Farm farm)
        {
            var result = new SoilCarbonEmissionResult()
            {
                CarbonChangeSource = LandUseCarbonChangeSource.Fallow,
                Name = fieldSystemComponent.Name,
            };

            var viewItem = fieldSystemComponent.GetSingleYearViewItem();
            if (viewItem == null || viewItem.CropType.IsFallow() == false)
            {
                return result;
            }

            var currentYear = fieldSystemComponent.YearOfObservation;
            var yearOfChange = viewItem.YearOfFallowChange;
            var timeSinceManagementChangeInYears = _soilEmissionsCalculator.CalculateTimeSinceManagementChangeInYears(currentYear, yearOfChange);

            var fallowAreaDifference = Math.Abs(viewItem.Area - viewItem.PastFallowArea);
            var carbonChangeRateForFallow = _soilEmissionsCalculator.CalculateCarbonChangeRate(viewItem.LumCMax, viewItem.KValue, timeSinceManagementChangeInYears);
            var carbonChangeForFallow = _soilEmissionsCalculator.CalculateCarbonChange(carbonChangeRateForFallow, fallowAreaDifference);
            var carbonDioxideChangeFromFallow = _soilEmissionsCalculator.CalculateCarbonDioxideChange(carbonChangeForFallow);

            // Equation 2.1.5-1
            result.CarbonChangeForSoil = (-1) * carbonChangeForFallow;

            result.CarbonDioxideChangeForSoil = carbonDioxideChangeFromFallow;

            return result;
        }

        /// <summary>
        /// 2.1.3.1 Current perennial crops
        /// 2.1.4.1 Seeded grasslands
        ///
        /// Calculates soil carbon change when a perennial is grown. This results in a carbon sequestration (negative carbon emission)
        /// </summary>
        public SoilCarbonEmissionResult CalculateCarbonDioxideChangeFromCurrentPerennials(
            FieldSystemComponent fieldSystemComponent, 
            Farm farm)
        {
            var result = new SoilCarbonEmissionResult()
            {
                CarbonChangeSource = LandUseCarbonChangeSource.CurrentPerennials,
                Name = fieldSystemComponent.Name,
            };

            var viewItem = fieldSystemComponent.GetSingleYearViewItem();
            if (fieldSystemComponent.IsCurrentPerennial == false || viewItem == null)
            {
                return result;
            }

            // Native grasslands get carbon sequestration values as if they were seeded grasslands that began in 1910. This is the minimum year a field can have according to algorithm document.

            var currentYear = fieldSystemComponent.YearOfObservation;
            var yearOfChange = viewItem.YearOfConversion;
            var timeSinceManagementChangeInYears = _soilEmissionsCalculator.CalculateTimeSinceManagementChangeInYears(currentYear, yearOfChange);

            var carbonChangeRateForCurrentPerennials = _soilEmissionsCalculator.CalculateCarbonChangeRate(viewItem.LumCMax, viewItem.KValue, timeSinceManagementChangeInYears);
            var carbonChangeForCurrentPerennials = _soilEmissionsCalculator.CalculateCarbonChange(carbonChangeRateForCurrentPerennials, viewItem.Area);
            var carbonDioxideChangeFromCurrentPerennials = _soilEmissionsCalculator.CalculateCarbonDioxideChange(carbonChangeForCurrentPerennials);

            // Equation 2.1.5-1
            result.CarbonChangeForSoil = (-1) * carbonChangeForCurrentPerennials;

            result.CarbonDioxideChangeForSoil = carbonDioxideChangeFromCurrentPerennials;

            return result;
        }

        /// <summary>
        /// 2.1.3.2 Past perennial crops
        /// 2.1.4.1 Broken grasslands
        ///
        /// Calculates soil carbon change when a perennial is removed to grow an annual crop. This results in a carbon emission.
        /// </summary>
        public SoilCarbonEmissionResult CalculateCarbonDioxideChangeFromPastPerennials(
            FieldSystemComponent fieldSystemComponent, 
            Farm farm)
        {
            var result = new SoilCarbonEmissionResult()
            {
                CarbonChangeSource = LandUseCarbonChangeSource.PastPerennials,
                Name = fieldSystemComponent.Name,
            };

            var viewItem = fieldSystemComponent.GetSingleYearViewItem();
            if (fieldSystemComponent.IsPastPerennial == false || viewItem == null)
            {
                return result;
            }

            var currentYear = fieldSystemComponent.YearOfObservation;
            var yearOfChange = viewItem.YearOfConversion;
            var timeSinceManagementChangeInYears = _soilEmissionsCalculator.CalculateTimeSinceManagementChangeInYears(currentYear, yearOfChange);

            var carbonChangeRateForPastPerennials = _soilEmissionsCalculator.CalculateCarbonChangeRate(viewItem.LumCMax, viewItem.KValue, timeSinceManagementChangeInYears);
            var carbonChangeForPastPerennials = _soilEmissionsCalculator.CalculateCarbonChange(carbonChangeRateForPastPerennials, viewItem.Area);
            var carbonDioxideChangeFromPastPerennials = _soilEmissionsCalculator.CalculateCarbonDioxideChange(carbonChangeForPastPerennials);

            // Equation 2.1.5-1
            result.CarbonChangeForSoil = (-1) * carbonChangeForPastPerennials;

            result.CarbonDioxideChangeForSoil = carbonDioxideChangeFromPastPerennials;

            return result;
        }
    }
}