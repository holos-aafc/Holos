﻿using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Providers.Animals;
using H.Core.Services;

namespace H.Core.Emissions.Results
{
    /// <summary>
    ///     A class to hold the monthly emissions for one animal group and one month.
    /// </summary>
    public class GroupEmissionsByMonth
    {
        #region Fields

        private readonly ITimePeriodHelper _timePeriodHelper = new TimePeriodHelper();
        private readonly EmissionTypeConverter _emissionsConverter;

        #endregion

        #region Constructors

        // Force clients to use overloaded constructor
        private GroupEmissionsByMonth()
        {
            _emissionsConverter = new EmissionTypeConverter();
            DailyEmissions = new List<GroupEmissionsByDay>();
        }

        public GroupEmissionsByMonth(MonthsAndDaysData monthsAndDaysData) : this()
        {
            MonthsAndDaysData = monthsAndDaysData;
        }

        public GroupEmissionsByMonth(MonthsAndDaysData monthsAndDaysData,
            IList<GroupEmissionsByDay> dailyEmissionsForMonth) : this(monthsAndDaysData)
        {
            DailyEmissions.AddRange(dailyEmissionsForMonth.OrderBy(x => x.DateTime));
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Emissions are calculated daily but some reports are by month or by year and so daily emissions are summed up per
        ///     month in this class
        /// </summary>
        public List<GroupEmissionsByDay> DailyEmissions { get; }

        public MonthsAndDaysData MonthsAndDaysData { get; set; }

        public int Month => MonthsAndDaysData.Month;

        public int Year => MonthsAndDaysData.Year;

        public string MonthString => _timePeriodHelper.GetMonthString(Month);

        public int DaysInMonth => MonthsAndDaysData.DaysInMonth;

        public double TotalCarbonDioxideEquivalentsForMonth =>
            _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsCH4, EmissionDisplayUnits.KilogramsC02e,
                MonthlyEntericMethaneEmission) +
            _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsCH4, EmissionDisplayUnits.KilogramsC02e,
                MonthlyManureMethaneEmission) +
            _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsN2O, EmissionDisplayUnits.KilogramsC02e,
                MonthlyDirectN2OEmission) +
            _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsN2O, EmissionDisplayUnits.KilogramsC02e,
                MonthlyIndirectN2OEmission) +
            _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsC02, EmissionDisplayUnits.KilogramsC02e,
                MonthlyEnergyCarbonDioxide);

        /// <summary>
        ///     (kg CH4)
        /// </summary>
        public double EntericMethaneRaminHuhtanenDairy
        {
            get { return DailyEmissions.Sum(x => x.EntericMethaneRaminHuhtanenDairy); }
        }

        /// <summary>
        ///     (kg CH4)
        /// </summary>
        public double EntericMethaneMillsEtAlDairy
        {
            get { return DailyEmissions.Sum(x => x.EntericMethaneMillsEtAlDairy); }
        }

        /// <summary>
        ///     (kg CH4)
        /// </summary>
        public double EntericMethaneEllisEtAlDairy
        {
            get { return DailyEmissions.Sum(x => x.EntericMethaneEllisEtAlDairy); }
        }

        /// <summary>
        ///     (kg CH4)
        /// </summary>
        public double EntericMethaneNuiEtAlDairy
        {
            get { return DailyEmissions.Sum(x => x.EntericMethaneNuiEtAlDairy); }
        }

        /// <summary>
        ///     (kg CH4)
        /// </summary>
        public double EntericMethaneEscobarEtAlAlOrBeef
        {
            get { return DailyEmissions.Sum(x => x.EntericMethaneEscobarEtAlAlOrBeef); }
        }

        /// <summary>
        ///     (kg CH4)
        /// </summary>
        public double EntericMethaneLingenEtAlBeef
        {
            get { return DailyEmissions.Sum(x => x.EntericMethaneLingenEtAlBeef); }
        }

        /// <summary>
        ///     (kg CH4)
        /// </summary>
        public double EntericMethaneEscobarEtAlLfMcBeef
        {
            get { return DailyEmissions.Sum(x => x.EntericMethaneEscobarEtAlLfMcBeef); }
        }

        /// <summary>
        ///     (kg CH4)
        /// </summary>
        public double EntericMethaneEllisEtAlBeef
        {
            get { return DailyEmissions.Sum(x => x.EntericMethaneEllisEtAlBeef); }
        }

        /// <summary>
        ///     (kg CH4)
        /// </summary>
        public double MonthlyEntericMethaneEmission
        {
            get { return DailyEmissions.Sum(x => x.EntericMethaneEmission); }
        }

        /// <summary>
        ///     (kg CH4)
        /// </summary>
        public double MonthlyManureMethaneEmission
        {
            get { return DailyEmissions.Sum(x => x.ManureMethaneEmission); }
        }

        /// <summary>
        ///     (kg N2O-N)
        /// </summary>
        public double MonthlyManureDirectN2ONEmission
        {
            get { return DailyEmissions.Sum(x => x.ManureDirectN2ONEmission); }
        }

        /// <summary>
        ///     (kg N2O)
        /// </summary>
        public double MonthlyManureDirectN2OEmission => CoreConstants.ConvertToN2O(MonthlyManureDirectN2ONEmission);

        /// <summary>
        ///     (kg N2O)
        /// </summary>
        public double MonthlyDirectN2OEmission => MonthlyManureDirectN2OEmission;

        /// <summary>
        ///     (kg N2O-N)
        /// </summary>
        public double MonthlyManureIndirectN2ONEmission
        {
            get { return DailyEmissions.Sum(x => x.ManureIndirectN2ONEmission); }
        }

        /// <summary>
        ///     (kg N2O)
        /// </summary>
        public double MonthlyManureIndirectN2OEmission => CoreConstants.ConvertToN2O(MonthlyManureIndirectN2ONEmission);

        /// <summary>
        ///     (kg N2O)
        /// </summary>
        public double MonthlyIndirectN2OEmission => MonthlyManureIndirectN2OEmission;

        /// <summary>
        ///     (kg N2O-N)
        /// </summary>
        public double MonthlyManureVolatilizationN2ONEmission
        {
            get { return DailyEmissions.Sum(x => x.ManureVolatilizationN2ONEmission); }
        }

        /// <summary>
        ///     (kg N2O)
        /// </summary>
        public double MonthlyManureVolatilizationN2OEmission =>
            CoreConstants.ConvertToN2O(MonthlyManureVolatilizationN2ONEmission);

        /// <summary>
        ///     (kg N2O-N)
        /// </summary>
        public double MonthlyManureLeachingN2ONEmission
        {
            get { return DailyEmissions.Sum(x => x.ManureN2ONLeachingEmission); }
        }

        public double MonthlyManureNitrateLeachingN2ONEmission
        {
            get { return DailyEmissions.Sum(x => x.ManureNitrateLeachingEmission); }
        }

        /// <summary>
        ///     (kg N2O)
        /// </summary>
        public double MonthlyManureLeachingN2OEmission => CoreConstants.ConvertToN2O(MonthlyManureLeachingN2ONEmission);

        /// <summary>
        ///     (kg N2O-N)
        /// </summary>
        public double MonthlyManureN2ONEmission
        {
            get { return DailyEmissions.Sum(x => x.ManureN2ONEmission); }
        }

        /// <summary>
        ///     (kg NH3)
        /// </summary>
        public double TotalAmmoniaEmissionsFromHousingSystem
        {
            get { return DailyEmissions.Sum(x => x.AmmoniaEmissionsFromHousingSystem); }
        }

        /// <summary>
        ///     (kg NH3)
        /// </summary>
        public double TotalAmmoniaEmissionsFromStorageSystem
        {
            get { return DailyEmissions.Sum(x => x.AmmoniaEmissionsFromStorageSystem); }
        }

        /// <summary>
        ///     (kg NH3)
        /// </summary>
        public double TotalAmmoniaEmissionsFromHousingAndStorageSystem
        {
            get { return DailyEmissions.Sum(x => x.AmmoniaEmissionsFromHousingAndStorage); }
        }

        /// <summary>
        ///     (kg NH3-N)
        /// </summary>
        public double TotalNitrogenLossesFromHousingAndStorage
        {
            get { return DailyEmissions.Sum(x => x.TotalNitrogenLossesFromHousingAndStorage); }
        }

        /// <summary>
        ///     (kg NH3)
        /// </summary>
        public double TotalAmmoniaLossesFromHousingAndStorage =>
            CoreConstants.ConvertToNH3(TotalNitrogenLossesFromHousingAndStorage);

        /// <summary>
        ///     (kg NH3)
        /// </summary>
        public double MonthlyTotalAmmoniaEmissions =>
            TotalAmmoniaEmissionsFromStorageSystem + TotalAmmoniaEmissionsFromHousingSystem;


        /// <summary>
        ///     (kg head^-1 day^-1)
        /// </summary>
        public double DryMatterIntake
        {
            get { return DailyEmissions.Average(x => x.DryMatterIntake); }
        }


        /// <summary>
        ///     Total carbon uptake by all animals in the group for the month.
        ///     (kg C)
        /// </summary>
        public double TotalMonthlyCarbonUptake
        {
            get { return DailyEmissions.Sum(x => x.TotalCarbonUptakeForGroup); }
        }

        /// <summary>
        ///     Total dry matter intake by all animals in the group for the month.
        ///     (kg)
        /// </summary>
        public double TotalMonthlyDmiUptakeForGroup
        {
            get { return DailyEmissions.Sum(x => x.TotalDmiUptakeForGroup); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (fraction)
        /// </summary>
        public double AverageFractionOfManureVolatilized
        {
            get { return DailyEmissions.Average(x => x.FractionOfManureVolatilized); }
        }


        /// <summary>
        ///     (1000 kg wet weight for solid manure, 1000 L for liquid manure)
        /// </summary>
        public double TotalVolumeOfManureAvailableForLandApplication
        {
            get { return DailyEmissions.Sum(x => x.TotalVolumeOfManureAvailableForLandApplication); }
        }

        /// <summary>
        ///     (1000 kg wet weight for solid manure, 1000 L for liquid manure)
        /// </summary>
        public double TotalVolumeOfManureAvailableForLandApplicationAccumulated =>
            DailyEmissions.Last().AccumulatedVolume;

        /// <summary>
        ///     (kg TAN)
        /// </summary>
        public double MonthlyTanExcretion
        {
            get { return DailyEmissions.Sum(x => x.TanExcretion); }
        }

        /// <summary>
        ///     (kg N)
        /// </summary>
        public double MonthlyFecalNitrogenExcretion
        {
            get { return DailyEmissions.Sum(x => x.FecalNitrogenExcretion); }
        }

        /// <summary>
        ///     (kg NH3-N)
        /// </summary>
        public double AmmoniaConcentrationInHousing
        {
            get { return DailyEmissions.Sum(x => x.AmmoniaConcentrationInHousing); }
        }

        #region Organic Nitrogen (ON)

        /*
         * ON
         */

        /// <summary>
        ///     Accumulated value over all months in management period
        ///     (kg N)
        /// </summary>
        public double TotalOrganicNitrogenAvailableForLandApplicationAtEndOfMonth =>
            // Get last daily result for the month and not the total of all days in the month since daily calculation are cumulative
            DailyEmissions.Last().AccumulatedOrganicNitrogenAvailableForLandApplicationOnDay;

        /// <summary>
        ///     Non-accumulated value for one the month
        ///     (kg N)
        /// </summary>
        public double TotalOrganicNitrogenAvailableCreatedInMonth
        {
            get
            {
                // Get last daily result for the month and not the total of all days in the month since daily calculation are cumulative
                return DailyEmissions.Sum(x => x.OrganicNitrogenCreatedOnDay);
            }
        }

        /// <summary>
        ///     (kg N)
        /// </summary>
        public double MonthlyOrganicNitrogenInStoredManure =>
            // Get last daily result for the month and not the total of all days in the month since daily calculation are cumulative
            DailyEmissions.Last().OrganicNitrogenInStoredManure;

        /// <summary>
        ///     Equation 4.5.2-7
        ///     (kg N)
        /// </summary>
        public double MonthlyNitrogenAvailableForLandApplication
        {
            get { return DailyEmissions.Sum(x => x.AccumulatedNitrogenAvailableForLandApplicationOnDay); }
        }

        #endregion

        #region Nitrogen

        /// <summary>
        ///     (kg N)
        /// </summary>
        public double TotalAvailableManureNitrogenInStoredManureAtEndOfMonth =>
            // Get last daily result for the month and not the total of all days in the month since daily calculation are cumulative
            DailyEmissions.Last().AccumulatedNitrogenAvailableForLandApplicationOnDay;

        /// <summary>
        ///     This is the total amount available for this month only, not the accumulated amount month-to-month
        ///     (kg TAN)
        /// </summary>
        public double TotalAmountOfNitrogenInStoredManureAvailableForMonth
        {
            get { return DailyEmissions.Sum(x => x.TotalAmountOfNitrogenInStoredManureAvailableForDay); }
        }

        #endregion

        #region Total Ammoniacal Nitrogen (TAN)

        /*
         * TAN
         */

        /// <summary>
        ///     This is the total amount available for this month only, not the accumulated amount month-to-month
        ///     (kg TAN)
        /// </summary>
        public double TotalAmountOfTanInStoredManureAvailableForMonth
        {
            get { return DailyEmissions.Sum(x => x.AdjustedAmountOfTanInStoredManureOnDay); }
        }

        /// <summary>
        ///     (kg TAN)
        /// </summary>
        public double MonthlyTanEnteringStorageSystem
        {
            get { return DailyEmissions.Sum(x => x.TanEnteringStorageSystem); }
        }

        /// <summary>
        ///     (kg TAN)
        /// </summary>
        public double TotalTANAvailableForLandApplicationAtEndOfMonth =>
            // Get last daily result for the month and not the total of all days in the month since daily calculation are cumulative
            DailyEmissions.Last().AccumulatedTANAvailableForLandApplicationOnDay;

        #endregion

        #region Carbon (C)

        /// <summary>
        ///     This is the total amount available for this month only, not the accumulated amount month-to-month
        ///     (kg C)
        /// </summary>
        public double TotalAmountOfCarbonInStoredManureAvailableForMonth
        {
            get { return DailyEmissions.Sum(x => x.AmountOfCarbonInStoredManure); }
        }

        #endregion

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (unitless)
        /// </summary>
        public double AverageAmbientAirTemperatureAdjustmentForHousing
        {
            get { return DailyEmissions.Average(x => x.AmbientAirTemperatureAdjustmentForHousing); }
        }

        /// <summary>
        ///     (kg NH3-N)
        /// </summary>
        public double AmmoniaConcentrationInStorage
        {
            get { return DailyEmissions.Sum(x => x.AmmoniaLostFromStorage); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (unitless)
        /// </summary>
        public double AverageAmbientAirTemperatureAdjustmentForStorage
        {
            get { return DailyEmissions.Average(x => x.AmbientAirTemperatureAdjustmentForStorage); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg NH3-N (kg TAN)^-1)
        /// </summary>
        public double AverageAdjustedAmmoniaEmissionFactorForHousing
        {
            get { return DailyEmissions.Average(x => x.AdjustedAmmoniaEmissionFactorForHousing); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg NH3-N (kg TAN)^-1)
        /// </summary>
        public double AverageAdjustedAmmoniaEmissionFactorForStorage
        {
            get { return DailyEmissions.Average(x => x.AdjustedAmmoniaEmissionFactorForStorage); }
        }

        /// <summary>
        ///     Equation 4.2.1-30
        ///     (kg N)
        /// </summary>
        public double MonthlyAmountOfNitrogenExcreted
        {
            get { return DailyEmissions.Sum(x => x.AmountOfNitrogenExcreted); }
        }

        /// <summary>
        ///     (kg NH3)
        /// </summary>
        public double MonthlyNH3FromGrazingAnimals
        {
            get { return DailyEmissions.Sum(x => x.NH3FromGrazingAnimals); }
        }


        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (unitless)
        /// </summary>
        public double AverageManureCarbonToNitrogenRatio
        {
            get { return DailyEmissions.Average(x => x.ManureCarbonNitrogenRatio); }
        }

        /// <summary>
        ///     Equation 4.1.1-5 (kg C)
        /// </summary>
        public double MonthlyFecalCarbonExcretion
        {
            get { return DailyEmissions.Sum(x => x.FecalCarbonExcretion); }
        }

        /// <summary>
        ///     (kg C)
        /// </summary>
        public double MonthlyAmountOfCarbonFromBedding
        {
            get { return DailyEmissions.Sum(x => x.CarbonAddedFromBeddingMaterial); }
        }

        /// <summary>
        ///     (kg N)
        /// </summary>
        public double MonthlyAmountOfNitrogenAddedFromBedding
        {
            get { return DailyEmissions.Sum(x => x.AmountOfNitrogenAddedFromBedding); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg head^-1 day^-1)
        /// </summary>
        public double AverageProteinIntake
        {
            get { return DailyEmissions.Average(x => x.ProteinIntake); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg N head^-1 day^-1)
        /// </summary>
        public double AverageNitrogenExcretionRate
        {
            get { return DailyEmissions.Average(x => x.NitrogenExcretionRate); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg N2O-N head^-1 day^-1)
        /// </summary>
        public double AverageDirectN2ONEmissionRate
        {
            get { return DailyEmissions.Average(x => x.ManureDirectN2ONEmissionRate); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg N head^-1 day^-1)
        /// </summary>
        public double AverageRateOfNitrogenAddedFromBeddingMaterial
        {
            get { return DailyEmissions.Average(x => x.RateOfNitrogenAddedFromBeddingMaterial); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg TAN head^-1 day^-1)
        /// </summary>
        public double AverageTanExcretionRate
        {
            get { return DailyEmissions.Average(x => x.TanExcretion); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (MJ head^-1 day^-1)
        /// </summary>
        public double AverageGrossEnergyIntake
        {
            get { return DailyEmissions.Average(x => x.GrossEnergyIntake); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg head^-1 day^-1)
        /// </summary>
        public double AverageDailyGain
        {
            get { return DailyEmissions.Average(x => x.AverageDailyGain); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg head^-1)
        /// </summary>
        public double AnimalWeight
        {
            get { return DailyEmissions.Average(x => x.AnimalWeight); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (MJ day^-1 kg^-1)
        /// </summary>
        public double AdjustedMaintenanceCoefficient
        {
            get { return DailyEmissions.Average(x => x.AdjustedMaintenanceCoefficient); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (MJ head^-1 day^-1)
        /// </summary>
        public double AverageNetEnergyForMaintenance
        {
            get { return DailyEmissions.Average(x => x.NetEnergyForMaintenance); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (MJ head^-1 day^-1)
        /// </summary>
        public double AverageNetEnergyForActivity
        {
            get { return DailyEmissions.Average(x => x.NetEnergyForActivity); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (MJ head^-1 day^-1)
        /// </summary>
        public double AverageNetEnergyForLactation
        {
            get { return DailyEmissions.Average(x => x.NetEnergyForLactation); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (MJ head^-1 day^-1)
        /// </summary>
        public double AverageNetEnergyForPregnancy
        {
            get { return DailyEmissions.Average(x => x.NetEnergyForPregnancy); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (MJ head^-1 day^-1)
        /// </summary>
        public double AverageNetEnergyForGain
        {
            get { return DailyEmissions.Average(x => x.NetEnergyForGain); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (MJ head^-1 day^-1)
        /// </summary>
        public double AverageNetEnergyForWoolProduction
        {
            get { return DailyEmissions.Average(x => x.NetEnergyForWoolProduction); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (unitless)
        /// </summary>
        public double AverageRatioOfEnergyAvailableForMaintenance
        {
            get { return DailyEmissions.Average(x => x.RatioOfEnergyAvailableForMaintenance); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (unitless)
        /// </summary>
        public double AverageRatioOfEnergyAvailableForGain
        {
            get { return DailyEmissions.Average(x => x.RatioOfEnergyAvailableForGain); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (unitless)
        /// </summary>
        public double AverageAdditiveReductionFactor
        {
            get { return DailyEmissions.Average(x => x.AdditiveReductionFactor); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg C head^-1 day^-1)
        /// </summary>
        public double AverageFecalCarbonExcretionRate
        {
            get { return DailyEmissions.Average(x => x.FecalCarbonExcretionRate); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg C head^-1 day^-1)
        /// </summary>
        public double AverageRateOfCarbonAddedFromBeddingMaterial
        {
            get { return DailyEmissions.Average(x => x.RateOfCarbonAddedFromBeddingMaterial); }
        }

        /// <summary>
        ///     (kg head^-1 day^-1)
        /// </summary>
        public double MonthlyVolatileSolids
        {
            get { return DailyEmissions.Sum(x => x.VolatileSolids); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg CH4 head^-1 day^-1)
        /// </summary>
        public double AverageManureMethaneEmissionRate
        {
            get { return DailyEmissions.Average(x => x.ManureMethaneEmissionRate); }
        }

        /// <summary>
        ///     (kg C)
        /// </summary>
        public double MonthlyAmountOfCarbonLostAsMethaneDuringManagement
        {
            get { return DailyEmissions.Sum(x => x.AmountOfCarbonLostAsMethaneDuringManagement); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg head^-1 day^-1)
        /// </summary>
        public double AverageProteinRetainedForPregnancy
        {
            get { return DailyEmissions.Average(x => x.ProteinRetainedForPregnancy); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg head^-1 day^-1)
        /// </summary>
        public double AverageProteinRetainedForLactation
        {
            get { return DailyEmissions.Average(x => x.ProteinRetainedForLactation); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg head^-1)
        /// </summary>
        public double AverageEmptyBodyWeight
        {
            get { return DailyEmissions.Average(x => x.EmptyBodyWeight); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg CH4 head^-1 day^-1)
        /// </summary>
        public double AverageEntericMethaneEmissionRate
        {
            get { return DailyEmissions.Average(x => x.EntericMethaneEmissionRate); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg head^-1 day^-1)
        /// </summary>
        public double AverageEmptyBodyGain
        {
            get { return DailyEmissions.Average(x => x.EmptyBodyGain); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (Mcal head^-1 day^-1)
        /// </summary>
        public double AverageRetainedEnergy
        {
            get { return DailyEmissions.Average(x => x.RetainedEnergy); }
        }

        /// <summary>
        ///     (kg head^-1 day^-1)
        /// </summary>
        public double AverageProteinRetainedForGain
        {
            get { return DailyEmissions.Average(x => x.ProteinRetainedForGain); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg N head^-1 day^-1)
        /// </summary>
        public double AverageFecalNitrogenExcretionRate
        {
            get { return DailyEmissions.Average(x => x.FecalNitrogenExcretionRate); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg N2O-N head^-1 day^-1)
        /// </summary>
        public double AverageManureVolatilizationEmissionRate
        {
            get { return DailyEmissions.Average(x => x.ManureVolatilizationRate); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg N2O-N head^-1 day^-1)
        /// </summary>
        public double AverageManureNitrogenLeachingRate
        {
            get { return DailyEmissions.Average(x => x.ManureNitrogenLeachingRate); }
        }

        /// <summary>
        ///     (kg N2O-N day^-1)
        /// </summary>
        public double ManureNitrogenLeaching
        {
            get { return DailyEmissions.Sum(x => x.ManureN2ONLeachingEmission); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg head^-1 day^-1)
        /// </summary>
        public double AverageProteinIntakeFromSolidFeed
        {
            get { return DailyEmissions.Average(x => x.ProteinIntakeFromSolidFood); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg head^-1 day^-1)
        /// </summary>
        public double AverageProteinIntakeFromMilk
        {
            get { return DailyEmissions.Average(x => x.ProteinIntakeFromMilk); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg head^-1 day^-1)
        /// </summary>
        public double AverageProteinRetained
        {
            get { return DailyEmissions.Average(x => x.ProteinRetained); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg head^-1 day^-1)
        /// </summary>
        public double AverageProteinRetainedFromMilk
        {
            get { return DailyEmissions.Average(x => x.ProteinRetainedFromMilk); }
        }

        /// <summary>
        ///     Use an average of the daily values when reporting monthly
        ///     (kg head^-1 day^-1)
        /// </summary>
        public double AverageProteinRetainedFromSolidFeed
        {
            get { return DailyEmissions.Average(x => x.ProteinRetainedFromSolidFood); }
        }

        /// <summary>
        ///     (kg TAN (kg manure-N)^-1)
        /// </summary>
        public double FractionOfNitrogenExcretedInUrine
        {
            get { return DailyEmissions.Average(x => x.FractionOfNitrogenExcretedInUrine); }
        }


        /// <summary>
        ///     (kg C)
        /// </summary>
        public double TotalAmountOfCarbonInStoredManureAtEndOfMonth =>
            // Get last daily result for the month and not the total of all days in the month since daily calculation are cumulative
            DailyEmissions.Last().AccumulatedAmountOfCarbonInStoredManureOnDay;

        public double AverageTemperature
        {
            get { return DailyEmissions.Average(x => x.Temperature); }
        }

        public double MonthlyElectricityProducedFromAnaerobicDigestion
        {
            get { return DailyEmissions.Sum(x => x.ElectricityProducedFromAnaerobicDigestion); }
        }

        public double MonthlyHeatProducedFromAnaerobicDigestion
        {
            get { return DailyEmissions.Sum(x => x.HeatProducedFromAnaerobicDigestion); }
        }

        public double MonthlyMethaneInjectionIntoGrid
        {
            get { return DailyEmissions.Sum(x => x.PotentialMethaneInjectionIntoGridFromAnaerobicDigestion); }
        }

        /// <summary>
        ///     (kg N2O)
        /// </summary>
        public double TotalIndirectN2OFromLandAppliedManure
        {
            get { return DailyEmissions.Sum(x => x.TotalIndirectN2OFromLandAppliedManure); }
        }

        /// <summary>
        ///     (kg NH3-N)
        /// </summary>
        public double TotalAmmoniacalNitrogenFromLandAppliedManure
        {
            get { return DailyEmissions.Sum(x => x.AmmoniacalNitrogenFromLandAppliedManure); }
        }

        /// <summary>
        ///     (kg NH3)
        /// </summary>
        public double TotalAmmoniaFromLandAppliedManure
        {
            get { return DailyEmissions.Sum(x => x.AmmoniaFromLandAppliedManure); }
        }

        /// <summary>
        ///     (kg N2O-N)
        /// </summary>
        public double TotalMonthlyLeachingEmissionsFromLandAppliedManure
        {
            get { return DailyEmissions.Sum(x => x.N2ONLeachingEmissionsFromLandAppliedManure); }
        }

        /// <summary>
        ///     (kg N2O)
        /// </summary>
        public double TotalMonthlyN2OLeachingEmissionsFromLandAppliedManureFromLandAppliedManure
        {
            get { return DailyEmissions.Sum(x => x.N2OLeachingEmissionsFromLandAppliedManure); }
        }

        /// <summary>
        ///     (kg)
        /// </summary>
        public double MonthlyMilkProduction { get; set; }

        /// <summary>
        ///     Fat and protein corrected milk production for month (kg)
        ///     FPCM
        /// </summary>
        public double MonthlyFatAndProteinCorrectedMilkProduction { get; set; }

        /// <summary>
        ///     Live weight of beef produced for month (kg) – from initial weight to final weight
        /// </summary>
        public double MonthlyBeefProduced { get; set; }

        /// <summary>
        ///     Live weight of lamb produced for month (kg) – from initial weight to final weight
        /// </summary>
        public double MonthlyLambProduced { get; set; }

        public Table_30_Default_Bedding_Material_Composition_Data BeddingMaterialComposition { get; set; }

        /// <summary>
        ///     Total carbon dioxide emissions associated with this <see cref="AnimalGroup" />.
        ///     (kg C02 month^-1)
        /// </summary>
        public double MonthlyEnergyCarbonDioxide { get; set; }

        #endregion

        #region Methods

        public List<DateTime> GetDatesWhereDmiIsGreaterThanDmiMax()
        {
            var result = DailyEmissions.Where(x => x.DryMatterIntake > x.DryMatterIntakeMax).Select(x => x.DateTime)
                .ToList();

            return result;
        }

        public override string ToString()
        {
            return
                $"{nameof(Month)}: {MonthsAndDaysData.MonthString}, {nameof(DaysInMonth)}: {DaysInMonth}, {nameof(MonthlyEntericMethaneEmission)}: {MonthlyEntericMethaneEmission}, {nameof(MonthlyManureMethaneEmission)}: {MonthlyManureMethaneEmission}, {nameof(MonthlyManureDirectN2OEmission)}: {MonthlyManureDirectN2OEmission}, {nameof(MonthlyManureIndirectN2OEmission)}: {MonthlyManureIndirectN2OEmission}, {nameof(MonthlyEnergyCarbonDioxide)}: {MonthlyEnergyCarbonDioxide}, {nameof(MonthlyManureVolatilizationN2ONEmission)}: {MonthlyManureVolatilizationN2ONEmission}, {nameof(MonthlyManureLeachingN2ONEmission)}: {MonthlyManureLeachingN2ONEmission}, {nameof(MonthlyManureN2ONEmission)}: {MonthlyManureN2ONEmission}, {nameof(MonthlyTotalAmmoniaEmissions)}: {MonthlyTotalAmmoniaEmissions}, {nameof(MonthlyMilkProduction)}: {MonthlyMilkProduction}, {nameof(MonthlyFatAndProteinCorrectedMilkProduction)}: {MonthlyFatAndProteinCorrectedMilkProduction}, {nameof(MonthlyBeefProduced)}: {MonthlyBeefProduced}, {nameof(MonthlyLambProduced)}: {MonthlyLambProduced}, {nameof(DryMatterIntake)}: {DryMatterIntake}";
        }

        #endregion
    }
}