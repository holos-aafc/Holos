using System;
using System.Diagnostics;
using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 40.
    ///
    /// Methane conversion factors (MCF) by climate zone, by livestock group and manure handling system
    /// </summary>
    public class Table_40_MCF_By_Climate_Livestock_MansureSystem_Provider
    {
        // MCFs by climate zone (kg kg^-1)

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
                        return 0.14;
                    case ClimateZones.WarmTemperateDry:
                        return 0.37;
                    case ClimateZones.WarmTemperateMoist:
                        return 0.41;
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
                        return 0.001;

                    case ClimateZones.WarmTemperateDry:
                    case ClimateZones.WarmTemperateMoist:
                        return 0.005;
                }
            }

            if (manureStateType == ManureStateType.DeepPit)
            {
                switch (climateZone)
                {
                    case ClimateZones.CoolTemperateMoist:
                        return 0.06;
                    case ClimateZones.CoolTemperateDry:
                        return 0.08;
                    case ClimateZones.BorealDry:
                        return 0.04;
                    case ClimateZones.BorealMoist:
                        return 0.04;

                    case ClimateZones.WarmTemperateDry:
                        return 0.15;
                    case ClimateZones.WarmTemperateMoist:
                        return 0.13;
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
            // Read Footnote 1 for data reference.

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

            Trace.TraceError($"{nameof(Table_40_MCF_By_Climate_Livestock_MansureSystem_Provider.GetClimateZone)}" +
                             $" unable to get data for methane conversion factor since climate zone is unknown" +
                             $" Returning default value of 0.");
            return 0;
        }



        #region Footnotes

        /*
        * Footnote 1 : For the determination of the MCF value, IPCC (2019) defines the different climate zones as follows:
        * Warm temperate moist: mean annual temperature (MAT) >10 °C, P:PE >1; Warm temperate dry: MAT >10 °C, P:PE < 1;
        * Cool temperate moist: MAT > 0 °C, P:PE >1; Cool temperate dry: MAT > 0 °C, P:PE <1;
        * Boreal moist: MAT < 0 °C but some monthly temperatures > 10 °C, P:PE >1; Boreal dry:
        * MAT < 0 °C but some monthly temperatures > 10 °C, P:PE <1. The MAT for cool temperate moist, cool temperate dry,
        * warm temperate moist and warm temperate dry were 4.6, 5.8, 13.9, 14.0, respectively.
        */

        #endregion

    }
}