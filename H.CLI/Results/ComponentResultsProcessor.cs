using H.CLI.Converters;
using H.CLI.UserInput;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Emissions;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Services;
using H.Core.Services.LandManagement;
using H.Infrastructure;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using H.Core;
using H.Core.Calculators.Infrastructure;
using H.Core.Calculators.Nitrogen;
using H.Core.Converters;
using H.Core.Providers.Climate;
using H.Core.Services.Animals;


namespace H.CLI.Results
{
    public class ComponentResultsProcessor
    {
        #region Fields

        private readonly Storage _storage;
        public List<KeyValuePair<string, List<AnimalComponentEmissionsResults>>> _animalEmissionResultsForAllFarms { get; set; } = new List<KeyValuePair<string, List<AnimalComponentEmissionsResults>>>();
        private readonly EnergyCarbonDioxideEmissionsCalculator _energyCalculator;
        private readonly Table_57_58_Expression_Of_Uncertainty_Calculator _uncertaintyCalculator;
        private readonly SummationsCalculator _summationsCalculator = new SummationsCalculator();
        private readonly KeyConverter.KeyConverter _keyConverter = new KeyConverter.KeyConverter();
        private readonly EmissionTypeConverter _emissionTypeConverter = new EmissionTypeConverter();
        private readonly ComponentConverterHandler _componentConverterHandler = new ComponentConverterHandler();
        private readonly UnitsOfMeasurementCalculator _uCalc = new UnitsOfMeasurementCalculator();
        private readonly IFieldResultsService _fieldResultsService;
        private readonly IFarmResultsService _farmResultsService;

        private List<FarmEmissionResults> _farmEmissionResults = new List<FarmEmissionResults>();

        // Two files are output, 1 for raw GHG display and the other for CO2e display
        private const double _numberOfOutputFileTypes = 2;
        private const int roundingDigits = 4;

        private List<EmissionDisplayUnits> _validFileOutputTypes = new List<EmissionDisplayUnits>()
        {
            EmissionDisplayUnits.KilogramsGhgs,
            EmissionDisplayUnits.MegagramsCO2e
        };

        #endregion

        #region Constructor

        public ComponentResultsProcessor(Storage storage, ITimePeriodHelper timePeriodHelper, IFieldResultsService fieldResultsService, N2OEmissionFactorCalculator n2OEmissionFactorCalculator)
        {
            if (fieldResultsService != null)
            {
                _fieldResultsService = fieldResultsService;
            }
            else
            {
                throw new ArgumentNullException(nameof(fieldResultsService));
            }

            _energyCalculator = new EnergyCarbonDioxideEmissionsCalculator();
            _uncertaintyCalculator = new Table_57_58_Expression_Of_Uncertainty_Calculator();

            var animalService = new AnimalResultsService();
            var manureService = new ManureService();

            _farmResultsService = new FarmResultsService(new EventAggregator(), _fieldResultsService, new ADCalculator(), manureService, animalService, n2OEmissionFactorCalculator );
        }

        #endregion

        /// <summary>
        /// Takes in a Storage object and iterates through every Farm.
        /// 
        /// Returns a List of KeyValuePairs that correspond to the farmName_farmSettingsFile_InfrastructureConstants.BaseOutputDirectoryPath and a list of the Emission Results for all the Components in that Farm.
        /// </summary>
        public void ProcessFarms(Storage storage)
        {
            var results = new List<FarmEmissionResults>();

            var numberOfFarms = storage.ApplicationData.Farms.Count;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Change ordering here
            var orderedFarms = storage.ApplicationData.Farms.OrderBy(x => x.Name);
            foreach (var farm in orderedFarms)
            {
                var farmResults = _farmResultsService.CalculateFarmEmissionResults(farm);
                results.Add(farmResults);

                var key = farm.Name + "_" + farm.SettingsFileName + "_";

                _animalEmissionResultsForAllFarms.Add(new KeyValuePair<string, List<AnimalComponentEmissionsResults>>(key, farmResults.AnimalComponentEmissionsResults.ToList()));

                // TODO: Add emission results from fields and shelterbelts

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(string.Format(H.CLI.Properties.Resources.FinishedCalculatingResultsForForName, farm.Name));
                Console.WriteLine(string.Format(H.CLI.Properties.Resources.NumberOfFarmsRemainingToProcess, numberOfFarms--));
            }

            stopwatch.Stop();
            Console.WriteLine(string.Format(H.CLI.Properties.Resources.FinishedCLIRunInTime, stopwatch.Elapsed.ToString("g")));

            _farmEmissionResults = results;
        }


        /// <summary>
        /// Responsible For Creating All Final Reports based on the base directory of the argument passed when running the CLI (location of the Farms folder)
        /// </summary>
        /// <param name="applicationData"></param>
        public void WriteEmissionsToFiles(ApplicationData applicationData)
        {
            WriteEstimatesOfProductionToFile();
            WriteGHGAndCO2EmissionsToFile(applicationData);
            WriteFeedEstimatesToFile();
            WriteGHGAndCO2EmissionsForEachFarmByMonth(applicationData);
            WriteEstimatesOfProductionToFileByMonth();
            WriteFeedEstimatesToFileMonthly_NEW();
            WriteFieldCarbonResultsToFile();
        }

        private void WriteFieldCarbonResultsToFile()
        {
            var path = InfrastructureConstants.BaseOutputDirectoryPath + @"\" + Properties.Resources.Outputs + @"\" + Properties.Resources.TotalResultsForAllFarms + @"\" + Properties.Resources.TotalResultsAllFields + CLILanguageConstants.OutputLanguageAddOn;

            //_fieldResultsService.ExportAllResultsToFile(path: path,
            //    measurementSystemType: CLIUnitsOfMeasurementConstants.measurementSystem,
            //    cultureInfo: CLILanguageConstants.culture, viewItems: TODO, farm: TODO);
        }

        /// <summary>
        /// Writes both the GHG and CO2e yearly emission results for all farms and is written to a total results folder that we create (using the InfrastructureConstants.BaseOutputDirectoryPath)
        /// Builds up a single string rather than creating a bunch of immutable strings to improve efficiency, but as a result, is more fragile. However, treat each section 
        /// similarly to CSS styling, where each group has its own set of adjustable placements of data (Example: if you see that your component totals are misaligned, go to the
        /// section where the component outputs are added to the stringBuilder and adjust the styling and formatting of the stringBuilder there).
        /// </summary>
        public void WriteGHGAndCO2EmissionsToFile(ApplicationData applicationData)
        {
            var stringBuilder = new StringBuilder();
            foreach (var outputType in _validFileOutputTypes)
            {
                stringBuilder.Clear();

                // Build the path depending on which output file we are building
                var path = outputType == EmissionDisplayUnits.KilogramsGhgs ? 
                    InfrastructureConstants.BaseOutputDirectoryPath + @"\" + Properties.Resources.Outputs + @"\" + Properties.Resources.TotalResultsForAllFarms + @"\" + Properties.Resources.TotalResultsGHG + CLILanguageConstants.OutputLanguageAddOn : 
                    InfrastructureConstants.BaseOutputDirectoryPath + @"\" + Properties.Resources.Outputs + @"\" + Properties.Resources.TotalResultsForAllFarms + @"\" + Properties.Resources.TotalResultsCO2E + CLILanguageConstants.OutputLanguageAddOn;

                // Build the headers
                stringBuilder.AppendLine(this.GetHeadersAllFarms(applicationData, outputType));

                // Output results for each individual farm
                foreach (var farmEmissionResult in _farmEmissionResults)
                {
                    // Add farm name
                    var farmName = farmEmissionResult.Farm.Name + "_" + farmEmissionResult.Farm.SettingsFileName + "_";
                    stringBuilder.AppendLine(farmName);

                    // Iterate over each animal component result (order by component category)
                    foreach (var animalComponentEmissionResults in farmEmissionResult.AnimalComponentEmissionsResults.GroupBy(x => x.Component.ComponentCategory))
                    {
                        // Farm name
                        stringBuilder.Append(CLILanguageConstants.Delimiter);

                        // Component category
                        stringBuilder.Append(animalComponentEmissionResults.Key.GetDescription() + CLILanguageConstants.Delimiter);
                        stringBuilder.AppendLine();

                        // Iterate over all group emission results for the component (order by animal type)
                        foreach (var animalComponentEmissionsResults in animalComponentEmissionResults)
                        {
                            // Farm name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Component category
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Component name
                            stringBuilder.Append(animalComponentEmissionsResults.Component.Name + CLILanguageConstants.Delimiter);
                            stringBuilder.AppendLine();

                            foreach (var groupEmissionResult in animalComponentEmissionsResults.EmissionResultsForAllAnimalGroupsInComponent)
                            {
                                // Farm name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);

                                // Component category
                                stringBuilder.Append(CLILanguageConstants.Delimiter);

                                // Component name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);

                                // Animal group name
                                stringBuilder.Append(groupEmissionResult.AnimalGroup.Name + CLILanguageConstants.Delimiter);

                                stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsCH4, outputType, groupEmissionResult.TotalEntericMethane), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsCH4, outputType, groupEmissionResult.TotalManureMethane), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsN2O, outputType, groupEmissionResult.TotalDirectNitrousOxide), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsN2O, outputType, groupEmissionResult.TotalIndirectNitrousOxide), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsC02, outputType, groupEmissionResult.TotalEnergyCarbonDioxide), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);

                                // CO2 is not applicable for animals (just energy CO2). Other rows in report do use CO2 though (i.e. land use change items)
                                stringBuilder.Append(CLILanguageConstants.NotApplicableResultsString + CLILanguageConstants.Delimiter);

                                // Output subtotals for this group
                                if (outputType != EmissionDisplayUnits.KilogramsGhgs)
                                {
                                    stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsC02e, outputType, groupEmissionResult.TotalCarbonDioxideEquivalentEmissionsFromAnimalGroup), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                }
                                else
                                {
                                    // Subtotal column is not shown in the GHG report because we can't take subtotals of differing types (kg N2O and kg CH4)
                                }

                                stringBuilder.AppendLine();
                            }

                            // Subtotals for this component

                            // Farm name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Component category
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Component name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Subtotal
                            stringBuilder.Append(animalComponentEmissionsResults.Component.Name + " " + Properties.Resources.Totals + CLILanguageConstants.Delimiter);

                            stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsCH4, outputType, animalComponentEmissionsResults.TotalEntericMethaneEmission), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                            stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsCH4, outputType, animalComponentEmissionsResults.TotalManureMethaneEmission), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                            stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsN2O, outputType, animalComponentEmissionsResults.TotalDirectNitrousOxideEmission), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                            stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsN2O, outputType, animalComponentEmissionsResults.TotalIndirectN2OEmission), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                            stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsC02, outputType, animalComponentEmissionsResults.TotalCarbonDioxide), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);

                            // CO2 is not applicable for animals (just energy CO2). Other rows in report do use CO2 though (i.e. land use change items)
                            stringBuilder.Append(CLILanguageConstants.NotApplicableResultsString + CLILanguageConstants.Delimiter);

                            // Output subtotals for this component
                            if (outputType != EmissionDisplayUnits.KilogramsGhgs)
                            {
                                stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsC02e, outputType, animalComponentEmissionsResults.TotalCarbonDioxideEquivalentsFromAllGroupsInComponent), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                            }
                            else
                            {
                                // Subtotal column is not shown in the GHG report because we can't take subtotals of differing types (kg N2O and kg CH4)
                            }

                            stringBuilder.AppendLine();
                            stringBuilder.AppendLine();
                        }
                    }

                    // Output totals for farm

                    // Farm name
                    stringBuilder.Append(CLILanguageConstants.Delimiter);

                    // Component category
                    stringBuilder.Append(CLILanguageConstants.Delimiter);

                    // Component name
                    stringBuilder.Append(CLILanguageConstants.Delimiter);

                    stringBuilder.Append(Properties.Resources.FarmTotal + CLILanguageConstants.Delimiter);
                    stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsCH4, outputType, farmEmissionResult.TotalEntericMethaneFromFarm), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                    stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsCH4, outputType, farmEmissionResult.TotalManureMethaneFromFarm), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                    stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsN2O, outputType, farmEmissionResult.TotalDirectNitrousOxideFromFarm), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                    stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsN2O, outputType, farmEmissionResult.TotalIndirectNitrousOxideFromFarm), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                    stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsC02, outputType, farmEmissionResult.TotalEnergyCarbonDioxideFromFarm), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                    stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsC02, outputType, farmEmissionResult.TotalCO2FromFarm)).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);

                    if (outputType != EmissionDisplayUnits.KilogramsGhgs)
                    {
                        stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsC02e, outputType, farmEmissionResult.TotalCarbonDioxideEquivalentsFromFarm), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                    }
                    else
                    {
                        // Subtotal column is not shown in the GHG report because we can't take subtotals of differing types (kg N2O and kg CH4)
                    }

                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                }

                // Output results for all farms
                stringBuilder.Append(Properties.Resources.AllFarms + CLILanguageConstants.Delimiter);

                // Component category
                stringBuilder.Append(CLILanguageConstants.Delimiter);

                // Component name
                stringBuilder.Append(CLILanguageConstants.Delimiter);

                // Group name
                stringBuilder.Append(CLILanguageConstants.Delimiter);

                stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsCH4, outputType, _farmEmissionResults.TotalEntericMethaneForAllFarms()), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsCH4, outputType, _farmEmissionResults.TotalManureMethaneForAllFarms()), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsN2O, outputType, _farmEmissionResults.TotalDirectNitrousOxideForAllFarms()), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsN2O, outputType, _farmEmissionResults.TotalIndirectNitrousOxideForAllFarms()), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsC02, outputType, _farmEmissionResults.TotalEnergyCarbonDioxideForAllFarms()), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                //stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsC02, outputType, _farmEmissionResults.TotalCarbonDioxideForAllFarms()), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);

                // Output subtotals for this component
                if (outputType != EmissionDisplayUnits.KilogramsGhgs)
                {
                    //stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsC02e, outputType, _farmEmissionResults.TotalCarbonDioxideEquivalentsForAllFarms()), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                }
                else
                {
                    // Subtotal column is not shown in the GHG report because we can't take subtotals of differing types (kg N2O and kg CH4)
                }

                try
                {
                    File.WriteAllText(path, stringBuilder.ToString(), Encoding.UTF8);
                }

                catch (Exception e)
                {
                }
            }
        }

        /// <summary>
        /// Writes both the GHG and CO2 monthly emission results for each farm and for each farm's settings file and is output to the
        /// appropriate farm's directory. When creating the Emission Results, it was considered to pass around the Farm object, but due to 
        /// the inefficiency of passing around Farms, I decided to just create a string that is delimited by "_" and split that string to
        /// retrieve the appropriate information (farmName, farmSettingsFile, InfrastructureConstants.BaseOutputDirectoryPath). 
        /// This string is created in the processFarms method when adding the KeyValuePair to the list of all our emissions results.
        /// </summary>
        public void WriteGHGAndCO2EmissionsForEachFarmByMonth(ApplicationData applicationData)
        {
            foreach (var outputType in _validFileOutputTypes)
            {
                foreach (var farmEmissionResult in _farmEmissionResults)
                {
                    // Build the path depending on which output file we are building
                    var path = outputType == EmissionDisplayUnits.KilogramsGhgs ?
                        InfrastructureConstants.BaseOutputDirectoryPath + @"\" + Properties.Resources.Outputs + @"\" + farmEmissionResult.Farm.Name + Properties.Resources.Results + @"\" + farmEmissionResult.Farm.Name + Properties.Resources.FarmResultsGHG + farmEmissionResult.Farm.SettingsFileName + CLILanguageConstants.OutputLanguageAddOn :
                        InfrastructureConstants.BaseOutputDirectoryPath + @"\" + Properties.Resources.Outputs + @"\" + farmEmissionResult.Farm.Name + Properties.Resources.Results + @"\" + farmEmissionResult.Farm.Name + Properties.Resources.FarmResultsCO2E + farmEmissionResult.Farm.SettingsFileName + CLILanguageConstants.OutputLanguageAddOn;

                    var stringBuilder = new StringBuilder();

                    // Add the headers
                    stringBuilder.AppendLine(this.GetHeadersEachFarmMonthly(applicationData, outputType));
                    stringBuilder.AppendLine(farmEmissionResult.Farm.Name + "_" + farmEmissionResult.Farm.SettingsFileName);

                    // Iterate over each animal component result (order by component category)
                    foreach (var animalComponentEmissionResults in farmEmissionResult.AnimalComponentEmissionsResults.GroupBy(x => x.Component.ComponentCategory))
                    {
                        // Farm name
                        stringBuilder.Append(CLILanguageConstants.Delimiter);

                        // Component category
                        stringBuilder.Append(animalComponentEmissionResults.Key.GetDescription() + CLILanguageConstants.Delimiter);
                        stringBuilder.AppendLine();

                        // Iterate over all group emission results for the component (order by animal type)
                        foreach (var animalComponentEmissionsResults in animalComponentEmissionResults)
                        {
                            // Farm name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Component name
                            stringBuilder.Append(animalComponentEmissionsResults.Component.Name + CLILanguageConstants.Delimiter);
                            stringBuilder.AppendLine();

                            foreach (var groupEmissionResult in animalComponentEmissionsResults.EmissionResultsForAllAnimalGroupsInComponent)
                            {
                                // Farm name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);

                                // Component category
                                stringBuilder.Append(CLILanguageConstants.Delimiter);

                                // Component name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);

                                // Animal group name
                                stringBuilder.Append(groupEmissionResult.AnimalGroup.Name + CLILanguageConstants.Delimiter);

                                foreach (var groupEmissionsByMonth in groupEmissionResult.GroupEmissionsByMonths)
                                {
                                    stringBuilder.AppendLine();

                                    // Farm name
                                    stringBuilder.Append(CLILanguageConstants.Delimiter);

                                    // Component category
                                    stringBuilder.Append(CLILanguageConstants.Delimiter);

                                    // Component name
                                    stringBuilder.Append(CLILanguageConstants.Delimiter);

                                    // Animal group name
                                    stringBuilder.Append(CLILanguageConstants.Delimiter);

                                    // Month name
                                    stringBuilder.Append(groupEmissionsByMonth.MonthString);
                                    stringBuilder.Append(CLILanguageConstants.Delimiter);

                                    // Year
                                    stringBuilder.Append(groupEmissionsByMonth.Year);
                                    stringBuilder.Append(CLILanguageConstants.Delimiter);

                                    stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsCH4, outputType, groupEmissionsByMonth.MonthlyEntericMethaneEmission), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                    stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsCH4, outputType, groupEmissionsByMonth.MonthlyManureMethaneEmission), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                    stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsN2O, outputType, groupEmissionsByMonth.MonthlyDirectN2OEmission), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                    stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsN2O, outputType, groupEmissionsByMonth.MonthlyIndirectN2OEmission), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                    stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsC02, outputType, groupEmissionsByMonth.MonthlyEnergyCarbonDioxide), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);

                                    // CO2 is not applicable for animals (just energy CO2). Other rows in report do use CO2 though (i.e. land use change items)
                                    stringBuilder.Append(CLILanguageConstants.NotApplicableResultsString + CLILanguageConstants.Delimiter);

                                    // Output subtotals for this group
                                    if (outputType != EmissionDisplayUnits.KilogramsGhgs)
                                    {
                                        stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsC02e, outputType, groupEmissionsByMonth.TotalCarbonDioxideEquivalentsForMonth), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                    }
                                    else
                                    {
                                        // Subtotal column is not shown in the GHG report because we can't take subtotals of differing types (kg N2O and kg CH4)
                                    }
                                }

                                stringBuilder.AppendLine();
                            }

                            // Subtotals for this component
                            stringBuilder.AppendLine();

                            // Farm name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Component category
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Component name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Animal group name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Month name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Subtotal
                            stringBuilder.Append(animalComponentEmissionsResults.Component.Name + " " + Properties.Resources.Totals + CLILanguageConstants.Delimiter);

                            stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsCH4, outputType, animalComponentEmissionsResults.TotalEntericMethaneEmission), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                            stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsCH4, outputType, animalComponentEmissionsResults.TotalManureMethaneEmission), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                            stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsN2O, outputType, animalComponentEmissionsResults.TotalDirectNitrousOxideEmission), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                            stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsN2O, outputType, animalComponentEmissionsResults.TotalIndirectN2OEmission), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                            stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsC02, outputType, animalComponentEmissionsResults.TotalCarbonDioxide), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);

                            // CO2 is not applicable for animals (just energy CO2). Other rows in report do use CO2 though (i.e. land use change items)
                            stringBuilder.Append(CLILanguageConstants.NotApplicableResultsString + CLILanguageConstants.Delimiter);

                            // Output subtotals for this component
                            if (outputType != EmissionDisplayUnits.KilogramsGhgs)
                            {
                                stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsC02e, outputType, animalComponentEmissionsResults.TotalCarbonDioxideEquivalentsFromAllGroupsInComponent), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                            }
                            else
                            {
                                // Subtotal column is not shown in the GHG report because we can't take subtotals of differing types (kg N2O and kg CH4)
                            }

                            stringBuilder.AppendLine();
                            stringBuilder.AppendLine();
                        }
                    }

                    // Output totals for farm
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();

                    // Farm name
                    stringBuilder.Append(CLILanguageConstants.Delimiter);

                    // Component category
                    stringBuilder.Append(CLILanguageConstants.Delimiter);

                    // Component name
                    stringBuilder.Append(CLILanguageConstants.Delimiter);

                    // Animal group name
                    stringBuilder.Append(CLILanguageConstants.Delimiter);

                    // Month name
                    stringBuilder.Append(CLILanguageConstants.Delimiter);

                    stringBuilder.Append(Properties.Resources.FarmTotal + CLILanguageConstants.Delimiter);

                    stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsCH4, outputType, farmEmissionResult.TotalEntericMethaneFromFarm), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                    stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsCH4, outputType, farmEmissionResult.TotalManureMethaneFromFarm), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                    stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsN2O, outputType, farmEmissionResult.TotalDirectNitrousOxideFromFarm), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                    stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsN2O, outputType, farmEmissionResult.TotalIndirectNitrousOxideFromFarm), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                    stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsC02, outputType, farmEmissionResult.TotalEnergyCarbonDioxideFromFarm), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                    stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsC02, outputType, farmEmissionResult.TotalCO2FromFarm)).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);

                    if (outputType != EmissionDisplayUnits.KilogramsGhgs)
                    {
                        stringBuilder.Append(Math.Round(_emissionTypeConverter.Convert(EmissionDisplayUnits.KilogramsC02e, outputType, farmEmissionResult.TotalCarbonDioxideEquivalentsFromFarm), roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                    }
                    else
                    {
                        // Subtotal column is not shown in the GHG report because we can't take subtotals of differing types (kg N2O and kg CH4)
                    }

                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();

                    try
                    {
                        File.WriteAllText(path, stringBuilder.ToString(), Encoding.UTF8);
                    }

                    catch(Exception e)
                    {

                    }
                }
            }
        }

        /// <summary>
        /// Writes the results for the Estimates Of Production for ALL THE FARMS and is outputed to a total results folder that we create (using the InfrastructureConstants.BaseOutputDirectoryPath)
        /// </summary>
        public void WriteEstimatesOfProductionToFile()
        {
            #region Setting Up Path For Total Results For All Farms
            var path = InfrastructureConstants.BaseOutputDirectoryPath + @"\" + Properties.Resources.Outputs + @"\" + Properties.Resources.TotalResultsForAllFarms + @"\" + Properties.Resources.TotalResultsEP + CLILanguageConstants.OutputLanguageAddOn;
            var stringBuilder = new StringBuilder();
            var results = new EstimatesOfProductionResults();
            #endregion

            #region Headers
            stringBuilder.AppendLine(String.Format(Properties.Resources.FarmName + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.ComponentCategory + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.ComponentName + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.ComponentGroupName + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.Harvest + _uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms) + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.Key_Area + _uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms) + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.LandAppliedManure + _uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.DefaultBeefInputFolder + _uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms) + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.Lamb + _uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms) + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.Milk + _uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms) + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.FPCM + _uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms) + CLILanguageConstants.Delimiter));

            #endregion

            #region Total Calculations For All Farms

            results.CalculateTotalsForAllFarms(_animalEmissionResultsForAllFarms);

            #endregion

            #region Total Calculations For Each Farm
            var allGroupedComponentsByFarm = _animalEmissionResultsForAllFarms.GroupBy(x => x.Key.Substring(0, x.Key.LastIndexOf('_'))).ToList();
            foreach (var groupedComponentsForAFarm in allGroupedComponentsByFarm)
            {
                var filteredFarmComponents = groupedComponentsForAFarm.SelectMany(x => x.Value.Where(y => y.Component.ComponentType != ComponentType.Rams &&
                                                                                         y.Component.ComponentType != ComponentType.DairyBulls &&
                                                                                         y.Component.ComponentType != ComponentType.DairyDry &&
                                                                                         y.Component.ComponentType != ComponentType.DairyCalf &&
                                                                                         y.Component.ComponentType != ComponentType.DairyHeifer &&
                                                                                         y.Component.ComponentType != ComponentType.DairyBulls &&
                                                                                         y.Component.ComponentCategory != ComponentCategory.OtherLivestock));
                #region Setting Up Current Farm's Name In String Builder
                stringBuilder.AppendLine(groupedComponentsForAFarm.Key);
                #endregion

                #region Calculate Totals For Current Farm
                results.CalculateTotalsForAFarm(filteredFarmComponents);
                #endregion

                #region Total Calculations For Each Component In A Farm
                var allGroupedComponents = filteredFarmComponents.GroupBy(y => y.Component.ComponentCategory).ToList();
                foreach (var componentGroup in allGroupedComponents)
                {
                    #region Calculate Totals For Current Component
                    results.CalculateTotalsForOneComponent(componentGroup);

                    #endregion

                    #region Set Up Current Component Name In String Builder
                    //Farm Name
                    stringBuilder.Append(CLILanguageConstants.Delimiter);
                    //Component Category
                    stringBuilder.AppendLine(componentGroup.Key.GetDescription() + CLILanguageConstants.Delimiter);
                    #endregion

                    #region Animal Component Subgroup Calculations/Output

                    if (componentGroup.Key != ComponentCategory.LandManagement)
                    {
                        var componentAnimalGroups = componentGroup.SelectMany(x => x.EmissionResultsForAllAnimalGroupsInComponent).GroupBy(y => y.AnimalGroup.GroupType).ToList();
                        #region Calculations For Each Animal Group
                        foreach (var animalGroup in componentAnimalGroups)
                        {
                            //Farm Name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Category
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Name
                            stringBuilder.AppendLine(animalGroup.Key.GetDescription() + CLILanguageConstants.Delimiter);
                            results.CalculateTotalsForOneAnimalGroup(animalGroup);

                            foreach (var animalSubGroup in animalGroup)
                            {
                                #region Calculate Animal SubGroup
                                results.CalculateTotalsForOneAnimalSubGroup(animalSubGroup);
                                #endregion

                                var animalType = animalSubGroup.AnimalGroup.GroupType;
                                switch (animalType)
                                {
                                    #region Dairy, Only Lactating Dairy. Land App, MilkProduced, FPCM
                                    case AnimalType.DairyLactatingCow:
                                        {
                                            //Farm Name
                                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                                            //Component Category
                                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                                            //Component Name
                                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                                            //Component Group Name
                                            stringBuilder.Append(animalSubGroup.AnimalGroup.Name + CLILanguageConstants.Delimiter);
                                            //Harvest
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            //Area
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            stringBuilder.Append(Math.Round(results.AnimalSubGroupLandManure, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                            //Beef
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            //Lamb
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            stringBuilder.Append(Math.Round(results.AnimalSubGroupMilkProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                            stringBuilder.Append(Math.Round(results.AnimalSubGroupFatAndProteinCorrectedMilkProduction, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                            stringBuilder.AppendLine();
                                        }
                                        continue;
                                    #endregion

                                    #region Beef. Land App and Beef Produced Is Relevant
                                    case AnimalType.BeefBackgrounderHeifer:
                                    case AnimalType.BeefBulls:
                                    case AnimalType.BeefCowLactating:
                                    case AnimalType.BeefBackgrounderSteer:
                                    case AnimalType.BeefFinishingHeifer:
                                    case AnimalType.BeefFinishingSteer:
                                    case AnimalType.StockerHeifers:
                                    case AnimalType.StockerSteers:
                                    case AnimalType.CowCalf:
                                    case AnimalType.BeefCowDry:
                                        {
                                            //Farm Name
                                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                                            //Component Category
                                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                                            //Component Name
                                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                                            //Component Group Name
                                            stringBuilder.Append(animalSubGroup.AnimalGroup.Name + CLILanguageConstants.Delimiter);
                                            //Harvest
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            //Area
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            stringBuilder.Append(Math.Round(results.AnimalSubGroupLandManure, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                            //We do not calculate Beef Produced for calves!
                                            if (animalType != AnimalType.BeefCalf)
                                            {
                                                stringBuilder.Append(Math.Round(results.AnimalSubGroupBeefProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                            }

                                            else
                                            {
                                                //Beef
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            }
                                            //Lamb
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            //Milk
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            //FPCM
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);

                                            stringBuilder.AppendLine();
                                        }
                                        continue;
                                    #endregion

                                    #region Sheep. Land App, Lamb Produced
                                    case AnimalType.SheepFeedlot:
                                    case AnimalType.Lambs:
                                    case AnimalType.Ewes:
                                    case AnimalType.LambsAndEwes:
                                        {
                                            //Farm Name
                                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                                            //Component Category
                                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                                            //Component Name
                                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                                            //Component Group Name
                                            stringBuilder.Append(animalSubGroup.AnimalGroup.Name + CLILanguageConstants.Delimiter);
                                            //Harvest
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            //Area
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            stringBuilder.Append(Math.Round(results.AnimalSubGroupLandManure, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                            //Beef
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            stringBuilder.Append(Math.Round(results.AnimalSubGroupLambProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                            //Milk
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            //FPCM
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            stringBuilder.AppendLine();
                                        }
                                        continue;
                                    #endregion

                                    #region Poultry and Swine - Only Land Application Is Relevant
                                    case AnimalType.LayersDryPoultry:
                                    case AnimalType.LayersWetPoultry:
                                    case AnimalType.Broilers:
                                    case AnimalType.Turkeys:
                                    case AnimalType.Ducks:
                                    case AnimalType.Geese:
                                    case AnimalType.SwineStarter:
                                    case AnimalType.SwineFinisher:
                                    case AnimalType.SwineGrower:
                                    case AnimalType.SwineLactatingSow:
                                    case AnimalType.SwineBoar:
                                    case AnimalType.SwineDrySow:
                                        {
                                            //Farm Name
                                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                                            //Component Category
                                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                                            //Component Name
                                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                                            //Component Group Name
                                            stringBuilder.Append(animalSubGroup.AnimalGroup.Name + CLILanguageConstants.Delimiter);
                                            //Harvest
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            //Area
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            stringBuilder.Append(Math.Round(results.AnimalSubGroupLandManure, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                            //Beef
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            //Lamb
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            //Milk
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            //FPCM
                                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                            stringBuilder.AppendLine();
                                        }
                                        continue;
                                        #endregion
                                }
                            }
                            stringBuilder.AppendLine();
                            //Farm Name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Category
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Group Name
                            stringBuilder.Append(animalGroup.Key + Properties.Resources.Totals + CLILanguageConstants.Delimiter);
                            //Harvest
                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                            //Area
                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                            //Land
                            stringBuilder.Append(Math.Round(results.AnimalGroupLandManure, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);

                            if (componentGroup.Key == ComponentCategory.BeefProduction)
                            {
                                stringBuilder.Append(Math.Round(results.AnimalGroupBeefProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                //Lamb
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                //Milk
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                //FPCM
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                            }

                            if (componentGroup.Key == ComponentCategory.Sheep)
                            {
                                //Beef
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                stringBuilder.Append(Math.Round(results.AnimalGroupLambProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                //Milk
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                //FPCM
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                stringBuilder.AppendLine();
                            }

                            if (componentGroup.Key == ComponentCategory.Dairy)
                            {
                                //Beef
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                //Lamb
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                stringBuilder.Append(Math.Round(results.AnimalGroupMilkProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                stringBuilder.Append(Math.Round(results.AnimalGroupFatAndProteinCorrectedMilkProduction, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);

                            }

                            if (componentGroup.Key == ComponentCategory.Poultry || componentGroup.Key == ComponentCategory.Swine)
                            {
                                //Beef
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                //Lamb
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                //Milk
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                //FPCM
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                            }
                            stringBuilder.AppendLine();
                            stringBuilder.AppendLine();

                            #endregion
                        }
                        #endregion
                    }
                    #endregion

                    #region Land Management Component Subgroup Calculations/Output
                    else
                    {
                        var landComponentSubGroups = componentGroup.GroupBy(x => x.Component.ComponentType);
                        foreach (var subGroup in landComponentSubGroups)
                        {
                            if (subGroup.Key == ComponentType.Field)
                            {

                            }

                            if (subGroup.Key == ComponentType.Shelterbelt)
                            {

                            }
                        }
                    }
                    #endregion

                    #region Outputs For Current Component In A Farm
                    //Farm Name
                    stringBuilder.Append(CLILanguageConstants.Delimiter);
                    //Component Category
                    stringBuilder.Append(CLILanguageConstants.Delimiter);
                    //Component Name
                    stringBuilder.Append(componentGroup.Key.ToString() + Properties.Resources.Totals + CLILanguageConstants.Delimiter);
                    //Component Group Name
                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                    //Harvest
                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                    //Land
                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                    stringBuilder.Append(Math.Round(results.ComponentLandManure, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                    if (componentGroup.Key == ComponentCategory.BeefProduction)
                    {
                        stringBuilder.Append(Math.Round(results.ComponentBeefProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                        //Lamb
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                        //Milk
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                        //FPCM
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                    }

                    if (componentGroup.Key == ComponentCategory.Sheep)
                    {
                        //Beef
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                        stringBuilder.Append(Math.Round(results.ComponentLambProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                        //Milk
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                        //FPCM
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                    }

                    if (componentGroup.Key == ComponentCategory.Dairy)
                    {
                        //Beef
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                        //Lamb
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                        stringBuilder.Append(Math.Round(results.ComponentMilkProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                        stringBuilder.Append(Math.Round(results.FarmFatAndProteinCorrectedMilkProduction, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                    }

                    if (componentGroup.Key == ComponentCategory.Poultry || componentGroup.Key == ComponentCategory.Swine)
                    {
                        //Beef
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                        //Lamb
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                        //Milk
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                        //FPCM
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                    }
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    #endregion
                }
                #endregion

                #region Outputs For A Farm
                //Farm Name
                stringBuilder.Append(CLILanguageConstants.Delimiter);
                //Component Category
                stringBuilder.Append(CLILanguageConstants.Delimiter);
                //Component Name
                stringBuilder.Append(Properties.Resources.FarmTotal + CLILanguageConstants.Delimiter);
                //Component Group Name
                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                //Harvest
                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                //Area
                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                stringBuilder.Append(Math.Round(results.FarmLandManure, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                stringBuilder.Append(Math.Round(results.FarmBeefProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                stringBuilder.Append(Math.Round(results.FarmLambProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                stringBuilder.Append(Math.Round(results.FarmMilkProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                stringBuilder.Append(Math.Round(results.FarmFatAndProteinCorrectedMilkProduction, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                #endregion
            }


            #region Outputs For All Farms
            //Farm Name
            stringBuilder.Append(Properties.Resources.AllFarms + CLILanguageConstants.Delimiter);
            //Component Category
            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
            //Component Name
            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
            //Component Group Name
            stringBuilder.Append(Properties.Resources.Totals + CLILanguageConstants.Delimiter);
            //Harvest
            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
            //Area
            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
            stringBuilder.Append(Math.Round(results.AllFarmsLandManure, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
            stringBuilder.Append(Math.Round(results.AllFarmsBeefProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
            stringBuilder.Append(Math.Round(results.AllFarmsLambProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
            stringBuilder.Append(Math.Round(results.AllFarmsMilkProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
            stringBuilder.Append(Math.Round(results.AllFarmsFatAndProteinCorrectedMilkProduction, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();

            #endregion

            try
            {
                File.WriteAllText(path, stringBuilder.ToString(), Encoding.UTF8);
            }

            catch
            {

            }
        }

        /// <summary>
        /// Writes the Estimates Of Production results for EACH FARM and for each Farm's settings file and is outputed to the
        /// appropriate farm's directory. 
        /// </summary>
        public void WriteEstimatesOfProductionToFileByMonth()
        {
            #region Setting Up Path For Total Results For All Farms
            string path = string.Empty;
            var stringBuilder = new StringBuilder();
            var results = new EstimatesOfProductionResults();
            #endregion

            #region Headers
            stringBuilder.AppendLine(String.Format(Properties.Resources.FarmName + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.ComponentCategory + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.ComponentName + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.ComponentGroupName + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.Month + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.Harvest + _uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms) + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.Key_Area + _uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms) + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.LandAppliedManure + _uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.DefaultBeefInputFolder + _uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms) + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.Lamb + _uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms) + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.Milk + _uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms) + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.FPCM + _uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms) + CLILanguageConstants.Delimiter));

            #endregion

            var allGroupedComponentsByFarm = _animalEmissionResultsForAllFarms.GroupBy(x => x.Key).ToList();
            foreach (var groupedComponentsForAFarm in allGroupedComponentsByFarm)
            {
                #region Setting Up Farm Path
                var splitFarmNameToSettingsFileToOutputPath = groupedComponentsForAFarm.Key.Split('_');
                var farmName = splitFarmNameToSettingsFileToOutputPath[0];
                var farmSettingsFile = splitFarmNameToSettingsFileToOutputPath[1];

                path = InfrastructureConstants.BaseOutputDirectoryPath + @"\" + Properties.Resources.Outputs + @"\" + farmName + Properties.Resources.Results + @"\" + farmName + Properties.Resources.FarmResultsEP + farmSettingsFile + CLILanguageConstants.OutputLanguageAddOn;
                #endregion

                var filteredFarmComponents = groupedComponentsForAFarm.SelectMany(x => x.Value.Where(y => y.Component.ComponentType != ComponentType.Rams &&
                                                                                         y.Component.ComponentType != ComponentType.DairyBulls &&
                                                                                         y.Component.ComponentType != ComponentType.DairyDry &&
                                                                                         y.Component.ComponentType != ComponentType.DairyCalf &&
                                                                                         y.Component.ComponentType != ComponentType.DairyHeifer &&
                                                                                         y.Component.ComponentCategory != ComponentCategory.OtherLivestock));
                #region Setting Up Current Farm's Name In String Builder
                stringBuilder.AppendLine(farmName + "_" + farmSettingsFile);
                #endregion

                #region Calculate Totals For Current Farm
                results.CalculateTotalsForAFarm(filteredFarmComponents);

                #endregion

                #region Total Calculations For Each Component In A Farm
                var allGroupedComponents = filteredFarmComponents.GroupBy(y => y.Component.ComponentCategory).ToList();
                foreach (var componentGroup in allGroupedComponents)
                {
                    #region Calculate Totals For Current Component
                    results.CalculateTotalsForOneComponent(componentGroup);

                    #endregion

                    #region Set Up Current Component Name In String Builder
                    stringBuilder.Append(CLILanguageConstants.Delimiter);
                    //This is the Component Category. Such as Dairy, Poultry, Sheep, etc.
                    stringBuilder.AppendLine(componentGroup.Key.GetDescription() + CLILanguageConstants.Delimiter);
                    #endregion

                    #region Animal Component Group Calculations/Output

                    if (componentGroup.Key != ComponentCategory.LandManagement)
                    {
                        var componentAnimalGroups = componentGroup.SelectMany(x => x.EmissionResultsForAllAnimalGroupsInComponent).GroupBy(y => y.AnimalGroup.GroupType).ToList();
                        #region Calculations For Each Animal Group
                        foreach (var animalGroup in componentAnimalGroups)
                        {
                            results.CalculateTotalsForOneAnimalGroup(animalGroup);
                            //Farm Name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Category
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Name
                            stringBuilder.AppendLine(animalGroup.Key.GetDescription() + CLILanguageConstants.Delimiter);

                            foreach (var animalSubGroup in animalGroup)
                            {
                                var emissionsForAnimalSubGroupByMonth = animalSubGroup.GroupEmissionsByMonths.GroupBy(x => x.Month);
                                //Farm Name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                //Component Category
                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                //Coomponent Name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                //Component Group Name
                                stringBuilder.Append(animalSubGroup.AnimalGroup.Name + CLILanguageConstants.Delimiter);
                                stringBuilder.AppendLine();
                                #region Monthly Emissions Calculations And Output For Each Animal Group
                                results.CalculateMonthlyTotals(emissionsForAnimalSubGroupByMonth);
                                foreach (var monthlyEmission in emissionsForAnimalSubGroupByMonth)
                                {
                                    var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthlyEmission.Key);
                                    results.CalculateMonthlyEmissionsForAnimalGroup(monthlyEmission);

                                    var animalType = animalSubGroup.AnimalGroup.GroupType;
                                    switch (animalType)
                                    {
                                        #region Dairy, Only Lactating Dairy. Land App, MilkProduced, FPCM
                                        case AnimalType.DairyLactatingCow:
                                            {
                                                //Farm Name
                                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                                //Component Category
                                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                                //Component Name
                                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                                //Component Group Name
                                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                                //Month Name
                                                stringBuilder.Append(monthName + CLILanguageConstants.Delimiter);
                                                //Harvest
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                //Area
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                stringBuilder.Append(Math.Round(results.MonthlyLandManure, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                                //Beef
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                //Lamb
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                stringBuilder.Append(Math.Round(results.MonthlyMilkProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                                stringBuilder.Append(Math.Round(results.MonthlyFatAndProteinCorrectedMilkProduction, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                                stringBuilder.AppendLine();

                                            }
                                            continue;
                                        #endregion

                                        #region Beef. Land App and Beef Produced Is Relevant
                                        case AnimalType.BeefBackgrounderHeifer:
                                        case AnimalType.BeefBulls:
                                        case AnimalType.BeefCowLactating:
                                        case AnimalType.BeefBackgrounderSteer:
                                        case AnimalType.BeefFinishingHeifer:
                                        case AnimalType.BeefFinishingSteer:
                                        case AnimalType.StockerHeifers:
                                        case AnimalType.StockerSteers:
                                        case AnimalType.CowCalf:
                                        case AnimalType.BeefCowDry:
                                            {
                                                //Farm Name
                                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                                //Component Category
                                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                                //Component Name
                                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                                //Component Group Name
                                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                                //Month Name
                                                stringBuilder.Append(monthName + CLILanguageConstants.Delimiter);
                                                //Harvest
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                //Area
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                //Land
                                                stringBuilder.Append(Math.Round(results.MonthlyLandManure, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                                //We do not output beef produced for cow calves!
                                                if (animalType != AnimalType.BeefCalf)
                                                {
                                                    stringBuilder.Append(Math.Round(results.MonthlyBeefProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                                }

                                                else
                                                {
                                                    //Beef
                                                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                }
                                                //Lamb
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                //Milk
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                //FPCM
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);

                                                stringBuilder.AppendLine();
                                            }
                                            continue;
                                        #endregion

                                        #region Sheep. Land App, Lamb Produced
                                        case AnimalType.SheepFeedlot:
                                        case AnimalType.LambsAndEwes:
                                            {

                                                //Farm Name
                                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                                //Component Category
                                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                                //Component Name
                                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                                //Component Group Name
                                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                                //Month Name
                                                stringBuilder.Append(monthName + CLILanguageConstants.Delimiter);
                                                //Harvest
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                //Area
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                //Land
                                                stringBuilder.Append(Math.Round(results.MonthlyLandManure, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                                //Beef
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                //Lamb
                                                stringBuilder.Append(Math.Round(results.MonthlyLambProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                                //Milk
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                //FPCM
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                stringBuilder.AppendLine();
                                            }
                                            continue;
                                        #endregion

                                        #region Poultry and Swine - Only Land Application Is Relevant
                                        case AnimalType.LayersDryPoultry:
                                        case AnimalType.LayersWetPoultry:
                                        case AnimalType.Broilers:
                                        case AnimalType.Turkeys:
                                        case AnimalType.Ducks:
                                        case AnimalType.Geese:
                                        case AnimalType.SwineStarter:
                                        case AnimalType.SwineFinisher:
                                        case AnimalType.SwineGrower:
                                        case AnimalType.SwineLactatingSow:
                                        case AnimalType.SwineBoar:
                                        case AnimalType.SwineDrySow:
                                            {
                                                //Farm Name
                                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                                //Component Category
                                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                                //Component Name
                                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                                //Component Group Name
                                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                                //Month Name
                                                stringBuilder.Append(monthName + CLILanguageConstants.Delimiter);
                                                //Harvest
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                //Area
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                //Land
                                                stringBuilder.Append(Math.Round(results.MonthlyLandManure, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                                //Beef
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                //Lamb
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                //Milk
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                                //FPCM
                                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);

                                                stringBuilder.AppendLine();
                                            }
                                            continue;
                                            #endregion
                                    }
                                }
                                //Farm Name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                //Component Category
                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                //Component Name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                //Component Group Name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                //Month Name
                                stringBuilder.Append(Properties.Resources.AllMonths + CLILanguageConstants.Delimiter);
                                //Harvest
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                //Area
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                stringBuilder.Append(Math.Round(results.TotalMonthlyLandManure, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                if (componentGroup.Key == ComponentCategory.BeefProduction)
                                {
                                    if (animalGroup.Key == AnimalType.CowCalf)
                                    {
                                        //Beef
                                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                        //Lamb
                                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                        //Milk
                                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                        //FPCM
                                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                        stringBuilder.AppendLine();
                                        stringBuilder.AppendLine();
                                        continue;
                                    }

                                    stringBuilder.Append(Math.Round(results.TotalMonthlyBeefProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                    //Lamb
                                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                    //Milk
                                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                    //FPCM
                                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                }

                                if (componentGroup.Key == ComponentCategory.Dairy)
                                {
                                    //Beef
                                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                    //Lamb
                                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                    stringBuilder.Append(Math.Round(results.TotalMonthlyMilkProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                    stringBuilder.Append(Math.Round(results.TotalMonthlyFatAndProteinCorrectedMilkProduction, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                }

                                if (componentGroup.Key == ComponentCategory.Sheep)
                                {
                                    //Beef
                                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                    stringBuilder.Append(Math.Round(results.TotalMonthlyBeefProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                    //Milk
                                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                    //FPCM
                                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                }

                                if (componentGroup.Key == ComponentCategory.Poultry ||
                                      componentGroup.Key == ComponentCategory.Swine)
                                {
                                    //Beef
                                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                    //Lamb
                                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                    //Milk
                                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                    //FPCM
                                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                }

                                stringBuilder.AppendLine();
                                stringBuilder.AppendLine();
                            }
                            #endregion
                            //Farm Name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Category
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Group Name
                            stringBuilder.Append(animalGroup.Key + Properties.Resources.Totals + CLILanguageConstants.Delimiter);
                            //Month
                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                            //Harvest
                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                            //Area
                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                            stringBuilder.Append(Math.Round(results.AnimalGroupLandManure, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);

                            if (componentGroup.Key == ComponentCategory.BeefProduction)
                            {
                                if (animalGroup.Key == AnimalType.CowCalf)
                                {
                                    //Beef
                                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                    //Lamb
                                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                    //Milk
                                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                    //FPCM
                                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                    stringBuilder.AppendLine();
                                    stringBuilder.AppendLine();
                                    continue;
                                }
                                stringBuilder.Append(Math.Round(results.AnimalGroupBeefProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                //Lamb
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                //Milk
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                //FPCM
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);

                            }

                            if (componentGroup.Key == ComponentCategory.Sheep)
                            {
                                //Beef
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                stringBuilder.Append(Math.Round(results.AnimalGroupLambProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                //Milk
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                //FPCM
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                            }

                            if (componentGroup.Key == ComponentCategory.Dairy)
                            {
                                //Beef
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                //Lamb
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                stringBuilder.Append(Math.Round(results.AnimalGroupMilkProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                stringBuilder.Append(Math.Round(results.AnimalGroupFatAndProteinCorrectedMilkProduction, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                            }

                            if (componentGroup.Key == ComponentCategory.Poultry ||
                                      componentGroup.Key == ComponentCategory.Swine)

                            {
                                //Beef
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                //Lamb
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                //Milk
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                                //FPCM
                                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                            }

                            stringBuilder.AppendLine();
                            stringBuilder.AppendLine();
                        }
                        #endregion
                    }
                    #endregion

                    #region Land Management Component Subgroup Calculations/Output
                    else
                    {
                        var landComponentSubGroups = componentGroup.GroupBy(x => x.Component.ComponentType);
                        foreach (var subGroup in landComponentSubGroups)
                        {
                            if (subGroup.Key == ComponentType.Field)
                            {

                            }

                            if (subGroup.Key == ComponentType.Shelterbelt)
                            {

                            }
                        }
                    }
                    #endregion

                    #region Outputs For Current Component In A Farm
                    //Farm Name
                    stringBuilder.Append(CLILanguageConstants.Delimiter);
                    //Component Category
                    stringBuilder.Append(CLILanguageConstants.Delimiter);
                    //Component Name
                    stringBuilder.Append(componentGroup.Key.ToString() + Properties.Resources.Totals + CLILanguageConstants.Delimiter);
                    //Component Group Name
                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                    //Month
                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                    //Harvest
                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                    //Area
                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                    stringBuilder.Append(Math.Round(results.ComponentLandManure, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);

                    if (componentGroup.Key == ComponentCategory.BeefProduction)
                    {
                        stringBuilder.Append(Math.Round(results.ComponentBeefProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                        //Lamb
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                        //Milk
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                        //FPCM
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                    }

                    if (componentGroup.Key == ComponentCategory.Sheep)
                    {
                        //Beef
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                        stringBuilder.Append(Math.Round(results.ComponentLambProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                        //Milk
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                        //FPCM
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                    }

                    if (componentGroup.Key == ComponentCategory.Dairy)
                    {
                        //Beef
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                        //Lamb
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                        stringBuilder.Append(Math.Round(results.ComponentMilkProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                        stringBuilder.Append(Math.Round(results.ComponentFatAndProteinCorrectedMilkProduction, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                    }

                    if (componentGroup.Key == ComponentCategory.Poultry ||
                                       componentGroup.Key == ComponentCategory.Swine)
                    {
                        //Beef
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                        //Lamb
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                        //Milk
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                        //FPCM
                        stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                    }

                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    #endregion
                }
                #endregion

                #region Outputs For A Farm
                //Farm Name
                stringBuilder.Append(CLILanguageConstants.Delimiter);
                //Component Category
                stringBuilder.Append(CLILanguageConstants.Delimiter);
                //Component Name
                stringBuilder.Append(Properties.Resources.FarmTotal + CLILanguageConstants.Delimiter);
                //Component Group Name
                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                //Month
                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                //Harvest
                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                //Area
                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                stringBuilder.Append(Math.Round(results.FarmLandManure, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                stringBuilder.Append(Math.Round(results.FarmBeefProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                stringBuilder.Append(Math.Round(results.FarmLambProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                stringBuilder.Append(Math.Round(results.FarmMilkProduced, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                stringBuilder.Append(Math.Round(results.FarmFatAndProteinCorrectedMilkProduction, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();

                try
                {
                    File.WriteAllText(path, stringBuilder.ToString(), Encoding.UTF8);
                }

                catch
                {

                }
                #endregion
            }
        }

        /// <summary>
        /// Writes the results for the Feed Estimates results for ALL THE FARMS and is outputed to a total results folder that we create (using the InfrastructureConstants.BaseOutputDirectoryPath)
        /// </summary>
        public void WriteFeedEstimatesToFile()
        {
            #region Setting Up Path For Total Results For All Farms
            var path = InfrastructureConstants.BaseOutputDirectoryPath + @"\" + Properties.Resources.Outputs + @"\" + Properties.Resources.TotalResultsForAllFarms + @"\" + Properties.Resources.TotalResultsFE + CLILanguageConstants.OutputLanguageAddOn;
            var stringBuilder = new StringBuilder();
            var results = new FeedEstimateResults();
            #endregion

            #region Headers
            stringBuilder.AppendLine(String.Format(Properties.Resources.FarmName + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.ComponentCategory + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.ComponentName + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.ComponentGroupName + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.DryMatterIntake + _uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.KilogramPerHeadPerDay)));


            #endregion

            #region Total Calculations For All Farms

            results.CalculateTotalsForAllFarms(_animalEmissionResultsForAllFarms);

            #endregion

            #region Total Calculations For Each Farm
            var allGroupedComponentsByFarm = _animalEmissionResultsForAllFarms.GroupBy(x => x.Key.Substring(0, x.Key.LastIndexOf('_'))).ToList();
            foreach (var groupedComponentsForAFarm in allGroupedComponentsByFarm)
            {
                var filteredFarmComponents = groupedComponentsForAFarm.SelectMany(x => x.Value.Where(y => y.Component.ComponentCategory == ComponentCategory.Sheep ||
                                                                                              y.Component.ComponentCategory == ComponentCategory.BeefProduction ||
                                                                                              y.Component.ComponentCategory == ComponentCategory.Dairy));
                #region Setting Up Current Farm's Name In String Builder
                stringBuilder.AppendLine(groupedComponentsForAFarm.Key);
                #endregion

                #region Calculate Totals For Current Farm
                results.CalculateTotalsForAFarm(filteredFarmComponents);

                #endregion

                #region Total Calculations For Each Component In A Farm
                var allGroupedComponents = filteredFarmComponents.GroupBy(y => y.Component.ComponentCategory).ToList();
                foreach (var componentCategoryGroup in allGroupedComponents)
                {
                    #region Calculate Totals For Current Component
                    results.CalculateTotalsForOneComponent(componentCategoryGroup);

                    #endregion

                    #region Set Up Current Component Name In String Builder
                    //Farm Name
                    stringBuilder.Append(CLILanguageConstants.Delimiter);
                    //Component Category
                    stringBuilder.AppendLine(componentCategoryGroup.Key.GetDescription() + CLILanguageConstants.Delimiter);
                    #endregion

                    #region Animal Group Calculations/Output

                    if (componentCategoryGroup.Key != ComponentCategory.LandManagement)
                    {
                        var componentAnimalGroups = componentCategoryGroup.SelectMany(x => x.EmissionResultsForAllAnimalGroupsInComponent).GroupBy(y => y.AnimalGroup.GroupType).ToList();
                        #region Calculations For Each Animal Group
                        foreach (var animalGroup in componentAnimalGroups)
                        {
                            results.CalculateTotalsForOneAnimalGroup(animalGroup);
                            //Farm Name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Category
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Name
                            stringBuilder.AppendLine(animalGroup.Key.GetDescription() + CLILanguageConstants.Delimiter);

                            foreach (var animalSubGroup in animalGroup)
                            {
                                #region Calculate Animal SubGroup
                                results.CalculateTotalsForOneAnimalSubGroup(animalSubGroup);
                                #endregion
                                //Farm Name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                //Component Category
                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                //Component Name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                stringBuilder.Append(animalSubGroup.AnimalGroup.Name + CLILanguageConstants.Delimiter);
                                stringBuilder.Append(Math.Round(results.AnimalSubGroupDryMatterIntake, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                stringBuilder.AppendLine();
                            }

                            stringBuilder.AppendLine();
                            //Farm Name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Category
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            stringBuilder.Append(animalGroup.Key + Properties.Resources.Totals + CLILanguageConstants.Delimiter);
                            stringBuilder.Append(Math.Round(results.AnimalGroupDryMatterIntake, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                            stringBuilder.AppendLine();
                            stringBuilder.AppendLine();
                        }
                        #endregion
                    }
                    #endregion

                    #region Land Management Component Subgroup Calculations/Output
                    else
                    {
                        var landComponentSubGroups = componentCategoryGroup.GroupBy(x => x.Component.ComponentType);
                        foreach (var subGroup in landComponentSubGroups)
                        {
                            if (subGroup.Key == ComponentType.Field)
                            {

                            }

                            if (subGroup.Key == ComponentType.Shelterbelt)
                            {

                            }
                        }
                    }
                    #endregion

                    #region Outputs For Current Component In A Farm
                    //Farm Name
                    stringBuilder.Append(CLILanguageConstants.Delimiter);
                    //Component Category
                    stringBuilder.Append(CLILanguageConstants.Delimiter);
                    //Component Name
                    stringBuilder.Append(CLILanguageConstants.Delimiter);
                    stringBuilder.Append(componentCategoryGroup.Key.ToString() + Properties.Resources.Totals + CLILanguageConstants.Delimiter);
                    stringBuilder.Append(Math.Round(results.ComponentDryMatterIntake, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    #endregion
                }
                #endregion

                #region Outputs For A Farm
                //Farm Name
                stringBuilder.Append(CLILanguageConstants.Delimiter);
                //Component Category
                stringBuilder.Append(CLILanguageConstants.Delimiter);
                //Component Name
                stringBuilder.Append(CLILanguageConstants.Delimiter);
                //Component Group Name
                stringBuilder.Append(Properties.Resources.FarmTotal + CLILanguageConstants.Delimiter);
                stringBuilder.Append(Math.Round(results.FarmDryMatterIntake, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                #endregion
            }
            #endregion

            #region Outputs For All Farms
            stringBuilder.Append(Properties.Resources.AllFarms + Properties.Resources.Totals + CLILanguageConstants.Delimiter);
            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
            stringBuilder.Append(Properties.Resources.Totals + CLILanguageConstants.Delimiter);
            stringBuilder.Append(Math.Round(results.AllFarmsDryMatterIntake, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            #endregion

            try
            {
                File.WriteAllText(path, stringBuilder.ToString(), Encoding.UTF8);
            }

            catch
            {

            }
        }

        /// <summary>
        /// Writes the Estimates Of Production results for EACH FARM and for each Farm's settings file and is outputed to the
        /// appropriate farm's directory.
        /// </summary>
        public void WriteFeedEstimatesToFileMonthly()
        {
            #region Setting Up Path For Total Results For All Farms
            var stringBuilder = new StringBuilder();
            #endregion

            #region Headers
            stringBuilder.AppendLine(String.Format(Properties.Resources.FarmName + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.ComponentCategory + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.ComponentName + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.ComponentGroupName + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.Month + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.DryMatterIntake + _uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.KilogramPerHeadPerDay)));


            #endregion

            #region Total Calculations For Each Farm
            var allGroupedComponentsByFarm = _animalEmissionResultsForAllFarms.GroupBy(x => x.Key).ToList();
            foreach (var groupedComponentsForAFarm in allGroupedComponentsByFarm)
            {
                var results = new FeedEstimateResults();
                var splitFarmNameToSettingsFileToOutputPath = groupedComponentsForAFarm.Key.Split('_');
                var farmName = splitFarmNameToSettingsFileToOutputPath[0];
                var farmSettingsFile = splitFarmNameToSettingsFileToOutputPath[1];

                var path = InfrastructureConstants.BaseOutputDirectoryPath + @"\" + Properties.Resources.Outputs + @"\" + farmName + Properties.Resources.Results + @"\" + farmName + Properties.Resources.FarmResultsFE + farmSettingsFile + CLILanguageConstants.OutputLanguageAddOn;

                var filteredFarmComponents = groupedComponentsForAFarm.SelectMany(x => x.Value.Where(y => y.Component.ComponentCategory == ComponentCategory.Sheep ||
                                                                                                      y.Component.ComponentCategory == ComponentCategory.BeefProduction ||
                                                                                                      y.Component.ComponentCategory == ComponentCategory.Dairy));
                #region Setting Up Current Farm's Name In String Builder
                stringBuilder.AppendLine(farmName + "_" + farmSettingsFile);
                #endregion

                #region Calculate Totals For Current Farm
                results.CalculateTotalsForAFarm(filteredFarmComponents);

                #endregion

                #region Total Calculations For Each Component In A Farm
                var allGroupedComponents = filteredFarmComponents.GroupBy(y => y.Component.ComponentCategory).ToList();
                foreach (var componentGroup in allGroupedComponents)
                {
                    #region Calculate Totals For Current Component
                    results.CalculateTotalsForOneComponent(componentGroup);

                    #endregion

                    #region Set Up Current Component Name In String Builder
                    //Farm Name
                    stringBuilder.Append(CLILanguageConstants.Delimiter);
                    //Component Category
                    stringBuilder.AppendLine(componentGroup.Key.GetDescription() + CLILanguageConstants.Delimiter);
                    #endregion

                    #region Animal Group Calculations/Output

                    if (componentGroup.Key != ComponentCategory.LandManagement)
                    {
                        var componentAnimalGroups = componentGroup.SelectMany(x => x.EmissionResultsForAllAnimalGroupsInComponent).GroupBy(y => y.AnimalGroup.GroupType).ToList();
                        #region Calculations For Each Animal Group
                        foreach (var animalGroup in componentAnimalGroups)
                        {
                            //Farm Name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Category
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Name
                            stringBuilder.AppendLine(animalGroup.Key.GetDescription() + CLILanguageConstants.Delimiter);

                            results.CalculateTotalsForOneAnimalGroup(animalGroup);

                            foreach (var animalSubGroup in animalGroup)
                            {
                                //Farm Name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                //Component Category
                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                //Component Name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                //Component Group Name
                                stringBuilder.Append(animalSubGroup.AnimalGroup.Name + CLILanguageConstants.Delimiter);
                                stringBuilder.AppendLine();
                                var emissionsForAnimalSubGroupByMonth = animalSubGroup.GroupEmissionsByMonths.GroupBy(x => x.Month);

                                #region Calculate Animal SubGroup
                                results.CalculateTotalsForOneAnimalSubGroup(animalSubGroup);
                                #endregion

                                #region Monthly Emissions Calculations And Output For Each Animal Group
                                //results.CalculateMonthlyTotals(emissionsForAnimalSubGroupByMonth);
                                foreach (var monthlyEmission in emissionsForAnimalSubGroupByMonth)
                                {
                                    var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthlyEmission.Key);
                                    results.CalculateMonthlyEmissionsForAnimalGroup(monthlyEmission);

                                    //Farm Name
                                    stringBuilder.Append(CLILanguageConstants.Delimiter);
                                    //Component Category
                                    stringBuilder.Append(CLILanguageConstants.Delimiter);
                                    //Component Name
                                    stringBuilder.Append(CLILanguageConstants.Delimiter);
                                    //Component Group Name
                                    stringBuilder.Append(CLILanguageConstants.Delimiter);
                                    //Month Name
                                    stringBuilder.Append(monthName + CLILanguageConstants.Delimiter);
                                    stringBuilder.Append(Math.Round(results.MonthlyDryMatterIntake, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                    stringBuilder.AppendLine();
                                }
                                stringBuilder.AppendLine();
                                //Farm Name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                //Component Category
                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                //Component Name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                //Component Group Name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);
                                //Month Name
                                stringBuilder.Append(Properties.Resources.AllMonths + CLILanguageConstants.Delimiter);
                                stringBuilder.Append(Math.Round(results.AnimalSubGroupDryMatterIntake, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                                stringBuilder.AppendLine();
                                stringBuilder.AppendLine();
                            }
                            #endregion
                            //Farm Name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Category
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);
                            //Component Group Name
                            stringBuilder.Append(animalGroup.Key + Properties.Resources.Totals + CLILanguageConstants.Delimiter);
                            //Month Name
                            stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                            stringBuilder.Append(Math.Round(results.AnimalGroupDryMatterIntake, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                            stringBuilder.AppendLine();
                            stringBuilder.AppendLine();
                        }
                        #endregion
                    }
                    #endregion

                    #region Land Management Component Subgroup Calculations/Output
                    else
                    {
                        var landComponentSubGroups = componentGroup.GroupBy(x => x.Component.ComponentType);
                        foreach (var subGroup in landComponentSubGroups)
                        {
                            if (subGroup.Key == ComponentType.Field)
                            {

                            }

                            if (subGroup.Key == ComponentType.Shelterbelt)
                            {

                            }
                        }
                    }
                    #endregion

                    #region Outputs For Current Component In A Farm 
                    //Farm Name
                    stringBuilder.Append(CLILanguageConstants.Delimiter);
                    //Component Category
                    stringBuilder.Append(CLILanguageConstants.Delimiter);
                    //Component Name
                    stringBuilder.Append(CLILanguageConstants.Delimiter);
                    stringBuilder.Append(componentGroup.Key.ToString() + Properties.Resources.Totals + CLILanguageConstants.Delimiter);
                    //Month
                    stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                    stringBuilder.Append(Math.Round(results.ComponentDryMatterIntake, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    #endregion
                }
                #endregion

                #region Outputs For A Farm
                stringBuilder.AppendLine();
                //Farm Name
                stringBuilder.Append(CLILanguageConstants.Delimiter);
                //Component Category
                stringBuilder.Append(CLILanguageConstants.Delimiter);
                //Component Name
                stringBuilder.Append(CLILanguageConstants.Delimiter);
                stringBuilder.Append(Properties.Resources.FarmTotal + CLILanguageConstants.Delimiter);
                //Month
                stringBuilder.Append("N/A" + CLILanguageConstants.Delimiter);
                stringBuilder.Append(Math.Round(results.FarmDryMatterIntake, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                #endregion

                try
                {
                    File.WriteAllText(path, stringBuilder.ToString(), Encoding.UTF8);
                }

                catch
                {

                }
                #endregion
            }
        }

        public void WriteFeedEstimatesToFileMonthly_NEW()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(String.Format(Properties.Resources.FarmName + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.ComponentCategory + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.ComponentName + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.ComponentGroupName + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.Month + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.Year + CLILanguageConstants.Delimiter +
                                                   Properties.Resources.DryMatterIntake + _uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.KilogramPerHeadPerDay)));

            foreach (var farmEmissionResult in _farmEmissionResults)
            {
                
                var farmName = farmEmissionResult.Farm.Name;
                var settingsFileName = farmEmissionResult.Farm.SettingsFileName;
                var path = InfrastructureConstants.BaseOutputDirectoryPath + @"\" + Properties.Resources.Outputs + @"\" + farmName + Properties.Resources.Results + @"\" + farmName + Properties.Resources.FarmResultsFE + settingsFileName + CLILanguageConstants.OutputLanguageAddOn;

                // Farm name
                stringBuilder.AppendLine(farmName + "_" + settingsFileName);

                // Output results each grouped category
                foreach (var animalComponentEmissionsResultGroupedByComponentCategory in farmEmissionResult.AnimalComponentEmissionsResults.GroupBy(result => result.Component.ComponentCategory))
                {
                    // Farm name
                    stringBuilder.Append(CLILanguageConstants.Delimiter);

                    var componentType = animalComponentEmissionsResultGroupedByComponentCategory.Key.GetDescription();
                    stringBuilder.AppendLine(componentType + CLILanguageConstants.Delimiter);

                    // Iterate over each animal component
                    foreach (var animalComponentEmissionsResult in animalComponentEmissionsResultGroupedByComponentCategory)
                    {
                        // Farm name
                        stringBuilder.Append(CLILanguageConstants.Delimiter);

                        // Component type
                        stringBuilder.Append(CLILanguageConstants.Delimiter);

                        // Component name
                        stringBuilder.AppendLine(animalComponentEmissionsResult.Component.Name + CLILanguageConstants.Delimiter);

                        // Iterate over each group in the animal component
                        foreach (var animalGroupEmissionResults in animalComponentEmissionsResult.EmissionResultsForAllAnimalGroupsInComponent)
                        {
                            // Farm name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Component type
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Component name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Group name
                            stringBuilder.AppendLine(animalGroupEmissionResults.AnimalGroup.Name + CLILanguageConstants.Delimiter);

                            // Iterate over each month
                            foreach (var groupEmissionsByMonth in animalGroupEmissionResults.GroupEmissionsByMonths)
                            {
                                // Farm name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);

                                // Component type
                                stringBuilder.Append(CLILanguageConstants.Delimiter);

                                // Component name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);

                                // Group name
                                stringBuilder.Append(CLILanguageConstants.Delimiter);

                                // Month
                                stringBuilder.Append(groupEmissionsByMonth.MonthString + CLILanguageConstants.Delimiter);

                                // Year
                                stringBuilder.Append(groupEmissionsByMonth.Year + CLILanguageConstants.Delimiter);

                                // DMI (feed intake)
                                stringBuilder.AppendLine(Math.Round(groupEmissionsByMonth.DryMatterIntake, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                            }

                            // Total for this animal group...

                            // Farm name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Component type
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Component name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Group name
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Month
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Year
                            stringBuilder.Append(CLILanguageConstants.Delimiter);

                            // Total
                            stringBuilder.AppendLine(Math.Round(animalGroupEmissionResults.TotalDryMatterIntake, roundingDigits).ToString(CLILanguageConstants.culture) + CLILanguageConstants.Delimiter);
                            stringBuilder.AppendLine();
                        }
                    }
                }

                try
                {
                    File.WriteAllText(path, stringBuilder.ToString(), Encoding.UTF8);
                }

                catch (Exception e)
                {

                }
            }
        }

        #region Private Methods

        /// <summary>
        /// Gets the headers for monthly results
        /// </summary>
        public string GetHeadersEachFarmMonthly(ApplicationData applicationData, EmissionDisplayUnits type)
        {
            switch (type)
            {
                case EmissionDisplayUnits.MegagramsCO2e:
                    return String.Format(Properties.Resources.FarmName + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.ComponentCategory + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.ComponentName + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.GroupName + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.Month + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.Year + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.EntericCH4 + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.ManureCH4 + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.DirectN2O + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.IndirectN2O + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.EnergyCO2 + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.CO2 + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.SubTotal + CLILanguageConstants.Delimiter, applicationData.DisplayUnitStrings.MegagramsCO2e);
                case EmissionDisplayUnits.KilogramsGhgs:
                    return String.Format(Properties.Resources.FarmName + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.ComponentCategory + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.ComponentName + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.GroupName + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.Month + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.Year + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.EntericCH4 + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.ManureCH4 + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.DirectN2O + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.IndirectN2O + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.EnergyCO2 + CLILanguageConstants.Delimiter +
                                                           // Note that there is no subtotal column in the GHG report (only the CO2e report)
                                                           Properties.Resources.CO2 + CLILanguageConstants.Delimiter, applicationData.DisplayUnitStrings.KilogramsGhgs);
                default:
                    throw new NotImplementedException();
            }
        }


        /// <summary>
        /// Gets the headers for yearly results based on the type
        /// </summary>
        public string GetHeadersAllFarms(ApplicationData applicationData, EmissionDisplayUnits type)
        {
            switch (type)
            {
                case EmissionDisplayUnits.MegagramsCO2e:
                    return
                    String.Format(Properties.Resources.FarmName + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.ComponentCategory + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.ComponentName + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.ComponentGroupName + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.EntericCH4 + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.ManureCH4 + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.DirectN2O + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.IndirectN2O + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.EnergyCO2 + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.CO2 + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.SubTotal + CLILanguageConstants.Delimiter, applicationData.DisplayUnitStrings.MegagramsCO2e);
                case EmissionDisplayUnits.KilogramsGhgs:
                    return
                       String.Format(Properties.Resources.FarmName + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.ComponentCategory + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.ComponentName + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.ComponentGroupName + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.EntericCH4 + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.ManureCH4 + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.DirectN2O + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.IndirectN2O + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.EnergyCO2 + CLILanguageConstants.Delimiter +
                                                       // Note that there is no subtotal column in the GHG report (only the CO2e report)
                                                       Properties.Resources.CO2 + CLILanguageConstants.Delimiter, applicationData.DisplayUnitStrings.KilogramsGhgs);

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}