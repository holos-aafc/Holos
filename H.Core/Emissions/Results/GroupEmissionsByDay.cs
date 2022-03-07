using System;
using System.Configuration;
using H.Core.Providers.Animals;
using H.Infrastructure;

namespace H.Core.Emissions.Results
{
    public class GroupEmissionsByDay : ModelBase
    {
        #region Fields

        private DateTime _dateTime;

        private double _averageWeight;
        private double _adjustedMaintenanceCoefficient;
        private double _netEnergyForMaintenance;
        private double _netEnergyForActivity;
        private double _netEnergyForLactation;
        private double _netEnergyForPregnancy;
        private double _averageDailyGain;
        private double _netEnergyForGain;
        private double _ratioOfEnergyAvailableForMaintenance;
        private double _ratioOfEnergyAvailableForGain;
        private double _grossEnergyIntake;
        private double _additiveReductionFactor;
        private double _entericMethaneEmissionRate;
        private double _entericMethaneEmission;
        private double _dryMatterIntake;
        private double _dryMatterIntakeForGroup;
        private double _carbonUptake;
        private double _dryMatterIntakeMax;
        private double _crudeFatIntake;
        private double _crudeProteinIntake;
        private double _neutralDetergentFiberIntake;
        private double _starchIntake;
        private double _entericMethaneEscobarEtAlAlOrBeef;
        private double _entericMethaneLingenEtAlBeef;
        private double _entericMethaneEscobarEtAlLfMcBeef;
        private double _entericMethaneEllisEtAlBeef;
        private double _fecalCarbonExcretionRate;
        private double _fecalCarbonExcretion;
        private double _rateOfCarbonAddedFromBeddingMaterial;
        private double _carbonAddedFromBeddingMaterial;
        private double _carbonFromManureAndBedding;
        private double _volatileSolids;
        private double _manureMethaneEmissionRate;
        private double _manureMethaneEmission;
        private double _amountOfCarbonLostAsMethaneDuringManagement;
        private double _amountOfCarbonInStoredManure;
        private double _proteinIntake;
        private double _proteinRetainedForPregnancy;
        private double _proteinRetainedForLactation;
        private double _emptyBodyWeight;
        private double _emptyBodyGain;
        private double _retainedEnergy;
        private double _proteinRetainedForGain;
        private double _nitrogenExcretionRate;
        private double _amountOfNitrogenExcreted;
        private double _rateOfNitrogenAddedFromBeddingMaterial;
        private double _amountOfNitrogenAddedFromBedding;
        private double _manureDirectN2OnEmissionRate;
        private double _manureDirectN2OnEmission;
        private double _fractionOfNitrogenExcretedInUrine;
        private double _tanExcretionRate;
        private double _tanExcretion;
        private double _fecalNitrogenExcretionRate;
        private double _fecalNitrogenExcretion;
        private double _organicNitrogenInStoredManure;
        private double _ambientAirTemperatureAdjustmentForHousing;
        private double _adjustedAmmoniaEmissionFactorForHousing;
        private double _ammoniaEmissionRateFromHousing;
        private double _ammoniaConcentrationInHousing;
        private double _ammoniaEmissionsFromHousingSystem;
        private double _tanEnteringStorageSystem;
        private double _adjustedAmountOfTanInStoredManure;
        private double _ambientAirTemperatureAdjustmentForStorage;
        private double _adjustedAmmoniaEmissionFactorForStorage;
        private double _ammoniaLostFromStorage;
        private double _ammoniaEmissionsFromStorageSystem;
        private double _fractionOfManureVolatilized;
        private double _manureVolatilizationRate;
        private double _manureVolatilizationN2ONEmission;
        private double _manureNitrogenLeachingRate;
        private double _manureN2ONLeachingEmission;
        private double _manureIndirectN2ONEmission;
        private double _manureN2ONEmission;
        private double _tanAvailableForLandApplication;
        private double _organicNitrogenAvailableForLandApplication;
        private double _nitrogenAvailableForLandApplication;
        private double _entericMethaneRaminHuhtanenDairy;
        private double _entericMethaneMillsEtAlDairy;
        private double _entericMethaneEllisEtAlDairy;
        private double _entericMethaneNuiEtAlDairy;
        private double _acidDetergentFiberIntake;
        private double _kelvinAirTemperature;
        private double _climateFactor;
        private double _volatileSolidsProduced;
        private double _volatileSolidsLoaded;
        private double _volatileSolidsAvailable;
        private double _volatileSolidsAdjusted;
        private double _volatileSolidsConsumed;
        private double _manureCarbonNitrogenRatio;
        private double _totalVolumeOfManureAvailableForLandApplication;
        private double _ammoniaEmissionsFromLandAppliedManure;
        private double _ammoniaEmissionsFromGrazingAnimals;
        private double _proteinIntakeFromMilk;
        private double _proteinIntakeFromSolidFood;
        private double _proteinRetainedFromSolidFood;
        private double _proteinRetainedFromMilk;
        private double _proteinRetained;
        private double _energyCarbonDioxide;
        private double _netEnergyForWoolProduction;
        private double _amountOfBeefProduced;
        private double _lambEweRatio;
        private double _proteinRetainedByPiglets;
        private double _optimumTdn;
        private double _flowOfFreshVolatileSolidsEnteringDigestor;
        private double _flowOfFreshNitrogenEnteringDigestor;
        private double _flowOfFreshOrganicNitrogenEnteringDigestor;
        private double _flowOfFreshTanEnteringDigestor;
        private double _flowOfFreshCarbonEnteringDigestor;
        private double _totalNitrogenEnteringDigestor;
        private double _flowOfStoredVolatileSolidsEnteringDigestor;

        #endregion

        #region Properties

        public DateTime DateTime
        {
            get => _dateTime;
            set => SetProperty(ref _dateTime, value);
        }

        /// <summary>
        /// Equation 3.1.1-1
        /// 
        /// (kg head^-1)
        /// </summary>
        public double AnimalWeight
        {
            get => _averageWeight;
            set => SetProperty(ref _averageWeight, value);
        }

        /// <summary>
        /// Equation 3.1.1-2
        /// 
        /// (MJ day^-1 kg^-1)
        /// </summary>
        public double AdjustedMaintenanceCoefficient
        {
            get => _adjustedMaintenanceCoefficient;
            set => SetProperty(ref _adjustedMaintenanceCoefficient, value);
        }

        /// <summary>
        /// Equation 3.1.1-3
        /// Equation 3.2.1-2
        /// Equation 3.3.1-3
        /// 
        /// (MJ head^-1 day^-1)
        /// </summary>
        public double NetEnergyForMaintenance
        {
            get => _netEnergyForMaintenance;
            set => SetProperty(ref _netEnergyForMaintenance, value);
        }

        /// <summary>
        /// Equation 3.1.1-4
        /// Equation 3.1.1-3
        /// Equation 3.3.1-4
        ///
        /// (MJ head^-1 day^-1)
        /// </summary>
        public double NetEnergyForActivity
        {
            get => _netEnergyForActivity;
            set => SetProperty(ref _netEnergyForActivity, value);
        }

        /// <summary>
        /// Equation 3.1.1-5
        /// Equation 3.2.1-4
        /// Equation 3.3.1-5
        ///
        /// (MJ head^-1 day^-1)
        /// </summary>
        public double NetEnergyForLactation
        {
            get => _netEnergyForLactation;
            set => SetProperty(ref _netEnergyForLactation, value);
        }

        /// <summary>
        /// Equation 3.1.1-6
        /// Equation 3.3.1-5
        ///
        /// (MJ head^-1 day^-1)
        /// </summary>
        public double NetEnergyForPregnancy
        {
            get => _netEnergyForPregnancy;
            set => SetProperty(ref _netEnergyForPregnancy, value);
        }

        /// <summary>
        /// Equation 3.1.1-7
        /// Equation 3.3.1-8
        ///
        /// (kg head^-1 day^-1)
        /// </summary>
        public double AverageDailyGain
        {
            get => _averageDailyGain;
            set => SetProperty(ref _averageDailyGain, value);
        }

        /// <summary>
        /// Equation 3.1.1-8
        /// Equation 3.2.1-7
        /// 
        /// (MJ head^-1 day^-1)
        /// </summary>
        public double NetEnergyForGain
        {
            get => _netEnergyForGain;
            set => SetProperty(ref _netEnergyForGain, value);
        }

        /// <summary>
        /// Equation 3.1.1-9
        /// Equation 3.2.1-8
        /// Equation 3.3.1-10
        /// 
        /// (unitless)
        /// </summary>
        public double RatioOfEnergyAvailableForMaintenance
        {
            get => _ratioOfEnergyAvailableForMaintenance;
            set => SetProperty(ref _ratioOfEnergyAvailableForMaintenance, value);
        }

        /// <summary>
        /// Equation 3.1.1-10
        /// Equation 3.2.1-9
        /// Equation 3.3.1-11
        /// 
        /// (unitless)
        /// </summary>
        public double RatioOfEnergyAvailableForGain
        {
            get => _ratioOfEnergyAvailableForGain;
            set => SetProperty(ref _ratioOfEnergyAvailableForGain, value);
        }

        /// <summary>
        /// Equation 3.1.1-11
        /// Equation 3.2.1-10
        /// Equation 3.1.2-3
        /// Equation 3.3.1-12
        /// 
        /// (MJ head^-1 day^-1)
        /// </summary>
        public double GrossEnergyIntake
        {
            get => _grossEnergyIntake;
            set => SetProperty(ref  _grossEnergyIntake, value);
        }

        /// <summary>
        /// (unitless)
        /// </summary>
        public double AdditiveReductionFactor
        {
            get => _additiveReductionFactor;
            set => SetProperty(ref _additiveReductionFactor, value);
        }

        /// <summary>
        /// Equation 3.1.1-12
        /// Equation 3.2.1-11
        /// Equation 3.3.1-13
        ///
        /// (kg CH4 head^-1 day^-1)
        /// </summary>
        public double EntericMethaneEmissionRate
        {
            get => _entericMethaneEmissionRate;
            set => SetProperty(ref _entericMethaneEmissionRate, value);
        }

        /// <summary>
        /// Equation 3.1.1-13
        /// Equation 3.2.1-12
        /// Equation 3.2.2-1
        /// Equation 3.3.1-14
        /// Equation 3.4.1-1
        ///
        /// (kg CH4)
        /// </summary>
        public double EntericMethaneEmission
        {
            get => _entericMethaneEmission;
            set => SetProperty(ref _entericMethaneEmission, value);
        }

        /// <summary>
        /// Equation 12.3.1-1
        ///
        /// (kg head^-1 day^-1)
        /// </summary>
        public double DryMatterIntake
        {
            get => _dryMatterIntake;
            set => SetProperty(ref _dryMatterIntake, value);
        }

        /// <summary>
        /// Equation 12.3.1-4
        ///
        /// Amount of carbon foraged by all animals in group
        ///
        /// (kg C day^-1)
        /// </summary>
        public double TotalCarbonUptakeForGroup
        {
            get => _carbonUptake;
            set => SetProperty(ref _carbonUptake, value);
        }

        /// <summary>
        /// Crude fat/ether extract intake
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double CrudeFatIntake
        {
            get => _crudeFatIntake;
            set => SetProperty(ref _crudeFatIntake, value);
        }

        /// <summary>
        /// (kg head^-1 day^-1)
        /// </summary>
        public double CrudeProteinIntake
        {
            get => _crudeProteinIntake;
            set => SetProperty(ref _crudeProteinIntake, value);
        }

        /// <summary>
        /// (kg head^-1 day^-1)
        /// </summary>
        public double NeutralDetergentFiberIntake
        {
            get => _neutralDetergentFiberIntake;
            set => SetProperty(ref _neutralDetergentFiberIntake, value);
        }

        /// <summary>
        /// (kg head^-1 day^-1)
        /// </summary>
        public double StarchIntake
        {
            get => _starchIntake;
            set => SetProperty(ref _starchIntake, value);
        }

        /// <summary>
        /// (kg head^-1 day^-1)
        /// </summary>
        public double AcidDetergentFiberIntake
        {
            get => _acidDetergentFiberIntake;
            set => SetProperty(ref _acidDetergentFiberIntake, value);
        }

        /// <summary>
        /// Equation 3.1.1-14, 3.1.1-18
        ///
        /// (kg CH4)
        /// </summary>
        public double EntericMethaneEscobarEtAlAlOrBeef
        {
            get => _entericMethaneEscobarEtAlAlOrBeef;
            set => SetProperty(ref _entericMethaneEscobarEtAlAlOrBeef, value);
        }

        /// <summary>
        /// Equation 3.1.1-15, 3.1.1-18
        ///
        /// (kg CH4)
        /// </summary>
        public double EntericMethaneLingenEtAlBeef
        {
            get => _entericMethaneLingenEtAlBeef;
            set => SetProperty(ref _entericMethaneLingenEtAlBeef, value);
        }

        /// <summary>
        /// Equation 3.1.1-16, 3.1.1-18
        /// 
        /// (kg CH4)
        /// </summary>
        public double EntericMethaneEscobarEtAlLfMcBeef
        {
            get => _entericMethaneEscobarEtAlLfMcBeef;
            set => SetProperty(ref _entericMethaneEscobarEtAlLfMcBeef, value);
        }

        /// <summary>
        /// Equation 3.1.1-17, 3.1.1-18
        /// 
        /// (kg CH4)
        /// </summary>
        public double EntericMethaneEllisEtAlBeef
        {
            get => _entericMethaneEllisEtAlBeef;
            set => SetProperty(ref _entericMethaneEllisEtAlBeef, value);
        }

        /// <summary>
        /// Equation 4.1.1-1
        /// Equation 4.1.1-3
        /// Equation 4.1.1-2
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double FecalCarbonExcretionRate
        {
            get => _fecalCarbonExcretionRate;
            set => SetProperty(ref _fecalCarbonExcretionRate, value);
        }

        /// <summary>
        /// Equation 4.1.1-4
        ///
        /// (kg C)
        /// </summary>
        public double FecalCarbonExcretion
        {
            get => _fecalCarbonExcretion;
            set => SetProperty(ref _fecalCarbonExcretion, value);
        }

        /// <summary>
        /// Equation 4.1.1-5
        ///
        /// (kg C head^-1 day^-1)
        /// </summary>
        public double RateOfCarbonAddedFromBeddingMaterial
        {
            get => _rateOfCarbonAddedFromBeddingMaterial;
            set => SetProperty(ref _rateOfCarbonAddedFromBeddingMaterial, value);
        }

        /// <summary>
        /// Equation 4.1.1-6
        ///
        /// (kg C)
        /// </summary>
        public double CarbonAddedFromBeddingMaterial
        {
            get => _carbonAddedFromBeddingMaterial;
            set => SetProperty(ref _carbonAddedFromBeddingMaterial, value);
        }

        /// <summary>
        /// Equation 4.1.1-7
        ///
        /// (kg C)
        /// </summary>
        public double CarbonFromManureAndBedding
        {
            get => _carbonFromManureAndBedding;
            set => SetProperty(ref _carbonFromManureAndBedding, value);
        }

        /// <summary>
        /// Equation 4.1.2-1
        /// Equation 4.1.2-2
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double VolatileSolids
        {
            get => _volatileSolids;
            set => SetProperty(ref  _volatileSolids, value);
        }

        /// <summary>
        /// Equation 4.1.2-4
        ///
        /// (kg CH4 head^-1 day^-1)
        /// </summary>
        public double ManureMethaneEmissionRate
        {
            get => _manureMethaneEmissionRate;
            set => SetProperty(ref _manureMethaneEmissionRate, value);
        }

        /// <summary>
        /// Equation 4.1.2-5
        /// Equation 4.1.3-8
        /// Equation 4.1.3-9
        ///
        /// (kg CH4)
        /// </summary>
        public double ManureMethaneEmission
        {
            get => _manureMethaneEmission;
            set => SetProperty(ref _manureMethaneEmission, value);
        }

        /// <summary>
        /// Equation 4.1.3-13
        ///
        /// (kg C)
        /// </summary>
        public double AmountOfCarbonLostAsMethaneDuringManagement
        {
            get => _amountOfCarbonLostAsMethaneDuringManagement;
            set => SetProperty(ref _amountOfCarbonLostAsMethaneDuringManagement, value);
        }

        /// <summary>
        /// Equation 4.1.3-14
        ///
        /// (kg C)
        /// </summary>
        public double AmountOfCarbonInStoredManure
        {
            get => _amountOfCarbonInStoredManure;
            set => SetProperty(ref _amountOfCarbonInStoredManure, value);
        }

        /// <summary>
        /// Equation 4.2.1-1
        /// Equation 4.2.1-11
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double ProteinIntake
        {
            get => _proteinIntake;
            set => SetProperty(ref _proteinIntake, value);
        }

        /// <summary>
        /// Equation 4.2.1-2
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double ProteinRetainedForPregnancy
        {
            get => _proteinRetainedForPregnancy;
            set => SetProperty(ref _proteinRetainedForPregnancy, value);
        }

        /// <summary>
        /// Equation 4.2.1-3
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double ProteinRetainedForLactation
        {
            get => _proteinRetainedForLactation;
            set => SetProperty(ref _proteinRetainedForLactation, value);
        }

        /// <summary>
        /// Equation 4.2.1-4
        /// 
        /// (kg head^-1)
        /// </summary>
        public double EmptyBodyWeight
        {
            get => _emptyBodyWeight;
            set => SetProperty(ref _emptyBodyWeight, value);
        }

        /// <summary>
        /// Equation 4.2.1-5
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double EmptyBodyGain
        {
            get => _emptyBodyGain;
            set => SetProperty(ref _emptyBodyGain, value);
        }

        /// <summary>
        /// Equation 4.2.1-6
        /// 
        /// (Mcal head^-1 day^-1)
        /// </summary>
        public double RetainedEnergy
        {
            get => _retainedEnergy;
            set => SetProperty(ref _retainedEnergy, value);
        }

        /// <summary>
        /// Equation 4.2.1-7
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double ProteinRetainedForGain
        {
            get => _proteinRetainedForGain;
            set => SetProperty(ref _proteinRetainedForGain, value);
        }

        /// <summary>
        /// Equation 4.2.1-8
        /// Equation 4.2.1-15
        /// Equation 4.2.1-17
        /// Equation 4.2.1-23
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        public double NitrogenExcretionRate
        {
            get => _nitrogenExcretionRate;
            set => SetProperty(ref _nitrogenExcretionRate, value);
        }

        /// <summary>
        /// Equation 4.2.1-29
        /// 
        /// (kg N)
        /// </summary>
        public double AmountOfNitrogenExcreted
        {
            get => _amountOfNitrogenExcreted;
            set => SetProperty(ref _amountOfNitrogenExcreted, value);
        }

        /// <summary>
        /// Equation 4.2.1-30
        /// 
        /// (kg N head^-1 day^-1)
        /// </summary>
        public double RateOfNitrogenAddedFromBeddingMaterial
        {
            get => _rateOfNitrogenAddedFromBeddingMaterial;
            set => SetProperty(ref _rateOfNitrogenAddedFromBeddingMaterial, value);
        }

        /// <summary>
        /// Equation 4.2.1-31
        /// 
        /// (kg N)
        /// </summary>
        public double AmountOfNitrogenAddedFromBedding
        {
            get => _amountOfNitrogenAddedFromBedding;
            set => SetProperty(ref _amountOfNitrogenAddedFromBedding, value);
        }

        /// <summary>
        /// Equation 4.2.2-1
        /// 
        /// (kg N2O-N head^-1 day^-1)
        /// </summary>
        public double ManureDirectN2ONEmissionRate
        {
            get => _manureDirectN2OnEmissionRate;
            set => SetProperty(ref _manureDirectN2OnEmissionRate, value);
        }

        /// <summary>
        /// Equation 4.2.2-2
        /// 
        /// (kg N2O-N)
        /// </summary>
        public double ManureDirectN2ONEmission
        {
            get => _manureDirectN2OnEmission;
            set => SetProperty(ref _manureDirectN2OnEmission, value);
        }

        /// <summary>
        /// Equation 4.3.1-1
        /// Equation 4.3.1-2
        /// 
        /// (kg TAN (kg manure-N)^-1)
        /// </summary>
        public double FractionOfNitrogenExcretedInUrine
        {
            get => _fractionOfNitrogenExcretedInUrine;
            set => SetProperty(ref _fractionOfNitrogenExcretedInUrine, value);
        }

        /// <summary>
        /// Equation 4.3.1-3
        /// 
        /// (kg TAN head^-1 day^-1)
        /// </summary>
        public double TanExcretionRate
        {
            get => _tanExcretionRate;
            set => SetProperty(ref _tanExcretionRate, value);
        }

        /// <summary>
        /// Equation 4.3.1-4
        /// 
        /// (kg TAN)
        /// </summary>
        public double TanExcretion
        {
            get => _tanExcretion;
            set => SetProperty(ref _tanExcretion, value);
        }

        /// <summary>
        /// Equation 4.3.1-5
        /// 
        /// (kg N head^-1 day^-1)
        /// </summary>
        public double FecalNitrogenExcretionRate
        {
            get => _fecalNitrogenExcretionRate;
            set => SetProperty(ref  _fecalNitrogenExcretionRate, value);
        }

        /// <summary>
        /// Equation 4.3.1-6
        /// 
        /// (kg N day^-1)
        /// </summary>
        public double FecalNitrogenExcretion
        {
            get => _fecalNitrogenExcretion;
            set => SetProperty(ref _fecalNitrogenExcretion, value);
        }

        /// <summary>
        /// Equation 4.3.1-7
        /// 
        /// (kg N)
        /// </summary>
        public double OrganicNitrogenInStoredManure
        {
            get => _organicNitrogenInStoredManure;
            set => SetProperty(ref _organicNitrogenInStoredManure, value);
        }

        /// <summary>
        /// Equation 4.3.1-6
        /// Equation 4.3.1-11
        ///
        /// (unitless)
        /// </summary>
        public double AmbientAirTemperatureAdjustmentForHousing
        {
            get => _ambientAirTemperatureAdjustmentForHousing;
            set => SetProperty(ref _ambientAirTemperatureAdjustmentForHousing, value);
        }

        /// <summary>
        /// Equation 4.3.1-7
        /// Equation 4.3.1-12
        ///
        /// (kg NH3-N (kg TAN)^-1)
        /// </summary>
        public double AdjustedAmmoniaEmissionFactorForHousing
        {
            get => _adjustedAmmoniaEmissionFactorForHousing;
            set => SetProperty(ref _adjustedAmmoniaEmissionFactorForHousing, value);
        }

        /// <summary>
        /// Equation 4.3.1-8
        ///
        /// (kg NH3-N head^-1 day^-1)
        /// </summary>
        public double AmmoniaEmissionRateFromHousing
        {
            get => _ammoniaEmissionRateFromHousing;
            set => SetProperty(ref _ammoniaEmissionRateFromHousing, value);
        }

        /// <summary>
        /// Equation 4.3.1-9
        ///
        /// (kg NH3-N)
        /// </summary>
        public double AmmoniaConcentrationInHousing
        {
            get => _ammoniaConcentrationInHousing;
            set => SetProperty(ref _ammoniaConcentrationInHousing, value);
        }

        /// <summary>
        /// Equation 4.3.1-10
        ///
        /// (kg NH3)
        /// </summary>
        public double AmmoniaEmissionsFromHousingSystem
        {
            get => _ammoniaEmissionsFromHousingSystem;
            set => SetProperty(ref _ammoniaEmissionsFromHousingSystem, value);
        }

        /// <summary>
        /// Equation 4.3.2-1
        ///
        /// (kg TAN)
        /// </summary>
        public double TanEnteringStorageSystem
        {
            get => _tanEnteringStorageSystem;
            set => SetProperty(ref _tanEnteringStorageSystem, value);
        }

        /// <summary>
        /// Equation 4.3.2-2
        ///
        /// (kg TAN)
        /// </summary>
        public double AdjustedAmountOfTanInStoredManure
        {
            get => _adjustedAmountOfTanInStoredManure;
            set => SetProperty(ref _adjustedAmountOfTanInStoredManure, value);
        }

        /// <summary>
        /// Equation 4.3.2-3
        /// Equation 4.3.2-4
        /// Equation 4.3.2-5
        ///
        /// (degrees C)
        /// </summary>
        public double AmbientAirTemperatureAdjustmentForStorage
        {
            get => _ambientAirTemperatureAdjustmentForStorage;
            set => SetProperty(ref _ambientAirTemperatureAdjustmentForStorage, value);
        }

        /// <summary>
        /// Equation 4.3.2-6
        /// 
        /// kg NH3-N (kg TAN)^-1
        /// </summary>
        public double AdjustedAmmoniaEmissionFactorForStorage
        {
            get => _adjustedAmmoniaEmissionFactorForStorage;
            set => SetProperty(ref _adjustedAmmoniaEmissionFactorForStorage, value);
        }

        /// <summary>
        /// Equation 4.3.2-7
        /// 
        /// (kg NH3-N)
        /// </summary>
        public double AmmoniaLostFromStorage
        {
            get => _ammoniaLostFromStorage;
            set => SetProperty(ref _ammoniaLostFromStorage, value);
        }

        /// <summary>
        /// Equation 4.3.2-8
        /// 
        /// (kg NH3)
        /// </summary>
        public double AmmoniaEmissionsFromStorageSystem
        {
            get => _ammoniaEmissionsFromStorageSystem;
            set => SetProperty(ref _ammoniaEmissionsFromStorageSystem, value);
        }

        /// <summary>
        /// Equation 4.3.3-1
        ///
        /// (fraction)
        /// </summary>
        public double FractionOfManureVolatilized
        {
            get => _fractionOfManureVolatilized;
            set => SetProperty(ref _fractionOfManureVolatilized, value);
        }

        /// <summary>
        /// Equation 4.3.4-2
        ///
        /// (kg N2O-N head^-1 day^-1)
        /// </summary>
        public double ManureVolatilizationRate
        {
            get => _manureVolatilizationRate;
            set => SetProperty(ref _manureVolatilizationRate, value);
        }

        /// <summary>
        /// Equation 4.3.3-4
        ///
        /// (kg N2O-N)
        /// </summary>
        public double ManureVolatilizationN2ONEmission
        {
            get => _manureVolatilizationN2ONEmission;
            set => SetProperty(ref _manureVolatilizationN2ONEmission, value);
        }

        /// <summary>
        /// Equation 4.3.4-1
        ///
        /// (kg N2O-N head^-1 day^-1)
        /// </summary>
        public double ManureNitrogenLeachingRate
        {
            get => _manureNitrogenLeachingRate;
            set => SetProperty(ref _manureNitrogenLeachingRate, value);
        }

        /// <summary>
        /// Equation 4.3.4-2
        ///
        /// (kg N2O-N)
        /// </summary>
        public double ManureN2ONLeachingEmission
        {
            get => _manureN2ONLeachingEmission;
            set => SetProperty(ref _manureN2ONLeachingEmission, value);
        }

        /// <summary>
        /// Equation 4.3.5-1
        ///
        /// (kg N2O-N)
        /// </summary>
        public double ManureIndirectN2ONEmission
        {
            get => _manureIndirectN2ONEmission;
            set => SetProperty(ref _manureIndirectN2ONEmission, value);
        }

        /// <summary>
        /// Equation 4.3.7-1
        ///
        /// (kg N2O-N)
        /// </summary>
        public double ManureN2ONEmission
        {
            get => _manureN2ONEmission;
            set => SetProperty(ref _manureN2ONEmission, value);
        }

        /// <summary>
        /// Equation 4.5.2-1
        ///
        /// (kg TAN)
        /// </summary>
        public double TanAvailableForLandApplication
        {
            get => _tanAvailableForLandApplication;
            set => SetProperty(ref _tanAvailableForLandApplication, value);
        }

        /// <summary>
        /// Equation 4.5.2-3
        ///
        /// (kg N)
        /// </summary>
        public double OrganicNitrogenAvailableForLandApplication
        {
            get => _organicNitrogenAvailableForLandApplication;
            set => SetProperty(ref _organicNitrogenAvailableForLandApplication, value);
        }

        /// <summary>
        /// Equation 4.5.2-5
        /// Equation 4.5.2-6
        ///
        /// (kg N)
        /// </summary>
        public double NitrogenAvailableForLandApplication
        {
            get => _nitrogenAvailableForLandApplication;
            set => SetProperty(ref _nitrogenAvailableForLandApplication, value);
        }

        /// <summary>
        /// Equation 3.2.1-13
        ///
        /// (kg CH4)
        /// </summary>
        public double EntericMethaneRaminHuhtanenDairy
        {
            get => _entericMethaneRaminHuhtanenDairy;
            set => SetProperty(ref  _entericMethaneRaminHuhtanenDairy, value);
        }

        /// <summary>
        /// Equation 3.2.1-14
        ///
        /// (kg CH4)
        /// </summary>
        public double EntericMethaneMillsEtAlDairy
        {
            get => _entericMethaneMillsEtAlDairy;
            set => SetProperty(ref _entericMethaneMillsEtAlDairy, value);
        }

        /// <summary>
        /// Equation 3.2.1-15
        ///
        /// (kg CH4)
        /// </summary>
        public double EntericMethaneEllisEtAlDairy
        {
            get => _entericMethaneEllisEtAlDairy;
            set => SetProperty(ref _entericMethaneEllisEtAlDairy, value);
        }

        /// <summary>
        /// Equation 3.2.1-16
        ///
        /// (kg CH4)
        /// </summary>
        public double EntericMethaneNuiEtAlDairy
        {
            get => _entericMethaneNuiEtAlDairy;
            set => SetProperty(ref _entericMethaneNuiEtAlDairy, value);
        }

        /// <summary>
        /// Equation 4.1.3-8
        ///
        /// (degrees Kelvin)
        /// </summary>
        public double KelvinAirTemperature
        {
            get => _kelvinAirTemperature;
            set => SetProperty(ref _kelvinAirTemperature, value);
        }

        /// <summary>
        /// Equation 4.1.3-9
        ///
        /// (unitless)
        /// </summary>
        public double ClimateFactor
        {
            get => _climateFactor;
            set => SetProperty(ref _climateFactor, value);
        }

        /// <summary>
        /// Equation 4.1.3-3
        ///
        /// (kg day^-1)
        /// </summary>
        public double VolatileSolidsProduced
        {
            get => _volatileSolidsProduced;
            set => SetProperty(ref _volatileSolidsProduced, value);
        }

        /// <summary>
        /// Equation 4.1.3-4
        ///
        /// (kg day^-1)
        /// </summary>
        public double VolatileSolidsLoaded
        {
            get => _volatileSolidsLoaded;
            set => SetProperty(ref _volatileSolidsLoaded, value);
        }

        /// <summary>
        /// Equation 4.1.3-5
        ///
        /// (kg day^-1)
        /// </summary>
        public double VolatileSolidsAvailable
        {
            get => _volatileSolidsAvailable;
            set => SetProperty(ref _volatileSolidsAvailable, value);
        }

        /// <summary>
        /// Equation 4.1.3-7
        ///
        /// (kg day^-1)
        /// </summary>
        public double VolatileSolidsConsumed
        {
            get => _volatileSolidsConsumed;
            set => SetProperty(ref _volatileSolidsConsumed, value);
        }

        /// <summary>
        /// Equation 4.5.3-1
        ///
        /// (fraction)
        /// </summary>
        public double ManureCarbonNitrogenRatio
        {
            get => _manureCarbonNitrogenRatio;
            set => SetProperty(ref _manureCarbonNitrogenRatio, value);
        }

        /// <summary>
        /// Equation 4.5.3-2
        ///
        /// (1000 kg wet weight for solid manure, 1000 L for liquid manure)
        /// </summary>
        public double TotalVolumeOfManureAvailableForLandApplication
        {
            get => _totalVolumeOfManureAvailableForLandApplication;
            set => SetProperty(ref _totalVolumeOfManureAvailableForLandApplication, value);
        }

        /// <summary>
        /// Equation 4.6.1-4
        ///
        /// (kg NH3)
        /// </summary>
        public double AmmoniaEmissionsFromLandAppliedManure
        {
            get => _ammoniaEmissionsFromLandAppliedManure;
            set => SetProperty(ref _ammoniaEmissionsFromLandAppliedManure, value);
        }

        /// <summary>
        /// Equation 5.2.5-5
        ///
        /// (kg NH3)
        /// </summary>
        public double AmmoniaEmissionsFromGrazingAnimals
        {
            get => _ammoniaEmissionsFromGrazingAnimals;
            set => SetProperty(ref _ammoniaEmissionsFromGrazingAnimals, value);
        }

        /// <summary>
        /// Equation 4.2.1-9
        ///
        /// (kg head^-1 day^-1)
        /// </summary>
        public double ProteinIntakeFromSolidFood
        {
            get => _proteinIntakeFromSolidFood;
            set => SetProperty(ref _proteinIntakeFromSolidFood, value);
        }

        /// <summary>
        /// Equation 4.2.1-10
        ///
        /// (kg head^-1 day^-1)
        /// </summary>
        public double ProteinIntakeFromMilk
        {
            get => _proteinIntakeFromMilk;
            set => SetProperty(ref _proteinIntakeFromMilk, value);
        }

        /// <summary>
        /// Equation 4.2.1-12
        ///
        /// (kg head^-1 day^-1)
        /// </summary>
        public double ProteinRetainedFromSolidFood
        {
            get => _proteinRetainedFromSolidFood;
            set => SetProperty(ref _proteinRetainedFromSolidFood, value);
        }

        /// <summary>
        /// Equation 4.2.1-13
        ///
        /// (kg head^-1 day^-1)
        /// </summary>
        public double ProteinRetainedFromMilk
        {
            get => _proteinRetainedFromMilk;
            set => SetProperty(ref _proteinRetainedFromMilk, value);
        }

        /// <summary>
        /// Equation 4.2.1-14
        /// Equation 4.2.1-20
        /// Equation 4.2.1-23
        /// Equation 4.2.1-20
        /// Equation 4.2.1-23
        ///
        /// (kg head^-1 day^-1)
        /// </summary>
        public double ProteinRetained
        {
            get => _proteinRetained;
            set => SetProperty(ref _proteinRetained, value);
        }

        public double EnergyCarbonDioxide
        {
            get => _energyCarbonDioxide;
            set => SetProperty(ref _energyCarbonDioxide, value);
        }

        /// <summary>
        /// Equation 3.4.1-7
        ///
        /// (MJ head^-1 day^-1)
        /// </summary>
        public double NetEnergyForWoolProduction
        {
            get => _netEnergyForWoolProduction;
            set => SetProperty(ref _netEnergyForWoolProduction, value);
        }

        /// <summary>
        /// (kg)
        /// </summary>
        public double AmountOfBeefProduced
        {
            get => _amountOfBeefProduced;
            set => SetProperty(ref _amountOfBeefProduced, value);
        }

        /// <summary>
        /// Equation 3.3.1-2
        /// 
        /// (unitless)
        /// </summary>
        public double LambEweRatio
        {
            get => _lambEweRatio;
            set => SetProperty(ref _lambEweRatio, value);
        }

        /// <summary>
        /// Equation 4.1.2-2
        ///
        /// (unitless)
        /// </summary>
        public double VolatileSolidsAdjusted
        {
            get => _volatileSolidsAdjusted;
            set => SetProperty(ref _volatileSolidsAdjusted, value);
        }

        /// <summary>
        /// Equation 4.2.1-22
        ///
        /// (kg head^-1 day^-1)
        /// </summary>
        public double ProteinRetainedByPiglets
        {
            get => _proteinRetainedByPiglets;
            set => SetProperty(ref _proteinRetainedByPiglets, value);
        }

        /// <summary>
        /// Equation 12.3.1-5
        /// </summary>
        public double DryMatterIntakeMax 
        { 
            get => _dryMatterIntakeMax; 
            set => SetProperty(ref _dryMatterIntakeMax, value); 
        }

        /// <summary>
        /// This is the TDN value that is needed so that the DMI does not go over the DMI_max (no equation)
        /// 
        /// (% DM)
        /// </summary>
        public double OptimumTdn 
        { 
            get => _optimumTdn; 
            set => _optimumTdn = value; 
        }

        /// <summary>
        /// Equation 4.8.1-1
        /// 
        /// (kg day^-1)
        /// </summary>
        public double FlowOfFreshVolatileSolidsEnteringDigestor 
        { 
            get => _flowOfFreshVolatileSolidsEnteringDigestor; 
            set => SetProperty(ref _flowOfFreshVolatileSolidsEnteringDigestor, value); 
        }

        /// <summary>
        /// Equation 4.8.1-2
        /// 
        /// (kg day^-1)
        /// </summary>
        public double FlowOfFreshNitrogenEnteringDigestor 
        { 
            get => _flowOfFreshNitrogenEnteringDigestor; 
            set => SetProperty(ref _flowOfFreshNitrogenEnteringDigestor, value); 
        }

        /// <summary>
        /// Equation 4.8.1-3
        /// Equation 4.8.1-4
        /// </summary>
        public double FlowOfFreshOrganicNitrogenEnteringDigestor 
        { 
            get => _flowOfFreshOrganicNitrogenEnteringDigestor; 
            set => SetProperty(ref _flowOfFreshOrganicNitrogenEnteringDigestor, value); 
        }

        /// <summary>
        /// Equation 4.8.1-5
        /// </summary>
        public double FlowOfFreshTanEnteringDigestor 
        { 
            get => _flowOfFreshTanEnteringDigestor; 
            set => SetProperty(ref _flowOfFreshTanEnteringDigestor, value); 
        }

        /// <summary>
        /// Equation 4.8.1-6
        /// </summary>
        public double FlowOfFreshCarbonEnteringDigestor 
        { 
            get => _flowOfFreshCarbonEnteringDigestor; 
            set => SetProperty(ref _flowOfFreshCarbonEnteringDigestor, value); 
        }

        /// <summary>
        /// Equation 4.8.1-16
        /// </summary>
        public double TotalNitrogenEnteringDigestor 
        { 
            get => _totalNitrogenEnteringDigestor; 
            set => SetProperty(ref _totalNitrogenEnteringDigestor, value); 
        }

        /// <summary>
        /// Equation 4.8.1-13
        /// </summary>
        public double FlowOfStoredVolatileSolidsEnteringDigestor 
        { 
            get => _flowOfStoredVolatileSolidsEnteringDigestor; 
            set => SetProperty(ref _flowOfStoredVolatileSolidsEnteringDigestor, value); 
        }

        /// <summary>
        /// Equation 12.3.1-2
        ///
        /// (kg group^-1 day^-1)
        /// </summary>
        public double DryMatterIntakeForGroup
        {
            get => _dryMatterIntakeForGroup;
            set => SetProperty(ref _dryMatterIntakeForGroup, value);
        }

        #endregion
    }
}