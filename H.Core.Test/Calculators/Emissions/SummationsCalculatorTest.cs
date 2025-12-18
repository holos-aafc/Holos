using H.Core.Emissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace H.Core.Test.Calculators.Emissions
{
   [TestClass]
    public class SummationsCalculatorTest
    {
        private SummationsCalculator calc;

        [TestInitialize]
        public void testInitialize()
        {
            calc = new SummationsCalculator();
        }
        // <summary>
        /// Equation 6.1.1-1
        /// </summary>
        [TestMethod]
        public void CalculateDirectNitrousOxideEmissionsFromSoilsToMgGlobalWarming()
        {
            var result = calc.CalculateDirectNitrousOxideEmissionsFromSoilsToMgGlobalWarming(50);
            Assert.AreEqual(14.8, result);
        }

        // <summary>
        /// Equation 6.1.2-1
        /// </summary>
        [TestMethod]
        public void CalculateIndirectNitrousOxideEmissionsFromSoilsToMgGlobalWarming()
        {
            var result = calc.CalculateIndirectNitrousOxideEmissionsFromSoilsToMgGlobalWarming(50);
            Assert.AreEqual(14.8, result);
        }

        // <summary>
        /// Equation 6.2-1
        /// </summary>
        [TestMethod]
        public void CalculateCarbonDioxideEmissionsFromSoilsToMgGlobalWarming()
        {
            var result = calc.CalculateCarbonDioxideEmissionsFromSoilsToMgGlobalWarming(3.523);
            Assert.AreEqual(0.003523, result);
        }

        /// <summary>
        /// Equation 6.3-1
        /// </summary>
        [TestMethod]
        public void CalculateCarbonDioxideEmissionsFromTreePlantingsAndShelterbeltToMgGlobalWarming()
        {
            var result = calc.CalculateCarbonDioxideEmissionsFromTreePlantingsAndShelterbeltToMgGlobalWarming(12);
            Assert.AreEqual(0.012, result);
        }

        /// <summary>
        /// Equation 6.4-1
        /// </summary>
        [TestMethod]
        public void CalculateTotalCarbonDioxideEmissionsFromEnergyUseToMgGlobalWarmin()
        {
            var result = calc.CalculateTotalCarbonDioxideEmissionsFromEnergyUseToMgGlobalWarming(15);
            Assert.AreEqual(0.015, result);
        }

        /// <summary>
        /// Equation 6.5-1
        /// </summary>
        [TestMethod]
        public void CalculateEntericMethaneEmissionFromLivestockToMgGlobalWarming()
        {
            var totalEntericMethaneEmissionsFromLivestock = new List<double>
            {
                5,
                6,
                7,
                8,
                9
            };

            var result = calc.CalculateEntericMethaneEmissionFromLivestockToMgGlobalWarming(totalEntericMethaneEmissionsFromLivestock);
            Assert.AreEqual(0.805, result);
        }

        /// <summary>
        /// Equation 6.6-1
        /// </summary>
        [TestMethod]
        public void CalculateManureMethaneEmissionFromLivestockToMgGlobalWarming()
        {
            var result = calc.CalculateManureMethaneEmissionFromLivestockToMgGlobalWarming(
                new List<double> {
                    1,
                    2,
                    3,
                    4
                });
            Assert.AreEqual(0.23, result);
        }

        /// <summary>
        /// Equation 6.7.1-1
        /// </summary>
        [TestMethod]
        public void CalculateManureDirectNitrousOxideEmissionFromLivestockToMgGlobalWarming()
        {
            var totalManureDirectNitrousOxideEmissionsFromLivestock = new List<double>
            {
                22,
                43,
                14,
                87,
                31
            };

            var result = calc.CalculateManureDirectNitrousOxideEmissionFromLivestockToMgGlobalWarming(totalManureDirectNitrousOxideEmissionsFromLivestock);
            Assert.AreEqual(58.312, result);
        }

        /// <summary>
        /// Equation 6.7.2-1
        /// </summary>
        [TestMethod]
        public void CalculateManureIndirectNitrousOxideEmissionFromLivestockToMgGlobalWarming()
        {
            var result = calc.CalculateManureIndirectNitrousOxideEmissionFromLivestockToMgGlobalWarming(
                new List<double>
                {
                    2,
                    4,
                    6
                });
            Assert.AreEqual(3.552, result);
        }

        /// <summary>
        /// Equation 6.8-1
        /// </summary>
        [TestMethod]
        public void CalculateIndirectNitrousOxideEmissionsFromFarmToMgGlobalWarming()
        {
            var result = calc.CalculateIndirectNitrousOxideEmissionsFromFarmToMgGlobalWarming(58.312, 14.8);
            Assert.AreEqual(73.112, result);
        }

        /// <summary>
        /// Equation 6.9-1
        /// </summary>
        [TestMethod]
        public void CalculateTotalAnnualFarmCarbonDioxideEQEmissionsMg()
        {
            var result = calc.CalculateTotalAnnualFarmCarbonDioxideEQEmissionsMg(14.8, 5.76, 6.66, 0.012, 0.015, 0.805, 12.15, 73.112);
            Assert.AreEqual(113.314, result);
        }


    }
}
