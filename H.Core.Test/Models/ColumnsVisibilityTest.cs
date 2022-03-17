using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Models;
using System.Collections.Generic;

namespace H.Core.Test.Models
{
    [TestClass]
    public class ColumnsVisibilityTest
    {
        [TestMethod]
        public void ManureVisibilityTest()
        {
            var vis = new ManureModelColumnsVisibility();
            foreach (var prop in vis.GetType().GetProperties())
            {
                Assert.AreEqual(true, prop.GetValue(vis));
            }

        }

        [TestMethod]
        public void FieldResultsVisibilityTest()
        {
            var vis = new FieldResultsColumnsVisibility();
            var defaults = new List<string>()
            {
                "Name",
                "TimePeriod",
                "Year",
                "CropType",
                "ClimateParameter",
                "TillageFactor",
                "ManagementFactor",
                "PlantCarbonInAgriculturalProduct",
                "CarbonInputFromProduct",
                "CarbonInputFromRoots",
                "CarbonInputFromStraw",
                "CarbonInputFromExtraRoots",
                "AboveGroundCarbonInput",
                "BelowGrounCarbonInput",
                "YoungPoolSoilCarbonAboveGround",
                "YoungPoolSoilCarbonBelowGround",
                "YoungPoolManureCarbon",
                "AverageSoilCarbonAcrossAllFieldsInFarm",
                "SoilCarbon",
                "ChangeInCarbon",
            };

            foreach (var prop in vis.GetType().GetProperties())
            {
                var result = (bool)prop.GetValue(vis);
                if (defaults.Contains(prop.Name))
                {
                    Assert.IsTrue(result);
                }
                else
                {
                    Assert.IsFalse(result);
                }
            }
        }

        [TestMethod]
        public void FieldDetailsVisibilityTest()
        {
            var vis = new FieldSystemDetailsColumnsVisibility();
            var defaults = new List<string>()
            {
                "FieldName",
                "TimePeriod",
                "Year",
                "CropType",
                "Yield",
                "Description",
            };

            foreach (var prop in vis.GetType().GetProperties())
            {
                var result = (bool)prop.GetValue(vis);
                if (defaults.Contains(prop.Name))
                {
                    Assert.IsTrue(result);
                }
                else
                {
                    Assert.IsFalse(result);
                }
            }

        }
    }
}
