using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Providers.Animals;
using H.Core.Services;

namespace H.Core.Emissions.Results
{
    /// <summary>
    /// A class to hold the monthly emissions for one animal group and one month.
    /// </summary>
    public class GroupEmissionsByMonth
    {
        #region Fields

        private readonly ITimePeriodHelper _timePeriodHelper = new TimePeriodHelper();
        private readonly EmissionTypeConverter _emissionsConverter;

        /// <summary>
        /// Emissions are calculated daily but some reports are by month or by year and so daily emissions are summed up per month in this class
        /// </summary>
        private readonly List<GroupEmissionsByDay> _dailyEmissions;

        #endregion

        #region Constructors

        // Force clients to use overloaded constructor
        private GroupEmissionsByMonth()
        {
            _emissionsConverter = new EmissionTypeConverter();
            _dailyEmissions = new List<GroupEmissionsByDay>();
        }

        public GroupEmissionsByMonth(MonthsAndDaysData monthsAndDaysData) : this()
        {
            this.MonthsAndDaysData = monthsAndDaysData;
        }

        public GroupEmissionsByMonth(MonthsAndDaysData monthsAndDaysData, IList<GroupEmissionsByDay> dailyEmissionsForMonth) : this(monthsAndDaysData)
        {
            _dailyEmissions.AddRange(dailyEmissionsForMonth);
        }

        #endregion

        #region Properties

        public MonthsAndDaysData MonthsAndDaysData { get; set; }

        public int Month
        {
            get { return this.MonthsAndDaysData.Month; }
        }

        public int Year
        {
            get
            {
                return this.MonthsAndDaysData.Year;
            }
        }

        public string MonthString
        {
            get { return _timePeriodHelper.GetMonthString(this.Month); }
        }

        public int DaysInMonth
        {
            get { return this.MonthsAndDaysData.DaysInMonth; }
        }

        public double TotalCarbonDioxideEquivalentsForMonth
        {
            get
            {
                return _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsCH4, EmissionDisplayUnits.KilogramsC02e, this.MonthlyEntericMethaneEmission) +
                       _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsCH4, EmissionDisplayUnits.KilogramsC02e, this.MonthlyManureMethaneEmission) +
                       _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsN2O, EmissionDisplayUnits.KilogramsC02e, this.MonthlyDirectN2OEmission) +
                       _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsN2O, EmissionDisplayUnits.KilogramsC02e, this.MonthlyIndirectN2OEmission) +
                       _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsC02, EmissionDisplayUnits.KilogramsC02e, this.MonthlyEnergyCarbonDioxide);
            }
        }

        /// <summary>
        /// (kg CH4)
        /// </summary>
        public double EntericMethaneRaminHuhtanenDairy
        {
            get
            {
                return _dailyEmissions.Sum(x => x.EntericMethaneRaminHuhtanenDairy);
            }
        }

        /// <summary>
        /// (kg CH4)
        /// </summary>
        public double EntericMethaneMillsEtAlDairy
        {
            get
            {
                return _dailyEmissions.Sum(x => x.EntericMethaneMillsEtAlDairy);
            }
        }

        /// <summary>
        /// (kg CH4)
        /// </summary>
        public double EntericMethaneEllisEtAlDairy
        {
            get
            {
                return _dailyEmissions.Sum(x => x.EntericMethaneEllisEtAlDairy);
            }
        }

        /// <summary>
        /// (kg CH4)
        /// </summary>
        public double EntericMethaneNuiEtAlDairy
        {
            get
            {
                return _dailyEmissions.Sum(x => x.EntericMethaneNuiEtAlDairy);
            }
        }

        /// <summary>
        /// (kg CH4)
        /// </summary>
        public double EntericMethaneEscobarEtAlAlOrBeef
        {
            get
            {
                return _dailyEmissions.Sum(x => x.EntericMethaneEscobarEtAlAlOrBeef);
            }
        }

        /// <summary>
        /// (kg CH4)
        /// </summary>
        public double EntericMethaneLingenEtAlBeef
        {
            get
            {
                return _dailyEmissions.Sum(x => x.EntericMethaneLingenEtAlBeef);
            }
        }

        /// <summary>
        /// (kg CH4)
        /// </summary>
        public double EntericMethaneEscobarEtAlLfMcBeef
        {
            get
            {
                return _dailyEmissions.Sum(x => x.EntericMethaneEscobarEtAlLfMcBeef);
            }
        }

        /// <summary>
        /// (kg CH4)
        /// </summary>
        public double EntericMethaneEllisEtAlBeef
        {
            get
            {
                return _dailyEmissions.Sum(x => x.EntericMethaneEllisEtAlBeef);
            }
        }

        /// <summary>
        /// (kg CH4)
        /// </summary>
        public double MonthlyEntericMethaneEmission
        {
            get
            {
                return _dailyEmissions.Sum(x => x.EntericMethaneEmission);
            }
        }

        /// <summary>
        /// (kg CH4)
        /// </summary>
        public double MonthlyManureMethaneEmission
        {
            get
            {
                return _dailyEmissions.Sum(x => x.ManureMethaneEmission);
            }
        }

        /// <summary>
        /// (kg N2O-N)
        /// </summary>
        public double MonthlyManureDirectN2ONEmission
        {
            get
            {
                return _dailyEmissions.Sum(x => x.ManureDirectN2ONEmission);
            }
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double MonthlyManureDirectN2OEmission
        {
            get { return this.MonthlyManureDirectN2ONEmission * CoreConstants.ConvertN2ONToN2O; }
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double MonthlyDirectN2OEmission
        {
            get
            {
                return this.MonthlyManureDirectN2OEmission;
            }
        }

        /// <summary>
        /// (kg N2O-N)
        /// </summary>
        public double MonthlyManureIndirectN2ONEmission 
        {
            get
            {
                return _dailyEmissions.Sum(x => x.ManureIndirectN2ONEmission);
            }
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double MonthlyManureIndirectN2OEmission
        {
            get { return this.MonthlyManureIndirectN2ONEmission * CoreConstants.ConvertN2ONToN2O; }
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double MonthlyIndirectN2OEmission
        {
            get
            {
                return this.MonthlyManureIndirectN2OEmission;
            }
        }

        /// <summary>
        /// (kg N2O-N)
        /// </summary>
        public double MonthlyManureVolatilizationN2ONEmission 
        {
            get
            {
                return _dailyEmissions.Sum(x => x.ManureVolatilizationN2ONEmission);
            }
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double MonthlyManureVolatilizationN2OEmission
        {
            get
            {
                return this.MonthlyManureVolatilizationN2ONEmission * CoreConstants.ConvertN2ONToN2O;
            }
        }

        /// <summary>
        /// (kg N2O-N)
        /// </summary>
        public double MonthlyManureLeachingN2ONEmission 
        {
            get
            {
                return _dailyEmissions.Sum(x => x.ManureN2ONLeachingEmission);
            }
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public double MonthlyManureLeachingN2OEmission
        {
            get
            {
                return this.MonthlyManureLeachingN2ONEmission * CoreConstants.ConvertN2ONToN2O;
            }
        }

        /// <summary>
        /// (kg N2O-N)
        /// </summary>
        public double MonthlyManureN2ONEmission 
        {
            get
            {
                return _dailyEmissions.Sum(x => x.ManureN2ONEmission);
            }
        }

        /// <summary>
        /// (kg NH3)
        /// </summary>
        public double TotalAmmoniaEmissionsFromHousingSystem
        {
            get
            {
                return _dailyEmissions.Sum(x => x.AmmoniaEmissionsFromHousingSystem);
            }
        }

        /// <summary>
        /// (kg NH3)
        /// </summary>
        public double TotalAmmoniaEmissionsFromStorageSystem
        {
            get
            {
                return _dailyEmissions.Sum(x => x.AmmoniaEmissionsFromStorageSystem);
            }
        }

        /// <summary>
        /// (kg NH3)
        /// </summary>
        public double MonthlyTotalAmmoniaEmissions
        {
            get
            {
                return this.TotalAmmoniaEmissionsFromStorageSystem + TotalAmmoniaEmissionsFromHousingSystem;
            }
        }

        /// <summary>
        /// (kg C)
        /// </summary>
        public double TotalAmountOfCarbonInStoredManure
        {
            get
            {
                return _dailyEmissions.Sum(x => x.AmountOfCarbonInStoredManure);
            }
        }

        /// <summary>
        /// (kg head^-1 day^-1)
        /// </summary>
        public double DryMatterIntake 
        {
            get
            {
                return _dailyEmissions.Average(x => x.DryMatterIntake);
            }
        }

        /// <summary>
        /// Total carbon uptake by all animals in the group for the month.
        ///
        /// (kg C)
        /// </summary>
        public double TotalMonthlyCarbonUptake
        {
            get
            {
                return _dailyEmissions.Sum(x => x.TotalCarbonUptakeForGroup);
            }
        }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double MonthlyOrganicNitrogenInStoredManure 
        {
            get
            {
                return _dailyEmissions.Sum(x => x.OrganicNitrogenInStoredManure);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        ///
        /// (fraction)
        /// </summary>
        public double AverageFractionOfManureVolatilized 
        {
            get
            {
                return _dailyEmissions.Average(x => x.FractionOfManureVolatilized);
            }

        }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double TotalAvailableManureNitrogenInStoredManure
        {
            get
            {
                return _dailyEmissions.Sum(x => x.NitrogenAvailableForLandApplication);
            }
        }

        /// <summary>
        /// (1000 kg wet weight for solid manure, 1000 L for liquid manure)
        /// </summary>
        public double TotalVolumeOfManureAvailableForLandApplication
        {
            get
            {
                return _dailyEmissions.Sum(x => x.TotalVolumeOfManureAvailableForLandApplication);
            }
        }

        /// <summary>
        /// (kg TAN)
        /// </summary>
        public double MonthlyTanExcretion
        {
            get
            {
                return _dailyEmissions.Sum(x => x.TanExcretion);
            }
        }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double MonthlyFecalNitrogenExcretion
        {
            get
            {
                return _dailyEmissions.Sum(x => x.FecalNitrogenExcretion);
            }
        }

        /// <summary>
        /// (kg NH3-N)
        /// </summary>
        public double AmmoniaConcentrationInHousing
        {
            get
            {
                return _dailyEmissions.Sum(x => x.AmmoniaConcentrationInHousing);
            }
        }

        /// <summary>
        /// (kg TAN)
        /// </summary>
        public double AdjustedMonthlyAmountOfTanInStoredManure
        {
            get
            {
                return _dailyEmissions.Sum(x => x.AdjustedAmountOfTanInStoredManure);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (unitless)
        /// </summary>
        public double AverageAmbientAirTemperatureAdjustmentForHousing
        {
            get
            {
                return _dailyEmissions.Average(x => x.AmbientAirTemperatureAdjustmentForHousing);
            }
        }

        /// <summary>
        /// (kg NH3-N)
        /// </summary>
        public double AmmoniaConcentrationInStorage
        {
            get
            {
                return _dailyEmissions.Sum(x => x.AmmoniaLostFromStorage);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (unitless)
        /// </summary>
        public double AverageAmbientAirTemperatureAdjustmentForStorage
        {
            get
            {
                return _dailyEmissions.Average(x => x.AmbientAirTemperatureAdjustmentForStorage);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg NH3-N (kg TAN)^-1)
        /// </summary>
        public double AverageAdjustedAmmoniaEmissionFactorForHousing
        {
            get
            {
                return _dailyEmissions.Average(x => x.AdjustedAmmoniaEmissionFactorForHousing);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg NH3-N (kg TAN)^-1)
        /// </summary>
        public double AverageAdjustedAmmoniaEmissionFactorForStorage
        {
            get
            {
                return _dailyEmissions.Average(x => x.AdjustedAmmoniaEmissionFactorForStorage);
            }
        }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double MonthlyAmountOfNitrogenExcreted
        {
            get
            {
                return _dailyEmissions.Sum(x => x.AmountOfNitrogenExcreted);
            }
        }

        /// <summary>
        /// (kg TAN)
        /// </summary>
        public double MonthlyTanEnteringStorageSystem
        {
            get
            {
                return _dailyEmissions.Sum(x => x.TanEnteringStorageSystem);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        ///
        /// (unitless)
        /// </summary>
        public double AverageManureCarbonToNitrogenRatio
        {
            get
            {
                return _dailyEmissions.Average(x => x.ManureCarbonNitrogenRatio);
            }
        }

        /// <summary>
        /// (kg C)
        /// </summary>
        public double MonthlyFecalCarbonExcretion
        {
            get
            {
                return _dailyEmissions.Sum(x => x.FecalCarbonExcretion);
            }
        }

        /// <summary>
        /// (kg C)
        /// </summary>
        public double MonthlyAmountOfCarbonFromBedding 
        {
            get
            {
                return _dailyEmissions.Sum(x => x.CarbonAddedFromBeddingMaterial);
            }
        }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double MonthlyAmountOfNitrogenAddedFromBedding
        {
            get
            {
                return _dailyEmissions.Sum(x => x.AmountOfNitrogenAddedFromBedding);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double AverageProteinIntake
        {
            get
            {
                return _dailyEmissions.Average(x => x.ProteinIntake);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg N head^-1 day^-1)
        /// </summary>
        public double AverageNitrogenExcretionRate
        {
            get
            {
                return _dailyEmissions.Average(x => x.NitrogenExcretionRate);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg N2O-N head^-1 day^-1)
        /// </summary>
        public double AverageDirectN2ONEmissionRate
        {
            get
            {
                return _dailyEmissions.Average(x => x.ManureDirectN2ONEmissionRate);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg N head^-1 day^-1)
        /// </summary>
        public double AverageRateOfNitrogenAddedFromBeddingMaterial
        {
            get
            {
                return _dailyEmissions.Average(x => x.RateOfNitrogenAddedFromBeddingMaterial);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg TAN head^-1 day^-1)
        /// </summary>
        public double AverageTanExcretionRate 
        {
            get
            {
                return _dailyEmissions.Average(x => x.TanExcretion);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (MJ head^-1 day^-1)
        /// </summary>
        public double AverageGrossEnergyIntake
        {
            get
            {
                return _dailyEmissions.Average(x => x.GrossEnergyIntake);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double AverageDailyGain
        {
            get
            {
                return _dailyEmissions.Average(x => x.AverageDailyGain);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg head^-1)
        /// </summary>
        public double AnimalWeight
        {
            get
            {
                return _dailyEmissions.Average(x => x.AnimalWeight);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (MJ day^-1 kg^-1)
        /// </summary>
        public double AdjustedMaintenanceCoefficient
        {
            get
            {
                return _dailyEmissions.Average(x => x.AdjustedMaintenanceCoefficient);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (MJ head^-1 day^-1)
        /// </summary>
        public double AverageNetEnergyForMaintenance
        {
            get
            {
                return _dailyEmissions.Average(x => x.NetEnergyForMaintenance);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (MJ head^-1 day^-1)
        /// </summary>
        public double AverageNetEnergyForActivity 
        {
            get
            {
                return _dailyEmissions.Average(x => x.NetEnergyForActivity);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (MJ head^-1 day^-1)
        /// </summary>
        public double AverageNetEnergyForLactation
        {
            get
            {
                return _dailyEmissions.Average(x => x.NetEnergyForLactation);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (MJ head^-1 day^-1)
        /// </summary>
        public double AverageNetEnergyForPregnancy
        {
            get
            {
                return _dailyEmissions.Average(x => x.NetEnergyForPregnancy);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (MJ head^-1 day^-1)
        /// </summary>
        public double AverageNetEnergyForGain
        {
            get
            {
                return _dailyEmissions.Average(x => x.NetEnergyForGain);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (MJ head^-1 day^-1)
        /// </summary>
        public double AverageNetEnergyForWoolProduction
        {
            get
            {
                return _dailyEmissions.Average(x => x.NetEnergyForWoolProduction);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (unitless)
        /// </summary>
        public double AverageRatioOfEnergyAvailableForMaintenance
        {
            get
            {
                return _dailyEmissions.Average(x => x.RatioOfEnergyAvailableForMaintenance);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (unitless)
        /// </summary>
        public double AverageRatioOfEnergyAvailableForGain
        {
            get
            {
                return _dailyEmissions.Average(x => x.RatioOfEnergyAvailableForGain);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (unitless)
        /// </summary>
        public double AverageAdditiveReductionFactor 
        {
            get
            {
                return _dailyEmissions.Average(x => x.AdditiveReductionFactor);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg C head^-1 day^-1)
        /// </summary>
        public double AverageFecalCarbonExcretionRate
        {
            get
            {
                return _dailyEmissions.Average(x => x.FecalCarbonExcretionRate);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg C head^-1 day^-1)
        /// </summary>
        public double AverageRateOfCarbonAddedFromBeddingMaterial
        {
            get
            {
                return _dailyEmissions.Average(x => x.RateOfCarbonAddedFromBeddingMaterial);
            }
        }

        /// <summary>
        /// (kg head^-1 day^-1)
        /// </summary>
        public double MonthlyVolatileSolids 
        { 
            get
            {
                return _dailyEmissions.Sum(x => x.VolatileSolids);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg CH4 head^-1 day^-1)
        /// </summary>
        public double AverageManureMethaneEmissionRate
        {
            get
            {
                return _dailyEmissions.Average(x => x.ManureMethaneEmissionRate);
            }
        }

        /// <summary>
        /// (kg C)
        /// </summary>
        public double MonthlyAmountOfCarbonLostAsMethaneDuringManagement
        {
            get
            {
                return _dailyEmissions.Sum(x => x.AmountOfCarbonLostAsMethaneDuringManagement);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double AverageProteinRetainedForPregnancy
        {
            get
            {
                return _dailyEmissions.Average(x => x.ProteinRetainedForPregnancy);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double AverageProteinRetainedForLactation
        {
            get
            {
                return _dailyEmissions.Average(x => x.ProteinRetainedForLactation);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg head^-1)
        /// </summary>
        public double AverageEmptyBodyWeight
        {
            get
            {
                return _dailyEmissions.Average(x => x.EmptyBodyWeight);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg CH4 head^-1 day^-1)
        /// </summary>
        public double AverageEntericMethaneEmissionRate 
        {
            get
            {
                return _dailyEmissions.Average(x => x.EntericMethaneEmissionRate);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double AverageEmptyBodyGain
        {
            get
            {
                return _dailyEmissions.Average(x => x.EmptyBodyGain);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (Mcal head^-1 day^-1)
        /// </summary>
        public double AverageRetainedEnergy
        {
            get
            {
                return _dailyEmissions.Average(x => x.RetainedEnergy);
            }
        }

        /// <summary>
        /// (kg head^-1 day^-1)
        /// </summary>
        public double AverageProteinRetainedForGain
        {
            get
            {
                return _dailyEmissions.Average(x => x.ProteinRetainedForGain);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg N head^-1 day^-1)
        /// </summary>
        public double AverageFecalNitrogenExcretionRate 
        {
            get
            {
                return _dailyEmissions.Average(x => x.FecalNitrogenExcretionRate);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg N2O-N head^-1 day^-1)
        /// </summary>
        public double AverageManureVolatilizationEmissionRate
        {
            get
            {
                return _dailyEmissions.Average(x => x.ManureVolatilizationRate);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg N2O-N head^-1 day^-1)
        /// </summary>
        public double AverageManureNitrogenLeachingRate
        {
            get
            {
                return _dailyEmissions.Average(x => x.ManureNitrogenLeachingRate);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double AverageProteinIntakeFromSolidFeed 
        {
            get
            {
                return _dailyEmissions.Average(x => x.ProteinIntakeFromSolidFood);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double AverageProteinIntakeFromMilk
        {
            get
            {
                return _dailyEmissions.Average(x => x.ProteinIntakeFromMilk);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double AverageProteinRetained 
        {
            get
            {
                return _dailyEmissions.Average(x => x.ProteinRetained);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double AverageProteinRetainedFromMilk
        {
            get
            {
                return _dailyEmissions.Average(x => x.ProteinRetainedFromMilk);
            }
        }

        /// <summary>
        /// Use an average of the daily values when reporting monthly
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double AverageProteinRetainedFromSolidFeed
        {
            get
            {
                return _dailyEmissions.Average(x => x.ProteinRetainedFromSolidFood);
            }
        }

        /// <summary>
        /// (kg TAN (kg manure-N)^-1)
        /// </summary>
        public double FractionOfNitrogenExcretedInUrine
        {
            get
            {
                return _dailyEmissions.Average(x => x.FractionOfNitrogenExcretedInUrine);
            }
        }

        /// <summary>
        /// (kg TAN)
        /// </summary>
        public double MonthlyTanAvailableForLandApplication
        {
            get
            {
                return _dailyEmissions.Sum(x => x.TanAvailableForLandApplication);
            }
        }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double MonthlyOrganicNitrogenAvailableForLandApplication
        {
            get
            {
                return _dailyEmissions.Sum(x => x.OrganicNitrogenAvailableForLandApplication);
            }
        }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double MonthlyNitrogenAvailableForLandApplication
        {
            get
            {
                return _dailyEmissions.Sum(x => x.NitrogenAvailableForLandApplication);
            }
        }

        public double AverageTemperature
        {
            get
            {
                return _dailyEmissions.Average(x => x.Temperature);
            }
        }

        public double MonthlyElectricityProducedFromAnaerobicDigestion
        {
            get
            {
                return _dailyEmissions.Sum(x => x.ElectricityProducedFromAnaerobicDigestion);
            }
        }

        public double MonthlyHeatProducedFromAnaerobicDigestion
        {
            get
            {
                return _dailyEmissions.Sum(x => x.HeatProducedFromAnaerobicDigestion);
            }
        }

        public double MonthlyMethaneInjectionIntoGrid
        {
            get
            {
                return _dailyEmissions.Sum(x => x.PotentialMethaneInjectionIntoGridFromAnaerobicDigestion);
            }
        }

        /// <summary>
        /// (kg)
        /// </summary>
        public double MonthlyMilkProduction { get; set; }

        /// <summary>
        /// Fat and protein corrected milk production for month (kg)
        /// 
        /// FPCM
        /// </summary>
        public double MonthlyFatAndProteinCorrectedMilkProduction { get; set; }

        /// <summary>
        /// Live weight of beef produced for month (kg) – from initial weight to final weight
        /// </summary>
        public double MonthlyBeefProduced { get; set; }

        /// <summary>
        /// Live weight of lamb produced for month (kg) – from initial weight to final weight
        /// </summary>
        public double MonthlyLambProduced { get; set; }

        public Table_33_Default_Bedding_Material_Composition_Data BeddingMaterialComposition { get; set; }

        /// <summary>
        /// Total carbon dioxide emissions associated with this <see cref="AnimalGroup"/>.
        /// 
        /// (kg C02 month^-1)
        /// </summary>
        public double MonthlyEnergyCarbonDioxide { get; set; }

        /// <summary>
        /// Carbon dioxide emissions from manure spreading/application (kg CO₂)
        /// </summary>
        public double MonthlyCarbonDioxideFromManureSpreading { get; set; }

        #endregion

        #region Methods

        public List<DateTime> GetDatesWhereDmiIsGreaterThanDmiMax()
        {
            var result = _dailyEmissions.Where(x => x.DryMatterIntake > x.DryMatterIntakeMax).Select(x => x.DateTime).ToList();

            return result;
        }

        public override string ToString()
        {
            return $"{nameof(Month)}: {this.MonthsAndDaysData.MonthString}, {nameof(DaysInMonth)}: {DaysInMonth}, {nameof(MonthlyEntericMethaneEmission)}: {MonthlyEntericMethaneEmission}, {nameof(MonthlyManureMethaneEmission)}: {MonthlyManureMethaneEmission}, {nameof(MonthlyManureDirectN2OEmission)}: {MonthlyManureDirectN2OEmission}, {nameof(MonthlyManureIndirectN2OEmission)}: {MonthlyManureIndirectN2OEmission}, {nameof(MonthlyEnergyCarbonDioxide)}: {MonthlyEnergyCarbonDioxide}, {nameof(MonthlyManureVolatilizationN2ONEmission)}: {MonthlyManureVolatilizationN2ONEmission}, {nameof(MonthlyManureLeachingN2ONEmission)}: {MonthlyManureLeachingN2ONEmission}, {nameof(MonthlyManureN2ONEmission)}: {MonthlyManureN2ONEmission}, {nameof(MonthlyTotalAmmoniaEmissions)}: {MonthlyTotalAmmoniaEmissions}, {nameof(MonthlyMilkProduction)}: {MonthlyMilkProduction}, {nameof(MonthlyFatAndProteinCorrectedMilkProduction)}: {MonthlyFatAndProteinCorrectedMilkProduction}, {nameof(MonthlyBeefProduced)}: {MonthlyBeefProduced}, {nameof(MonthlyLambProduced)}: {MonthlyLambProduced}, {nameof(DryMatterIntake)}: {DryMatterIntake}";
        }

        #endregion
    }
}