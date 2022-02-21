using System;
using H.CLI.Interfaces;
using H.CLI.UserInput;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using H.CLI.FileAndDirectoryAccessors;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Climate;
using H.Core.Services;
using H.Core.Services.LandManagement;
using H.Core.Calculators.Economics;
using H.Core.Services.Animals;

namespace H.CLI.Processors
{
    public class FieldProcessor : IProcessor
    {
        #region Fields
        public List<ComponentBase> FieldComponents { get; set; } = new List<ComponentBase>();
        private readonly FieldResultsService _fieldResultsService;
        private FarmResultsService _farmResultsService;
        private readonly EconomicsCalculator _economicsCalculator;

        #endregion

        #region Constructors

        public FieldProcessor()
        {
            _fieldResultsService = new FieldResultsService();
            _farmResultsService = new FarmResultsService(new EventAggregator(), _fieldResultsService);
            _economicsCalculator = new EconomicsCalculator(_fieldResultsService);
        }

        #endregion

        #region Public Methods
        public void ProcessComponent(Farm farm, List<ComponentBase> componentList, ApplicationData applicationData)
        {
            /*
             * Do not recreate the details stage state here. This will be created FieldSystemInputConverter class when each row of an input file is read. This allows
             * a user to export a GUI farm with the existing stage state. That existing stage state will be used to create the rows in the input file (one for each year).
             */

            // Should re_crop be overwritten here if the user specifies a custom climate file?
            //var pathToCustomClimateData = farm.CliInputPath + @"\" + farm.ClimateDataFileName;
            //_customClimateDataProvider.LoadUserClimateFile(pathToCustomClimateData, farm);

            _fieldResultsService.CalculateFinalResults(farm);

            var farmName = farm.Name + Properties.Resources.Results;

            string filePath = InfrastructureConstants.BaseOutputDirectoryPath + @"\" + Properties.Resources.Outputs + @"\" + farmName + @"\" + Properties.Resources.DefaultFieldsInputFolder + @"\";

            //write field system data to file
            _fieldResultsService.ExportResultsToFile(
                results: farm.GetCropDetailViewItems(),
                path: filePath,
                cultureInfo: CLILanguageConstants.culture,
                measurementSystemType: CLIUnitsOfMeasurementConstants.measurementSystem,
                languageAddOn: CLILanguageConstants.OutputLanguageAddOn,
                exportedFromGui: false,
                farm: farm);

            //write econ data to file
            _economicsCalculator.ExportEconomicsDataToFile(
                farm: farm,
                path: filePath,
                exportFromGui: false, applicationData: applicationData, languageAddon: CLILanguageConstants.OutputLanguageAddOn);
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
            var detailViewItem = _fieldResultsService.MapDetailsScreenViewItemFromComponentScreenViewItem(fieldSystemComponent.GetSingleYearViewItem());

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
            var filePath = path + @"\" + fieldSystemComponent.Name + CLILanguageConstants.DefaultInputFileExtension;
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
                stringBuilder.Append(viewItem.PercentageOfStrawReturnedToSoil + columnSeparator);
                stringBuilder.Append(viewItem.PercentageOfRootsReturnedToSoil + columnSeparator);
                stringBuilder.Append(viewItem.PercentageOfProductYieldReturnedToSoil + columnSeparator);
                stringBuilder.Append(viewItem.IsPesticideUsed + columnSeparator);
                stringBuilder.Append(viewItem.NumberOfPesticidePasses + columnSeparator);
                stringBuilder.Append(viewItem.ManureApplied + columnSeparator);
                stringBuilder.Append(viewItem.AmountOfManureApplied + columnSeparator);
                stringBuilder.Append(viewItem.ManureApplicationType + columnSeparator);
                stringBuilder.Append(viewItem.ManureAnimalSourceType + columnSeparator);
                stringBuilder.Append(viewItem.ManureStateType + columnSeparator);
                stringBuilder.Append(viewItem.ManureLocationSourceType + columnSeparator);
                stringBuilder.Append(viewItem.UnderSownCropsUsed + columnSeparator);
                stringBuilder.Append(viewItem.CropIsGrazed + columnSeparator);
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

                stringBuilder.Append(Environment.NewLine);
            }

            if (writeToPath)
            {
                File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.UTF8);
            }

            return stringBuilder.ToString();
        }

        #endregion
    }
}



