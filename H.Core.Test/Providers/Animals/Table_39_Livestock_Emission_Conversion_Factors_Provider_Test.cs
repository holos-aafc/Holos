using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class Table_39_Livestock_Emission_Conversion_Factors_Provider_Test
    {
        private Table_39_Livestock_Emission_Conversion_Factors_Provider _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new Table_39_Livestock_Emission_Conversion_Factors_Provider();
        }

        [TestMethod]
        public void GetByAnimalType()
        {
            var result = _provider.GetFactors(manureStateType: ManureStateType.SolidStorage,
                componentCategory: ComponentCategory.BeefProduction,
                meanAnnualPrecipitation: 100,
                meanAnnualTemperature: 23,
                meanAnnualEvapotranspiration: 40, 
                beddingRate: 20, 
                animalType: AnimalType.Beef, farm: new Farm());

            Assert.AreEqual(0.040, result.MethaneConversionFactor);
        }
    }
}
