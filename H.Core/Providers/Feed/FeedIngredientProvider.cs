#region Imports

using System;
using System.Collections.Generic;
using System.Globalization;
using H.Content;
using H.Core.Converters;
using H.Infrastructure;
using System.Linq;
using AutoMapper;
using H.Core.Enumerations;

#endregion

namespace H.Core.Providers.Feed 
{
    
    /// <summary>
    /// </summary>
    public class FeedIngredientProvider : IFeedIngredientProvider 
    {
        
        #region Fields

        private readonly IList<FeedIngredient> _beefFeedIngredients;
        private readonly IList<FeedIngredient> _dairyIngredients;
        private readonly IList<FeedIngredient> _swineFeedIngredients;
        
        private readonly IMapper _feedIngredientMapper;


        #endregion

        #region Constructors

        public FeedIngredientProvider()
        {
            _beefFeedIngredients = this.ReadBeefFile().ToList();
            _dairyIngredients = this.ReadDairyFile().ToList();
            _swineFeedIngredients = this.ReadSwineFile().ToList();

            var feedIngredientMapper = new MapperConfiguration(x =>
            {
                x.CreateMap<FeedIngredient, FeedIngredient>();
            });

            _feedIngredientMapper = feedIngredientMapper.CreateMapper();
        }

        #endregion

        #region Properties


        #endregion

        #region Public Methods

        public FeedIngredient CopyIngredient(FeedIngredient ingredient, double defaultPercentageInDiet)
        {
            var copiedIngredient = new FeedIngredient();

            _feedIngredientMapper.Map(ingredient, copiedIngredient);

            copiedIngredient.PercentageInDiet = defaultPercentageInDiet;

            return copiedIngredient;
        }

        public IList<FeedIngredient> GetBeefFeedIngredients()
        {
            return _beefFeedIngredients;
        }

        public IList<FeedIngredient> GetDairyFeedIngredients()
        {
            return _dairyIngredients;
        }

        public IList<FeedIngredient> GetSwineFeedIngredients()
        {
            return _swineFeedIngredients;
        }
        #endregion

        #region Private Methods        

        private IEnumerable<FeedIngredient> ReadSwineFile()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.SwineFeedIngredientList);
            var feedNameStringConverter = new FeedIngredientStringConverter();
            double parseResult = 0;

            var result = new List<FeedIngredient>();

            foreach (var line in fileLines.Skip(1))
            {
                var feedData = new FeedIngredient
                {
                    IngredientType = feedNameStringConverter.Convert(line[1]),
                    AAFCO = line[2],
                    AAFCO2010 = line[3],
                    IFN = line[4],
                    DryMatter = double.TryParse(line[5], NumberStyles.Any, cultureInfo, out parseResult)
                        ? parseResult
                        : 0,
                    CrudeProtein = double.TryParse(line[6], NumberStyles.Any, cultureInfo, out parseResult)
                        ? parseResult
                        : 0,
                    CrudeFiber = double.TryParse(line[7], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    EE = double.TryParse(line[8], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    AcidEtherExtract = double.TryParse(line[9], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Ash = double.TryParse(line[10], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    GrossEnergy = double.TryParse(line[11], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    DeSwine = double.TryParse(line[12], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ME =
                        double.TryParse(line[13], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    NE = double.TryParse(line[14], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Lactose =
                        double.TryParse(line[15], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Sucrose = double.TryParse(line[16], NumberStyles.Any, cultureInfo, out parseResult)
                        ? parseResult
                        : 0,
                    Raffinose = double.TryParse(line[17], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Stachyose = double.TryParse(line[18], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Verbascose = double.TryParse(line[19], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Oligosaccharides = double.TryParse(line[20], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Starch = double.TryParse(line[21], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    NDF = double.TryParse(line[22], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ADF = double.TryParse(line[23], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Hemicellulose = double.TryParse(line[24], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ADL = double.TryParse(line[25], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    TDF = double.TryParse(line[26], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    IDF = double.TryParse(line[27], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    SDF = double.TryParse(line[28], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ARG = double.TryParse(line[30], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    HIS = double.TryParse(line[31], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ILE = double.TryParse(line[32], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    LEU = double.TryParse(line[33], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    LYS = double.TryParse(line[34], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    MET = double.TryParse(line[35], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    PHE = double.TryParse(line[36], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    THR = double.TryParse(line[37], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    TRP = double.TryParse(line[38], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    VAL = double.TryParse(line[39], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ALA = double.TryParse(line[40], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ASP = double.TryParse(line[41], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    CYS = double.TryParse(line[42], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    GLU = double.TryParse(line[43], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    GLY = double.TryParse(line[44], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    PRO = double.TryParse(line[45], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    SER = double.TryParse(line[46], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    TYR = double.TryParse(line[47], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    CpDigestAID = double.TryParse(line[48], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ArgDigestAID = double.TryParse(line[49], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    HisDigestAID = double.TryParse(line[50], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    IleDigestAID = double.TryParse(line[51], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    LeuDigestAID = double.TryParse(line[52], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    LysDigestAID = double.TryParse(line[53], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    MetDigestAID = double.TryParse(line[54], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    PheDigestAID = double.TryParse(line[55], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ThrDigestAID = double.TryParse(line[56], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    TrpDigestAID = double.TryParse(line[57], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ValDigestAID = double.TryParse(line[58], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    AlaDigestAID = double.TryParse(line[59], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    AspDigestAID = double.TryParse(line[60], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    CysDigestAID = double.TryParse(line[61], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    GluDigestAID = double.TryParse(line[62], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    GlyDigestAID = double.TryParse(line[63], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ProDigestAID = double.TryParse(line[64], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    SerDigestAID = double.TryParse(line[65], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    TyrDigestAID = double.TryParse(line[66], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    CpDigestSID = double.TryParse(line[67], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ArgDigestSID = double.TryParse(line[68], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    HisDigestSID = double.TryParse(line[69], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    IleDigestSID = double.TryParse(line[70], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    LeuDigestSID = double.TryParse(line[71], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    LysDigestSID = double.TryParse(line[72], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    MetDigestSID = double.TryParse(line[73], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    PheDigestSID = double.TryParse(line[74], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ThrDigestSID = double.TryParse(line[75], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    TrpDigestSID = double.TryParse(line[76], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ValDigestSID = double.TryParse(line[77], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    AlaDigestSID = double.TryParse(line[78], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    AspDigestSID = double.TryParse(line[79], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    CysDigestSID = double.TryParse(line[80], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    GluDigestSID = double.TryParse(line[81], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    GlyDigestSID = double.TryParse(line[82], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ProDigestSID = double.TryParse(line[83], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    SerDigestSID = double.TryParse(line[84], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    TyrDigestSID = double.TryParse(line[85], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Ca = double.TryParse(line[86], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Cl = double.TryParse(line[87], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    K = double.TryParse(line[88], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Mg = double.TryParse(line[89], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Na = double.TryParse(line[90], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    P = double.TryParse(line[91], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    S = double.TryParse(line[92], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    CR = double.TryParse(line[93], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Cu = double.TryParse(line[94], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Fe = double.TryParse(line[95], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    I = double.TryParse(line[96], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Mn = double.TryParse(line[97], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Se = double.TryParse(line[98], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Zn = double.TryParse(line[99], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    PhytatePhosphorus = double.TryParse(line[100], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ATTDPhosphorus = double.TryParse(line[101], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    STTDPhosphorus = double.TryParse(line[102], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    BetaCarotene = double.TryParse(line[103], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    VitE = double.TryParse(line[104], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    VitB6 = double.TryParse(line[105], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    VitB12 = double.TryParse(line[106], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Biotin = double.TryParse(line[107], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Folacin = double.TryParse(line[108], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Niacin = double.TryParse(line[109], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    PantothenicAcid = double.TryParse(line[110], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Riboflavin = double.TryParse(line[111], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Thiamin = double.TryParse(line[112], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Choline = double.TryParse(line[113], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    EtherExtractDup = double.TryParse(line[114], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C120 = double.TryParse(line[115], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C140 = double.TryParse(line[116], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C160 = double.TryParse(line[117], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C161 = double.TryParse(line[118], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C180 = double.TryParse(line[119], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C181 = double.TryParse(line[120], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C182 = double.TryParse(line[121], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C183 = double.TryParse(line[122], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C184 = double.TryParse(line[123], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C200 = double.TryParse(line[124], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C201 = double.TryParse(line[125], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C204 = double.TryParse(line[126], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C205 = double.TryParse(line[127], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C220 = double.TryParse(line[128], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C221 = double.TryParse(line[129], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C225 = double.TryParse(line[130], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C226 = double.TryParse(line[131], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C240 = double.TryParse(line[132], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    SFA = double.TryParse(line[133], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    MUFA = double.TryParse(line[134], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    PUFA = double.TryParse(line[135], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    IV = double.TryParse(line[136], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    IVP = double.TryParse(line[137], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                };
                feedData.IngredientTypeString = feedData.IngredientType.GetDescription();

                result.Add(feedData);
            }

            // Need to add Urea.
            result.Add(new FeedIngredient()
            {
                // This value is constant.
                IngredientType = IngredientType.Urea,
                IngredientTypeString = IngredientType.Urea.GetDescription(),
                CrudeProtein = 291,
            });

            return result;
        }

        private IEnumerable<FeedIngredient> ReadBeefFile()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.FeedNames);
            var feedNameStringConverter = new FeedIngredientStringConverter();
            double parseResult = 0;

            var result = new List<FeedIngredient>();

            foreach (var line in fileLines)
            {
                var feedData = new FeedIngredient
                {
                    IngredientType = feedNameStringConverter.Convert(line[1]),
                    IFN = line[2],
                    Cost = double.TryParse(line[3], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Forage = double.TryParse(line[4], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    DryMatter = double.TryParse(line[5], NumberStyles.Any, cultureInfo, out parseResult)
                        ? parseResult
                        : 0,
                    CrudeProtein = double.TryParse(line[6], NumberStyles.Any, cultureInfo, out parseResult)
                        ? parseResult
                        : 0,
                    SP = double.TryParse(line[7], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ADICP = double.TryParse(line[8], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Sugars = double.TryParse(line[9], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    OA = double.TryParse(line[10], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Fat = double.TryParse(line[11], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Ash = double.TryParse(line[12], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Starch =
                        double.TryParse(line[13], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    NDF = double.TryParse(line[14], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Lignin =
                        double.TryParse(line[15], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    TotalDigestibleNutrient = double.TryParse(line[16], NumberStyles.Any, cultureInfo, out parseResult)
                        ? parseResult
                        : 0,
                    ME = double.TryParse(line[17], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    NEma = double.TryParse(line[18], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    NEga = double.TryParse(line[19], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    RUP = double.TryParse(line[20], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    kdPB = double.TryParse(line[21], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    KdCB1 = double.TryParse(line[22], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    KdCB2 = double.TryParse(line[23], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    KdCB3 = double.TryParse(line[24], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    PBID = double.TryParse(line[25], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    CB1ID = double.TryParse(line[26], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    CB2ID = double.TryParse(line[27], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Pef = double.TryParse(line[28], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ARG = double.TryParse(line[29], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    HIS = double.TryParse(line[30], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ILE = double.TryParse(line[31], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    LEU = double.TryParse(line[32], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    LYS = double.TryParse(line[33], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    MET = double.TryParse(line[34], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    CYS = double.TryParse(line[35], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    PHE = double.TryParse(line[36], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    TYR = double.TryParse(line[37], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    THR = double.TryParse(line[38], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    TRP = double.TryParse(line[39], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    VAL = double.TryParse(line[40], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Ca = double.TryParse(line[41], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    P = double.TryParse(line[42], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Mg = double.TryParse(line[43], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Cl = double.TryParse(line[44], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    K = double.TryParse(line[45], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Na = double.TryParse(line[46], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    S = double.TryParse(line[47], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Co = double.TryParse(line[48], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Cu = double.TryParse(line[49], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    I = double.TryParse(line[50], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Fe = double.TryParse(line[51], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Mn = double.TryParse(line[52], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Se = double.TryParse(line[53], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Zn = double.TryParse(line[54], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    VitA = double.TryParse(line[55], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    VitD = double.TryParse(line[56], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    VitE = double.TryParse(line[57], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0
                };

                feedData.IngredientTypeString = feedData.IngredientType.GetDescription();

                result.Add(feedData);
            }

            // Need to add Urea.
            result.Add(new FeedIngredient()
            {
                // This value is constant.
                IngredientType = IngredientType.Urea,
                IngredientTypeString = IngredientType.Urea.GetDescription(),
                CrudeProtein = 291,
            });

            return result;
        }

        private IEnumerable<FeedIngredient> ReadDairyFile()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.DairyFeedComposition).ToList();
            var feedNameStringConverter = new FeedIngredientStringConverter();
            var dairyClassTypeConverter = new DairyFeedClassTypeConverter();
            double parseResult = 0;

            var result = new List<FeedIngredient>();

            foreach (var line in fileLines.Skip(2))
            {
                //line[0] is the line number starting from 1, if it has no number, go to the next line
                if (String.IsNullOrEmpty(line[0]))
                {
                    continue;
                }

                var feedData = new FeedIngredient()
                {
                    IngredientType = feedNameStringConverter.Convert(line[1]),
                    //FeedNumber = line[2],
                    DairyFeedClass = dairyClassTypeConverter.Convert(line[2]),
                    TotalDigestibleNutrient = double.TryParse(line[3], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    DryMatter = double.TryParse(line[4], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    CrudeProtein = double.TryParse(line[5], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    NDICP = double.TryParse(line[6], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ADICP = double.TryParse(line[7], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    EE = double.TryParse(line[8], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    NDF = double.TryParse(line[9], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ADF = double.TryParse(line[10], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Lignin = double.TryParse(line[11], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    Ash = double.TryParse(line[12], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    DE = double.TryParse(line[13], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    ME = double.TryParse(line[14], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    NEL_ThreeX = double.TryParse(line[15], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    NEL_FourX = double.TryParse(line[16], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    NEM = double.TryParse(line[17], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    NEG = double.TryParse(line[18], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                };

                feedData.IngredientTypeString = feedData.IngredientType.GetDescription();

                result.Add(feedData);
            }

            // Need to add Urea.
            result.Add(new FeedIngredient()
            {
                // This value is constant.
                IngredientType = IngredientType.Urea,
                IngredientTypeString = IngredientType.Urea.GetDescription(),
                CrudeProtein = 291,
            });

            return result;
        }

        #endregion
    }
}