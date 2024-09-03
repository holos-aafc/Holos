using System;
using System.Collections.Generic;
using System.Windows.Documents;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Plants;
using H.Core.Services.Animals;

namespace H.Core.Calculators.Carbon
{
    public class CarbonInputCalculator : ICarbonCalculator
    {
        #region Fields

        private readonly IICBMCarbonInputCalculator _icbmCarbonInputCalculator;
        private readonly IIPCCTier2CarbonInputCalculator _ipccTier2CarbonInputCalculator;

        #endregion

        #region Constructors

        public CarbonInputCalculator(IICBMCarbonInputCalculator icbmCarbonInputCalculator, IIPCCTier2CarbonInputCalculator ipccTier2CarbonInputCalculator)
        {
            if (icbmCarbonInputCalculator != null)
            {
                _icbmCarbonInputCalculator= icbmCarbonInputCalculator;
            }
            else
            {
                throw new ArgumentNullException(nameof(icbmCarbonInputCalculator));
            }

            if (ipccTier2CarbonInputCalculator != null)
            {
                _ipccTier2CarbonInputCalculator = ipccTier2CarbonInputCalculator;
            }
            else
            {
                throw new ArgumentNullException(nameof(ipccTier2CarbonInputCalculator));
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(Farm farm)
        {
            _icbmCarbonInputCalculator.Initialize(farm);
            _ipccTier2CarbonInputCalculator.Initialize(farm);
        }

        public void SetInputs(CropViewItem previousYearViewItem, CropViewItem currentYearViewItem, CropViewItem nextYearViewItem, Farm farm)
        {
            if (farm.Defaults.CarbonModellingStrategy == CarbonModellingStrategies.ICBM)
            {
                _icbmCarbonInputCalculator.SetCarbonInputs(
                    previousYearViewItem: previousYearViewItem,
                    currentYearViewItem: currentYearViewItem,
                    nextYearViewItem: nextYearViewItem,
                    farm: farm);
            }
            else
            {
                if (_ipccTier2CarbonInputCalculator.CanCalculateInputsForCrop(currentYearViewItem))
                {
                    _ipccTier2CarbonInputCalculator.CalculateInputs(currentYearViewItem, farm);
                }
                else
                {
                    _icbmCarbonInputCalculator.SetCarbonInputs(
                        previousYearViewItem: previousYearViewItem,
                        currentYearViewItem: currentYearViewItem,
                        nextYearViewItem: nextYearViewItem,
                        farm: farm);
                }
            }
        }

        /// <summary>
        /// Equation 5.6.1-1
        ///
        /// (kg C ha^-1)
        /// </summary>
        public void CalculateManureCarbonInputByGrazingAnimals(FieldSystemComponent fieldSystemComponent,
            List<CropViewItem> cropViewItems, Farm farm)
        {
            foreach (var cropViewItem in cropViewItems)
            {
                cropViewItem.TotalCarbonInputFromManureFromAnimalsGrazingOnPasture =
                    _icbmCarbonInputCalculator.CalculateManureCarbonInputFromGrazingAnimals(fieldSystemComponent, cropViewItem, farm);
            }
        }

        #endregion
    }
}