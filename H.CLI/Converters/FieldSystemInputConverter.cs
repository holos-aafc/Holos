using H.CLI.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using H.CLI.FileAndDirectoryAccessors;
using H.CLI.TemporaryComponentStorage;
using H.Core;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Fertilizer;

namespace H.CLI.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class FieldSystemInputConverter : IConverter
    {
        #region Fields
        public List<ComponentBase> FieldComponents { get; set; } = new List<ComponentBase>();
        private KeyConverter.KeyConverter _keyConverter = new KeyConverter.KeyConverter();

        #endregion

        #region Public Methods

        /// <summary>
        /// Casts the list of lists into FieldTemporaryInput's and processes the data for each Field file into a FieldSystemComponent
        /// </summary>
        /// <param name="fieldList"></param>
        /// <param name="farm"></param>
        /// <returns></returns>
        public List<ComponentBase> ConvertParsedComponent(List<List<IComponentTemporaryInput>> fieldList, Farm farm)
        {
            var stageState = farm.StageStates.OfType<FieldSystemDetailsStageState>().FirstOrDefault();
            if (stageState == null)
            {
                stageState = new FieldSystemDetailsStageState();
                farm.StageStates.Add(stageState);
            }

            var listOfTemporaryFieldInputs = new List<List<FieldTemporaryInput>>();
            foreach (var list in fieldList)
            {
                //var groupedFields = list.Cast<FieldTemporaryInput>().GroupBy(x => x.GroupId).ToList();
                listOfTemporaryFieldInputs.Add(list.Cast<FieldTemporaryInput>().ToList());
            }

            foreach (var rowInputs in listOfTemporaryFieldInputs)
            {
                bool fieldNameHasBeenSetOnce = false;

                //A field contains a list of ComponentSelectionScreenCropViewItems that
                //correspond to a row in our excel file
                var fieldSystemComponent = new FieldSystemComponent();

                // Set this for the cases when a farm
                fieldSystemComponent.Guid = Guid.NewGuid();

                foreach (var rowInput in rowInputs)
                {
                    var viewItem = new CropViewItem();

                    if (!fieldNameHasBeenSetOnce)
                    {                        
                        fieldSystemComponent.Name = rowInput.Name;                        
                        fieldSystemComponent.YearOfObservation = rowInput.CurrentYear;
                        fieldSystemComponent.FieldArea = rowInput.Area;
                        fieldSystemComponent.EndYear = rowInput.CurrentYear;
                        fieldSystemComponent.ComponentType = ComponentType.Field;
                        fieldNameHasBeenSetOnce = true;
                    }

                    viewItem.Guid = Guid.NewGuid();
                    viewItem.FieldSystemComponentGuid = rowInput.FieldSystemComponentGuid;
                    viewItem.FieldName = fieldSystemComponent.Name;
                    viewItem.Name = rowInput.Name;                    
                    viewItem.Area = rowInput.Area;
                    viewItem.Year = rowInput.CropYear;
                    viewItem.HarvestMethod = rowInput.HarvestMethod;
                    viewItem.UnderSownCropsUsed = _keyConverter.ConvertResponseToBool(rowInput.UnderSownCropsUsed.ToString());
                    viewItem.CropIsGrazed = _keyConverter.ConvertResponseToBool(rowInput.CropIsGrazed.ToString());
                    viewItem.NumberOfPesticidePasses = rowInput.NumberOfPesticidePasses;
                    viewItem.IsPesticideUsed = rowInput.IsPesticideUsed;
                    viewItem.PhaseNumber = rowInput.PhaseNumber;
                    viewItem.CropType = rowInput.CropType;

                    if (viewItem.CropType.IsPerennial())
                    {
                        viewItem.YearInPerennialStand = rowInput.YearInPerennialStand;
                        viewItem.PerennialStandLength = rowInput.PerennialStandLength;
                        viewItem.PerennialStandGroupId = rowInput.Guid;
                    }

                    viewItem.ManureApplied = _keyConverter.ConvertResponseToBool(rowInput.ManureApplied.ToString());
                    viewItem.AmountOfManureApplied = rowInput.AmountOfManureApplied;
                    viewItem.ManureApplicationType = rowInput.ManureApplicationType;
                    viewItem.ManureAnimalSourceType = rowInput.ManureAnimalSourceType;
                    viewItem.ManureLocationSourceType = rowInput.ManureLocationSourceType;
                    viewItem.ManureStateType = rowInput.ManureStateType;
                    viewItem.MoistureContentOfCrop = rowInput.MoistureContentOfCrop;
                    viewItem.MoistureContentOfCropPercentage = rowInput.MoistureContentOfCropPercentage;

                    viewItem.NitrogenContentInExtraroot = rowInput.NitrogenContentInExtraroot;
                    viewItem.NitrogenContentInProduct = rowInput.NitrogenContentInProduct;
                    viewItem.NitrogenContentInRoots = rowInput.NitrogenContentInRoots;
                    viewItem.NitrogenContentInStraw = rowInput.NitrogenContentInStraw;
                    viewItem.NitrogenFixation = rowInput.NitrogenFixation;
                    viewItem.NitrogenDepositionAmount = rowInput.NitrogenDeposit;

                    viewItem.BiomassCoefficientProduct = rowInput.BiomassCoefficientProduct;
                    viewItem.BiomassCoefficientExtraroot = rowInput.BiomassCoefficientExtraroot;
                    viewItem.BiomassCoefficientRoots = rowInput.BiomassCoefficientRoots;
                    viewItem.BiomassCoefficientStraw = rowInput.BiomassCoefficientStraw;

                    viewItem.PhosphorusFertilizerRate = rowInput.PhosphorousFertilizerRate;
                    viewItem.NitrogenFertilizerRate = rowInput.NitrogenFertilizerRate;

                    viewItem.IsIrrigated = rowInput.IsIrrigated;
                    viewItem.TillageType = rowInput.TillageType;
                    viewItem.IrrigationType = rowInput.IrrigationType;
                    viewItem.AmountOfIrrigation = rowInput.AmountOfIrrigation;

                    viewItem.Yield = rowInput.Yield;
                    viewItem.CarbonConcentration = rowInput.CarbonConcentration;
                    viewItem.PercentageOfStrawReturnedToSoil = rowInput.PercentageOfStrawReturnedToSoil;
                    viewItem.PercentageOfProductYieldReturnedToSoil = rowInput.PercentageOfProductYieldReturnedToSoil;
                    viewItem.PercentageOfRootsReturnedToSoil = rowInput.PercentageOfRootsReturnedToSoil;

                    viewItem.TimePeriodCategoryString = rowInput.TimePeriodCategoryString;
                    viewItem.ClimateParameter = rowInput.ClimateParameter;
                    viewItem.TillageFactor = rowInput.TillageFactor;
                    viewItem.ManagementFactor = rowInput.ManagementFactor;
                    viewItem.PlantCarbonInAgriculturalProduct = rowInput.PlantCarbonInAgriculturalProduct;
                    viewItem.CarbonInputFromProduct = rowInput.CarbonInputFromProduct;
                    viewItem.CarbonInputFromStraw = rowInput.CarbonInputFromStraw;
                    viewItem.CarbonInputFromRoots = rowInput.CarbonInputFromRoots;
                    viewItem.CarbonInputFromExtraroots = rowInput.CarbonInputFromExtraroots;
                    viewItem.SizeOfFirstRotationForField = rowInput.SizeOfFirstRotationForField;
                    viewItem.AboveGroundCarbonInput = rowInput.AboveGroundCarbonInput;
                    viewItem.BelowGroundCarbonInput = rowInput.BelowGroundCarbonInput;
                    viewItem.ManureCarbonInputsPerHectare = rowInput.ManureCarbonInputsPerHectare;
                    viewItem.DigestateCarbonInputsPerHectare = rowInput.DigestateCarbonInputsPerHectare;
                    viewItem.TotalCarbonInputs = rowInput.TotalCarbonInputs;
                    viewItem.Sand = rowInput.Sand;
                    viewItem.LigninContent = rowInput.Lignin;

                    viewItem.WFac = rowInput.WFac;
                    viewItem.TFac = rowInput.TFac;
                    viewItem.TotalNitrogenInputsForIpccTier2 = rowInput.TotalNitrogenInputsForIpccTier2;
                    viewItem.NitrogenContent = rowInput.NitrogenContent;
                    viewItem.AboveGroundResidueDryMatter = rowInput.AboveGroundResidueDryMatter;
                    viewItem.BelowGroundResidueDryMatter = rowInput.BelowGroundResidueDryMatter;
                    viewItem.FuelEnergy = rowInput.FuelEnergy;
                    viewItem.HerbicideEnergy = rowInput.HerbicideEnergy;
                    
                    // CLI only supports 1 fertilizer application for now
                    if (rowInput.NitrogenFertilizerRate > 0)
                    {
                        var fertilizerBlend = new Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data() { FertilizerBlend = rowInput.FertilizerBlend };
                        viewItem.FertilizerApplicationViewItems = new ObservableCollection<FertilizerApplicationViewItem>()
                        {
                            new FertilizerApplicationViewItem()
                            {
                                FertilizerBlendData = fertilizerBlend,
                                AmountOfBlendedProductApplied = rowInput.NitrogenFertilizerRate
                            }
                        };
                    }

                    fieldSystemComponent.Guid = viewItem.FieldSystemComponentGuid;

                    stageState.DetailsScreenViewCropViewItems.Add(viewItem);
                }

                // If there is only one input row then it is a single year field and we need to populate the CropViewItems property so that calls to GetSingleYearViewItem can be made.
                if (stageState.DetailsScreenViewCropViewItems.Count == 1)
                {
                    fieldSystemComponent.CropViewItems.Add(stageState.DetailsScreenViewCropViewItems.Single());
                }

                fieldSystemComponent.StartYear = stageState.DetailsScreenViewCropViewItems.OrderBy(viewItem => viewItem.Year).Select(viewItem => viewItem.Year).Min();
                FieldComponents.Add(fieldSystemComponent);
            }
            return FieldComponents;

        } 

        #endregion
    }
}
