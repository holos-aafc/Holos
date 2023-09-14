using H.Avalonia.Models.ClassMaps;
using H.Avalonia.ViewModels;
using H.Avalonia.Infrastructure;

namespace H.Avalonia.Test
{
    [Ignore]
    [TestClass]
    public class ImportDataTest
    {
        private static ImportHelpers _importHelpers;
        private static ClimateDataViewModel _climateDataViewModel;
        private static ClimateViewItemMap _climateViewItemMap;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _importHelpers = new ImportHelpers();
            _climateDataViewModel = new ClimateDataViewModel();
            _climateViewItemMap = new ClimateViewItemMap();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public void TestImportCsvFile()
        {
            var directory = Directory.GetCurrentDirectory();
            var fileName = "Climate_Data.csv";
            _importHelpers.ImportPath = $"{directory}.\\{fileName}";
            var result = _importHelpers.ImportFromCsv(_climateViewItemMap);
            Assert.IsTrue(result?.Any());
        }
    }
}