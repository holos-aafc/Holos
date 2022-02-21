using System;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Converters
{
    public  class EmissionTypeConverter
    {
        #region Public Methods

        public double Convert(EmissionDisplayUnits sourceType, EmissionDisplayUnits targetType, double emissionValue)
        {
            if (sourceType == EmissionDisplayUnits.KilogramsCH4)
            {
                if (targetType == EmissionDisplayUnits.KilogramsC02e)
                {
                    return emissionValue * CoreConstants.CH4ToCO2eConversionFactor;
                }

                // No conversion needed
                if (targetType == EmissionDisplayUnits.KilogramsGhgs || 
                    targetType == EmissionDisplayUnits.KilogramsCH4)
                {
                    return emissionValue;
                }

                if (targetType == EmissionDisplayUnits.MegagramsCO2e)
                {
                    return (emissionValue * CoreConstants.CH4ToCO2eConversionFactor) / 1000;
                }
            }

            if (sourceType == EmissionDisplayUnits.KilogramsN2O)
            {
                if (targetType == EmissionDisplayUnits.KilogramsC02e)
                {
                    return emissionValue * CoreConstants.N2OToCO2eConversionFactor;
                }

                // No conversion needed
                if (targetType == EmissionDisplayUnits.KilogramsGhgs ||
                    targetType == EmissionDisplayUnits.KilogramsN2O)
                {
                    return emissionValue;
                }

                if (targetType == EmissionDisplayUnits.MegagramsCO2e)
                {
                    return (emissionValue * CoreConstants.N2OToCO2eConversionFactor) / 1000;
                }
            }

            if (sourceType == EmissionDisplayUnits.KilogramsC02)
            {
                if (targetType == EmissionDisplayUnits.KilogramsC02e)
                {
                    return emissionValue * CoreConstants.CO2ToCO2eConversionFactor;
                }

                // No conversion needed
                if (targetType == EmissionDisplayUnits.KilogramsGhgs ||
                    targetType == EmissionDisplayUnits.KilogramsC02)
                {
                    return emissionValue;
                }

                if (targetType == EmissionDisplayUnits.MegagramsCO2e)
                {
                    return (emissionValue * CoreConstants.CO2ToCO2eConversionFactor) / 1000;
                }
            }

            if (sourceType == EmissionDisplayUnits.KilogramsC02e)
            {
                if (targetType == EmissionDisplayUnits.MegagramsCO2e)
                {
                    return (emissionValue / 1000);
                }

                // No conversion needed
                if (targetType == EmissionDisplayUnits.KilogramsC02e)
                {
                    return emissionValue;
                }
            }

            throw new Exception($"Unknown conversion for source type: '{sourceType.GetDescription()}' to target type: '{targetType.GetDescription()}'");
        }

        #endregion
    }
}