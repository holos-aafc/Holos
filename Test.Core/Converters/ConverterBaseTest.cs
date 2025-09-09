using H.Core.Converters;
using H.Core.Enumerations;

namespace H.Core.Test.Converters;

[TestClass]
public class ConverterBaseTest
{
    private CropTypeStringConverter _converter;

    [TestInitialize]
    public void Initialize()
    {
        _converter = new CropTypeStringConverter();
    }

    [TestMethod]
    public void GetLettersAsLowerCaseTest()
    {
        var result = _converter.Convert("Peas, dry");

        Assert.AreEqual(result, CropType.DryPeas);
    }
}