using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Providers.Animals;

namespace H.Core.Test.Providers.Animals;

[TestClass]
public class Table_36_Livestock_Emission_Conversion_Factors_Provider_Test : UnitTestBase
{
    #region Fields

    private Table_36_Livestock_Emission_Conversion_Factors_Provider _provider;

    #endregion

    #region Initialization

    [TestInitialize]
    public void TestInitialize()
    {
        _provider = new Table_36_Livestock_Emission_Conversion_Factors_Provider();
    }

    #endregion

    #region Tests

    [TestMethod]
    public void GetByAnimalType()
    {
        var result = _provider.GetFactors(ManureStateType.SolidStorage,
            100,
            23,
            40,
            20,
            AnimalType.Beef, new Farm(), DateTime.Now.Year);

        Assert.AreEqual(0.040, result.MethaneConversionFactor);
    }

    [TestMethod]
    public void GetLandEmissionFactorsReturnsNonZeroForAllAnimalTypes()
    {
        var farm = GetTestFarm();
        var volatilizationFractions = new List<Tuple<AnimalType, double>>();

        foreach (var animalType in Enum.GetValues(typeof(AnimalType)).Cast<AnimalType>())
        {
            var result = _provider.GetLandApplicationFactors(
                farm,
                10,
                20,
                animalType,
                DateTime.Now.Year);

            volatilizationFractions.Add(new Tuple<AnimalType, double>(animalType, result.VolatilizationFraction));

            Assert.IsTrue(result.VolatilizationFraction > 0);
        }
    }

    #endregion
}