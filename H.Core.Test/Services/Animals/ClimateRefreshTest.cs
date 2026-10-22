using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Climate;
using H.Core.Services;
using H.Core.Services.Animals;
using H.Core.Services.Initialization.Climate;
using H.Core.Services.LandManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Services.Animals
{
    /// <summary>
    /// The details screen rows hold values calculated from the farm's climate - the climate parameter ICBM decomposes
    /// with, and the carbon and nitrogen inputs derived alongside it - and they are saved with the farm. Replacing the
    /// climate does not rebuild them, so FieldResultsService.RefreshClimateDependentInputs recalculates them in place.
    /// Farm4 is used because it has a full daily climate and a hayed perennial.
    /// </summary>
    [TestClass]
    public class ClimateRefreshTest : UnitTestBase
    {
        #region Tests

        /// <summary>
        /// Rows built on a farm's inflated climate and refreshed after the climate is replaced end up as rows built on
        /// the replacement from the start would be.
        /// </summary>
        [TestMethod]
        public void RefreshAfterTheClimateIsReplacedMatchesRowsBuiltOnTheNewClimate()
        {
            // Both farms go through the replacement, which also recalculates the normals from the daily records, so
            // the only difference left between them is which climate the rows were first built on.
            var reference = LoadFarm();
            new ClimateInitializationService().ReplaceClimate(reference, CopyClimate(reference, precipitationScale: 1));
            var referenceService = BuildRows(reference);
            var expected = Rows(referenceService, reference);

            var farm = LoadFarm();
            var original = CopyClimate(farm, precipitationScale: 1);
            new ClimateInitializationService().ReplaceClimate(farm, CopyClimate(farm, precipitationScale: 24));
            var service = BuildRows(farm);

            var inflatedClimateParameters = Rows(service, farm).Select(x => x.ClimateParameter).ToList();

            new ClimateInitializationService().ReplaceClimate(farm, original);
            service.RefreshClimateDependentInputs(farm);

            var actual = Rows(service, farm);

            Assert.AreEqual(expected.Count, actual.Count);
            Assert.IsTrue(expected.Select(x => x.ClimateParameter).Zip(inflatedClimateParameters, (a, b) => Math.Abs(a - b)).Any(d => d > 1e-6),
                "the inflated climate gave the same climate parameters, so this test shows nothing");

            for (var i = 0; i < expected.Count; i++)
            {
                var label = $"{actual[i].Year} {actual[i].CropType}";
                Assert.AreEqual(expected[i].ClimateParameter, actual[i].ClimateParameter, 1e-9, label + ": climate parameter");
                Assert.AreEqual(expected[i].ManagementFactor, actual[i].ManagementFactor, 1e-9, label + ": management factor");
                Assert.AreEqual(expected[i].PlantCarbonInAgriculturalProduct, actual[i].PlantCarbonInAgriculturalProduct, 1e-6, label + ": plant carbon");
                Assert.AreEqual(ResidueCarbon(expected[i]), ResidueCarbon(actual[i]), 1e-6, label + ": residue carbon inputs");
                Assert.AreEqual(expected[i].CombinedAboveGroundResidueNitrogen, actual[i].CombinedAboveGroundResidueNitrogen, 1e-6, label + ": aboveground residue N");
                Assert.AreEqual(expected[i].MoistureContentOfCropPercentage, actual[i].MoistureContentOfCropPercentage, 1e-9, label + ": moisture shown on the row");
            }
        }

        /// <summary>
        /// A value the user overrode on the details screen survives the refresh, with its flag.
        /// </summary>
        [TestMethod]
        public void RefreshKeepsTheUsersOverrides()
        {
            var farm = LoadFarm();
            var service = BuildRows(farm);
            var rows = Rows(service, farm);

            var yieldRow = rows[1];
            yieldRow.Yield = 12345;
            yieldRow.DoNotRecalculateYield = true;

            var carbonRow = rows[2];
            carbonRow.PlantCarbonInAgriculturalProduct = 4321;
            carbonRow.DoNotRecalculatePlantCarbonInAgriculturalProduct = true;

            var returnRow = rows[3];
            returnRow.PercentageOfProductYieldReturnedToSoil = 17;
            returnRow.DoNotRecalculatePercentageReturnedToSoil = true;

            new ClimateInitializationService().ReplaceClimate(farm, CopyClimate(farm, precipitationScale: 0.5));
            service.RefreshClimateDependentInputs(farm);

            Assert.AreEqual(12345, yieldRow.Yield, 1e-9);
            Assert.IsTrue(yieldRow.DoNotRecalculateYield);
            Assert.AreEqual(4321, carbonRow.PlantCarbonInAgriculturalProduct, 1e-9);
            Assert.IsTrue(carbonRow.DoNotRecalculatePlantCarbonInAgriculturalProduct);
            Assert.AreEqual(17, returnRow.PercentageOfProductYieldReturnedToSoil, 1e-9);
            Assert.IsTrue(returnRow.DoNotRecalculatePercentageReturnedToSoil);
        }

        /// <summary>
        /// With the climate unchanged, a refresh changes nothing. In particular a hayed row, which carries the bale's
        /// moisture, keeps its plant carbon: deriving it again at that moisture would read a standing-crop yield as
        /// mostly dry matter, over four times the carbon.
        /// </summary>
        [TestMethod]
        public void RefreshWithTheSameClimateChangesNothing()
        {
            var farm = LoadFarm();
            var service = BuildRows(farm);
            var rows = Rows(service, farm);
            Assert.IsTrue(rows.Any(YieldInputPolicy.HasHayedHarvest), "Farm4 no longer has a hayed row, so the moisture case is not covered");

            var before = rows.Select(Snapshot).ToList();

            service.RefreshClimateDependentInputs(farm);

            var after = rows.Select(Snapshot).ToList();
            for (var i = 0; i < before.Count; i++)
            {
                foreach (var key in before[i].Keys)
                {
                    Assert.AreEqual(before[i][key], after[i][key], 1e-6, $"{rows[i].Year} {rows[i].CropType}: {key} changed");
                }
            }
        }



        #endregion

        #region Helpers

        private static Farm LoadFarm()
        {
            return new Storage().GetFarmsFromExportFile(GetFixtureFilePath("Farm4.json")).Single();
        }

        /// <summary>
        /// Builds the details screen rows as the GUI does. Irrigation is kept at the amounts saved in the file: it is an
        /// input on the component selection screen, which downloading the climate again does not change, while
        /// ReInitializeFarms would recalculate its default from whichever climate the farm has at the time.
        /// </summary>
        private FieldResultsService BuildRows(Farm farm)
        {
            var crops = farm.FieldSystemComponents.SelectMany(field => field.CropViewItems.Concat(field.CoverCrops)).ToList();
            var savedIrrigation = crops.ToDictionary(crop => crop, crop => crop.AmountOfIrrigation);

            base._initializationService.ReInitializeFarms(new[] { farm });

            foreach (var crop in crops)
            {
                crop.AmountOfIrrigation = savedIrrigation[crop];
            }

            farm.ResetAnimalResults();

            var service = new FieldResultsService(base._iCbmSoilCarbonCalculator, base._ipcc, base._n2OEmissionFactorCalculator, base._initializationService)
            {
                AnimalResults = new AnimalResultsService().GetAnimalResults(farm),
            };

            service.InitializeStageState(farm);

            return service;
        }

        private static List<CropViewItem> Rows(FieldResultsService service, Farm farm)
        {
            return service.GetStageState(farm).DetailsScreenViewCropViewItems
                .OrderBy(x => x.FieldSystemComponentGuid)
                .ThenBy(x => x.Year)
                .ThenBy(x => x.IsSecondaryCrop)
                .ToList();
        }

        private static List<DailyClimateData> CopyClimate(Farm farm, double precipitationScale)
        {
            return farm.ClimateData.DailyClimateData.Select(d => new DailyClimateData()
            {
                Year = d.Year,
                JulianDay = d.JulianDay,
                Date = d.Date,
                MeanDailyPrecipitation = d.MeanDailyPrecipitation * precipitationScale,
                MeanDailyPET = d.MeanDailyPET,
                MeanDailyAirTemperature = d.MeanDailyAirTemperature,
                MinimumAirTemperature = d.MinimumAirTemperature,
                MaximumAirTemperature = d.MaximumAirTemperature,
                RelativeHumidity = d.RelativeHumidity,
                SolarRadiation = d.SolarRadiation,
            }).ToList();
        }

        private static Dictionary<string, double> Snapshot(CropViewItem row)
        {
            return new Dictionary<string, double>()
            {
                {"yield", row.Yield},
                {"moisture", row.MoistureContentOfCropPercentage},
                {"plant carbon", row.PlantCarbonInAgriculturalProduct},
                {"carbon from product", row.CarbonInputFromProduct},
                {"carbon from roots", row.CarbonInputFromRoots},
                {"residue carbon inputs", ResidueCarbon(row)},
                {"climate parameter", row.ClimateParameter},
                {"aboveground residue N", row.CombinedAboveGroundResidueNitrogen},
            };
        }

        /// <summary>
        /// The carbon a row's crop returns, above and below ground. Its total also counts manure, and the share of a
        /// farm's leftover manure a row is given depends on how many of the farm's rows exist when it is calculated -
        /// fewer while the screen is first built field by field than at any later pass. That is unrelated to the
        /// climate, so the comparisons here leave it out.
        /// </summary>
        private static double ResidueCarbon(CropViewItem row)
        {
            return row.AboveGroundCarbonInput + row.BelowGroundCarbonInput;
        }

        private static string GetFixtureFilePath(string fileName, [CallerFilePath] string thisFile = "")
        {
            return Path.Combine(Path.GetDirectoryName(thisFile), "Baselines", fileName);
        }

        #endregion
    }
}
