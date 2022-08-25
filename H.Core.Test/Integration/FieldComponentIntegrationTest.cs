using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using H.CLI.UserInput;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models.LandManagement.Rotation;
using H.Core.Providers;
using H.Core.Providers.Climate;
using H.Core.Services;
using H.Core.Services.LandManagement;
using Prism.Interactivity.DefaultPopupWindows;

namespace H.Core.Test.Integration
{
    [TestClass]
    public class FieldComponentIntegrationTest
    {
        private GeographicDataProvider _geographicDataProvider;
        private ClimateProvider _climateProvider;
        private FieldResultsService _fieldResultsService;
        private GlobalSettings _globalSettings;
        private RotationComponentHelper _rotationComponentHelper;

        [TestInitialize]
        public void TestInitialize()
        {
            // Initialize the soil information provider
            _geographicDataProvider = new GeographicDataProvider();
            _geographicDataProvider.Initialize();

            // Initialize the climate provider
            _climateProvider = new ClimateProvider();

            // Initialize the calculation class for fields
            _fieldResultsService = new FieldResultsService();

            // Initial system defaults
            _globalSettings = new GlobalSettings();

            // This class will create a rotation on our farm
            _rotationComponentHelper = new RotationComponentHelper();
        }

        [TestMethod]
        public void TestMethod1()
        {
            /*
             * Note: The Visual Studio output window will display logging information from the various method calls and the associated successes/failures of those calls
             */

            // Create a new farm which is a container object for all components that can be added to a farm
            var farm = new Farm();

            farm.Name = "My Test Farm";

            // Add location information for the farm
            farm.Province = Province.Alberta;

            // All farms need to have an SLC polygon number assigned (latitude & longitude coordinates not currently supported)
            farm.PolygonId = 793011; // Lethbridge, AB

            // Get soil data that is associated with this location
            farm.GeographicData = _geographicDataProvider.GetGeographicalData(farm.PolygonId);

            // Now can set or read various soil details for this location by reading the DefaultSoilData property. For example we can get or set the soil Ph
            var soilPh = farm.GeographicData.DefaultSoilData.SoilPh;

            // We will now get climate data from the NASA API. Note that if the latitude and longitude are known, we can get precise climate data for our location. If precise location
            // is not know, we call use slightly less accurate 30 year climate normals.

            farm.ClimateData = _climateProvider.Get(49.699498, -112.775813, TimeFrame.NineteenNinetyToTwoThousand); // Lethbridge research station coordinates

            // We now have daily climate data going back to 1981 for this location and can access precipitation, evapotranspiration, temperature, etc.
            var climateDataFromLastMonth = farm.ClimateData.DailyClimateData.Single(data => data.Date.Date.Equals(DateTime.Now.Date.Subtract(TimeSpan.FromDays(30))));
            var precipitation30DaysAgo = climateDataFromLastMonth.MeanDailyPrecipitation;

            var crop1 = new CropViewItem() { CropType = CropType.Wheat };
            var crop2 = new CropViewItem() { CropType = CropType.Oats };
            var crop3 = new CropViewItem() { CropType = CropType.Fallow };

            // We'll create three fields with 3 different crops rotated over the three fields
            var cropsInRotation = new List<CropViewItem>()
            {
                crop1, crop2, crop3,
            };

            // Assign default properties to
            foreach (var cropViewItem in cropsInRotation)
            {
                _fieldResultsService.AssignSystemDefaults(cropViewItem, farm, _globalSettings);
            }

            // Override default properties for some crops (optional)
            crop2.NitrogenFertilizerRate = 60;

            // Create a new rotation component.
            var rotationComponent = new RotationComponent();
            rotationComponent.FieldSystemComponent = new FieldSystemComponent();
            rotationComponent.FieldSystemComponent.BeginOrderingAtStartYearOfRotation = true;
            rotationComponent.FieldSystemComponent.StartYear = 1980;
            rotationComponent.FieldSystemComponent.EndYear = 2022;
            rotationComponent.ShiftLeft = true;

            // Build the rotation and add it to the farm
            _rotationComponentHelper.CreateRotation(cropsInRotation, rotationComponent, _globalSettings, farm);

            // Set the yield assignment method
            farm.YieldAssignmentMethod = YieldAssignmentMethod.SmallAreaData;

            // Build the field history
            farm.StageStates.Add(new FieldSystemDetailsStageState());
            _fieldResultsService.CreateDetailViewItems(farm);

            var finalResults = _fieldResultsService.CalculateFinalResults(farm);

            // Create a directory to hold the outputs of the run
            const string OutputDirectory = "HOLOS_OUTPUT\\";
            Directory.CreateDirectory(OutputDirectory);

            _fieldResultsService.ExportResultsToFile(finalResults, OutputDirectory, CultureInfo.CurrentCulture, MeasurementSystemType.Metric, CLILanguageConstants.OutputLanguageAddOn, false, farm);
        }
    }
}
