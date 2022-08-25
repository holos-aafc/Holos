using AutoMapper;
using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum ImperialUnitsOfMeasurement
    {
        [LocalizedDescription("EnumInches", typeof(Resources))]
        EnumInches,

        [LocalizedDescription("EnumPounds", typeof(Resources))]
        EnumPounds,

        [LocalizedDescription("EnumGrains", typeof(Resources))]
        EnumGrains,

        [LocalizedDescription("EnumAcres", typeof(Resources))]
        EnumAcres,

        [LocalizedDescription("EnumYards", typeof(Resources))]
        EnumYards,

        [LocalizedDescription("InchesToMm", typeof(Resources))]
        InchesToMm,

        [LocalizedDescription("InchesToCm", typeof(Resources))]
        InchesToCm,

        [LocalizedDescription("Pounds", typeof(Resources))]
        Pounds,

        [LocalizedDescription("Grains", typeof(Resources))]
        Grains,

        [LocalizedDescription("Acres", typeof(Resources))]
        Acres,

        [LocalizedDescription("Yards", typeof(Resources))]
        Yards,

        [LocalizedDescription("BushelsPerAcre", typeof(Resources))]
        BushelsPerAcre,

        [LocalizedDescription("PoundsPerAcre", typeof(Resources))]
        PoundsPerAcre,

        [LocalizedDescription("InchesPerYear", typeof(Resources))]
        InchesPerYear,


        [LocalizedDescription("DegreesFahrenheit", typeof(Resources))]
        DegreesFahrenheit,

        [LocalizedDescription("BritishThermalUnitPerPound", typeof(Resources))]
        BritishThermalUnitPerPound,

        [LocalizedDescription("GrainsPerPound", typeof(Resources))]
        GrainsPerPound,

        [LocalizedDescription("InternationalUnitsPerGrain", typeof(Resources))]
        InternationalUnitsPerGrain,

        [LocalizedDescription("PoundsPerPound", typeof(Resources))]
        PoundsPerPound,

        [LocalizedDescription("PoundsPerYear", typeof(Resources))]
        PoundsPerYear,

        [LocalizedDescription("PoundPerHeadPerYear", typeof(Resources))]
        PoundPerHeadPerYear,

        [LocalizedDescription("PoundPerHeadPerDay", typeof(Resources))]
        PoundPerHeadPerDay,

        [LocalizedDescription("PoundsN2ONPerPoundN", typeof(Resources))]
        PoundsN2ONPerPoundN,

        [LocalizedDescription("PoundsN2ONPerYear", typeof(Resources))]
        PoundsN2ONPerYear,

        [LocalizedDescription("PoundsN2OPerYear", typeof(Resources))]
        PoundsN2OPerYear,

        [LocalizedDescription("PoundsN2ON", typeof(Resources))]
        PoundsN2ON,

        [LocalizedDescription("PoundsNitrogen", typeof(Resources))]
        PoundsNitrogen,

        [LocalizedDescription("PoundsNitrogenPerAcre", typeof(Resources))]
        PoundsNitrogenPerAcre,

        [LocalizedDescription("PoundsNitrogenPerAcrePerYear", typeof(Resources))]
        PoundsNitrogenPerAcrePerYear,

        [LocalizedDescription("PoundsPhosphorousPerAcre", typeof(Resources))]
        PoundsPhosphorousPerAcre,

        [LocalizedDescription("CubicYardsMethanePerPoundVolatileSolids", typeof(Resources))]
        CubicYardsMethanePerPoundVolatileSolids,

        [LocalizedDescription("PoundsMethanePerPoundMethane", typeof(Resources))]
        PoundsMethanePerPoundMethane,

        [LocalizedDescription("PoundsMethanePerYear", typeof(Resources))]
        PoundsMethanePerYear,

        [LocalizedDescription("PoundsMethane", typeof(Resources))]
        PoundsMethane,

        [LocalizedDescription("BritishThermalUnitPerDayPerPound", typeof(Resources))]
        BritishThermalUnitPerDayPerPound,

        [LocalizedDescription("BritishThermalUnitPerPoundSquared", typeof(Resources))]
        BritishThermalUnitPerPoundSquared,

        [LocalizedDescription("PoundsNitrogenPerPound", typeof(Resources))]
        PoundsNitrogenPerPound,

        [LocalizedDescription("PoundsPerPoundProteinIntake", typeof(Resources))]
        PoundsPerPoundProteinIntake,

        [LocalizedDescription("PoundsVolatileSolidsPerPoundFeed", typeof(Resources))]
        PoundsVolatileSolidsPerPoundFeed,

        [LocalizedDescription("BritishThermalUnitsPerAcre", typeof(Resources))]
        BritishThermalUnitsPerAcre,

        [LocalizedDescription("PoundsPerDay", typeof(Resources))]
        PoundsPerDay,

        [LocalizedDescription("PoundsCarbonPerAcre", typeof(Resources))]
        PoundsCarbonPerAcre,

        [LocalizedDescription("PoundsCarbonPerTree", typeof(Resources))]
        PoundsCarbonPerTree,

        [LocalizedDescription("PoundsCarbonPerPlanting", typeof(Resources))]
        PoundsCarbonPerPlanting,

        [LocalizedDescription("PoundsCarbonDioxidePerShelterbelt", typeof(Resources))]
        PoundsCarbonDioxidePerShelterbelt,

        [LocalizedDescription("PercentageAF", typeof(Resources))]
        PercentageAF,

        [LocalizedDescription("PercentageH", typeof(Resources))]
        PercentageH,

        [LocalizedDescription("PercentageNdf", typeof(Resources))]
        PercentageNdf,

        [LocalizedDescription("PercentageCrudeProtein", typeof(Resources))]
        PercentageCrudeProtein,

        [LocalizedDescription("PercentageDryMatter", typeof(Resources))]
        PercentageDryMatter,

        [LocalizedDescription("Percentage", typeof(Resources))]
        Percentage,

        [LocalizedDescription("Days", typeof(Resources))]
        Days,

        [LocalizedDescription("Months", typeof(Resources))]
        Months,

        [LocalizedDescription("Years", typeof(Resources))]
        Years,

        [LocalizedDescription("PoundsN2ONPerAcre", typeof(Resources))]
        PoundsN2ONPerAcre,

        [LocalizedDescription("PoundsNONPerAcre", typeof(Resources))]
        PoundsNONPerAcre,

        [LocalizedDescription("PoundsNO3NPerAcre", typeof(Resources))]
        PoundsNO3NPerAcre,

        [LocalizedDescription("PoundsNH4NPerAcre", typeof(Resources))]
        PoundsNH4NPerAcre,

        [LocalizedDescription("PoundsN2NPerAcre", typeof(Resources))]
        PoundsN2NPerAcre,

        [LocalizedDescription("PoundsCO2PerBTU", typeof(Resources))]
        PoundsCO2PerBTU,

        [LocalizedDescription("BTUPerAnimal", typeof(Resources))]
        BTUPerAnimal,

        [LocalizedDescription("BTUPerPoultryPlacement", typeof(Resources))]
        BTUPerPoultryPlacement,

        [LocalizedDescription("BTUPer1000Quarts", typeof(Resources))]
        BTUPer1000Quarts,

        [LocalizedDescription("EnumTon", typeof(Resources))]
        Ton,

        [LocalizedDescription("EnumShortTon", typeof(Resources))]
        ShortTon,

        [LocalizedDescription("NormalCubicYardsPerTonVolatileSolids", typeof(Resources))]
        NormalCubicYardsPerTonVolatileSolids,

        [LocalizedDescription("PoundVolatileSolidsPerCubicYardPerDay", typeof(Resources))]
        PoundVolatileSolidsPerCubicYardPerDay,

        [LocalizedDescription("BTUPerStandardCubicFoot", typeof(Resources))]
        BTUPerStandardCubicFoot,

        // incorrect conversion
        [LocalizedDescription("BTUPerKiloWattHour", typeof(Resources))]
        BTUPerKiloWattHour,

        [LocalizedDescription("MethanePerCubicYardPerDay", typeof(Resources))]
        MethanePerCubicYardPerDay,

        [LocalizedDescription("NitrousOxidePerCubicYardPerDay", typeof(Resources))]
        NitrousOxidePerCubicYardPerDay,

        [LocalizedDescription("AmmoniaPerCubicYardPerDay", typeof(Resources))]
        AmmoniaPerCubicYardPerDay,

        [LocalizedDescription("PerDay", typeof(Resources))]
        PerDay,

        [LocalizedDescription("DollarsPerTon", typeof(Resources))]
        DollarsPerTon,

        [LocalizedDescription("DollarsPerPound", typeof(Resources))]
        DollarsPerPound,
    }
}