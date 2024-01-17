using Prism.Mvvm;
using System;

namespace H.Core.Models
{
    public class FieldResultsColumnsVisibility : ColumnVisibilityBase
    {
        #region Fields

        private bool _name;
        private bool _timePeriod;
        private bool _year;
        private bool _cropType;
        private bool _climateParameter;
        private bool _tillageFactor;
        private bool _managementFactor;
        private bool _percentageOfProductReturned;
        private bool _percentageOfStrawReturned;
        private bool _percentageOfRootsReturned;
        private bool _plantCarbonInAgriculturalProduct;
        private bool _carbonInputFromProduct;
        private bool _carbonInputFromStraw;
        private bool _carbonInputFromRoots;
        private bool _carbonInputFromExtraRoots;
        private bool _manureCarbonInput;
        private bool _digestateCarbonInput;
        private bool _aboveGroundCarbonInput;
        private bool _belowGroundCarbonInput;
        private bool _youngPoolSoilCarbonAboveGround;
        private bool _youngPoolSoilCarbonBelowGround;
        private bool _youngPoolManureCarbon;
        private bool _averageSoilCarbonAcrossAllFieldsInFarm;
        private bool _soilCarbon;
        private bool _oldPoolSoilCarbon;
        private bool _ChangeInCarbon;
        private bool _activePoolCarbon;
        private bool _passivePoolCarbon;
        private bool _slowPoolCarbon;
        private bool _syntheticInputsBeforeAdjustment;
        private bool _aboveGroundNitrogenResidueForCrop;
        private bool _belowGroundResidueNitrogenForCrop;
        private bool _aboveGroundResiduePool_AGresidueN;
        private bool _belowGroundResiduePool_BGredidueN;
        private bool _cropResiduesBeforeAdjustment;
        private bool _organicNitrogenResiduesBeforeAdjustment;
        private bool _cropNitrogenDemand;
        private bool _nMinFromDecompositionOfOldCarbon;
        private bool _oldPoolNitrogenRequirement;
        private bool _microbialPoolAfterCloseOfBudget;
        private bool _microbialPoolAfterOldPoolDemandAdjustment;
        private bool _microbialPoolAfterCropDemandAdjustment;
        private bool _mineralNitrogenPoolNMineralN;
        private bool _microbeNitrogenPoolNMicrobeN;
        private bool _microbeDeath;
        private bool _mineralNitrogenBalance;
        private bool _microbialNitrogenBalance;
        private bool _totalNitrogenInputs;
        private bool _totalNitrogenEmissions;
        private bool _totalUptake;
        private bool _totalNitrogenOutputs;
        private bool _differenceBetweenInputsAndOutputs;
        private bool _sumOfMineralAndMicrobialPools;
        private bool _overflow;
        private bool _ratio;
        private bool _totalDirectNitrousOxideForArea;
        private bool _directNitrousOxideEmissionsFromSyntheticNitrogenForArea;
        private bool _directNitrousOxideEmissionsFromCropResiduesForArea;
        private bool _directNitrousOxideEmissionsFromMineralizedNitrogenForArea;
        private bool _directNitrousOxideEmissionsFromOrganicNitrogenForArea;
        private bool _totalIndirectNitrousOxideForArea;
        private bool _indirectNitrousOxideEmissionsFromSyntheticNitrogenForArea;
        private bool _indirectNitrousOxideEmissionsFromCropResiduesForArea;
        private bool _indirectNitrousOxideEmissionsFromMineralizedNitrogenForArea;
        private bool _indirectNitrousOxideEmissionsFromOrganicNitrogenForArea;
        private bool _indirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForArea;
        private bool _totalNitricOxideForArea;
        private bool _totalNitrateLeachingForArea;
        private bool _totalAmmoniaForArea;
        private bool _denitrificationForArea;

        private bool _totalDirectNitrousOxidePerHectare;
        private bool _totalDirectNitrousOxidePerHectareExcludingRemainingAmounts;
        private bool _totalIndirectNitrousOxidePerHectare;
        private bool _totalIndirectNitrousOxidePerHectareExcludingRemainingAmounts;

        private bool _directNitrousOxideEmissionsFromOrganicNitrogenForAreaExcludingRemainingAmounts;
        private bool _totalIndirectNitrousOxideForAreaExcludingRemainingAmounts;

        private bool _indirectNitrousOxideEmissionsFromOrganicNitrogenForAreaExcludingRemaining;
        private bool _indirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogenForArea;

        #endregion

        #region Constructors
        public FieldResultsColumnsVisibility()
        {
            this.DefaultVisibility();
        }

        #endregion

        #region Public Methods

        public void DefaultVisibility()
        {
            // Remove current selections
            base.SetAllColumnsInvisible();

            this.Name = true;
            this.Year = true;
            this.CropType = true;
            this.AboveGroundCarbonInput = true;
            this.BelowGrounCarbonInput = true;
            this.ManureCarbonInput = true;
            this.DigestateCarbonInput = true;
            this.SoilCarbon = true;
            this.ChangeInCarbon = true;

            this.TotalDirectNitrousOxidePerHectare = true;
            this.TotalDirectNitrousOxidePerHectareExcludingRemainingAmounts = true;

            this.TotalIndirectNitrousOxidePerHectare = true;
            this.TotalIndirectNitrousOxidePerHectareExcludingRemainingAmounts = true;

            this.DirectNitrousOxideEmissionsFromOrganicNitrogenForAreaExcludingRemainingAmounts = true;
            this.TotalIndirectNitrousOxideForAreaExcludingRemainingAmounts = true;
            this.IndirectNitrousOxideEmissionsFromOrganicNitrogenForAreaExcludingRemaining = true;

            this.IndirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogenForArea = true;
        }

        #endregion

        #region Properties

        public bool Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        public bool TimePeriod
        {
            get { return _timePeriod; }
            set { SetProperty(ref _timePeriod, value); }
        }
        public bool Year
        {
            get { return _year; }
            set { SetProperty(ref _year, value); }
        }
        public bool CropType
        {
            get { return _cropType; }
            set { SetProperty(ref _cropType, value); }
        }
        public bool ClimateParameter
        {
            get { return _climateParameter; }
            set { SetProperty(ref _climateParameter, value); }
        }
        public bool TillageFactor
        {
            get { return _tillageFactor; }
            set { SetProperty(ref _tillageFactor, value); }
        }
        public bool ManagementFactor
        {
            get { return _managementFactor; }
            set { SetProperty(ref _managementFactor, value); }
        }
        public bool PlantCarbonInAgriculturalProduct
        {
            get { return _plantCarbonInAgriculturalProduct; }
            set { SetProperty(ref _plantCarbonInAgriculturalProduct, value); }
        }
        public bool CarbonInputFromProduct
        {
            get { return _carbonInputFromProduct; }
            set { SetProperty(ref _carbonInputFromProduct, value); }
        }
        public bool CarbonInputFromRoots
        {
            get { return _carbonInputFromRoots; }
            set { SetProperty(ref _carbonInputFromRoots, value); }
        }
        public bool CarbonInputFromStraw
        {
            get { return _carbonInputFromStraw; }
            set { SetProperty(ref _carbonInputFromStraw, value); }
        }
        public bool CarbonInputFromExtraRoots
        {
            get { return _carbonInputFromExtraRoots; }
            set { SetProperty(ref _carbonInputFromExtraRoots, value); }
        }
        public bool ManureCarbonInput
        {
            get { return _manureCarbonInput; }
            set { SetProperty(ref _manureCarbonInput, value); }
        }
        public bool AboveGroundCarbonInput
        {
            get { return _aboveGroundCarbonInput; }
            set { SetProperty(ref _aboveGroundCarbonInput, value); }
        }
        public bool BelowGrounCarbonInput
        {
            get { return _belowGroundCarbonInput; }
            set { SetProperty(ref _belowGroundCarbonInput, value); }
        }
        public bool YoungPoolSoilCarbonAboveGround
        {
            get { return _youngPoolSoilCarbonAboveGround; }
            set { SetProperty(ref _youngPoolSoilCarbonAboveGround, value); }
        }
        public bool YoungPoolSoilCarbonBelowGround
        {
            get { return _youngPoolSoilCarbonBelowGround; }
            set { SetProperty(ref _youngPoolSoilCarbonBelowGround, value); }
        }
        public bool YoungPoolManureCarbon
        {
            get { return _youngPoolManureCarbon; }
            set { SetProperty(ref _youngPoolManureCarbon, value); }
        }
        public bool AverageSoilCarbonAcrossAllFieldsInFarm
        {
            get { return _averageSoilCarbonAcrossAllFieldsInFarm; }
            set { SetProperty(ref _averageSoilCarbonAcrossAllFieldsInFarm, value); }
        }
        public bool SoilCarbon
        {
            get { return _soilCarbon; }
            set { SetProperty(ref _soilCarbon, value); }
        }
        public bool OldPoolSoilCarbon
        {
            get { return _oldPoolSoilCarbon; }
            set { SetProperty(ref _oldPoolSoilCarbon, value); }
        }
        public bool ChangeInCarbon
        {
            get { return _ChangeInCarbon; }
            set { SetProperty(ref _ChangeInCarbon, value); }
        }

        public bool SyntheticInputsBeforeAdjustment
        {
            get { return _syntheticInputsBeforeAdjustment; }
            set { SetProperty(ref _syntheticInputsBeforeAdjustment, value); }
        }
        public bool AboveGroundNitrogenResidueForCrop
        {
            get { return _aboveGroundNitrogenResidueForCrop; }
            set { SetProperty(ref _aboveGroundNitrogenResidueForCrop, value); }
        }
        public bool BelowGroundResidueNitrogenForCrop
        {
            get { return _belowGroundResidueNitrogenForCrop; }
            set { SetProperty(ref _belowGroundResidueNitrogenForCrop, value); }
        }
        public bool AboveGroundResiduePool_AGresidueN
        {
            get { return _aboveGroundResiduePool_AGresidueN; }
            set { SetProperty(ref _aboveGroundResiduePool_AGresidueN, value); }
        }
        public bool BelowGroundResiduePool_BGresidueN
        {
            get { return _belowGroundResiduePool_BGredidueN; }
            set { SetProperty(ref _belowGroundResiduePool_BGredidueN, value); }
        }
        public bool CropResiduesBeforeAdjustment
        {
            get { return _cropResiduesBeforeAdjustment; }
            set { SetProperty(ref _cropResiduesBeforeAdjustment, value); }
        }
        public bool OrganicNitrogenResiduesBeforeAdjustment
        {
            get { return _organicNitrogenResiduesBeforeAdjustment; }
            set { SetProperty(ref _organicNitrogenResiduesBeforeAdjustment, value); }
        }
        public bool CropNitrogenDemand
        {
            get { return _cropNitrogenDemand; }
            set { SetProperty(ref _cropNitrogenDemand, value); }
        }
        public bool NMinFromDecompositionOfOldCarbon
        {
            get { return _nMinFromDecompositionOfOldCarbon; }
            set { SetProperty(ref _nMinFromDecompositionOfOldCarbon, value); }
        }
        public bool OldPoolNitrogenRequirement
        {
            get { return _oldPoolNitrogenRequirement; }
            set { SetProperty(ref _oldPoolNitrogenRequirement, value); }
        }
        public bool MicrobialPoolAfterCloseOfBudget
        {
            get { return _microbialPoolAfterCloseOfBudget; }
            set { SetProperty(ref _microbialPoolAfterCloseOfBudget, value); }
        }
        public bool MicrobialPoolAfterOldPoolDemandAdjustment
        {
            get { return _microbialPoolAfterOldPoolDemandAdjustment; }
            set { SetProperty(ref _microbialPoolAfterOldPoolDemandAdjustment, value); }
        }
        public bool MicrobialPoolAfterCropDemandAdjustment
        {
            get { return _microbialPoolAfterCropDemandAdjustment; }
            set { SetProperty(ref _microbialPoolAfterCropDemandAdjustment, value); }
        }
        public bool MineralNitrogenPoolNMineralN
        {
            get { return _mineralNitrogenPoolNMineralN; }
            set { SetProperty(ref _mineralNitrogenPoolNMineralN, value); }
        }
        public bool MicrobeNitrogenPoolNMicrobeN
        {
            get { return _microbeNitrogenPoolNMicrobeN; }
            set { SetProperty(ref _microbeNitrogenPoolNMicrobeN, value); }
        }
        public bool MicrobeDeath
        {
            get { return _microbeDeath; }
            set { SetProperty(ref _microbeDeath, value); }
        }
        public bool MineralNitrogenBalance
        {
            get { return _mineralNitrogenBalance; }
            set { SetProperty(ref _mineralNitrogenBalance, value); }
        }
        public bool MicrobialNitrogenBalance
        {
            get { return _microbialNitrogenBalance; }
            set { SetProperty(ref _microbialNitrogenBalance, value); }
        }
        public bool TotalNitrogenInputs
        {
            get { return _totalNitrogenInputs; }
            set { SetProperty(ref _totalNitrogenInputs, value); }
        }
        public bool TotalNitrogenEmissions
        {
            get { return _totalNitrogenEmissions; }
            set { SetProperty(ref _totalNitrogenEmissions, value); }
        }
        public bool TotalNitrogenOutputs
        {
            get { return _totalNitrogenOutputs; }
            set { SetProperty(ref _totalNitrogenOutputs, value); }
        }
        public bool TotalUptake
        {
            get { return _totalUptake; }
            set { SetProperty(ref _totalUptake, value); }
        }
        public bool DifferenceBetweenInputsAndOutputs
        {
            get { return _differenceBetweenInputsAndOutputs; }
            set { SetProperty(ref _differenceBetweenInputsAndOutputs, value); }
        }
        public bool SumOfMineralAndMicrobialPools
        {
            get { return _sumOfMineralAndMicrobialPools; }
            set { SetProperty(ref _sumOfMineralAndMicrobialPools, value); }
        }
        public bool Overflow
        {
            get { return _overflow; }
            set { SetProperty(ref _overflow, value); }
        }
        public bool Ratio
        {
            get { return _ratio; }
            set { SetProperty(ref _ratio, value); }
        }
        public bool TotalDirectNitrousOxideForArea
        {
            get { return _totalDirectNitrousOxideForArea; }
            set { SetProperty(ref _totalDirectNitrousOxideForArea, value); }
        }
        public bool DirectNitrousOxideEmissionsFromSyntheticNitrogenForArea
        {
            get { return _directNitrousOxideEmissionsFromSyntheticNitrogenForArea; }
            set { SetProperty(ref _directNitrousOxideEmissionsFromSyntheticNitrogenForArea, value); }
        }
        public bool DirectNitrousOxideEmissionsCropResiduesNitrogenForArea
        {
            get { return _directNitrousOxideEmissionsFromCropResiduesForArea; }
            set { SetProperty(ref _directNitrousOxideEmissionsFromCropResiduesForArea, value); }
        }
        public bool DirectNitrousOxideEmissionsFromMineralizedNitrogenForArea
        {
            get { return _directNitrousOxideEmissionsFromMineralizedNitrogenForArea; }
            set { SetProperty(ref _directNitrousOxideEmissionsFromMineralizedNitrogenForArea, value); }
        }
        public bool DirectNitrousOxideEmissionsFromOrganicNitrogenForArea
        {
            get { return _directNitrousOxideEmissionsFromOrganicNitrogenForArea; }
            set { SetProperty(ref _directNitrousOxideEmissionsFromOrganicNitrogenForArea, value); }
        }

        public bool DirectNitrousOxideEmissionsFromOrganicNitrogenForAreaExcludingRemainingAmounts
        {
            get { return _directNitrousOxideEmissionsFromOrganicNitrogenForAreaExcludingRemainingAmounts; }
            set { SetProperty(ref _directNitrousOxideEmissionsFromOrganicNitrogenForAreaExcludingRemainingAmounts, value); }
        }
        public bool IndirectNitrousOxideEmissionsFromSyntheticNitrogenForArea
        {
            get { return _indirectNitrousOxideEmissionsFromSyntheticNitrogenForArea; }
            set { SetProperty(ref _indirectNitrousOxideEmissionsFromSyntheticNitrogenForArea, value); }
        }
        public bool IndirectNitrousOxideEmissionsCropResiduesNitrogenForArea
        {
            get { return _indirectNitrousOxideEmissionsFromCropResiduesForArea; }
            set { SetProperty(ref _indirectNitrousOxideEmissionsFromCropResiduesForArea, value); }
        }
        public bool IndirectNitrousOxideEmissionsFromMineralizedNitrogenForArea
        {
            get { return _indirectNitrousOxideEmissionsFromMineralizedNitrogenForArea; }
            set { SetProperty(ref _indirectNitrousOxideEmissionsFromMineralizedNitrogenForArea, value); }
        }
        public bool IndirectNitrousOxideEmissionsFromOrganicNitrogenForArea
        {
            get { return _indirectNitrousOxideEmissionsFromOrganicNitrogenForArea; }
            set { SetProperty(ref _indirectNitrousOxideEmissionsFromOrganicNitrogenForArea, value); }
        }

        public bool IndirectNitrousOxideEmissionsFromOrganicNitrogenForAreaExcludingRemaining
        {
            get { return _indirectNitrousOxideEmissionsFromOrganicNitrogenForAreaExcludingRemaining; }
            set { SetProperty(ref _indirectNitrousOxideEmissionsFromOrganicNitrogenForAreaExcludingRemaining, value); }
        }

        public bool IndirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForArea
        {
            get { return _indirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForArea; }
            set { SetProperty(ref _indirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForArea, value); }
        }

        public bool IndirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogenForArea
        {
            get { return _indirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogenForArea; }
            set { SetProperty(ref _indirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogenForArea, value); }
        }

        public bool TotalIndirectNitrousOxideForArea
        {
            get { return _totalIndirectNitrousOxideForArea; }
            set { SetProperty(ref _totalIndirectNitrousOxideForArea, value); }
        }

        public bool TotalIndirectNitrousOxideForAreaExcludingRemainingAmounts
        {
            get { return _totalIndirectNitrousOxideForAreaExcludingRemainingAmounts; }
            set { SetProperty(ref _totalIndirectNitrousOxideForAreaExcludingRemainingAmounts, value); }
        }
        public bool TotalNitricOxideForArea
        {
            get { return _totalNitricOxideForArea; }
            set { SetProperty(ref _totalNitricOxideForArea, value); }
        }
        public bool TotalNitrateLeachingForArea
        {
            get { return _totalNitrateLeachingForArea; }
            set { SetProperty(ref _totalNitrateLeachingForArea, value); }
        }
        public bool TotalAmmoniaForArea
        {
            get { return _totalAmmoniaForArea; }
            set { SetProperty(ref _totalAmmoniaForArea, value); }
        }
        public bool DenitrificationArea
        {
            get { return _denitrificationForArea; }
            set { SetProperty(ref _denitrificationForArea, value); }
        }

        public bool PercentageOfProductReturned
        {
            get => _percentageOfProductReturned;
            set => SetProperty(ref _percentageOfProductReturned, value);
        }

        public bool PercentageOfStrawReturned
        {
            get => _percentageOfStrawReturned;
            set => SetProperty(ref _percentageOfStrawReturned, value);
        }

        public bool PercentageOfRootsReturned
        {
            get => _percentageOfRootsReturned;
            set => SetProperty(ref _percentageOfRootsReturned, value);
        }

        public bool TotalDirectNitrousOxidePerHectare
        {
            get => _totalDirectNitrousOxidePerHectare;
            set => SetProperty(ref _totalDirectNitrousOxidePerHectare, value);
        }

        public bool TotalDirectNitrousOxidePerHectareExcludingRemainingAmounts
        {
            get => _totalDirectNitrousOxidePerHectareExcludingRemainingAmounts;
            set => SetProperty(ref _totalDirectNitrousOxidePerHectareExcludingRemainingAmounts, value);
        }

        public bool TotalIndirectNitrousOxidePerHectare
        {
            get => _totalIndirectNitrousOxidePerHectare;
            set => SetProperty(ref _totalIndirectNitrousOxidePerHectare, value);
        }

        public bool TotalIndirectNitrousOxidePerHectareExcludingRemainingAmounts
        {
            get => _totalIndirectNitrousOxidePerHectareExcludingRemainingAmounts;
            set => SetProperty(ref _totalIndirectNitrousOxidePerHectareExcludingRemainingAmounts, value);
        }

        public bool DigestateCarbonInput
        {
            get => _digestateCarbonInput;
            set => SetProperty(ref _digestateCarbonInput, value);
        }

        public bool ActivePoolCarbon
        {
            get => _activePoolCarbon;
            set => SetProperty(ref _activePoolCarbon, value);
        }

        public bool PassivePoolCarbon
        {
            get => _passivePoolCarbon;
            set => SetProperty(ref _passivePoolCarbon, value);
        }

        public bool SlowPoolCarbon
        {
            get => _slowPoolCarbon;
            set => SetProperty(ref _slowPoolCarbon, value);
        }

        #endregion

    }
}
