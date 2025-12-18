using System;
using System.Collections.Generic;
using System.Diagnostics;
using H.Core.Enumerations;
using H.Core.Properties;

namespace H.Core.Converters
{
    public class CropTypeStringConverter : ConverterBase
    {
        #region Properties

        public static Dictionary<string, CropType> Cache { get; set; } = new Dictionary<string, CropType>();

        #endregion

        #region Methods

        public CropType Convert(string input)
        {
            CropType result;

            if (Cache.ContainsKey(input))
            {
                return Cache[input];
            }

            switch (this.GetLettersAsLowerCase(input))
            {
                case "alfalfaseed":
                    result = CropType.AlfalfaSeed;
                    break;

                case "barley":
                    result = CropType.Barley;
                    break;

                case "barleysilage":
                    result = CropType.BarleySilage;
                    break;

                case "barleysilageunderseed":
                    result = CropType.BarleySilageUnderSeed;
                    break;

                case "bromehay":
                    result = CropType.BromeHay;
                    break;

                case "grasssilage":
                    result = CropType.GrassSilage;
                    break;

                case "beans":
                    result = CropType.Beans;
                    break;

                case "beansdryfield":
                case "dryfieldbeans":
                    result = CropType.BeansDryField;
                    break;

                case "otherdryfieldbeans":
                    result = CropType.OtherDryFieldBeans;
                    break;

                case "berriesgrapes":
                    result = CropType.BerriesAndGrapes;
                    break;

                case "buckwheat":
                    result = CropType.Buckwheat;
                    break;

                case "canaryseed":
                case "canaryseeds":
                    result = CropType.CanarySeed;
                    break;

                case "canola":
                    result = CropType.Canola;
                    break;

                case "caraway":
                    result = CropType.Caraway;
                    break;

                case "carrot":
                    result = CropType.Carrot;
                    break;

                case "chickpeas":
                case "chickpea":
                    result = CropType.Chickpeas;
                    break;

                case "colouredwhitefababeans":
                    result = CropType.ColouredWhiteFabaBeans;
                    break;

                case "cpswheat":
                    result = CropType.CPSWheat;
                    break;

                case "drybean":
                    result = CropType.DryBean;
                    break;

                case "drypeas":
                case "drypea":
                case "peasdry":
                    result = CropType.DryPeas;
                    break;

                case "dryfieldpeas":
                    result = CropType.DryFieldPeas;
                    break;

                case "dill":
                    result = CropType.Dill;
                    break;

                case "durum":
                case "wheatdurum":
                case "durumwheat":
                    result = CropType.Durum;
                    break;

                case "fababean":
                    result = CropType.FabaBeans;
                    break;

                case "fallow":
                    result = CropType.Fallow;
                    break;

                case "fallrye":
                case "ryefallremaining":
                    result = CropType.FallRye;
                    break;

                case "fieldpea":
                    result = CropType.FieldPeas;
                    break;

                case "Flaxseed":
                    result = CropType.Flax;
                    break;

                case "flaxseed":
                    result = CropType.FlaxSeed;
                    break;

                case "freshcornsweet":
                    result = CropType.FreshCornSweet;
                    break;

                case "freshpeas":
                    result = CropType.FreshPeas;
                    break;

                case "forage":
                    result = CropType.Forage;
                    break;

                case "foddercorn":
                    result = CropType.FodderCorn;
                    break;

                case "forageforseed":
                    result = CropType.ForageForSeed;
                    break;

                case "graincorn":
                case "cornforgrain":
                    result = CropType.GrainCorn;
                    break;

                case "grains":
                    result = CropType.Grains;
                    break;

                case "genericgrains":
                    result = CropType.GenericGrains;
                    break;

                case "corn":
                case "maize":
                    result = CropType.Corn;
                    break;

                case "grainsorghum":
                    result = CropType.GrainSorghum;
                    break;

                case "grassseed":
                    result = CropType.GrassSeed;
                    break;

                case "rangelandlandnative":
                    result = CropType.RangelandNative;
                    break;

                case "greenfeed":
                    result = CropType.GreenFeed;
                    break;

                case "hardredspringwheat":
                    result = CropType.HardRedSpringWheat;
                    break;

                case "tamegrass":
                    result = CropType.TameGrass;
                    break;

                case "grasshay":
                    result = CropType.GrassHay;
                    break;

                case "tamelegume":
                    result = CropType.TameLegume;
                    break;

                case "nonlegumehay":
                    result = CropType.NonLegumeHay;
                    break;

                case "tamemixed":
                case "mixedhay":
                    result = CropType.TameMixed;
                    break;

                case "hayandforageseed":
                    result = CropType.HayAndForageSeed;
                    break;

                case "hairyvetch":
                    result = CropType.HairyVetch;
                    break;

                case "hairyvetchrye":
                    result = CropType.HairyVetchAndRye;
                    break;

                case "hyola":
                    result = CropType.Hyola;
                    break;

                case "lentils":
                case "lentil":
                    result = CropType.Lentils;
                    break;

                case "linola":
                    result = CropType.Linola;
                    break;

                case "maltbarley":
                    result = CropType.MaltBarley;
                    break;

                case "marketgarden":
                    result = CropType.MarketGarden;
                    break;

                case "milkvetch":
                    result = CropType.MilkVetch;
                    break;

                case "millet":
                    result = CropType.Millet;
                    break;

                case "mint":
                    result = CropType.Mint;
                    break;

                case "mixedgrains":
                case "mixedgrain":
                    result = CropType.MixedGrains;
                    break;

                case "mustard":
                    result = CropType.Mustard;
                    break;

                case "mustardseed":
                    result = CropType.MustardSeed;
                    break;

                case "monarda":
                    result = CropType.Monarda;
                    break;

                case "nativepasture":
                    result = CropType.NativePasture;
                    break;

                case "oats":
                    result = CropType.Oats;
                    break;

                case "oatsilage":
                    result = CropType.OatSilage;
                    break;

                case "oilseeds":
                    result = CropType.Oilseeds;
                    break;

                case "onion":
                    result = CropType.Onion;
                    break;

                case "otherfieldcrops":
                    result = CropType.OtherFieldCrops;
                    break;

                case "peas":
                    result = CropType.Peas;
                    break;

                case "pulses":
                    result = CropType.Pulses;
                    break;

                case "pulsecrops":
                    result = CropType.PulseCrops;
                    break;

                case "seededgrassland":
                    result = CropType.SeededGrassland;
                    break;

                case "peanuts":
                    result = CropType.Peanuts;
                    break;

                case "perennialforages":
                    result = CropType.PerennialForages;
                    break;

                case "perennialgrasses":
                    result = CropType.PerennialGrasses;
                    break;

                case "potatoes":
                case "potato":
                    result = CropType.Potatoes;
                    break;

                case "rice":
                    result = CropType.Rice;
                    break;

                case "rye":
                case "ryeall":
                    result = CropType.Rye;
                    break;

                case "safflower":
                    result = CropType.Safflower;
                    break;

                case "seedpotato":
                    result = CropType.SeedPotato;
                    break;

                case "silagecorn":
                case "cornsilage":
                    result = CropType.SilageCorn;
                    break;

                case "smallfruit":
                    result = CropType.SmallFruit;
                    break;

                case "softwheat":
                    result = CropType.SoftWheat;
                    break;

                case "soybeans":
                case "soybean":
                    result = CropType.Soybeans;
                    break;

                case "sorghum":
                    result = CropType.Sorghum;
                    break;

                case "sorghumsudangrass":
                    result = CropType.SorghumSudanGrass;
                    break;

                case "smallgraincereals":
                    result = CropType.SmallGrainCereals;
                    break;

                case "springwheat":
                case "wheatspring":
                    result = CropType.SpringWheat;
                    break;

                case "springrye":
                case "ryespring":
                    result = CropType.SpringRye;
                    break;

                case "sugarbeets":
                    result = CropType.SugarBeets;
                    break;

                case "summerfallow":
                    result = CropType.SummerFallow;
                    break;

                case "sunflower":
                    result = CropType.Sunflower;
                    break;

                case "sunflowerseed":
                case "sunflowerseeds":
                    result = CropType.SunflowerSeed;
                    break;

                case "tamepasture":
                    result = CropType.TamePasture;
                    break;

                case "timothyhay":
                    result = CropType.TimothyHay;
                    break;

                case "tobacco":
                    result = CropType.Tobacco;
                    break;

                case "totaltreefruitsnuts":
                    result = CropType.TreeFruitAndNuts;
                    break;

                case "triticale":
                    result = CropType.Triticale;
                    break;

                case "tubers":
                    result = CropType.Tubers;
                    break;

                case "turfsod":
                    result = CropType.TurfSod;
                    break;

                case "undersownbarley":
                    result = CropType.UndersownBarley;
                    break;

                case "vegetables":
                    result = CropType.Vegetables;
                    break;

                case "wheatbolinder":
                    result = CropType.WheatBolinder;
                    break;

                case "wheatgan":
                    result = CropType.WheatGan;
                    break;

                case "wheat":
                case "wheatall":
                    result = CropType.Wheat;
                    break;

                case "wheatrye":
                    result = CropType.WheatRye;
                    break;

                case "winterwheat":
                case "wheatwinter":
                case "wheatwinterremaining":
                    result = CropType.WinterWheat;
                    break;

                case "winterweeds":
                    result = CropType.WinterWeeds;
                    break;

                case "fieldpeas":
                    result = CropType.FieldPeas;
                    break;

                case "berriesandgrapes":
                    result = CropType.BerriesAndGrapes;
                    break;

                case "flax":
                    result = CropType.Flax;
                    break;

                case "triticalesilage":
                    result = CropType.TriticaleSilage;
                    break;

                case "wheatsilage":
                    result = CropType.WheatSilage;
                    break;

                case "grassclovermixtures":
                    result = CropType.GrassCloverMixtures;
                    break;

                case "redclovertrifoliumpratensel":
                    result = CropType.RedCloverTrifoliumPratenseL;
                    break;

                case "berseemclovertrifoliumalexandriuml":
                    result = CropType.BerseemCloverTrifoliumAlexandriumL;
                    break;

                case "sweetclovermelilotusofficinalis":
                    result = CropType.SweetCloverMelilotusOfficinalis;
                    break;

                case "crimsonclovertrifoliumincarnatum":
                    result = CropType.CrimsonCloverTrifoliumIncarnatum;
                    break;

                case "hairyvetchviciavillosaroth":
                    result = CropType.HairyVetchViciaVillosaRoth;
                    break;

                case "alfalfamedicagosatival":
                    result = CropType.AlfalfaMedicagoSativaL;
                    break;

                case "fababeanbroadbeanviciafaba":
                    result = CropType.FabaBeanBroadBeanViciaFaba;
                    break;

                case "cowpeavignaunguiculata":
                    result = CropType.CowpeaVignaUnguiculata;
                    break;

                case "austrianwinterpea":
                    result = CropType.AustrianWinterPea;
                    break;

                case "rapeseedbrassicanapusl":
                    result = CropType.RapeseedBrassicaNapusL;
                    break;

                case "winterturniprapebrassicarapasppoleiferalcvlargo":
                    result = CropType.WinterTurnipRapeBrassicaRapaSppOleiferaLCVLargo;
                    break;

                case "phaceliaphaceliatanacetifoliacvphaci":
                    result = CropType.PhaceliaPhaceliaTanacetifoliaCVPhaci;
                    break;

                case "forageradishraphanussativusl":
                    result = CropType.ForageRadishRaphanusSativusL;
                    break;

                case "mustardsinapusalbalsubspmaireihlindbmaire":
                    result = CropType.MustardSinapusAlbaLSubspMaireiHLindbMaire;
                    break;

                case "barleyhordeumvulgare":
                    result = CropType.BarleyHordeumVulgare;
                    break;

                case "oatavenasativa":
                    result = CropType.OatAvenaSativa;
                    break;

                case "ryesecalecerealewinterryecerealrye":
                    result = CropType.RyeSecaleCerealeWinterRyeCerealRye;
                    break;

                case "sesamesesamumindicum":
                    result = CropType.SesameSesamumIndicum;
                    break;

                case "flaxlinumusitatissimum":
                    result = CropType.FlaxLinumUsitatissimum;
                    break;

                case "ryegrassloliumperennel":
                    result = CropType.RyeGrassLoliumPerenneL;
                    break;

                case "annualryegrassloliummultiflorum":
                    result = CropType.AnnualRyeGrassLoliumMultiflorum;
                    break;

                case "sorghumsorghumbicolour":
                    result = CropType.SorghumSorghumBicolour;
                    break;

                case "pigeonbean":
                    result = CropType.PigeonBean;
                    break;

                case "shepherdspurse":
                    result = CropType.ShepherdsPurse;
                    break;

                case "winterwheattriticumaestivum":
                    result = CropType.WinterWheatTriticumAestivum;
                    break;

                case "feedbarley":
                    result = CropType.FeedBarley;
                    break;

                case "redlentil":
                    result = CropType.RedLentils;
                    break;

                case "millingoats":
                    result = CropType.MillingOats;
                    break;

                case "polishcanola":
                    result = CropType.PolishCanola;
                    break;

                case "argentinehtcanola":
                    result = CropType.ArgentineHTCanola;
                    break;

                case "kabulichickpea":
                    result = CropType.KabuliChickpea;
                    break;

                case "yellowmustard":
                    result = CropType.YellowMustard;
                    break;

                case "cerealsilage":
                    result = CropType.CerealSilage;
                    break;

                case "cereals":
                    result = CropType.Cereals;
                    break;

                case "alfalfahay":
                case "alfalfa":
                    result = CropType.AlfalfaHay;
                    break;

                case "ediblegreenpeas":
                    result = CropType.EdibleGreenPeas;
                    break;

                case "edibleyellowpeas":
                    result = CropType.EdibleYellowPeas;
                    break;

                case "blackbean":
                    result = CropType.BlackBean;
                    break;

                case "hybridfallrye":
                    result = CropType.HybridFallRye;
                    break;

                case "brownmustard":
                    result = CropType.BrownMustard;
                    break;

                case "orientalmustard":
                    result = CropType.OrientalMustard;
                    break;

                case "sunfloweroilseedemss":
                    result = CropType.SunflowerOilseedEMSS;
                    break;

                case "desichickpea":
                    result = CropType.DesiChickpeas;
                    break;

                case "camelina":
                    result = CropType.Camelina;
                    break;

                case "carawayfirstseason":
                    result = CropType.CarawayFirstSeason;
                    break;

                case "carawaysecondseason":
                    result = CropType.CarawaySecondSeason;
                    break;

                case "coriander":
                    result = CropType.Coriander;
                    break;

                case "fenugreek":
                    result = CropType.Fenugreek;
                    break;

                case "quinoa":
                    result = CropType.Quinoa;
                    break;

                case "wheathardredspring":
                    result = CropType.WheatHardRedSpring;
                    break;

                case "wheatprairiespring":
                    result = CropType.WheatPrairieSpring;
                    break;

                case "wheatotherspring":
                    result = CropType.WheatOtherSpring;
                    break;

                case "beanspinto":
                    result = CropType.BeansPinto;
                    break;

                case "sunflowerconfection":
                    result = CropType.SunflowerConfection;
                    break;

                case "largegreenlentils":
                    result = CropType.LargeGreenLentils;
                    break;

                case "wheatnorthernhardred":
                    result = CropType.WheatNorthernHardRed;
                    break;

                case "sunfloweroil":
                    result = CropType.SunflowerOil;
                    break;

                case "beanswhite":
                    result = CropType.BeansWhite;
                    break;

                case "kabulichickpealarge":
                    result = CropType.LargeKabuliChickpea;
                    break;

                case "kabulichickpeasmall":
                    result = CropType.SmallKabuliChickpea;
                    break;

                case "colouredbeans":
                    result = CropType.ColouredBeans;
                    break;

                case "hardredwinterwheat":
                    result = CropType.HardRedWinterWheat;
                    break;

                case "northernontariobarley":
                    result = CropType.NorthernOntarioBarley;
                    break;

                case "southernontariobarley":
                    result = CropType.SouthernOntarioBarley;
                    break;

                case "northernontariooats":
                    result = CropType.NorthernOntarioOats;
                    break;

                case "rangelandnative":
                    result = CropType.RangelandNative;
                    break;

                case "southernontariooats":
                    result = CropType.SouthernOntarioOats;
                    break;

                case "springcanolaht":
                    result = CropType.SpringCanolaHt;
                    break;

                case "soybeansnotill":
                    result = CropType.SoybeanNoTill;
                    break;

                case "soybeansroundupready":
                    result = CropType.SoybeansRoundUpReady;
                    break;

                case "switchgrassdirect":
                    result = CropType.SwitchgrassDirect;
                    break;

                case "switchgrassdirectnotill":
                    result = CropType.SwitchgrassDirectNoTill;
                    break;

                case "switchgrassunderseeded":
                    result = CropType.SwitchgrassUnderseeded;
                    break;

                case "switchgrassunderseedednotill":
                    result = CropType.SwitchgrassUnderseededNoTill;
                    break;

                case "softwinterwheat":
                    result = CropType.SoftWinterWheat;
                    break;

                case "softwinterwheatnotill":
                    result = CropType.SoftWinterWheatNoTill;
                    break;

                case "whiteblackbeans":
                    result = CropType.WhiteBlackBeans;
                    break;

                case "whitebeans":
                    result = CropType.WhiteBeans;
                    break;

                case "wintercanolahybrid":
                    result = CropType.WinterCanolaHybrid;
                    break;

                case "hardredwinterwheatnotillage":
                    result = CropType.HardRedWinterWheatNoTill;
                    break;

                case "nfixingforages":
                    result = CropType.NFixingForages;
                    break;

                case "nonnfixingforages":
                    result = CropType.NonNFixingForages;
                    break;

                default:
                    {
                        Trace.TraceError($"{nameof(CropTypeStringConverter)}: Crop type '{input}' not mapped, returning default value.");

                        result = CropType.NotSelected;

                        break;
                    }
            }

            Cache.Add(input, result);

            return result;
        } 

        #endregion
    }
}