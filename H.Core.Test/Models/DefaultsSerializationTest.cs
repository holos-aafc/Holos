using H.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace H.Core.Test.Models
{
    /// <summary>
    /// A farm file written by an earlier version carries whatever properties <see cref="Defaults"/> had at the time.
    /// Loading one must not fail because the class has since lost a property, so these read a payload holding names
    /// the class does not define.
    ///
    /// The serializer is configured the way Storage configures it, so what these exercise is the behaviour a farm
    /// file actually gets.
    /// </summary>
    [TestClass]
    public class DefaultsSerializationTest
    {
        #region Fields

        private JsonSerializerSettings _settings;

        #endregion

        #region Initialization

        [TestInitialize]
        public void TestInitialize()
        {
            _settings = new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.Auto};
        }

        #endregion

        #region Tests

        /// <summary>
        /// DefaultNitrogenFixation was stored on every farm saved before it was removed. Nitrogen fixation is a
        /// property of the crop, read from the view item.
        /// </summary>
        [TestMethod]
        public void DeserializingAPayloadHoldingARemovedPropertySucceeds()
        {
            var json = "{\"DefaultNitrogenFixation\":0.7,\"CarbonConcentration\":0.45,\"FTopo\":14.03}";

            var defaults = JsonConvert.DeserializeObject<Defaults>(json, _settings);

            Assert.IsNotNull(defaults);
        }

        /// <summary>
        /// The unknown name must be skipped rather than stopping the read, so the properties around it still arrive.
        /// </summary>
        [TestMethod]
        public void PropertiesAroundARemovedOneAreStillRead()
        {
            var json = "{\"CarbonConcentration\":0.42,\"DefaultNitrogenFixation\":0.7,\"FTopo\":99.5}";

            var defaults = JsonConvert.DeserializeObject<Defaults>(json, _settings);

            Assert.AreEqual(0.42, defaults.CarbonConcentration, 0.0001);
            Assert.AreEqual(99.5, defaults.FTopo, 0.0001);
        }

        /// <summary>
        /// Nothing in the class answers to the removed name any more, so a saved farm no longer carries it.
        /// </summary>
        [TestMethod]
        public void ARemovedPropertyIsNotWrittenBackOut()
        {
            var json = JsonConvert.SerializeObject(new Defaults(), _settings);

            StringAssert.DoesNotMatch(json, new System.Text.RegularExpressions.Regex("DefaultNitrogenFixation"));
        }

        /// <summary>
        /// A round trip through the serializer keeps the values the class does define.
        /// </summary>
        [TestMethod]
        public void DefaultsRoundTripThroughTheSerializer()
        {
            var original = new Defaults() {CarbonConcentration = 0.44, FTopo = 12.5};

            var restored = JsonConvert.DeserializeObject<Defaults>(
                JsonConvert.SerializeObject(original, _settings), _settings);

            Assert.AreEqual(0.44, restored.CarbonConcentration, 0.0001);
            Assert.AreEqual(12.5, restored.FTopo, 0.0001);
        }

        #endregion
    }
}
