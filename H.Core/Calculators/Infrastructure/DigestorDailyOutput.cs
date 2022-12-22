using System;
using H.Core.Emissions.Results;

namespace H.Core.Calculators.Infrastructure
{
    public class DigestorDailyOutput
    {
        public DateTime Date { get; set; }

        public GroupEmissionsByDay GroupEmissionsByDay { get; set; }

        /*
         * Flows
         */

        /// <summary>
        /// (kg VS day^-1)
        /// </summary>
        public double TotalFlowOfDegradedVolatileSolids { get; set; }

        public double FlowRateOfAllSubstratesInDigestate { get; set; }

        public double FlowRateOfAllTotalSolidsInDigestate { get; set; }

        public double FlowRateOfAllVolatileSolidsInDigestate { get; set; }

        public double FlowRateOfTotalNitrogenInDigestate { get; set; }

        public double FlowOfAllTanInDigestate { get; set; }

        public double FlowRateOfAllOrganicNitrogenInDigestate { get; set; }

        public double FlowOfAllCarbon { get; set; }

        /*
         * Liquid/solid separation
         */

        public double FlowRateLiquidFraction { get; set; }

        public double FlowRateSolidFraction { get; set; }

        public double FlowOfTotalSolidsInLiquidFraction { get; set; }

        public double FlowOfTotalSolidsInSolidFraction { get; set; }

        public double TotalVolatileSolidsLiquidFraction { get; set; }

        public double TotalVolatileSolidsSolidFraction { get; set; }

        public double TotalTanLiquidFraction { get; set; }

        public double TotalTanSolidFraction { get; set; }

        public double OrganicNLiquidFraction { get; set; }

        public double OrganicNSolidFraction { get; set; }

        public double TotalNitrogenLiquidFraction { get; set; }

        public double TotalNitrogenSolidFraction { get; set; }

        public double CarbonLiquidFraction { get; set; }

        public double CarbonSolidFraction { get; set; }

        /*
         * Biogas production
         */

        /// <summary>
        /// (Nm^3 day^-1)
        /// </summary>
        public double TotalMethaneProduction { get; set; }

        /// <summary>
        /// (Nm^3 day^-1)
        /// </summary>
        public double TotalBiogasProduction { get; set; }

        /// <summary>
        /// (Nm^3 day^-1)
        /// </summary>
        public double TotalCarbonDioxideProduction { get; set; }

        /// <summary>
        /// (Nm^3 day^-1)
        /// </summary>
        public double TotalRecoverableMethane { get; set; }

        /// <summary>
        /// (kWh day^-1)
        /// </summary>
        public double TotalPrimaryEnergyProduction { get; set; }

        /// <summary>
        /// (kWh day^-1)
        /// </summary>
        public double ElectricityProduction { get; set; }

        /// <summary>
        /// (kWh day^-1)
        /// </summary>
        public double HeatProduced { get; set; }

        /// <summary>
        /// (kWh day^-1)
        /// </summary>
        public double MethaneToGrid { get; set; }

        /*
         * Storage emissions
         */

        /// <summary>
        /// (kg day^-1)
        /// </summary>
        public double MethaneEmissionsDuringStorage { get; set; }

        /// <summary>
        /// (kg day^-1)
        /// </summary>
        public double N2OEmissionsDuringStorage { get; set; }

        /// <summary>
        /// (kg day^-1)
        /// </summary>
        public double AmmoniaEmissionsDuringStorage { get; set; }

        /*
         * Land application
         */

        public double TotalAmountRawDigestateAvailableForLandApplication { get; set; }

        public double TotalAmountOfNitrogenFromRawDigestateAvailableForLandApplication { get; set; }
        public double TotalAmountOfTanInRawDigestateAvailalbleForLandApplication { get; set; }
        public double TotalAmountOfOrganicNitrogenInRawDigestateAvailableForLandApplication { get; set; }
        public double TotalAmountOfCarbonInRawDigestateAvailableForLandApplication { get; set; }

        public double TotalAmountRawDigestateAvailableForLandApplicationFromLiquidFraction { get; set; }
        public double TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromLiquidFraction { get; set; }
        public double TotalAmountOfTanInRawDigestateAvailalbleForLandApplicationFromLiquidFraction { get; set; }
        public double TotalAmountOfOrganicNitrogenInRawDigestateAvailableForLandApplicationFromLiquidFraction { get; set; }
        public double TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromLiquidFraction { get; set; }

        public double TotalAmountRawDigestateAvailableForLandApplicationFromSolidFraction { get; set; }
        public double TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromSolidFraction { get; set; }
        public double TotalAmountOfTanInRawDigestateAvailalbleForLandApplicationFromSolidFraction { get; set; }
        public double TotalAmountOfOrganicNitrogenInRawDigestateAvailableForLandApplicationFromSolidFraction { get; set; }
        public double TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromSolidFraction { get; set; }

        public double TotalAmountOfStoredDigestateAvailableForLandApplication { get; set; }
        public double TotalAmountOfStoredDigestateAvailableForLandApplicationLiquidFraction { get; set; }
        public double TotalAmountOfStoredDigestateAvailableForLandApplicationSolidFraction { get; set; }

        /// <summary>
        /// (kg N day^-1)
        /// </summary>
        public double TotalNitrogenInDigestateAvailableForLandApplication { get; set; }

        /// <summary>
        /// (kg C day^-1)
        /// </summary>
        public double TotalCarbonInDigestateAvailableForLandApplication { get; set; }
    }
}