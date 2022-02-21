using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.TemporaryComponentStorage;

namespace H.CLI.Test.ComponentTemporaryInputs
{
    [TestClass]
    public class FieldTemporaryInputTest
    {
        private readonly FieldTemporaryInput fieldTempInput = new FieldTemporaryInput();

        [TestMethod]
        public void TestConvertToComponentProperties_ExpectInvalidPropertyNameException()
        {
         
            Assert.ThrowsException<InvalidPropertyNameException>(() => fieldTempInput.ConvertToComponentProperties("NotAValidPropertyName", null, "FieldName", 1, 1, "fileName"));
        }

        [TestMethod] 
        public void TestConvertToComponentProperties_ExpectException_InvalidResponseType()
        {
            
            Assert.ThrowsException<FormatException>(() => fieldTempInput.ConvertToComponentProperties("IsIrrigated", null, "NotYesOrNo", 1, 1, "fileName"));
        }

        [TestMethod]
        public void TestConvertToComponentProperties_ExpectException_InvalidManureLocationSourceType()
        {
            
            Assert.ThrowsException<FormatException>(() => fieldTempInput.ConvertToComponentProperties("ManureLocationSourceType", null, "NotAValidManureLocationSourceType", 1, 1, "fileName"));
        }

        
        [TestMethod]
        public void TestConvertToComponentProperties_ExpectException_ManureStateType()
        {
        
            Assert.ThrowsException<FormatException>(() => fieldTempInput.ConvertToComponentProperties("ManureStateType", null, "NotAValidManureStateType", 1, 1, "fileName"));
        }
        

        [TestMethod]
        public void TestConvertToComponentProperties_ExpectException_InvalidManureApplicationType()
        {
        
            Assert.ThrowsException<FormatException>(() => fieldTempInput.ConvertToComponentProperties("ManureApplicationType", null, "NotAValidManureApplicationType", 1, 1, "fileName"));
        }

        [TestMethod]
        public void TestConvertToComponentProperties_ExpectException_InvalidCoverCropTerminationType()
        {
          
            Assert.ThrowsException<FormatException>(() => fieldTempInput.ConvertToComponentProperties("CoverCropTerminationType", null, "NotAValidCoverCropTerminationType", 1, 1, "fileName"));
        }

        [TestMethod]
        public void TestConvertToComponentProperties_ExpectException_InvalidCoverCropType()
        {
         
            Assert.ThrowsException<FormatException>(() => fieldTempInput.ConvertToComponentProperties("CoverCropType", null, "NotAValidCoverCropType", 1, 1, "fileName"));
        }

        [TestMethod]
        public void TestConvertToComponentProperties_ExpectException_InvalidManureAnimalSourceType()
        {
          
            Assert.ThrowsException<FormatException>(() => fieldTempInput.ConvertToComponentProperties("ManureAnimalSourceType", null, "NotAValidManureAnimalSourceType", 1, 1, "fileName"));
        }

        [TestMethod]
        public void TestConvertToComponentProperties_ExpectException_InvalidIrrigationType()
        {
            
            Assert.ThrowsException<FormatException>(() => fieldTempInput.ConvertToComponentProperties("IrrigationType", null, "NotAValidIrrigationType", 1, 1, "fileName"));
        }

        [TestMethod]
        public void TestConvertToComponentProperties_ExpectException_InvalidCropType()
        {
           
            Assert.ThrowsException<FormatException>(() => fieldTempInput.ConvertToComponentProperties("CropType", null, "NotAValidCropType", 1, 1, "fileName"));
        }

        [TestMethod]
        public void TestConvertToComponentProperties_ExpectException_InvalidHarvestMethod()
        {
            
            Assert.ThrowsException<FormatException>(() => fieldTempInput.ConvertToComponentProperties("HarvestMethod", null, "NotAValidHarvestMethod", 1, 1, "fileName"));
        }

        [TestMethod]
        public void TestConvertToComponentProperties_ExpectException_InvalidTillageType()
        {
           
            Assert.ThrowsException<FormatException>(() => fieldTempInput.ConvertToComponentProperties("TillageType", null, "NotAValidTillageType", 1, 1, "fileName"));
        }

        [TestMethod]
        public void TestConvertToComponentProperties_ExpectYearInPerennialToBe0_ValueIsNotApplicable()
        {
          
            fieldTempInput.ConvertToComponentProperties("YearInPerennialStand", null, "N/A", 1, 1, "fileName");
            Assert.AreEqual(0, fieldTempInput.YearInPerennialStand);
        }


        [TestMethod]
        public void TestConvertToComponentProperties_ExpectRowIdToBe0_ValueIsNotApplicable()
        {
           
            fieldTempInput.ConvertToComponentProperties("PerennialStandID", null, "N/A", 1, 1, "fileName");
            Assert.AreEqual(default(Guid), fieldTempInput.PerennialStandID);
        }

        [TestMethod]
        public void TestConvertToComponentProperties_ExpectPerennialStandLengthToBe0_ValueIsNotApplicable()
        {
          
            fieldTempInput.ConvertToComponentProperties("PerennialStandLength", null, "N/A", 1, 1, "fileName");
            Assert.AreEqual(0, fieldTempInput.PerennialStandLength);
        }

        [TestMethod]
        public void TestConvertToComponentProperties_ExpectIsIrrigatedToBeSet_ValueIsYes()
        {
           
            fieldTempInput.ConvertToComponentProperties("IsIrrigated", null, "Yes", 1, 1, "fileName");
            Assert.AreEqual(fieldTempInput.IsIrrigated, Response.Yes);
        }

        [TestMethod]
        public void TestConvertToComponentProperties_ExpectIrrigationTypeToBeSet_ValueIsIrrigated()
        {
     
            fieldTempInput.ConvertToComponentProperties("IrrigationType", null, "iRriGATed", 1, 1, "fileName");
            Assert.AreEqual(fieldTempInput.IrrigationType, IrrigationType.Irrigated);
        }

        [TestMethod]
        public void TestConvertToComponentProperties_ExpectCropTypeToBeSet_ValueIsBarley()
        {
     
            fieldTempInput.ConvertToComponentProperties("CropType", null, "bARLEY", 1, 1, "fileName");
            Assert.AreEqual(fieldTempInput.CropType, CropType.Barley);
        }

        [TestMethod]
        public void TestConvertToComponentProperties_ExpectFormatException_ValueIsEmpty()
        {
        
            Assert.ThrowsException<FormatException>(() => fieldTempInput.ConvertToComponentProperties("CropType", null, " ", 1, 1, "fileName"));
        }

    }
}
