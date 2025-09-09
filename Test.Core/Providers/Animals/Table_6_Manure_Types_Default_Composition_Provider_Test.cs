using H.Core.Enumerations;
using H.Core.Providers.Animals;

namespace H.Core.Test.Providers.Animals;

[TestClass]
public class Table_6_Manure_Types_Default_Composition_Provider_Test
{
    private static Table_6_Manure_Types_Default_Composition_Provider _provider;

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        _provider = new Table_6_Manure_Types_Default_Composition_Provider();
    }

    [TestMethod]
    public void GetManureCompositionData()
    {
        var result = _provider.ManureCompositionData.SingleOrDefault(x => x.AnimalType == AnimalType.Beef &&
                                                                          x.ManureStateType ==
                                                                          ManureStateType.DeepBedding);

        Assert.AreEqual(AnimalType.Beef, result.AnimalType);
        Assert.AreEqual(ManureStateType.DeepBedding, result.ManureStateType);
        Assert.AreEqual(60.08, result.MoistureContent);
        Assert.AreEqual(0.715, result.NitrogenFraction);
        Assert.AreEqual(12.63, result.CarbonFraction);
        Assert.AreEqual(17.66, result.CarbonToNitrogenRatio);
    }

    [TestMethod]
    public void TestDairyCattleData()
    {
        var data = _provider.GetManureCompositionDataByType(AnimalType.Dairy, ManureStateType.SolidStorage);

        Assert.AreEqual(77.3, data.MoistureContent);
        Assert.AreEqual(0.392, data.NitrogenFraction);
        Assert.AreEqual(2.99, data.CarbonFraction);
        Assert.AreEqual(7.63, data.CarbonToNitrogenRatio);
    }

    [TestMethod]
    public void TestSheepData()
    {
        var data = _provider.GetManureCompositionDataByType(AnimalType.Sheep, ManureStateType.Pasture);

        Assert.AreEqual(75, data.MoistureContent);
        Assert.AreEqual(0.765, data.NitrogenFraction);
        Assert.AreEqual(0.211, data.PhosphorusFraction);
        Assert.AreEqual(6.182, data.CarbonFraction);
        Assert.AreEqual(8.08, data.CarbonToNitrogenRatio);
    }

    [TestMethod]
    public void TestPoultryData()
    {
        var data = _provider.GetManureCompositionDataByType(AnimalType.Poultry,
            ManureStateType.SolidStorageWithOrWithoutLitter);

        Assert.AreEqual(44.83, data.MoistureContent);
        Assert.AreEqual(2.427, data.NitrogenFraction);
        Assert.AreEqual(10.12, data.CarbonFraction);
        Assert.AreEqual(1.06, data.PhosphorusFraction);
        Assert.AreEqual(4.17, data.CarbonToNitrogenRatio);
    }

    [TestMethod]
    public void TestSwineData()
    {
        var data = _provider.GetManureCompositionDataByType(AnimalType.Swine, ManureStateType.LiquidWithNaturalCrust);

        Assert.AreEqual(95.16, data.MoistureContent);
        Assert.AreEqual(0.325, data.NitrogenFraction);
        Assert.AreEqual(1.29, data.CarbonFraction);
        Assert.AreEqual(0.118, data.PhosphorusFraction);
        Assert.AreEqual(3.97, data.CarbonToNitrogenRatio);
    }

    [TestMethod]
    public void TestSwineLiquidNaturalCrustData()
    {
        var data = _provider.GetManureCompositionDataByType(AnimalType.Swine,
            ManureStateType.LiquidWithNaturalCrust);

        Assert.AreEqual(95.16, data.MoistureContent);
        Assert.AreEqual(0.325, data.NitrogenFraction);
        Assert.AreEqual(1.29, data.CarbonFraction);
        Assert.AreEqual(0.118, data.PhosphorusFraction);
        Assert.AreEqual(3.97, data.CarbonToNitrogenRatio);
    }


    [TestMethod]
    public void TestIncorrectAnimalType()
    {
        var data = _provider.GetManureCompositionDataByType(AnimalType.Cattle, ManureStateType.AnaerobicDigester);

        Assert.AreEqual(0, data.CarbonContent);
        Assert.AreEqual(0, data.NitrogenContent);
        Assert.AreEqual(0, data.PhosphorusContent);
    }

    [TestMethod]
    public void TestIncorrectManureStateType()
    {
        var data = _provider.GetManureCompositionDataByType(AnimalType.Poultry, ManureStateType.DeepBedding);

        Assert.AreEqual(0, data.CarbonContent);
        Assert.AreEqual(0, data.NitrogenContent);
        Assert.AreEqual(0, data.PhosphorusContent);
    }
}