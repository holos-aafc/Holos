using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
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
            var result = _provider.GetFactors(manureStateType: ManureStateType.SolidStorage,
                componentCategory: ComponentCategory.BeefProduction,
                meanAnnualPrecipitation: 100,
                meanAnnualTemperature: 23,
                meanAnnualEvapotranspiration: 40,
                beddingRate: 20,
                animalType: AnimalType.Beef, farm: new Farm(), year: DateTime.Now.Year);

            Assert.AreEqual(0.040, result.MethaneConversionFactor);
        }

        [TestMethod]
        public void GetLandEmissionFactorsReturnsNonZeroForAllAnimalTypes()
        {
            var farm = base.GetTestFarm();
            var volatilizationFractions = new List<Tuple<AnimalType, double>>();

            foreach (var animalType in Enum.GetValues(typeof(AnimalType)).Cast<AnimalType>())
            {
                var result = _provider.GetLandApplicationFactors(
                    farm: farm,
                    meanAnnualPrecipitation: 10,
                    meanAnnualEvapotranspiration: 20,
                    animalType: animalType,
                    DateTime.Now.Year);

                volatilizationFractions.Add(new Tuple<AnimalType, double>(animalType, result.VolatilizationFraction));

                Assert.IsTrue(result.VolatilizationFraction > 0);
            }
        }

        #endregion
    }
}
