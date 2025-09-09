using H.Core.Enumerations;
using H.Core.Providers.Animals;

namespace H.Core.Test.Providers.Animals;

[TestClass]
public class Table_30_Default_Bedding_Material_Composition_Provider_Test
{
    private Table_30_Default_Bedding_Material_Composition_Provider _provider;

    [TestInitialize]
    public void TestInitialize()
    {
        _provider = new Table_30_Default_Bedding_Material_Composition_Provider();
    }

    [TestMethod]
    public void GetBeddingMaterialsDataFromBeddingMaterialType()
    {
        var value = _provider.GetDefaultBeddingRate(HousingType.Confined, BeddingMaterialType.Straw,
            AnimalType.BeefBackgrounder);
        Assert.AreEqual(1.5, value);

        value = _provider.GetDefaultBeddingRate(HousingType.HousedInBarn,
            BeddingMaterialType.WoodChip,
            AnimalType.Beef);
        Assert.AreEqual(5.0, value);


        value = _provider.GetDefaultBeddingRate(HousingType.TieStall,
            BeddingMaterialType.StrawChopped,
            AnimalType.Dairy);
        Assert.AreEqual(0.7, value);

        value = _provider.GetDefaultBeddingRate(HousingType.DryLot,
            BeddingMaterialType.Shavings,
            AnimalType.SheepFeedlot);
        Assert.AreEqual(0.57, value);

        value = _provider.GetDefaultBeddingRate(HousingType.HousedInBarn,
            BeddingMaterialType.Sawdust,
            AnimalType.TurkeyHen);
        Assert.AreEqual(0.011, value);

        value = _provider.GetDefaultBeddingRate(HousingType.DryLot,
            BeddingMaterialType.Straw,
            AnimalType.Goats);
        Assert.AreEqual(0.57, value);
    }
}