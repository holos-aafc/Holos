using System;
using System.IO;
using H.CLI.UserInput;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.CLI.Test.UserInput
{
    [TestClass]
    public class CLIUnitsOfMeasurementConstantsTest
    {
        private TextReader _originalIn;
        private TextWriter _originalOut;

        [TestInitialize]
        public void TestInitialize()
        {
            _originalIn = Console.In;
            _originalOut = Console.Out;

            // The prompt writes to the console; keep it out of the test output.
            Console.SetOut(TextWriter.Null);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Console.SetIn(_originalIn);
            Console.SetOut(_originalOut);
        }

        /// <summary>
        /// An empty reader makes Console.ReadLine return null, which is what happens when the CLI is run without a
        /// terminal. The prompt loop used to re-ask forever in that case: int.TryParse(null) does not throw, it
        /// simply fails the loop condition, so the loop went round again against a stream that would never produce
        /// anything. One observed spin burned eleven minutes of CPU before being killed.
        ///
        /// The timeout is the point of the test - a regression here hangs rather than failing, and without it the
        /// suite would stop instead of reporting.
        /// </summary>
        [TestMethod]
        [Timeout(10000)]
        public void PromptUserForUnitsOfMeasurementDefaultsToMetricWhenThereIsNoInput()
        {
            CLIUnitsOfMeasurementConstants.measurementSystem = MeasurementSystemType.Imperial;
            Console.SetIn(new StringReader(string.Empty));

            CLIUnitsOfMeasurementConstants.PromptUserForUnitsOfMeasurement(null);

            Assert.AreEqual(MeasurementSystemType.Metric, CLIUnitsOfMeasurementConstants.measurementSystem);
        }

        /// <summary>
        /// An answer that is neither 1 nor 2 re-prompts, and must still reach a usable value rather than spinning
        /// once the input runs out.
        /// </summary>
        [TestMethod]
        [Timeout(10000)]
        public void PromptUserForUnitsOfMeasurementStopsRePromptingWhenTheInputRunsOut()
        {
            CLIUnitsOfMeasurementConstants.measurementSystem = MeasurementSystemType.Imperial;
            Console.SetIn(new StringReader("banana" + Environment.NewLine));

            CLIUnitsOfMeasurementConstants.PromptUserForUnitsOfMeasurement(null);

            Assert.AreEqual(MeasurementSystemType.Metric, CLIUnitsOfMeasurementConstants.measurementSystem);
        }

        [TestMethod]
        [Timeout(10000)]
        public void PromptUserForUnitsOfMeasurementHonoursATypedAnswer()
        {
            CLIUnitsOfMeasurementConstants.measurementSystem = MeasurementSystemType.Metric;
            Console.SetIn(new StringReader("2" + Environment.NewLine));

            CLIUnitsOfMeasurementConstants.PromptUserForUnitsOfMeasurement(null);

            Assert.AreEqual(MeasurementSystemType.Imperial, CLIUnitsOfMeasurementConstants.measurementSystem);
        }

        /// <summary>
        /// The -u argument short-circuits the prompt, so no console input is read at all.
        /// </summary>
        [TestMethod]
        [Timeout(10000)]
        public void PromptUserForUnitsOfMeasurementUsesTheArgumentWithoutPrompting()
        {
            CLIUnitsOfMeasurementConstants.measurementSystem = MeasurementSystemType.Imperial;
            Console.SetIn(new StringReader(string.Empty));

            CLIUnitsOfMeasurementConstants.PromptUserForUnitsOfMeasurement("metric");

            Assert.AreEqual(MeasurementSystemType.Metric, CLIUnitsOfMeasurementConstants.measurementSystem);
        }
    }
}
