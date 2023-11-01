using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using H.CLI.UserInput;

namespace H.CLI.Test.UserInput
{
    [TestClass]
    public class CLIArgumentsTest
    {
        [TestMethod]
        public void TestParseArgs()
        {
            CLIArguments argValues = new CLIArguments();

            string test1 = @"H.CLI C:\FarmFolder -i testFarm.json -u metric -s settings.settings -p 2112 -o C:\FarmFolder\Out";
            string[] testArray1 = test1.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            argValues.ParseArgs(testArray1);
            Assert.AreEqual(argValues.FileName, "testFarm.json");
            Assert.AreEqual(argValues.Units, "metric");
            Assert.AreEqual(argValues.Settings, "settings.settings");
            Assert.AreEqual(argValues.PolygonID, "2112");
            Assert.AreEqual(argValues.OutputPath, @"C:\FarmFolder\Out");

            string test2 = @"H.CLI C:\FarmFolder -p 2112 -i testFarm.json -s SETTINGS.settings -u imperial -o C:\FarmFolder\Out";
            string[] testArray2 = test2.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            argValues.ParseArgs(testArray2);
            Assert.AreEqual(argValues.FileName, "testFarm.json");
            Assert.AreEqual(argValues.Units, "imperial");
            Assert.AreEqual(argValues.Settings, "SETTINGS.settings");
            Assert.AreEqual(argValues.PolygonID, "2112");
            Assert.AreEqual(argValues.OutputPath, @"C:\FarmFolder\Out");

            string test3 = @"H.CLI C:\FarmFolder -u METRIC -s settings.settings -p 21122112 -i testFarm.json -o C:\FarmFolder\Out";
            string[] testArray3 = test3.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            argValues.ParseArgs(testArray3);
            Assert.AreEqual(argValues.FileName, "testFarm.json");
            Assert.AreEqual(argValues.Units, "METRIC");
            Assert.AreEqual(argValues.Settings, "settings.settings");
            Assert.AreEqual(argValues.PolygonID, "21122112");
            Assert.AreEqual(argValues.OutputPath, @"C:\FarmFolder\Out");

            Assert.IsFalse(argValues.IsFileNameFound);
            Assert.IsFalse(argValues.IsFolderNameFound);
        }
    }
}
