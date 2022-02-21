using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using H.CLI.UserInput;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Providers;
using H.Core.Providers.Climate;

namespace H.CLI.FileAndDirectoryAccessors
{
    public class SettingsHandler
    {
        #region Fields

        private readonly DirectoryHandler _directoryHandler = new DirectoryHandler();
        private readonly UnitsOfMeasurementCalculator _unitsOfMeasurementCalculator = new UnitsOfMeasurementCalculator();
        private readonly SlcClimateDataProvider _slcClimateDataProvider = new SlcClimateDataProvider();
        public List<int> PolygonIDList { get; set; } = new List<int>();

        #endregion

        #region Public Methods

        public void InitializePolygonIDList(GeographicDataProvider geographicDataProvider)
        {
            PolygonIDList = geographicDataProvider.GetPolygonIdList().ToList();
        }

        /// <summary>
        /// Takes in a dictionary that corresponds to the key for the settings file and the value as a string as well as a reference to
        /// the applicationData being passed by referenced from the main program. I.E. any changes to applicationData in this function
        /// will be reflected in the main program. For every farm, we will set the defaults based on the User's settings file.
        /// </summary> 
        public void ApplySettingsFromUserFile(ref ApplicationData applicationData, ref Farm farm, Dictionary<string, string> userSettings)
        {
            var userDefaults = new Defaults();
            userDefaults.CarbonConcentration = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.KilogramPerKilogram, double.Parse(userSettings[Properties.Resources.Settings_CarbonConcentration]), false);

            // Climate parameter
            userDefaults.EmergenceDay = int.Parse(userSettings[Properties.Resources.Settings_EmergenceDay]);
            userDefaults.RipeningDay = int.Parse(userSettings[Properties.Resources.Settings_RipeningDay]);
            userDefaults.Variance = double.Parse(userSettings[Properties.Resources.Settings_Variance]);
            userDefaults.Alfa = double.Parse(userSettings[Properties.Resources.Settings_Alfa]);
            userDefaults.DecompositionMinimumTemperature = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius, double.Parse(userSettings[Properties.Resources.Settings_DecompositionMinimumTemperature]), false);
            userDefaults.DecompositionMaximumTemperature = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius, double.Parse(userSettings[Properties.Resources.Settings_DecompositionMaximumTemperature]), false);
            userDefaults.MoistureResponseFunctionAtSaturation = double.Parse(userSettings[Properties.Resources.Settings_MoistureResponseFunctionAtSaturation]);
            userDefaults.MoistureResponseFunctionAtWiltingPoint = double.Parse(userSettings[Properties.Resources.Settings_MoistureResponseFunctionAtWiltingPoint]);

           //Annuals
            userDefaults.PercentageOfProductReturnedToSoilForAnnuals = double.Parse(userSettings[Properties.Resources.Settings_PercentageOfProductReturnedToSoilForAnnuals]);
            userDefaults.PercentageOfStrawReturnedToSoilForAnnuals = double.Parse(userSettings[Properties.Resources.Settings_PercentageOfStrawReturnedToSoilForAnnuals]);
            userDefaults.PercentageOfRootsReturnedToSoilForAnnuals = double.Parse(userSettings[Properties.Resources.Settings_PercentageOfRootsReturnedToSoilForAnnuals]);

           //Silage Crops
            userDefaults.PercentageOfProductYieldReturnedToSoilForSilageCrops = double.Parse(userSettings[Properties.Resources.Settings_PercentageOfProductYieldReturnedToSoilForSilageCrops]);
            userDefaults.PercentageOfRootsReturnedToSoilForSilageCrops = double.Parse(userSettings[Properties.Resources.Settings_PercentageOfRootsReturnedToSoilForSilageCrops]);

           //Cover Crops
            userDefaults.PercentageOfProductYieldReturnedToSoilForCoverCrops = double.Parse(userSettings[Properties.Resources.Settings_PercentageOfProductYieldReturnedToSoilForCoverCrops]);
            userDefaults.PercentageOfProductYieldReturnedToSoilForCoverCropsForage = double.Parse(userSettings[Properties.Resources.Settings_PercentageOfProductYieldReturnedToSoilForCoverCropsForage]);
            userDefaults.PercentageOfProductYieldReturnedToSoilForCoverCropsProduce = double.Parse(userSettings[Properties.Resources.Settings_PercentageOfProductYieldReturnedToSoilForCoverCropsProduce]);
            userDefaults.PercentageOfStrawReturnedToSoilForCoverCrops = double.Parse(userSettings[Properties.Resources.Settings_PercentageOfStrawReturnedToSoilForCoverCrops]);
            userDefaults.PercetageOfRootsReturnedToSoilForCoverCrops = double.Parse(userSettings[Properties.Resources.Settings_PercentageOfRootsReturnedToSoilForCoverCrops]);

           //Root Crops
            userDefaults.PercentageOfProductReturnedToSoilForRootCrops = double.Parse(userSettings[Properties.Resources.Settings_PercentageOfProductReturnedToSoilForRootCrops]);
            userDefaults.PercentageOfStrawReturnedToSoilForRootCrops = double.Parse(userSettings[Properties.Resources.Settings_PercentageOfStrawReturnedToSoilForRootCrops]);

           //Perennial Crops
            userDefaults.PercentageOfProductReturnedToSoilForPerennials = double.Parse(userSettings[Properties.Resources.Settings_PercentageOfProductReturnedToSoilForPerennials]);
            userDefaults.PercentageOfRootsReturnedToSoilForPerennials = double.Parse(userSettings[Properties.Resources.Settings_PercentageOfRootsReturnedToSoilForPerennials]);

           //Rangeland
           //PercentageOfProductReturnedToSoilForRangelandDueToGrazingLoss = double.Parse(userSettings["Percentage Of Product Returned To Soil For Rangeland Due To Grazing Loss"]));

            userDefaults.PercentageOfProductReturnedToSoilForRangelandDueToHarvestLoss = double.Parse(userSettings[Properties.Resources.Settings_PercentageOfProductReturnedToSoilForRangelandDueToHarvestLoss]);
            userDefaults.PercentageOfRootsReturnedToSoilForRangeland = double.Parse(userSettings[Properties.Resources.Settings_PercentageOfRootsReturnedToSoilForRangeland]);

           //Fodder Corn
            userDefaults.PercentageOfProductReturnedToSoilForFodderCorn = double.Parse(userSettings[Properties.Resources.Settings_PercentageOfProductReturnedToSoilForFodderCorn]);
            userDefaults.PercentageOfRootsReturnedToSoilForFodderCorn = double.Parse(userSettings[Properties.Resources.Settings_PercentageOfRootsReturnedToSoilForFodderCorn]);

           // ICBM
            userDefaults.HumificationCoefficientAboveGround = double.Parse(userSettings[Properties.Resources.Settings_HumificationCoefficientAboveGround]);
            userDefaults.HumificationCoefficientBelowGround = double.Parse(userSettings[Properties.Resources.Settings_HumificationCoefficientBelowGround]);
            userDefaults.HumificationCoefficientManure = double.Parse(userSettings[Properties.Resources.Settings_HumificationCoefficientManure]);
            userDefaults.DecompositionRateConstantYoungPool = double.Parse(userSettings[Properties.Resources.Settings_DecompositionRateConstantYoungPool]);
            userDefaults.DecompositionRateConstantOldPool = double.Parse(userSettings[Properties.Resources.Settings_DecompositionRateConstantOldPool]);
            userDefaults.OldPoolCarbonN = double.Parse(userSettings[Properties.Resources.Settings_OldPoolCarbonN]);
            userDefaults.NORatio = double.Parse(userSettings[Properties.Resources.Settings_NORatio]);
            userDefaults.EmissionFactorForLeachingAndRunoff = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerKilogramN, double.Parse(userSettings[Properties.Resources.Settings_EmissionFactorForLeachingAndRunOff]), false);
            userDefaults.EmissionFactorForVolatilization = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerKilogramN, double.Parse(userSettings[Properties.Resources.Settings_EmissionFactorForVolatilization]), false);
            userDefaults.FractionOfNLostByVolatilization = double.Parse(userSettings[Properties.Resources.Settings_FractionOfNLostByVolatilization]);
            userDefaults.MicrobeDeath = double.Parse(userSettings[Properties.Resources.Settings_MicrobeDeath]);
            userDefaults.Denitrification = double.Parse(userSettings[Properties.Resources.Settings_Denitrification]);

            if (userSettings.ContainsKey(Properties.Resources.Settings_UseClimateParameterInsteadOfManagementFactor))
            {
                farm.Defaults.UseClimateParameterInsteadOfManagementFactor = bool.Parse(userSettings[Properties.Resources.Settings_UseClimateParameterInsteadOfManagementFactor]);
            }

            if (userSettings.ContainsKey(Properties.Resources.Settings_EnableCarbonModelling))
            {
                farm.EnableCarbonModelling = bool.Parse(userSettings[Properties.Resources.Settings_EnableCarbonModelling]);
            }

            farm.Defaults = userDefaults;

            // This setting might not exist in old settings files
            if (userSettings.ContainsKey(Properties.Resources.Settings_ClimateFilename))
            {
                farm.ClimateDataFileName = userSettings[Properties.Resources.Settings_ClimateFilename];
            }
            if (userSettings.ContainsKey(Properties.Resources.Settings_Latitude) && userSettings.ContainsKey(Properties.Resources.Settings_Longitude))
            {
                farm.Longitude = double.Parse(userSettings[Properties.Resources.Settings_Longitude]);
                farm.Latitude = double.Parse(userSettings[Properties.Resources.Settings_Latitude]);
            }
            if (userSettings.ContainsKey(Properties.Resources.Settings_ClimateDataAcquisition))
            {
                farm.ClimateAcquisition = farm.ClimateAcquisitionStringToEnum(userSettings[Properties.Resources.Settings_ClimateDataAcquisition]);
            }

            var climateData = new ClimateData()
            {
                PrecipitationData =
                {
                    January = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters, double.Parse(userSettings[Properties.Resources.Settings_JanuaryPrecipitation]), false),
                    February = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters, double.Parse(userSettings[Properties.Resources.Settings_FebruaryPrecipitation]), false),
                    March = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters, double.Parse(userSettings[Properties.Resources.Settings_MarchPrecipitation]), false),
                    April = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters, double.Parse(userSettings[Properties.Resources.Settings_AprilPrecipitation]), false),
                    May = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters, double.Parse(userSettings[Properties.Resources.Settings_MayPrecipitation]), false),
                    June = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters, double.Parse(userSettings[Properties.Resources.Settings_JunePrecipitation]), false),
                    July = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters, double.Parse(userSettings[Properties.Resources.Settings_JulyPrecipitation]), false),
                    August = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters, double.Parse(userSettings[Properties.Resources.Settings_AugustPrecipitation]), false),
                    September = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters, double.Parse(userSettings[Properties.Resources.Settings_SeptemberPrecipitation]), false),
                    October = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters, double.Parse(userSettings[Properties.Resources.Settings_OctoberPrecipitation]), false),
                    November = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters, double.Parse(userSettings[Properties.Resources.Settings_NovemberPrecipitation]), false),
                    December = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters, double.Parse(userSettings[Properties.Resources.Settings_DecemberPrecipitation]), false),
                },

                EvapotranspirationData =
                {
                    January = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear, double.Parse(userSettings[Properties.Resources.Settings_JanuaryPotentialEvapotranspiration]), false),
                    February = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear, double.Parse(userSettings[Properties.Resources.Settings_FebruaryPotentialEvapotranspiration]), false),
                    March = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear, double.Parse(userSettings[Properties.Resources.Settings_MarchPotentialEvapotranspiration]), false),
                    April = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear, double.Parse(userSettings[Properties.Resources.Settings_AprilPotentialEvapotranspiration]), false),
                    May = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear, double.Parse(userSettings[Properties.Resources.Settings_MayPotentialEvapotranspiration]), false),
                    June = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear, double.Parse(userSettings[Properties.Resources.Settings_JunePotentialEvapotranspiration]), false),
                    July = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear, double.Parse(userSettings[Properties.Resources.Settings_JulyPotentialEvapotranspiration]), false),
                    August = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear, double.Parse(userSettings[Properties.Resources.Settings_AugustPotentialEvapotranspiration]), false),
                    September = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear, double.Parse(userSettings[Properties.Resources.Settings_SeptemberPotentialEvapotranspiration]), false),
                    October = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear, double.Parse(userSettings[Properties.Resources.Settings_OctoberPotentialEvapotranspiration]), false),
                    November = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear, double.Parse(userSettings[Properties.Resources.Settings_NovemberPotentialEvapotranspiration]), false),
                    December = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear, double.Parse(userSettings[Properties.Resources.Settings_DecemberPotentialEvapotranspiration]), false),
                },

                TemperatureData =
                {
                    January = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius, double.Parse(userSettings[Properties.Resources.Settings_JanuaryMeanTemperature]), false),
                    February = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius, double.Parse(userSettings[Properties.Resources.Settings_FebruaryMeanTemperature]), false),
                    March = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius, double.Parse(userSettings[Properties.Resources.Settings_MarchMeanTemperature]), false),
                    April = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius, double.Parse(userSettings[Properties.Resources.Settings_AprilMeanTemperature]), false),
                    May = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius, double.Parse(userSettings[Properties.Resources.Settings_MayMeanTemperature]), false),
                    June = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius, double.Parse(userSettings[Properties.Resources.Settings_JuneMeanTemperature]), false),
                    July = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius, double.Parse(userSettings[Properties.Resources.Settings_JulyMeanTemperature]), false),
                    August = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius, double.Parse(userSettings[Properties.Resources.Settings_AugustMeanTemperature]), false),
                    September = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius, double.Parse(userSettings[Properties.Resources.Settings_SeptemberMeanTemperature]), false),
                    October = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius, double.Parse(userSettings[Properties.Resources.Settings_OctoberMeanTemperature]), false),
                    November = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius, double.Parse(userSettings[Properties.Resources.Settings_NovemberMeanTemperature]), false),
                    December = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius, double.Parse(userSettings[Properties.Resources.Settings_DecemberMeanTemperature]), false),
                },
            };

            farm.ClimateData = climateData;

            var userGeographicData = new GeographicData()
            {
                DefaultSoilData =
                {
                   YearOfObservation = int.Parse(userSettings[Properties.Resources.Settings_YearOfObservation]),
                   EcodistrictId = int.Parse(userSettings[Properties.Resources.Settings_EcodistrictID]),
                   SoilGreatGroup =  (SoilGreatGroupType)Enum.Parse(typeof(SoilGreatGroupType), userSettings[Properties.Resources.Settings_SoilGreatGroup], true),
                   BulkDensity = double.Parse(userSettings[Properties.Resources.Settings_BulkDensity]),
                   SoilTexture = (SoilTexture)Enum.Parse(typeof(SoilTexture), userSettings[Properties.Resources.Settings_SoilTexture], true),
                   SoilPh = double.Parse(userSettings[Properties.Resources.Settings_SoilPh]),
                   TopLayerThickness = _unitsOfMeasurementCalculator.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters, double.Parse(userSettings[Properties.Resources.Settings_TopLayerThickness]), false),
                   ProportionOfSandInSoil = double.Parse(userSettings[Properties.Resources.Settings_ProportionOfSandInSoil]),
                   ProportionOfClayInSoil = double.Parse(userSettings[Properties.Resources.Settings_ProportionOfClayInSoil]),
                   ProportionOfSoilOrganicCarbon = double.Parse(userSettings[Properties.Resources.Settings_ProportionOfSoilOrganicCarbon]),
                },
            };

            farm.GeographicData = userGeographicData;

            if (userSettings.ContainsKey(Properties.Resources.Settings_SoilFunctionalCategory))
            {
                farm.GeographicData.DefaultSoilData.SoilFunctionalCategory = (SoilFunctionalCategory)Enum.Parse(typeof(SoilFunctionalCategory), userSettings[Properties.Resources.Settings_SoilFunctionalCategory], true);
            }
        }

        public void GetUserSettingsMenuChoice(string farmDirectoryPath, GeographicDataProvider geographicDataProvider)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            var farmName = Path.GetFileName(farmDirectoryPath);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Properties.Resources.PromptUserForPolygonSelection, farmName);
            Console.WriteLine(Environment.NewLine);

            int userChosenMenuNumber;
            string userMenuChoice;

            do
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(String.Format(Properties.Resources.SelectPolygonOptions + Environment.NewLine +
                                  Properties.Resources.ExitAndRunHolosToDetermineFarmLocation + Environment.NewLine +
                                  Properties.Resources.CreateASettingsFIleBasedOnPolygon + Environment.NewLine +
                                  Properties.Resources.CreateADefaultLethbridgeSettingsFile + Environment.NewLine +
                                  Properties.Resources.SettingsFileWillBeCreatedHere + Environment.NewLine +
                                  Properties.Resources.EnterYourChoice, farmDirectoryPath));
                userMenuChoice = Console.ReadLine();
                int.TryParse(userMenuChoice, out userChosenMenuNumber);
            } while (userChosenMenuNumber < 0 || userChosenMenuNumber > 3 || !int.TryParse(userMenuChoice, out userChosenMenuNumber));

            this.ExecuteUserChoice(userChosenMenuNumber, geographicDataProvider, farmDirectoryPath);
        }

        public void ExecuteUserChoice(int userChosenMenuNumber, GeographicDataProvider geographicDataProvider, string farmDirectoryPath)
        {
            if (userChosenMenuNumber == 1)
            {
                //exit the program
                Environment.Exit(1);
            }

            ///////////Get Polygon ID From User///////////////////////////
            if (userChosenMenuNumber == 2)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(Properties.Resources.PromptToEnterPolygonID);
                var polygonIDString = Console.ReadLine();
                var polygonID = int.Parse(polygonIDString);

                if (PolygonIDList.Contains(polygonID))
                {
                    var geographicData = geographicDataProvider.GetGeographicalData(polygonID);
                    var farm = new Farm() { PolygonId = polygonID, GeographicData = geographicData };
                    _directoryHandler.GenerateGlobalSettingsFile(farmDirectoryPath, farm);
                }

                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(String.Format(Properties.Resources.NotAValidPolygonID, polygonIDString));
                    throw new Exception("Not A Valid Polygon ID");
                }

                Console.ResetColor();

            }

            ///////////Create Default Lethbridge Settings File///////////////////////////
            if (userChosenMenuNumber == 3)
            {
                Console.WriteLine(String.Format(Properties.Resources.CreatingDefaultLethbridgeGeographicData, farmDirectoryPath));

                var lethbridgePolygonID = 793006;
                var lethbridgeGeographicData = geographicDataProvider.GetGeographicalData(lethbridgePolygonID);
                var climateData = _slcClimateDataProvider.GetClimateData(lethbridgePolygonID, TimeFrame.NineteenNinetyToTwoThousandSeventeen);

                _directoryHandler.GenerateGlobalSettingsFile(farmDirectoryPath, new Farm() { PolygonId = lethbridgePolygonID, GeographicData = lethbridgeGeographicData, ClimateData = climateData });
            }
        }

        #endregion
    }
}

