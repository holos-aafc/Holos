using H.CLI.Interfaces;
using H.CLI.UserInput;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.Animals;
using H.Core.Services.LandManagement;
using H.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using H.Core.Calculators.Carbon;

namespace H.CLI.Processors
{
    public class FieldProcessor : IProcessor
    {
        #region Fields
        public List<ComponentBase> FieldComponents { get; set; } = new List<ComponentBase>();
        private readonly IFieldResultsService _fieldResultsService;
        private AnimalResultsService _animalService;

        #endregion

        #region Constructors

        public FieldProcessor(IFieldResultsService fieldResultsService)
        {
            if (fieldResultsService != null)
            {
                _fieldResultsService = fieldResultsService;
            }
            else
            {
                throw new ArgumentNullException(nameof(fieldResultsService));
            }

            _animalService = new AnimalResultsService();
        }

        #endregion

        #region Public Methods

        public void ProcessComponent(Farm farm, List<ComponentBase> componentList, ApplicationData applicationData)
        {
            /*
             * Do not recreate the details stage state here. This will be created FieldSystemInputConverter class when each row of an input file is read. This allows
             * a user to export a GUI farm with the existing stage state. That existing stage state will be used to create the rows in the input file (one for each year).
             */

            var animalResults = _animalService.GetAnimalResults(farm);
            _fieldResultsService.AnimalResults = animalResults;

            var finalResults = _fieldResultsService.CalculateFinalResults(farm);

            var farmName = farm.Name + Properties.Resources.Results;

            string filePath = InfrastructureConstants.BaseOutputDirectoryPath + @"\" + Properties.Resources.Outputs + @"\" + farmName + @"\" + Properties.Resources.DefaultFieldsInputFolder + @"\";

            // Write field system data to file
            _fieldResultsService.ExportResultsToFile(
                results: finalResults,
                path: filePath,
                cultureInfo: CLILanguageConstants.culture,
                measurementSystemType: CLIUnitsOfMeasurementConstants.measurementSystem,
                languageAddOn: CLILanguageConstants.OutputLanguageAddOn,
                exportedFromGui: false,
                farm: farm);

            // Write econ data to file
            //_economicsCalculator.ExportEconomicsDataToFile(
            //    farm: farm,
            //    path: filePath,
            //    exportFromGui: false, applicationData: applicationData, farmEmissionResults: TODO, languageAddon: CLILanguageConstants.OutputLanguageAddOn);
        }

        /// <summary>
        /// Creates an input file from a single year mode field component.
        /// </summary>
        public string SetTemplateCSVFileBasedOnExportedField(Farm farm,
                                                             string path,
                                                             Dictionary<string, ImperialUnitsOfMeasurement?> componentKeys,
                                                             FieldSystemComponent fieldSystemComponent,
                                                             bool writeToPath = true)
        {
            var detailsScreenViewCropViewItems = new List<CropViewItem>();

            // This method is called when encountering a field from a single-year mode farm so there will only be one view item. Create a list of containing only one item and 
            // call the overload of this method
            var viewItem = fieldSystemComponent.GetSingleYearViewItem();

            var detailViewItem = _fieldResultsService.MapDetailsScreenViewItemFromComponentScreenViewItem(viewItem, viewItem.Year);

            detailsScreenViewCropViewItems.Add(detailViewItem);

            var result = this.SetTemplateCSVFileBasedOnExportedField(farm: farm,
                path: path,
                componentKeys: componentKeys,
                fieldSystemComponent: fieldSystemComponent,
                detailsScreenViewCropViewItems: detailsScreenViewCropViewItems,
                writeToPath: writeToPath);

            return result;
        }

        /// <summary>
        /// Builds a field component input file based on a farm exported from the GUI
        /// </summary>
        /// <param name="farm"></param>
        /// <param name="path">The location where the file is to be written to</param>
        /// <param name="componentKeys"></param>
        /// <param name="fieldSystemComponent"></param>
        /// <param name="detailsScreenViewCropViewItems"></param>
        /// <param name="writeToPath">Indicates if the file should be written to disk or the file contents should be returned to caller instead.</param>
        public string SetTemplateCSVFileBasedOnExportedField(Farm farm,
                                                             string path,
                                                             Dictionary<string, ImperialUnitsOfMeasurement?> componentKeys,
                                                             FieldSystemComponent fieldSystemComponent,
                                                             List<CropViewItem> detailsScreenViewCropViewItems,
                                                             bool writeToPath = true)
        {
            string columnSeparator = CLILanguageConstants.Delimiter;
            var fileName = FileSystemHelper.SanitizeFileName(fieldSystemComponent.Name);
            var filePath = path + @"\" + fileName + CLILanguageConstants.DefaultInputFileExtension;
            var stringBuilder = new StringBuilder();
            foreach (var keyValuePair in componentKeys)
            {
                var convertedKey = keyValuePair.Key.Trim();
                stringBuilder.Append(convertedKey + columnSeparator);
            }

            stringBuilder.Append(Environment.NewLine);

            foreach (var viewItem in detailsScreenViewCropViewItems)
            {
                // In some old farms, the tillage factor was not calculated, do that now when reading the file (if the tillage factor was 0).

                if (Math.Abs(viewItem.TillageFactor) < double.Epsilon)
                {
                    viewItem.TillageFactor = _fieldResultsService.CalculateTillageFactor(viewItem, farm);
                }

                stringBuilder.Append(viewItem.PhaseNumber + columnSeparator);
                stringBuilder.Append(fieldSystemComponent.Name + columnSeparator);
                stringBuilder.Append(viewItem.Area + columnSeparator);
                stringBuilder.Append(viewItem.YearOfObservation + columnSeparator);
                stringBuilder.Append(viewItem.Year + columnSeparator);
                stringBuilder.Append(viewItem.CropType + columnSeparator);
                stringBuilder.Append(viewItem.TillageType + columnSeparator);
                stringBuilder.Append(viewItem.YearInPerennialStand + columnSeparator);
                stringBuilder.Append(viewItem.PerennialStandGroupId + columnSeparator);
                stringBuilder.Append(viewItem.PerennialStandLength + columnSeparator);
                stringBuilder.Append(viewItem.BiomassCoefficientProduct + columnSeparator);
                stringBuilder.Append(viewItem.BiomassCoefficientStraw + columnSeparator);
                stringBuilder.Append(viewItem.BiomassCoefficientRoots + columnSeparator);
                stringBuilder.Append(viewItem.BiomassCoefficientExtraroot + columnSeparator);
                stringBuilder.Append(viewItem.NitrogenContentInProduct + columnSeparator);
                stringBuilder.Append(viewItem.NitrogenContentInStraw + columnSeparator);
                stringBuilder.Append(viewItem.NitrogenContentInRoots + columnSeparator);
                stringBuilder.Append(viewItem.NitrogenContentInExtraroot + columnSeparator);
                stringBuilder.Append(viewItem.NitrogenFixation + columnSeparator);
                stringBuilder.Append(viewItem.NitrogenDepositionAmount + columnSeparator);
                stringBuilder.Append(viewItem.CarbonConcentration + columnSeparator);
                stringBuilder.Append(viewItem.Yield + columnSeparator);
                stringBuilder.Append(viewItem.HarvestMethod + columnSeparator);
                stringBuilder.Append(viewItem.NitrogenFertilizerRate + columnSeparator);
                stringBuilder.Append(viewItem.PhosphorusFertilizerRate + columnSeparator);
                stringBuilder.Append(viewItem.IsIrrigated + columnSeparator);
                stringBuilder.Append(viewItem.IrrigationType + columnSeparator);
                stringBuilder.Append(viewItem.AmountOfIrrigation + columnSeparator);
                stringBuilder.Append(viewItem.MoistureContentOfCrop + columnSeparator);
                stringBuilder.Append(viewItem.MoistureContentOfCropPercentage + columnSeparator);
                stringBuilder.Append(viewItem.PercentageOfStrawReturnedToSoil + columnSeparator);
                stringBuilder.Append(viewItem.PercentageOfRootsReturnedToSoil + columnSeparator);
                stringBuilder.Append(viewItem.PercentageOfProductYieldReturnedToSoil + columnSeparator);
                stringBuilder.Append(viewItem.IsPesticideUsed + columnSeparator);
                stringBuilder.Append(viewItem.NumberOfPesticidePasses + columnSeparator);

                this.ProcessManureApplications(stringBuilder, viewItem, columnSeparator, fieldSystemComponent);

                stringBuilder.Append(viewItem.UnderSownCropsUsed + columnSeparator);

                this.ProcessGrazingViewItems(stringBuilder, viewItem, columnSeparator, fieldSystemComponent);

                stringBuilder.Append(viewItem.FieldSystemComponentGuid + columnSeparator);
                stringBuilder.Append(viewItem.TimePeriodCategoryString + columnSeparator);
                stringBuilder.Append(viewItem.ClimateParameter + columnSeparator);
                stringBuilder.Append(viewItem.TillageFactor + columnSeparator);
                stringBuilder.Append(viewItem.ManagementFactor + columnSeparator);
                stringBuilder.Append(viewItem.PlantCarbonInAgriculturalProduct + columnSeparator);
                stringBuilder.Append(viewItem.CarbonInputFromProduct + columnSeparator);
                stringBuilder.Append(viewItem.CarbonInputFromStraw + columnSeparator);
                stringBuilder.Append(viewItem.CarbonInputFromRoots + columnSeparator);
                stringBuilder.Append(viewItem.CarbonInputFromExtraroots + columnSeparator);
                stringBuilder.Append(fieldSystemComponent.SizeOfFirstRotationInField() + columnSeparator);
                stringBuilder.Append(viewItem.AboveGroundCarbonInput + columnSeparator);
                stringBuilder.Append(viewItem.BelowGroundCarbonInput + columnSeparator);
                stringBuilder.Append(viewItem.ManureCarbonInputsPerHectare + columnSeparator);
                stringBuilder.Append(viewItem.DigestateCarbonInputsPerHectare + columnSeparator);
                stringBuilder.Append(viewItem.TotalCarbonInputs + columnSeparator);
                stringBuilder.Append(viewItem.Sand + columnSeparator);
                stringBuilder.Append(viewItem.LigninContent + columnSeparator);
                stringBuilder.Append(viewItem.WFac + columnSeparator);
                stringBuilder.Append(viewItem.TFac + columnSeparator);
                stringBuilder.Append(viewItem.TotalNitrogenInputsForIpccTier2 + columnSeparator);
                stringBuilder.Append(viewItem.NitrogenContent + columnSeparator);
                stringBuilder.Append(viewItem.AboveGroundResidueDryMatter + columnSeparator);
                stringBuilder.Append(viewItem.BelowGroundResidueDryMatter + columnSeparator);
                stringBuilder.Append(viewItem.FuelEnergy + columnSeparator);
                stringBuilder.Append(viewItem.HerbicideEnergy + columnSeparator);

                var fertilizerApplication = viewItem.FertilizerApplicationViewItems.FirstOrDefault();
                if (fertilizerApplication != null && fertilizerApplication.FertilizerBlendData != null)
                {
                    stringBuilder.Append(fertilizerApplication.FertilizerBlendData.FertilizerBlend + columnSeparator);
                }
                else
                {
                    stringBuilder.Append(FertilizerBlends.Custom + columnSeparator);
                }

                stringBuilder.Append(Environment.NewLine);
            }

            if (writeToPath)
            {
                File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.UTF8);
            }

            return stringBuilder.ToString();
        }

        #endregion

        #region Private Methods


        private void ProcessGrazingViewItems(
            StringBuilder stringBuilder,
            CropViewItem viewItem,
            string columnSeparator,
            FieldSystemComponent fieldComponent)
        {
            var grazingViewItemsInYear = fieldComponent.GetGrazingViewItemsInYear(viewItem.Year);
            var cropIsGrazed = grazingViewItemsInYear.Any();
            stringBuilder.Append(cropIsGrazed.ToString().ToUpperInvariant() + columnSeparator);
        }

        /// <summary>
        /// Convert the farm's manure applications to the field into a string representation that can be read by the CLI input system
        /// </summary>
        private void ProcessManureApplications(
            StringBuilder stringBuilder, 
            CropViewItem viewItem,
            string columnSeparator, 
            FieldSystemComponent fieldComponent)
        {
            // Get both livestock and import source manure applications
            var fieldManureApplicationsInYear = fieldComponent.GetAllManureApplicationsInYear(viewItem.Year);

            if (fieldManureApplicationsInYear.Any())
            {
                /*
                 * We have at least one manure application
                 */

                if (fieldManureApplicationsInYear.Count == 1)
                {
                    /*
                     * There is only one manure application made to this field
                     */

                    var singleApplication = fieldManureApplicationsInYear[0];

                    var sourceType = singleApplication.AnimalType.GetManureAnimalSource();

                    stringBuilder.Append(true.ToString().ToUpperInvariant() + columnSeparator); // ManureApplied (bool)
                    stringBuilder.Append(singleApplication.AmountOfManureAppliedPerHectare + columnSeparator);
                    stringBuilder.Append(singleApplication.ManureApplicationMethod + columnSeparator);
                    stringBuilder.Append(sourceType + columnSeparator);
                    stringBuilder.Append(singleApplication.ManureStateType + columnSeparator);
                    stringBuilder.Append(singleApplication.ManureLocationSourceType + columnSeparator);
                }
                else
                {
                    /*
                     * TODO: in the future, append multiple manure application to input file, instead of current approach to take the largest application
                     */

                    /*
                     * There are multiple manure applications. The CLI can only support adding one manure application to the field in any given year. Strategy right now is to
                     * take the largest application (by volume applied per hectare) and use that.
                     */

                    var sortedByAmount = fieldManureApplicationsInYear.OrderByDescending(x => x.AmountOfManureAppliedPerHectare);
                    var firstLargestApplication = sortedByAmount.First();

                    var sourceType = firstLargestApplication.AnimalType.GetManureAnimalSource();

                    stringBuilder.Append(true.ToString().ToUpperInvariant() + columnSeparator); // ManureApplied (bool)
                    stringBuilder.Append(firstLargestApplication.AmountOfManureAppliedPerHectare + columnSeparator);
                    stringBuilder.Append(firstLargestApplication.ManureApplicationMethod + columnSeparator);
                    stringBuilder.Append(sourceType + columnSeparator);
                    stringBuilder.Append(firstLargestApplication.ManureStateType + columnSeparator);
                    stringBuilder.Append(firstLargestApplication.ManureLocationSourceType + columnSeparator);
                }
            }
            else
            {
                stringBuilder.Append(viewItem.ManureApplied + columnSeparator);
                stringBuilder.Append(viewItem.AmountOfManureApplied + columnSeparator);
                stringBuilder.Append(viewItem.ManureApplicationType + columnSeparator);
                stringBuilder.Append(viewItem.ManureAnimalSourceType + columnSeparator);
                stringBuilder.Append(viewItem.ManureStateType + columnSeparator);
                stringBuilder.Append(viewItem.ManureLocationSourceType + columnSeparator);
            }
        }

        #endregion
    }
}