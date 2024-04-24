#region Imports

using System.Linq;
using H.Core.Enumerations;
using H.Core.Providers.Carbon;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace H.Core.Test.Providers.Carbon
{
    [TestClass]
    public class Table_7_Relative_Biomass_Information_Provider_Test
    {
        #region Fields

        private IResidueDataProvider _sut;

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
            _sut = new Table_7_Relative_Biomass_Information_Provider();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void GetData()
        {
            var result = _sut.GetData().ToList();
            Assert.AreEqual(90, result.Count);
        }

        [TestMethod]
        public void GetCoverCrop()
        {
            var result = _sut.GetResidueData(
                irrigationType: IrrigationType.RainFed,
                irrigationAmount: 0,
                cropType: CropType.AnnualRyeGrassLoliumMultiflorum,
                soilFunctionalCategory: SoilFunctionalCategory.Black,
                province: Province.Alberta);

            Assert.AreEqual(5.2, result.NitrogenContentExtraroot);
        }

        [TestMethod]
        public void GetCanadaWidePotatoes()
        {
            // In the table there is an entry for potatoes not in Alberta and one for potatoes in Canada. Test getting the Canada entry

            var result = _sut.GetResidueData(
                irrigationType: IrrigationType.Irrigated,
                irrigationAmount: 200,
                cropType: CropType.Potatoes,
                soilFunctionalCategory: SoilFunctionalCategory.Brown,
                province: Province.Saskatchewan);

            Assert.AreEqual(75, result.MoistureContentOfProduct);
        }

        [TestMethod]
        public void GetAlbertaPotatoes()
        {
            // In the table there is an entry for potatoes not in Alberta and one for potatoes in Canada. Test getting the Alberta entry

            var result = _sut.GetResidueData(
                irrigationType: IrrigationType.Irrigated,
                irrigationAmount: 200,
                cropType: CropType.Potatoes,
                soilFunctionalCategory: SoilFunctionalCategory.Brown,
                province: Province.Alberta);

            Assert.AreEqual(80, result.MoistureContentOfProduct);
        }

        [TestMethod]
        public void GetSmallGrainCereals()
        {
            var result = _sut.GetData().ToList().Single(x => x.CropType == CropType.Oats);
                        
            Assert.AreEqual(12d, result.MoistureContentOfProduct);
            Assert.AreEqual(0.319d, result.RelativeBiomassProduct);
            Assert.AreEqual(0.283d, result.RelativeBiomassStraw);
            Assert.AreEqual(0.241d, result.RelativeBiomassRoot);
            Assert.AreEqual(0.157d, result.RelativeBiomassExtraroot);
            Assert.AreEqual(18d, result.NitrogenContentProduct);
            Assert.AreEqual(6d, result.NitrogenContentStraw);
            Assert.AreEqual(10d, result.NitrogenContentRoot);
            Assert.AreEqual(10d, result.NitrogenContentExtraroot);
        }

        [TestMethod]
        public void GetCanola()
        {
            var result = _sut.GetResidueData(IrrigationType.Irrigated, 202, CropType.Canola, SoilFunctionalCategory.Brown, Province.Alberta);

            Assert.AreEqual(0.529, result.RelativeBiomassStraw);
        }

        [TestMethod]
        public void GetOilseeds()
        {
            var result = _sut.GetResidueData(IrrigationType.RainFed, 0, CropType.Oilseeds, SoilFunctionalCategory.Brown, Province.Alberta);

            Assert.AreEqual(0.184, result.RelativeBiomassProduct);
        }

        [TestMethod]
        public void GetOats()
        {
            var result = _sut.GetResidueData(IrrigationType.RainFed, 0, CropType.Oats, SoilFunctionalCategory.Brown, Province.Alberta);

            Assert.AreEqual(0.319, result.RelativeBiomassProduct);
        }

        [TestMethod]
        public void GetLigninContentValue()
        {
            Table_7_Relative_Biomass_Information_Data data = _sut.GetResidueData(IrrigationType.RainFed, 0, CropType.SmallGrainCereals, SoilFunctionalCategory.Black, Province.Alberta);
            Assert.AreEqual(0.073, data.LigninContent);

            data = _sut.GetResidueData(IrrigationType.RainFed, 0, CropType.AlfalfaMedicagoSativaL, SoilFunctionalCategory.DarkBrown, Province.Alberta);
            Assert.AreEqual(0.133, data.LigninContent);
        }

        #endregion
    }
}