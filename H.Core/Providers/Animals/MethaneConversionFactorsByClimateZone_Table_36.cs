using System;
using System.Diagnostics;
using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 36.
    ///
    /// Methane conversion factors (MCF) by climate zone, by livestock group and manure handling system
    /// </summary>
    public class MethaneConversionFactorsByClimateZone_Table_36
    {
        public double GetByClimateAndHandlingSystem(
            ManureStateType manureStateType,
            double meanAnnualPrecipitation,
            double meanAnnualPotentialEvapotranspiration,
            double meanAnnualTemperature)
        {
            var climateZone = this.GetClimateZone(
                temperature: meanAnnualTemperature,
                precipitation: meanAnnualPrecipitation,
                potentialEvapotranspiration: meanAnnualPotentialEvapotranspiration);

            if (manureStateType == ManureStateType.SolidStorage || manureStateType == ManureStateType.Solid)
            {
                switch (climateZone)
                {
                    case ClimateZones.CoolTemperateMoist:
                    case ClimateZones.CoolTemperateDry:
                    case ClimateZones.BorealDry:
                    case ClimateZones.BorealMoist:
                        return 0.02;

                    case ClimateZones.WarmTemperateDry:
                    case ClimateZones.WarmTemperateMoist:
                        return 0.04;
                }
            }

            if (manureStateType == ManureStateType.CompostIntensive)
            {
                switch (climateZone)
                {
                    case ClimateZones.CoolTemperateMoist:
                    case ClimateZones.CoolTemperateDry:
                    case ClimateZones.BorealDry:
                    case ClimateZones.BorealMoist:
                        return 0.005;

                    case ClimateZones.WarmTemperateDry:
                    case ClimateZones.WarmTemperateMoist:
                        return 0.01;
                }
            }

            if (manureStateType == ManureStateType.CompostPassive)
            {
                switch (climateZone)
                {
                    case ClimateZones.CoolTemperateMoist:
                    case ClimateZones.CoolTemperateDry:
                    case ClimateZones.BorealDry:
                    case ClimateZones.BorealMoist:
                        return 0.01;

                    case ClimateZones.WarmTemperateDry:
                    case ClimateZones.WarmTemperateMoist:
                        return 0.02;
                }
            }

            if (manureStateType == ManureStateType.DeepBedding)
            {
                switch (climateZone)
                {
                    case ClimateZones.CoolTemperateMoist:
                        return 0.21;
                    case ClimateZones.CoolTemperateDry:
                        return 0.26;
                    case ClimateZones.BorealDry:
                    case ClimateZones.BorealMoist:
                        return 0.014;

                    case ClimateZones.WarmTemperateDry:
                        return 0.41;
                    case ClimateZones.WarmTemperateMoist:
                        return 0.37;
                }
            }

            if (manureStateType == ManureStateType.DailySpread)
            {
                switch (climateZone)
                {
                    case ClimateZones.CoolTemperateMoist:
                    case ClimateZones.CoolTemperateDry:
                    case ClimateZones.BorealDry:
                    case ClimateZones.BorealMoist:
                        return 0.02;

                    case ClimateZones.WarmTemperateDry:
                    case ClimateZones.WarmTemperateMoist:
                        return 0.04;
                }
            }

            // Pasture, etc. have non-temperature dependent values
            return 0;
        }

        private ClimateZones GetClimateZone(
            double temperature,
            double precipitation,
            double potentialEvapotranspiration)
        {
            if (temperature > 10 && ((precipitation / potentialEvapotranspiration) > 1))
            {
                return ClimateZones.WarmTemperateMoist;
            }

            if (temperature > 10 && ((precipitation / potentialEvapotranspiration) < 1))
            {
                return ClimateZones.WarmTemperateDry;
            }

            if ((temperature > 0 && temperature < 10) && ((precipitation / potentialEvapotranspiration) > 1))
            {
                return ClimateZones.CoolTemperateMoist;
            }

            if ((temperature > 0 && temperature < 10) && ((precipitation / potentialEvapotranspiration) < 1))
            {
                return ClimateZones.CoolTemperateDry;
            }

            if (temperature <= 0 && ((precipitation / potentialEvapotranspiration) > 1))
            {
                return ClimateZones.WarmTemperateMoist;
            }

            if (temperature <= 0 && ((precipitation / potentialEvapotranspiration) < 1))
            {
                return ClimateZones.WarmTemperateDry;
            }

            Trace.TraceError($"{nameof(MethaneConversionFactorsByClimateZone_Table_36.GetClimateZone)}" +
                             $" unable to get data for methane conversion factor since climate zone is unknown" +
                             $" Returning default value of 0.");
            return 0;
        }

    }
}