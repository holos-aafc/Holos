using H.Avalonia.Core.Models.ClassMaps;
using H.Avalonia.Core.Models.Results;
using H.Avalonia.Infrastructure;
using H.Avalonia.ViewModels.ResultViewModels;

namespace H.Avalonia.Test
{
    [TestClass]
    public class ExportDataTest
    {
        private static ExportHelpers _exportHelpers;
        private static ClimateResultsViewModel _climateResultsViewModel;
        private static ClimateResultsViewItemMap _climateResultsViewItemMap;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _exportHelpers = new ExportHelpers();
            _climateResultsViewModel = new ClimateResultsViewModel();
            _climateResultsViewItemMap = new ClimateResultsViewItemMap();


            var item = new ClimateResultsViewItem
            {
                Year = 1991,
                TotalPET = 510.10,
                TotalPPT = 505.05,
                MonthlyPPT = 123.23
            };
            _climateResultsViewModel.ClimateResultsViewItems.Add(item);
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
        public void ExportClimateData()
        {
            var directory = Directory.GetCurrentDirectory();
            var fileName = "file.csv";
            _exportHelpers.ExportPath = $"{directory}.\\{fileName}";
            _exportHelpers.ExportToCSV(_climateResultsViewModel.ClimateResultsViewItems, _climateResultsViewItemMap);
            Assert.IsTrue(Path.Exists(_exportHelpers.ExportPath));
        }
    }
}