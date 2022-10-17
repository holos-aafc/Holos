using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers;
using H.Core.Providers.Climate;
using H.Core.Providers.Evapotranspiration;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Temperature;
using H.Core.Services.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Services
{
    [TestClass]
    public class PoultryResultsServiceTest
    {
        #region Fields
        
        private PoultryResultsService _resultsService; 
        
        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _resultsService = new PoultryResultsService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }
        #endregion

        #region Tests

        /// <summary>
        ///  Equation 3.5.1-1
        /// </summary>
        [TestMethod]
        public void CalculateEntericMethaneEmissionReturnsCorrectValue()
        {
            var entericMethaneEmissionRate = 150.125;
            var numberOfPoultry = 59.125;
            var numberOfDaysInMonth = 70.750;
            var result =
                _resultsService.CalculateEntericMethaneEmission(entericMethaneEmissionRate, numberOfPoultry, numberOfDaysInMonth);
            Assert.AreEqual(1720.5121896404109589041095890411, result);
        }


        /// <summary>
        ///    Equation 3.5.2-1
        /// </summary>
        [TestMethod]
        public void CalculateManureMethaneEmissionReturnsCorrectValue()
        {
            var manureMethaneEmissionRate = 88.250;
            var numberOfPoultry = 126.500;
            var numberOfDaysInMonth = 215.500;
            var result =
                _resultsService.CalculateManureMethaneEmission(manureMethaneEmissionRate, numberOfPoultry, numberOfDaysInMonth);
            Assert.AreEqual(6591.126541095890410958904109589, result);
        }

        /// <summary>
        /// Equation 3.5.3-5
        /// </summary>
        [TestMethod]
        public void CalculateManureIndirectNitrogenEmissionReturnsCorrectValue()
        {
            var manureVolatilizationNitrogenEmission = 18.750;
            var manureLeachingNitrogenEmission = 231.000;
            var result = _resultsService.CalculateManureIndirectNitrogenEmission(manureVolatilizationNitrogenEmission,
                                                                      manureLeachingNitrogenEmission);
            Assert.AreEqual(249.75, result);
        }

        /// <summary>
        /// Equation 4.2.3-2
        /// </summary>
        [TestMethod]
        public void CalculateTotalCarbonDioxideEmissionsFromPoultryOperationsReturnsCorrectValue()
        {
            var barnCapacityForPoultry = 70;

            var numberOfDaysInMonth = 30;
            var energyConversion = 2;
            var result = _resultsService.CalculateTotalEnergyCarbonDioxideEmissionsFromPoultryOperations(barnCapacityForPoultry,
                numberOfDaysInMonth, energyConversion);
            Assert.AreEqual(barnCapacityForPoultry * (2.88/365) * energyConversion * numberOfDaysInMonth, result);
        }

        [TestMethod]
        public void CalculateAmmoniaEmissionsFromLandAppliedManure()
        {
            var climateData = new ClimateData()
            {
                PrecipitationData = new PrecipitationData()
                {
                    January = 20,
                },

                TemperatureData = new TemperatureData()
                {
                    January = -10,
                },

                EvapotranspirationData = new EvapotranspirationData()
                {
                    January = 5,
                }
            };

            var farm = new Farm()
            {
                ClimateData = climateData,
            };

            var date = DateTime.Now;

            var manureApplicationViewItem = new ManureApplicationViewItem()
            {
                DateOfApplication = date,
                ManureStateType = ManureStateType.Liquid,
                ManureApplicationMethod = ManureApplicationTypes.ShallowInjection,
                AmountOfManureAppliedPerHectare = 200,
                AnimalType = AnimalType.Poultry,
                AmountOfNitrogenAppliedPerHectare = 1,
            };

            var cropViewItem = new CropViewItem()
            {
                Area = 5,
            };

            cropViewItem.ManureApplicationViewItems.Add(manureApplicationViewItem);

            var field = new FieldSystemComponent();
            field.CropViewItems.Add(cropViewItem);

            farm.Components.Add(field);

            var dailyEmissions = new List<GroupEmissionsByDay>()
            {
                new GroupEmissionsByDay()
                {
                    DateTime = date,
                    TotalVolumeOfManureAvailableForLandApplication = 100,
                    TanAvailableForLandApplication = 20,
                    NitrogenAvailableForLandApplication = 10,
                    LeachingFraction = 0.5,
                    EmissionFactorForLeaching = 0.25,
                },
            };

            farm.StageStates.Add(new FieldSystemDetailsStageState() { DetailsScreenViewCropViewItems = new ObservableCollection<CropViewItem>() { cropViewItem } });

            var results = _resultsService.CalculateAmmoniaEmissionsFromLandAppliedManure(
                farm: farm,
                dailyEmissions: dailyEmissions,
                ComponentCategory.Poultry,
                AnimalType.Poultry);

            Assert.AreEqual(3.5700755, results.First().TotalIndirectN2ONEmissions, 0.0001);
        }

        #endregion
    }
}

