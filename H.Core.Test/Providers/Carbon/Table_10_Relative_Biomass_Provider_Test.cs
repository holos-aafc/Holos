#region Imports

using System.Linq;
using H.Core.Enumerations;
using H.Core.Providers.Carbon;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace H.Core.Test.Providers.Carbon
{
    [TestClass]
    public class Table_10_Relative_Biomass_Provider_Test
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
            _sut = new Table_10_Relative_Biomass_Provider();
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

            Assert.AreEqual(89, result.Count);
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

            var albertaNRate = result.NitrogenFertilizerRateTable[Province.Alberta];
            Assert.AreEqual(61.65, albertaNRate[SoilFunctionalCategory.Brown]);
            Assert.AreEqual(72.86, albertaNRate[SoilFunctionalCategory.DarkBrown]);
            Assert.AreEqual(89.67, albertaNRate[SoilFunctionalCategory.Black]);
            Assert.AreEqual(112.09, albertaNRate[SoilFunctionalCategory.BlackGrayChernozem]);

            var saskatchewanNRate = result.NitrogenFertilizerRateTable[Province.Saskatchewan];
            Assert.AreEqual(68.37, saskatchewanNRate[SoilFunctionalCategory.Brown]);
            Assert.AreEqual(87.43, saskatchewanNRate[SoilFunctionalCategory.DarkBrown]);
            Assert.AreEqual(110.96, saskatchewanNRate[SoilFunctionalCategory.Black]);

            var manitobaNRate = result.NitrogenFertilizerRateTable[Province.Manitoba];
            Assert.AreEqual(100.88, manitobaNRate[SoilFunctionalCategory.All]);

            var albertaPRate = result.PhosphorusFertilizerRateTable[Province.Alberta];
            Assert.AreEqual(22.04, albertaPRate[SoilFunctionalCategory.Brown]);
            Assert.AreEqual(39.23, albertaPRate[SoilFunctionalCategory.DarkBrown]);
            Assert.AreEqual(39.23, albertaPRate[SoilFunctionalCategory.Black]);
            Assert.AreEqual(50.44, albertaPRate[SoilFunctionalCategory.BlackGrayChernozem]);

            var saskatchewanPRate = result.PhosphorusFertilizerRateTable[Province.Saskatchewan];
            Assert.AreEqual(28.02, saskatchewanPRate[SoilFunctionalCategory.Brown]);
            Assert.AreEqual(35.87, saskatchewanPRate[SoilFunctionalCategory.DarkBrown]);
            Assert.AreEqual(45.95, saskatchewanPRate[SoilFunctionalCategory.Black]);

            var manitobaPRate = result.PhosphorusFertilizerRateTable[Province.Manitoba];
            Assert.AreEqual(44.83, manitobaPRate[SoilFunctionalCategory.All]);
        }

        [TestMethod]
        public void GetCanola()
        {
            var result = _sut.GetResidueData(IrrigationType.Irrigated, 202, CropType.Canola, SoilFunctionalCategory.Brown, Province.Alberta);

            Assert.AreEqual(0.529, result.RelativeBiomassStraw);
            Assert.AreEqual(TillageType.NoTill, result.TillageTypeTable[Province.Alberta]);
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
        public void GetPotato()
        {
            var result = _sut.GetResidueData(IrrigationType.RainFed, 0, CropType.Potatoes, SoilFunctionalCategory.Brown, Province.Alberta);

            Assert.AreEqual(TillageType.Intensive, result.TillageTypeTable[Province.Alberta]);
        }

        [TestMethod]
        public void GetLigninContentValue()
        {
            Table_10_Relative_Biomass_Data data = _sut.GetResidueData(IrrigationType.RainFed, 0, CropType.SmallGrainCereals, SoilFunctionalCategory.Black, Province.Alberta);
            Assert.AreEqual(0.074, data.LigninContent);

            data = _sut.GetResidueData(IrrigationType.RainFed, 0, CropType.AlfalfaMedicagoSativaL, SoilFunctionalCategory.DarkBrown, Province.Alberta);
            Assert.AreEqual(0.133, data.LigninContent);
        }

        
        #endregion
    }
}