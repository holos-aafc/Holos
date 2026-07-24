using System.Linq;
using System.Reflection;
using H.Core.Mappers;
using H.Core.Models.LandManagement.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Mappers
{
    /// <summary>
    /// The results grid copies its view items before displaying and exporting them, so a calculated value that a copy
    /// does not carry is exported as zero. The mappers copy properties and not fields, so these values have to be
    /// declared as properties to survive.
    /// </summary>
    [TestClass]
    public class CropViewItemResultsCopyTest
    {
        /// <summary>
        /// The emission factors written to the results file. Declaring any of these as a public field silently drops it
        /// from every copy - which is how the grid's CSV export came to write zero for all three.
        /// </summary>
        private static readonly string[] ExportedEmissionFactors =
        {
            nameof(CropViewItem.EF_SN),
            nameof(CropViewItem.EF_CRN),
            nameof(CropViewItem.EF_ON),
        };

        /// <summary>The mapper the results grid uses to copy view items before display and export.</summary>
        private static ModelMapper<CropViewItem> ResultsGridMapper()
        {
            return new ModelMapper<CropViewItem>(
                nameof(CropViewItem.Guid),
                nameof(CropViewItem.HarvestViewItems),
                nameof(CropViewItem.GrazingViewItems),
                nameof(CropViewItem.ManureApplicationViewItems),
                nameof(CropViewItem.CropEconomicData));
        }

        [TestMethod]
        public void CopyingAViewItemKeepsTheExportedEmissionFactors()
        {
            var source = new CropViewItem
            {
                EF_SN = 0.0123,
                EF_CRN = 0.0456,
                EF_ON = 0.0789,
            };

            var copy = new CropViewItem();
            ResultsGridMapper().Map(source, copy);

            Assert.AreEqual(source.EF_SN, copy.EF_SN, "EF_SN was not copied and would export as zero");
            Assert.AreEqual(source.EF_CRN, copy.EF_CRN, "EF_CRN was not copied and would export as zero");
            Assert.AreEqual(source.EF_ON, copy.EF_ON, "EF_ON was not copied and would export as zero");
        }

        [TestMethod]
        public void TheExportedEmissionFactorsAreDeclaredAsProperties()
        {
            // The root cause rather than the symptom: PropertyMapper enumerates GetProperties, so a public field is
            // invisible to it. This fails immediately if one of these is turned back into a field.
            foreach (var name in ExportedEmissionFactors)
            {
                var property = typeof(CropViewItem).GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                var field = typeof(CropViewItem).GetField(name, BindingFlags.Public | BindingFlags.Instance);

                Assert.IsNull(field,
                    $"{name} is a public field; the mappers copy properties only, so it would be dropped from every copy");
                Assert.IsNotNull(property, $"{name} should be a property");
                Assert.IsTrue(property.CanRead && property.CanWrite,
                    $"{name} must be readable and writable for the mappers to copy it");
            }
        }

        [TestMethod]
        public void ThePropertyMapperCopiesTheExportedEmissionFactors()
        {
            // Guards the mapper itself: these must appear in the set of members it copies.
            var copied = typeof(CropViewItem)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanRead && x.CanWrite && x.GetIndexParameters().Length == 0)
                .Select(x => x.Name)
                .ToList();

            foreach (var name in ExportedEmissionFactors)
            {
                CollectionAssert.Contains(copied, name, $"{name} is not a member the mappers would copy");
            }
        }
    }
}
