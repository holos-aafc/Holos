﻿using System.Collections.ObjectModel;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.LandManagement;
using Moq;

namespace H.Core.Test.Services;

public partial class FieldResultsServiceTest
{
    #region Energy CO2 Tests

    [TestMethod]
    public void GetTotalManureSpreadingEmissionsWhenOnlyOneFieldHasManureApplicationsTest()
    {
        _mockManureService.Setup(x => x.GetTotalVolumeRemainingForFarmAndYear(It.IsAny<int>(), It.IsAny<Farm>()))
            .Returns(200);
        _resultsService = new FieldResultsService(_iCbmSoilCarbonCalculator, _ipccSoilCarbonCalculator,
            _n2OEmissionFactorCalculator, _initializationService);

        var viewItem1 = new CropViewItem();
        viewItem1.Year = DateTime.Now.Year;

        var viewItem2 = new CropViewItem();
        viewItem2.Year = DateTime.Now.Year;

        var farm = GetTestFarm();

        var field1 = new FieldSystemComponent();
        field1.FieldArea = 50;
        viewItem1.FieldSystemComponentGuid = field1.Guid;
        farm.Components.Add(field1);

        var field2 = new FieldSystemComponent();
        field2.FieldArea = 33;
        viewItem2.FieldSystemComponentGuid = field2.Guid;
        farm.Components.Add(field2);

        farm.StageStates.Add(new FieldSystemDetailsStageState
        {
            DetailsScreenViewCropViewItems = new ObservableCollection<CropViewItem>
            {
                viewItem1,
                viewItem2
            }
        });

        var manureApplication = new ManureApplicationViewItem();
        manureApplication.DateOfApplication = DateTime.Now;
        manureApplication.AmountOfManureAppliedPerHectare = 100;
        manureApplication.ManureLocationSourceType = ManureLocationSourceType.Livestock;
        viewItem1.ManureApplicationViewItems.Add(manureApplication);

        field1.CropViewItems.Add(viewItem1);
        field2.CropViewItems.Add(viewItem2);

        var field1Results = _resultsService.GetManureSpreadingEmissions(viewItem1, farm).Sum(x => x.TotalEmissions);
        var field2Results = _resultsService.GetManureSpreadingEmissions(viewItem2, farm).Sum(x => x.TotalEmissions);

        Assert.AreNotEqual(field2Results, field1Results);
        Assert.AreEqual(0.1736, field1Results);
        Assert.AreEqual(0, field2Results);
    }

    [TestMethod]
    public void GetTotalManureSpreadingEmissionsWhenTwoFieldsHaveManureApplicationsTest()
    {
        _mockManureService.Setup(x => x.GetTotalVolumeRemainingForFarmAndYear(It.IsAny<int>(), It.IsAny<Farm>()))
            .Returns(200);
        _resultsService = new FieldResultsService(_iCbmSoilCarbonCalculator, _ipccSoilCarbonCalculator,
            _n2OEmissionFactorCalculator, _initializationService);

        var viewItem1 = new CropViewItem();
        viewItem1.Year = DateTime.Now.Year;

        var viewItem2 = new CropViewItem();
        viewItem2.Year = DateTime.Now.Year;

        var farm = new Farm();

        var field1 = new FieldSystemComponent();
        field1.FieldArea = 50;
        viewItem1.FieldSystemComponentGuid = field1.Guid;
        farm.Components.Add(field1);

        var field2 = new FieldSystemComponent();
        field2.FieldArea = 33;
        viewItem2.FieldSystemComponentGuid = field2.Guid;
        farm.Components.Add(field2);

        farm.StageStates.Add(new FieldSystemDetailsStageState
        {
            DetailsScreenViewCropViewItems = new ObservableCollection<CropViewItem>
            {
                viewItem1,
                viewItem2
            }
        });

        var manureApplication1 = new ManureApplicationViewItem();
        manureApplication1.DateOfApplication = DateTime.Now;
        manureApplication1.AmountOfManureAppliedPerHectare = 100;
        manureApplication1.ManureLocationSourceType = ManureLocationSourceType.Livestock;
        viewItem1.ManureApplicationViewItems.Add(manureApplication1);

        var manureApplication2 = new ManureApplicationViewItem();
        manureApplication2.DateOfApplication = DateTime.Now;
        manureApplication2.AmountOfManureAppliedPerHectare = 75;
        manureApplication2.ManureLocationSourceType = ManureLocationSourceType.Livestock;
        viewItem2.ManureApplicationViewItems.Add(manureApplication2);

        field1.CropViewItems.Add(viewItem1);
        field2.CropViewItems.Add(viewItem2);

        var field1Results = _resultsService.GetManureSpreadingEmissions(viewItem1, farm).Sum(x => x.TotalEmissions);
        var field2Results = _resultsService.GetManureSpreadingEmissions(viewItem2, farm).Sum(x => x.TotalEmissions);

        Assert.AreNotEqual(field2Results, field1Results);
        Assert.AreEqual(0.1736, field1Results);
        Assert.AreEqual(0.1302, field2Results, 0.0001);
    }

    [TestMethod]
    public void GetTotalManureSpreadingEmissionsWhenNoFieldApplicationsMadeAndOnlyOneFieldExistsTest()
    {
        var viewItem = GetTestCropViewItem();
        viewItem.Year = DateTime.Now.Year;

        var farm = GetTestFarm();
        var field = GetTestFieldComponent();
        field.FieldArea = 50;

        viewItem.FieldSystemComponentGuid = field.Guid;
        farm.Components.Add(field);

        farm.StageStates.Add(new FieldSystemDetailsStageState
        {
            DetailsScreenViewCropViewItems = new ObservableCollection<CropViewItem>
            {
                viewItem
            }
        });


        field.CropViewItems.Add(viewItem);


        _resultsService = new FieldResultsService(_iCbmSoilCarbonCalculator, _ipccSoilCarbonCalculator,
            _n2OEmissionFactorCalculator, _initializationService);
        _n2OEmissionFactorCalculator.Initialize(farm);

        var sut = _resultsService.GetManureSpreadingEmissions(viewItem, farm).Sum(x => x.TotalEmissions);

        Assert.AreEqual(13375.1350833923, sut, 0.0001);
    }

    [TestMethod]
    public void GetTotalManureSpreadingEmissionsWhenNoFieldApplicationsMadeAndTwoFieldsExistTest()
    {
        _mockManureService.Setup(x => x.GetTotalVolumeRemainingForFarmAndYear(It.IsAny<int>(), It.IsAny<Farm>()))
            .Returns(200);


        var viewItem1 = new CropViewItem();
        viewItem1.Year = DateTime.Now.Year;

        var viewItem2 = new CropViewItem();
        viewItem2.Year = DateTime.Now.Year;

        var farm = GetTestFarm();

        var field1 = GetTestFieldComponent();
        field1.FieldArea = 50;
        viewItem1.FieldSystemComponentGuid = field1.Guid;
        farm.Components.Add(field1);
        field1.CropViewItems.Add(viewItem1);

        var field2 = new FieldSystemComponent();
        field2.FieldArea = 60;
        field2.CropViewItems.Add(viewItem2);
        viewItem2.FieldSystemComponentGuid = field2.Guid;

        farm.Components.Add(field2);

        farm.StageStates.Add(new FieldSystemDetailsStageState
        {
            DetailsScreenViewCropViewItems = new ObservableCollection<CropViewItem>
            {
                viewItem1,
                viewItem2
            }
        });


        field1.CropViewItems.Add(viewItem1);
        field2.CropViewItems.Add(viewItem2);

        _n2OEmissionFactorCalculator.Initialize(farm);
        _resultsService = new FieldResultsService(_iCbmSoilCarbonCalculator, _ipccSoilCarbonCalculator,
            _n2OEmissionFactorCalculator, _initializationService);

        var field1Result = _resultsService.GetManureSpreadingEmissions(viewItem1, farm).Sum(x => x.TotalEmissions);
        var field2Result = _resultsService.GetManureSpreadingEmissions(viewItem2, farm).Sum(x => x.TotalEmissions);

        Assert.AreNotEqual(field2Result, field1Result);
        Assert.AreEqual(field1Result, 6079.60685608741, 0.00001);
        Assert.AreEqual(field2Result, 7295.52822730488, 0.00001);
    }

    #endregion
}