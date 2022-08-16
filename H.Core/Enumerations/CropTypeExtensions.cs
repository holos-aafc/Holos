#region Imports

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    /// <summary>
    /// </summary>
    public static class CropTypeExtensions
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public static bool IsPerennial(this CropType cropType)
        {
            switch (cropType)
            {
                case CropType.Forage:
                case CropType.TameGrass:
                case CropType.TameLegume:
                case CropType.TameMixed:
                case CropType.PerennialForages:
                case CropType.ForageForSeed:
                case CropType.SeededGrassland:
                case CropType.RangelandNative:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsPasture(this CropType cropType)
        {
            if (cropType.IsPerennial() || cropType.IsGrassland())
            {
                return true;
            }

            return false;
        }

        public static bool IsCoverCrop(this CropType cropType)
        {
            switch (cropType)
            {
                case CropType.RedCloverTrifoliumPratenseL:
                case CropType.BerseemCloverTrifoliumAlexandriumL:
                case CropType.SweetCloverMelilotusOfficinalis:
                case CropType.CrimsonCloverTrifoliumIncarnatum:
                case CropType.HairyVetchViciaVillosaRoth:
                case CropType.AlfalfaMedicagoSativaL:
                case CropType.FabaBeanBroadBeanViciaFaba:
                case CropType.CowpeaVignaUnguiculata:
                case CropType.AustrianWinterPea:
                case CropType.RapeseedBrassicaNapusL:
                case CropType.WinterTurnipRapeBrassicaRapaSppOleiferaLCVLargo:
                case CropType.PhaceliaPhaceliaTanacetifoliaCVPhaci:
                case CropType.ForageRadishRaphanusSativusL:
                case CropType.MustardSinapusAlbaLSubspMaireiHLindbMaire:
                case CropType.BarleyHordeumVulgare:
                case CropType.OatAvenaSativa:
                case CropType.RyeSecaleCerealeWinterRyeCerealRye:
                case CropType.SesameSesamumIndicum:
                case CropType.FlaxLinumUsitatissimum:
                case CropType.RyeGrassLoliumPerenneL:
                case CropType.AnnualRyeGrassLoliumMultiflorum:
                case CropType.SorghumSorghumBicolour:
                case CropType.PigeonBean:
                case CropType.ShepherdsPurse:
                case CropType.WinterWheatTriticumAestivum:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsLeguminousCoverCrop (this CropType cropType)
        {
            switch (cropType)
            {
                case CropType.RedCloverTrifoliumPratenseL:
                case CropType.BerseemCloverTrifoliumAlexandriumL:
                case CropType.SweetCloverMelilotusOfficinalis:
                case CropType.CrimsonCloverTrifoliumIncarnatum:
                case CropType.HairyVetch:
                case CropType.AlfalfaMedicagoSativaL:
                case CropType.FabaBeanBroadBeanViciaFaba:
                case CropType.CowpeaVignaUnguiculata:
                case CropType.AustrianWinterPea:
                case CropType.PigeonBean:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNonLeguminousCoverCrop (this CropType cropType)
        {
            switch (cropType)
            {
                case CropType.WinterWeeds:
                case CropType.RapeseedBrassicaNapusL:
                case CropType.WinterTurnipRapeBrassicaRapaSppOleiferaLCVLargo:
                case CropType.PhaceliaPhaceliaTanacetifoliaCVPhaci:
                case CropType.ForageRadishRaphanusSativusL:
                case CropType.MustardSinapusAlbaLSubspMaireiHLindbMaire:
                case CropType.BarleyHordeumVulgare:
                case CropType.OatAvenaSativa:
                case CropType.RyeSecaleCerealeWinterRyeCerealRye:
                case CropType.SesameSesamumIndicum:
                case CropType.FlaxLinumUsitatissimum:
                case CropType.RyeGrassLoliumPerenneL:
                case CropType.AnnualRyeGrassLoliumMultiflorum:
                case CropType.SorghumSorghumBicolour:
                case CropType.WinterWheatTriticumAestivum:
                case CropType.FallRye:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsRangeland(this CropType cropType)
        {
            return false;
        }

        public static bool IsGrassland(this CropType cropType)
        {


            return cropType == CropType.BrokenGrassland ||
                   cropType == CropType.GrasslandSeeded ||
                   cropType == CropType.RangelandNative;
        }

        public static bool IsNativeGrassland(this CropType cropType)
        {
            return cropType == CropType.RangelandNative;
        }

        public static bool IsFallow(this CropType cropType)
        {
            return cropType == CropType.Fallow ||
                   cropType == CropType.SummerFallow;
        }

        public static bool IsAnnual(this CropType cropType)
        {
            if (cropType.IsSilageCrop() || cropType.IsRootCrop())
            {
                return true;
            }

            switch (cropType)
            {
                case CropType.SmallGrainCereals:
                case CropType.Wheat:
                case CropType.WheatSilage:
                case CropType.Barley:
                case CropType.BarleySilage:
                case CropType.UndersownBarley:
                case CropType.Oats:
                case CropType.OatSilage:
                case CropType.Triticale:
                case CropType.TriticaleSilage:
                case CropType.Sorghum:
                case CropType.CanarySeed:
                case CropType.Buckwheat:
                case CropType.FallRye:
                case CropType.MixedGrains:
                case CropType.Oilseeds:
                case CropType.Canola:
                case CropType.Mustard:
                case CropType.Flax:
                case CropType.PulseCrops:
                case CropType.Soybeans:
                case CropType.BeansDryField:
                case CropType.Chickpeas:
                case CropType.DryPeas:
                case CropType.FieldPeas:
                case CropType.Lentils:
                case CropType.GrainCorn:
                case CropType.SilageCorn:
                case CropType.Safflower:
                case CropType.SunflowerSeed:
                case CropType.Tobacco:
                case CropType.Vegetables:
                case CropType.BerriesAndGrapes:
                case CropType.OtherFieldCrops:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsSilageCrop(this CropType cropType)
        {
            switch (cropType)
            {
                case CropType.SilageCorn:
                case CropType.BarleySilage:
                case CropType.OatSilage:
                case CropType.TriticaleSilage:
                case CropType.WheatSilage:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Check whether default yield data is available for a given silage crop.
        /// </summary>
        /// <param name="cropType">The silage crop which we need to check.</param>
        /// <returns>True if the silage crop does not have default data, false if the crop does have default data available.</returns>
        public static bool IsSilageCropWithoutDefaults(this CropType cropType)
        {
            switch (cropType)
            {
                case CropType.BarleySilage:
                case CropType.OatSilage:
                case CropType.TriticaleSilage:
                case CropType.WheatSilage:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns the grain crop equivalent of a silage crop. For example, if silage crop is Barley Silage, then its grain
        /// crop equivalent will be Barley.
        /// </summary>
        /// <param name="cropType">The silage crop for which we need the equivalent grain crop.</param>
        /// <returns>Returns the equivalent grain crop.</returns>
        public static CropType GetGrainCropEquivalentOfSilageCrop(this CropType cropType)
        {
            switch (cropType)
            {
                case CropType.BarleySilage:
                    return CropType.Barley;
                case CropType.OatSilage:
                    return CropType.Oats;
                case CropType.TriticaleSilage:
                    return CropType.Triticale;
                case CropType.WheatSilage:
                    return CropType.Wheat;
                case CropType.CornSilage:
                case CropType.SilageCorn:
                    return CropType.Corn;
                case CropType.CerealSilage:
                    return CropType.Cereals;
                default:
                    return CropType.None;
            }
        }

        public static bool IsRootCrop(this CropType cropType)
        {
            return cropType == CropType.Potatoes || cropType == CropType.SugarBeets;
        }

        public static bool IsSmallGrains(this CropType cropType)
        {
            switch (cropType)
            {
                case CropType.SmallGrainCereals:
                case CropType.Wheat:
                case CropType.WinterWheat:
                case CropType.WheatSilage:
                case CropType.Barley:
                case CropType.GrainCorn:
                case CropType.BarleySilage:
                case CropType.UndersownBarley:
                case CropType.Oats:
                case CropType.OatSilage:
                case CropType.Triticale:
                case CropType.TriticaleSilage:
                case CropType.Sorghum:
                case CropType.CanarySeed:
                case CropType.Buckwheat:
                case CropType.FallRye:
                case CropType.MixedGrains:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsOilSeed(this CropType cropType)
        {
            switch (cropType)
            {
                case CropType.Oilseeds:
                case CropType.Canola:
                case CropType.Mustard:
                case CropType.Soybeans:
                case CropType.Flax:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsOtherFieldCrop(this CropType cropType)
        {
            switch (cropType)
            {
                case CropType.Safflower:
                case CropType.SunflowerSeed:
                case CropType.Tobacco:
                case CropType.Vegetables:
                case CropType.BerriesAndGrapes:
                case CropType.OtherFieldCrops:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsPulseCrop(this CropType cropType)
        {
            switch (cropType)
            {
                case CropType.PulseCrops:
                case CropType.BeansDryField:
                case CropType.Chickpeas:
                case CropType.DryPeas:
                case CropType.FieldPeas:
                case CropType.Lentils:
                    return true;
                default:
                    return false;
            }
        }

        public static IEnumerable<CropType> GetValidCoverCropTypes()
        {
            // Concat two lists so first item is always CropType.None followed by a sorted list afterwards
            return new List<CropType>()
            {
            }.Concat(new List<CropType>()
            {
                CropType.RedCloverTrifoliumPratenseL,
                CropType.BerseemCloverTrifoliumAlexandriumL,
                CropType.SweetCloverMelilotusOfficinalis,
                CropType.CrimsonCloverTrifoliumIncarnatum,
                CropType.HairyVetchViciaVillosaRoth,
                CropType.AlfalfaMedicagoSativaL,
                CropType.FabaBeanBroadBeanViciaFaba,
                CropType.CowpeaVignaUnguiculata,
                CropType.AustrianWinterPea,
                CropType.RapeseedBrassicaNapusL,
                CropType.WinterTurnipRapeBrassicaRapaSppOleiferaLCVLargo,
                CropType.PhaceliaPhaceliaTanacetifoliaCVPhaci,
                CropType.ForageRadishRaphanusSativusL,
                CropType.MustardSinapusAlbaLSubspMaireiHLindbMaire,
                CropType.BarleyHordeumVulgare,
                CropType.OatAvenaSativa,
                CropType.RyeSecaleCerealeWinterRyeCerealRye,
                CropType.SesameSesamumIndicum,
                CropType.FlaxLinumUsitatissimum,
                CropType.RyeGrassLoliumPerenneL,
                CropType.AnnualRyeGrassLoliumMultiflorum,
                CropType.SorghumSorghumBicolour,
                CropType.PigeonBean,
                CropType.ShepherdsPurse,
                CropType.WinterWheatTriticumAestivum,
            }.OrderBy(x => x.GetDescription()));
        }

        public static bool IsEconomicType(this CropType cropType)
        {
            return GetEconomicCropTypes().Contains(cropType);
        }
        /// <summary>
        /// the croptypes that appear in the economics file
        /// </summary>
        /// <returns>ordered list of crops</returns>
        public static IOrderedEnumerable<CropType> GetEconomicCropTypes()
        {
            return new List<CropType>(GetAlbertaEconomicCropTypes())
                .Concat(GetSaskatchewanEconomicCropTypes())
                .Concat(GetManitobaEconomicCropTypes())
                .Concat(GetOntarioEconomicCropTypes())
                .OrderBy(x => x.GetDescription());
        }
        public static IOrderedEnumerable<CropType> GetValidCropTypes()
        {
            return new List<CropType>
            {
                //CropType.NotSelected,
                CropType.Barley,
                CropType.BarleySilage,
                CropType.BeansDryField,
                CropType.BerriesAndGrapes,
                CropType.Buckwheat,
                CropType.CanarySeed,
                CropType.Canola,
                CropType.Chickpeas,
                CropType.DryPeas,
                CropType.FallRye,
                CropType.FieldPeas,
                CropType.Flax,
                CropType.ForageForSeed,
                CropType.GrainCorn,
                CropType.TameGrass,
                CropType.TameLegume,
                CropType.TameMixed,
                CropType.Lentils,
                CropType.MixedGrains,
                CropType.Mustard,
                CropType.OatSilage,
                CropType.Oats,
                CropType.Oilseeds,
                CropType.RangelandNative,
                CropType.OtherFieldCrops,
                CropType.SeededGrassland,
                CropType.Potatoes,
                CropType.PulseCrops,
                CropType.Safflower,
                CropType.SilageCorn,
                CropType.SmallGrainCereals,
                CropType.Sorghum,
                CropType.Soybeans,
                CropType.SugarBeets,
                CropType.SummerFallow,
                CropType.SunflowerSeed,
                CropType.Tobacco,
                CropType.Triticale,
                CropType.TriticaleSilage,
                CropType.UndersownBarley,
                CropType.Vegetables,
                CropType.Wheat,
                CropType.WheatSilage,
            }.OrderBy(x => x.GetDescription());
        }

        public static IOrderedEnumerable<CropType> GetValidPerennialTypes()
        {
            return new List<CropType>
            {
                CropType.ForageForSeed,
                CropType.TameGrass,
                CropType.TameLegume,
                CropType.TameMixed,
                CropType.RangelandNative,
                CropType.SeededGrassland,
            }.OrderBy(x => x.GetDescription());
        }

        public static IOrderedEnumerable<CropType> GetAlbertaEconomicCropTypes()
        {
            return new List<CropType>()
            {
                CropType.AlfalfaHay,
                CropType.ArgentineHTCanola,
                CropType.CPSWheat,
                CropType.CerealSilage,
                CropType.DryBean,
                CropType.Durum,
                CropType.FeedBarley,
                CropType.FieldPeas,
                CropType.Flax,
                CropType.TameMixed,
                CropType.KabuliChickpea,
                CropType.MaltBarley,
                CropType.MillingOats,
                CropType.PolishCanola,
                CropType.RedLentils,
                CropType.SoftWheat,
                CropType.SpringWheat,
                CropType.SummerFallow,
                CropType.YellowMustard,
            }.OrderBy(x => x.GetDescription());
        }

        public static IOrderedEnumerable<CropType> GetSaskatchewanEconomicCropTypes()
        {
            return new List<CropType>()
            {
                CropType.BlackBean,
                CropType.BrownMustard,
                CropType.Camelina,
                CropType.CanarySeed,
                CropType.Canola,
                CropType.CarawayFirstSeason,
                CropType.CarawaySecondSeason,
                CropType.Coriander,
                CropType.Corn,
                CropType.DesiChickpeas,
                CropType.Durum,
                CropType.EdibleGreenPeas,
                CropType.EdibleYellowPeas,
                CropType.FabaBeans,
                CropType.FeedBarley,
                CropType.Fenugreek,
                CropType.Flax,
                CropType.HybridFallRye,
                CropType.LargeGreenLentils,
                CropType.LargeKabuliChickpea,
                CropType.MaltBarley,
                CropType.Oats,
                CropType.OrientalMustard,
                CropType.Quinoa,
                CropType.RedLentils,
                CropType.SmallKabuliChickpea,
                CropType.Soybeans,
                CropType.SpringWheat,
                CropType.SunflowerOilseedEMSS,
                CropType.WinterWheat,
                CropType.YellowMustard,
            }.OrderBy(x => x.GetDescription());
        }

        public static IOrderedEnumerable<CropType> GetManitobaEconomicCropTypes()
        {
            return new List<CropType>()
            {
                CropType.Barley,
                CropType.BeansPinto,
                CropType.BeansWhite,
                CropType.Canola,
                CropType.Corn,
                CropType.FlaxSeed,
                CropType.HardRedSpringWheat,
                CropType.HybridFallRye,
                CropType.Oats,
                CropType.Peas,
                CropType.Soybeans,
                CropType.SunflowerConfection,
                CropType.SunflowerOil,
                CropType.WheatNorthernHardRed,
                CropType.WheatOtherSpring,
                CropType.WheatPrairieSpring,
                CropType.WinterWheat,
            }.OrderBy(x => x.GetDescription());
        }

        public static IOrderedEnumerable<CropType> GetOntarioEconomicCropTypes()
        {
            return new List<CropType>()
            {
                CropType.AlfalfaHay,
                CropType.ColouredBeans,
                CropType.CornSilage,
                CropType.GrainCorn,
                CropType.HardRedSpringWheat,
                CropType.HardRedWinterWheat,
                CropType.HardRedWinterWheatNoTill,
                CropType.NorthernOntarioBarley,
                CropType.NorthernOntarioOats,
                CropType.SoftWinterWheat,
                CropType.SoftWinterWheatNoTill,
                CropType.SouthernOntarioBarley,
                CropType.SouthernOntarioOats,
                CropType.SoybeanNoTill,
                CropType.Soybeans,
                CropType.SoybeansRoundUpReady,
                CropType.SpringCanolaHt,
                CropType.SwitchgrassDirect,
                CropType.SwitchgrassDirectNoTill,
                CropType.SwitchgrassUnderseeded,
                CropType.SwitchgrassUnderseededNoTill,
                CropType.WhiteBlackBeans,
                CropType.WinterCanolaHybrid,
            }.OrderBy(x => x.GetDescription());
        }
        public static IOrderedEnumerable<CropType> GetGrasslandTypes()
        {
            return new List<CropType>()
            {
                CropType.BrokenGrassland,
                CropType.GrasslandSeeded,
            }.OrderBy(x => x.GetDescription());
        }

        public static bool IsNationalInventoryReport(this CropType cropType)
        {
            switch (cropType)
            {

                case CropType.Barley:
                case CropType.Buckwheat:
                case CropType.Canola:
                case CropType.SmallGrainCereals:
                case CropType.Chickpeas:
                case CropType.GrainCorn:
                case CropType.SilageCorn:
                case CropType.BeansDryField:
                case CropType.FieldPeas:
                case CropType.FabaBeans:
                case CropType.FlaxSeed:
                case CropType.Grains:
                case CropType.Lentils:
                case CropType.MustardSeed:
                case CropType.MixedGrains:
                case CropType.Oats:
                case CropType.OtherDryFieldBeans:
                case CropType.Oilseeds:
                case CropType.Peas:
                case CropType.Potatoes:
                case CropType.Pulses:
                case CropType.Rye:
                case CropType.FallRye:
                case CropType.SpringRye:
                case CropType.Safflower:
                case CropType.Soybeans:
                case CropType.SugarBeets:
                case CropType.SunflowerSeed:
                case CropType.Triticale:
                case CropType.WhiteBeans:
                case CropType.Wheat:
                case CropType.WheatRye:
                case CropType.SpringWheat:
                case CropType.WinterWheat:
                case CropType.Durum:
                case CropType.CanarySeed:
                case CropType.Tobacco:
                    return true;

                default:
                    return false;
            }
        }

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}