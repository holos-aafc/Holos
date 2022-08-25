using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Fertilizer;

namespace H.Core.Test.Models.LandManagement.Fields
{
    [TestClass]
    public class FertilizerApplicationViewItemTest
    {
        [TestMethod]
        public void UpdatingAmountUpdatesAmountOfNitrogenApplied()
        {
            var viewItem = new FertilizerApplicationViewItem()
            {
                AmountOfBlendedProductApplied = 100,
                FertilizerBlendData = new Table_51_Carbon_Footprint_For_Fertilizer_Blends_Data()
                {
                    PercentageNitrogen = 20,
                }
            };

            viewItem.AmountOfBlendedProductApplied = 200;

            Assert.AreEqual(40, viewItem.AmountOfNitrogenApplied);
        }
    }
}
