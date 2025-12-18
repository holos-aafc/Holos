using H.Core.Enumerations;
using H.Core.Providers.Climate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace H.Core.Test.Providers.Climate
{
    [TestClass]
    public class Table_1_Growing_Degree_Crop_Coefficients_Provider_Test
    {
        private Table_1_Growing_Degree_Crop_Coefficients_Provider _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new Table_1_Growing_Degree_Crop_Coefficients_Provider();
        }
        [TestMethod]
        public void GetGrowingDegreeCoefficientsCropNameTest()
        {
            var data = _provider.GetGrowingDegreeCoefficients();

            #region Asserts
            Assert.AreEqual(CropType.AlfalfaSeed, data[0].Crop);
            Assert.AreEqual(CropType.TameLegume, data[1].Crop);
            Assert.AreEqual(CropType.Dill, data[15].Crop);
            Assert.AreEqual(CropType.GreenFeed, data[33].Crop);
            Assert.AreEqual(CropType.FallRye, data[74].Crop);
            #endregion
        }

        [TestMethod]
        public void GetGrowingDegreeCoefficientsABCDETest()
        {

            var data = _provider.GetGrowingDegreeCoefficients();

            Assert.AreEqual(1.05E-01, data[0].A);
            Assert.AreEqual(3.82E-03, data[0].B);
            Assert.AreEqual(-6.77E-06, data[0].C);
            Assert.AreEqual(4.83E-09, data[0].D);
            Assert.AreEqual(-1.25E-12, data[0].E);
            Assert.AreEqual(1.01E+00, data[29].A);
            Assert.AreEqual(4.11E-03, data[29].B);
            Assert.AreEqual(0, data[29].C);
            Assert.AreEqual(0, data[29].D);
            Assert.AreEqual(0, data[29].E);
        }
        [TestMethod]
        public void TestGetByCropType()
        {
            foreach (CropType i in Enum.GetValues(typeof(CropType)))
            {
                var data = _provider.GetByCropType(i);
                Assert.IsNotNull(data);
            }
        }

        [TestMethod]
        public void GetByCropType_Returns_Entry_For_All_ValidCropTypes()
        {
            // Ensure every crop returned by GetValidCropTypes maps to an entry in the provider
            var validCropTypes = CropTypeExtensions.GetValidCropTypes().ToList();

            // Remove types that don't apply
            validCropTypes.Remove(CropType.SummerFallow);

            foreach (var crop in validCropTypes)
            {
                var data = _provider.GetByCropType(crop);
                Assert.IsNotNull(data, $"Provider returned null for crop {crop}");

                Assert.AreNotEqual(0, data.A);

                // Provider returns a default object when no match found; ensure Crop is set (not NotSelected)
                Assert.AreNotEqual(CropType.NotSelected, data.Crop, $"No table entry found for crop {crop}");
            }
        }
    }
}
