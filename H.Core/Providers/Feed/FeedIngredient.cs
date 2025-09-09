#region Imports

using System;
using AutoMapper;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;

#endregion

namespace H.Core.Providers.Feed
{
    /// <summary>
    /// </summary>
    public class FeedIngredient : ModelBase
    {
        #region Constructors

        static FeedIngredient()
        {
            var mapperConfig =
                new MapperConfiguration(
                    x =>
                    {
                        x.CreateMap<FeedIngredient, FeedIngredient>().ForMember(prop => prop.Guid, opt => opt.Ignore());
                    }, new NullLoggerFactory());

            _ingredientMapper = mapperConfig.CreateMapper();
        }

        #endregion

        #region Calculated Properties

        public double Nemf =>
            // NEmf (MJ/kg DM) = [NEma (Mcal/kg DM) + NEga (Mcal/kg DM)] * 4.184 (conversion factor for Mcal to MJ)
            (NEga + NEma) * 4.184;

        #endregion

        #region Fields

        private bool _isCustomIngredient;

        private string _ingredientStringType;
        private double _percentageInDiet;
        private IngredientType _ingredientType;
        private double _sugars;
        private double _solubleCrudeProtein;
        private double _dryMatter;
        private double _crudeProtein;
        private double _crudeFiber;
        private double _acidEtherExtract;
        private double _ash;
        private double _grossEnergy;
        private double _fat;
        private double _totalDigestibleNutrient;
        private double _starch;
        private string _ifn;
        private double _cost;
        private double _forage;
        private double _rdp;
        private double _adf;
        private double _adl;
        private double _hemicellulose;
        private double _tdf;
        private double _idf;
        private double _sdf;
        private double _sucrose;
        private double _lactose;
        private double _glucose;
        private double _oligosaccharides;
        private double _raffinose;
        private double _stachyose;
        private double _verbascose;
        private double _mo;
        private double _de;
        private double _deSwine;
        private double _sp;
        private double _adicp;
        private double _oa;
        private double _ndf;
        private double _lignin;
        private double _me;
        private double _nEma;
        private double _nEga;
        private double _rup;
        private double _kdCb1;
        private double _kdCb2;
        private double _kdCb3;
        private double _pbid;
        private double _cb1id;
        private double _cb2id;
        private double _arg;
        private double _his;
        private double _ile;
        private double _leu;
        private double _lys;
        private double _met;
        private double _cys;
        private double _phe;
        private double _thr;
        private double _trp;
        private double _val;
        private double _ala;
        private double _asp;
        private double _glu;
        private double _gly;
        private double _pro;
        private double _ser;
        private double _tyr;
        private double _cr;
        private double _ca;
        private double _p;
        private double _mg;
        private double _cl;
        private double _k;
        private double _na;
        private double _s;
        private double _co;
        private double _cu;
        private double _i;
        private double _fe;
        private double _mn;
        private double _se;
        private double _zn;
        private double _vitA;
        private double _vitD;
        private double _vitE;
        private double _vitD3;
        private double _vitB1;
        private double _vitB2;
        private double _vitB3;
        private double _vitB5;
        private double _vitB6;
        private double _vitB7;
        private double _vitB12;
        private double _kdPb;
        private double _pef;
        private string _feedNumber;
        private DairyFeedClassType _dairyFeedClass;
        private double _paf;
        private double _nel_threex;
        private double _nel_fourx;
        private double _nem;
        private double _neg;
        private double _ndicp;
        private double _ee;
        private double _phytate;
        private double _phytatePhosphorus;
        private double _nonphytatePhosphorus;
        private double _betaCarotene;
        private double _choline;
        private double _ne;
        private double _cpDigestAID;
        private double _argDigestAID;
        private double _hisDigestAID;
        private double _ileDigestAID;
        private double _leuDigestAID;
        private double _lysDigestAID;
        private double _metDigestAID;
        private double _pheDigestAID;
        private double _thrDigestAID;
        private double _trpDigestAID;
        private double _valDigestAID;
        private double _alaDigestAID;
        private double _aspDigestAID;
        private double _cysDigestAID;
        private double _gluDigestAID;
        private double _glyDigestAID;
        private double _proDigestAID;
        private double _serDigestAID;
        private double _tyrDigestAID;
        private double _cpDigestSID;
        private double _argDigestSID;
        private double _hisDigestSID;
        private double _ileDigestSID;
        private double _leuDigestSID;
        private double _lysDigestSID;
        private double _metDigestSID;
        private double _pheDigestSID;
        private double _thrDigestSID;
        private double _trpDigestSID;
        private double _valDigestSID;
        private double _alaDigestSID;
        private double _aspDigestSID;
        private double _cysDigestSID;
        private double _gluDigestSID;
        private double _glyDigestSID;
        private double _proDigestSID;
        private double _serDigestSID;
        private double _tyrDigestSID;
        private string _aafco;
        private string _aafco2010;
        private double _attdPhosphorous;
        private double _sttdPhosphorous;
        private double _biotin;
        private double _folacin;
        private double _niacin;
        private double _pantothenicAcid;
        private double _riboflavin;
        private double _thiamin;
        private double _etherExtractDup;
        private double _c120;
        private double _c140;
        private double _c160;
        private double _c161;
        private double _c180;
        private double _c181;
        private double _c182;
        private double _c183;
        private double _c184;
        private double _c200;
        private double _c201;
        private double _c204;
        private double _c205;
        private double _c220;
        private double _c221;
        private double _c225;
        private double _c226;
        private double _c240;
        private double _sfa;
        private double _mufa;
        private double _pufa;
        private double _iv;
        private double _ivp;
        private static IMapper _ingredientMapper;

        #endregion

        #region Properties

        /// <summary>
        ///     Used when display system (default) ingredients that should not be modifiable by user.
        /// </summary>
        public bool IsReadonly { get; set; }

        public string IngredientTypeString
        {
            get => _ingredientStringType;
            set => SetProperty(ref _ingredientStringType, value);
        }

        public IngredientType IngredientType
        {
            get => _ingredientType;
            set
            {
                SetProperty(ref _ingredientType, value, () => { RaisePropertyChanged(nameof(IngredientTypeString)); });
            }
        }

        public double Sugars
        {
            get => _sugars;
            set => SetProperty(ref _sugars, value);
        }

        public double SolubleCrudeProtein
        {
            get => _solubleCrudeProtein;
            set => SetProperty(ref _solubleCrudeProtein, value);
        }

        public double DryMatter
        {
            get => _dryMatter;
            set => SetProperty(ref _dryMatter, value);
        }

        public double CrudeProtein
        {
            get => _crudeProtein;
            set => SetProperty(ref _crudeProtein, value);
        }

        public double CrudeFiber
        {
            get => _crudeFiber;
            set => SetProperty(ref _crudeFiber, value);
        }

        public double AcidEtherExtract
        {
            get => _acidEtherExtract;
            set => SetProperty(ref _acidEtherExtract, value);
        }

        [Units(MetricUnitsOfMeasurement.KiloCaloriePerKilogram)]
        public double GrossEnergy
        {
            get => _grossEnergy;
            set => SetProperty(ref _grossEnergy, value);
        }

        public double Fat
        {
            get => _fat;
            set => SetProperty(ref _fat, value);
        }

        public double TotalDigestibleNutrient
        {
            get => _totalDigestibleNutrient;
            set => SetProperty(ref _totalDigestibleNutrient, value);
        }

        public double Starch
        {
            get => _starch;
            set => SetProperty(ref _starch, value);
        }

        public string IFN
        {
            get => _ifn;
            set => SetProperty(ref _ifn, value);
        }

        public double Cost
        {
            get => _cost;
            set => SetProperty(ref _cost, value);
        }

        public double Forage
        {
            get => _forage;
            set => SetProperty(ref _forage, value);
        }

        public double RDP
        {
            get => _rdp;
            set => SetProperty(ref _rdp, value);
        }

        public double ADF
        {
            get => _adf;
            set => SetProperty(ref _adf, value);
        }

        public double ADL
        {
            get => _adl;
            set => SetProperty(ref _adl, value);
        }

        public double Hemicellulose
        {
            get => _hemicellulose;
            set => SetProperty(ref _hemicellulose, value);
        }

        public double TDF
        {
            get => _tdf;
            set => SetProperty(ref _tdf, value);
        }

        public double IDF
        {
            get => _idf;
            set => SetProperty(ref _idf, value);
        }

        public double SDF
        {
            get => _sdf;
            set => SetProperty(ref _sdf, value);
        }

        public double Sucrose
        {
            get => _sucrose;
            set => SetProperty(ref _sucrose, value);
        }

        public double Lactose
        {
            get => _lactose;
            set => SetProperty(ref _lactose, value);
        }

        public double Glucose
        {
            get => _glucose;
            set => SetProperty(ref _glucose, value);
        }

        public double Oligosaccharides
        {
            get => _oligosaccharides;
            set => SetProperty(ref _oligosaccharides, value);
        }

        public double Raffinose
        {
            get => _raffinose;
            set => SetProperty(ref _raffinose, value);
        }

        public double Stachyose
        {
            get => _stachyose;
            set => SetProperty(ref _stachyose, value);
        }

        public double Verbascose
        {
            get => _verbascose;
            set => SetProperty(ref _verbascose, value);
        }

        public double Mo
        {
            get => _mo;
            set => SetProperty(ref _mo, value);
        }

        /// <summary>
        ///     used for cattle diets
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaCaloriePerKilogram)]
        public double DE
        {
            get => _de;
            set => SetProperty(ref _de, value);
        }

        /// <summary>
        ///     used only for swine diets
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KiloCaloriePerKilogram)]
        public double DeSwine
        {
            get => _deSwine;
            set => SetProperty(ref _deSwine, value);
        }


        public double SP
        {
            get => _sp;
            set => SetProperty(ref _sp, value);
        }

        public double ADICP
        {
            get => _adicp;
            set => SetProperty(ref _adicp, value);
        }

        public double OA
        {
            get => _oa;
            set => SetProperty(ref _oa, value);
        }

        public double Ash
        {
            get => _ash;
            set => SetProperty(ref _ash, value);
        }

        public double NDF
        {
            get => _ndf;
            set => SetProperty(ref _ndf, value);
        }

        public double Lignin
        {
            get => _lignin;
            set => SetProperty(ref _lignin, value);
        }

        public double Pef
        {
            get => _pef;
            set => SetProperty(ref _pef, value);
        }

        [Units(MetricUnitsOfMeasurement.MegaCaloriePerKilogram)]
        public double ME
        {
            get => _me;
            set => SetProperty(ref _me, value);
        }

        public double NEma
        {
            get => _nEma;
            set { SetProperty(ref _nEma, value, () => RaisePropertyChanged(nameof(Nemf))); }
        }

        public double NEga
        {
            get => _nEga;
            set { SetProperty(ref _nEga, value, () => RaisePropertyChanged(nameof(Nemf))); }
        }

        public double RUP
        {
            get => _rup;
            set => SetProperty(ref _rup, value);
        }

        public double kdPB
        {
            get => _kdPb;
            set => SetProperty(ref _kdPb, value);
        }

        public double KdCB1
        {
            get => _kdCb1;
            set => SetProperty(ref _kdCb1, value);
        }

        public double KdCB2
        {
            get => _kdCb2;
            set => SetProperty(ref _kdCb2, value);
        }

        public double KdCB3
        {
            get => _kdCb3;
            set => SetProperty(ref _kdCb3, value);
        }

        public double PBID
        {
            get => _pbid;
            set => SetProperty(ref _pbid, value);
        }

        public double CB1ID
        {
            get => _cb1id;
            set => SetProperty(ref _cb1id, value);
        }

        public double CB2ID
        {
            get => _cb2id;
            set => SetProperty(ref _cb2id, value);
        }

        public double ARG
        {
            get => _arg;
            set => SetProperty(ref _arg, value);
        }

        public double HIS
        {
            get => _his;
            set => SetProperty(ref _his, value);
        }

        public double ILE
        {
            get => _ile;
            set => SetProperty(ref _ile, value);
        }

        public double LEU
        {
            get => _leu;
            set => SetProperty(ref _leu, value);
        }

        public double LYS
        {
            get => _lys;
            set => SetProperty(ref _lys, value);
        }

        public double MET
        {
            get => _met;
            set => SetProperty(ref _met, value);
        }

        public double CYS
        {
            get => _cys;
            set => SetProperty(ref _cys, value);
        }

        public double PHE
        {
            get => _phe;
            set => SetProperty(ref _phe, value);
        }

        public double TYR
        {
            get => _tyr;
            set => SetProperty(ref _tyr, value);
        }

        public double THR
        {
            get => _thr;
            set => SetProperty(ref _thr, value);
        }

        public double TRP
        {
            get => _trp;
            set => SetProperty(ref _trp, value);
        }

        public double VAL
        {
            get => _val;
            set => SetProperty(ref _val, value);
        }

        public double ALA
        {
            get => _ala;
            set => SetProperty(ref _ala, value);
        }

        public double ASP
        {
            get => _asp;
            set => SetProperty(ref _asp, value);
        }

        public double GLU
        {
            get => _glu;
            set => SetProperty(ref _glu, value);
        }

        public double GLY
        {
            get => _gly;
            set => SetProperty(ref _gly, value);
        }

        public double PRO
        {
            get => _pro;
            set => SetProperty(ref _pro, value);
        }

        public double SER
        {
            get => _ser;
            set => SetProperty(ref _ser, value);
        }

        public double CR
        {
            get => _cr;
            set => SetProperty(ref _cr, value);
        }

        public double Ca
        {
            get => _ca;
            set => SetProperty(ref _ca, value);
        }

        public double P
        {
            get => _p;
            set => SetProperty(ref _p, value);
        }

        public double Mg
        {
            get => _mg;
            set => SetProperty(ref _mg, value);
        }

        public double Cl
        {
            get => _cl;
            set => SetProperty(ref _cl, value);
        }

        public double K
        {
            get => _k;
            set => SetProperty(ref _k, value);
        }

        public double Na
        {
            get => _na;
            set => SetProperty(ref _na, value);
        }

        public double S
        {
            get => _s;
            set => SetProperty(ref _s, value);
        }

        public double Co
        {
            get => _co;
            set => SetProperty(ref _co, value);
        }

        public double Cu
        {
            get => _cu;
            set => SetProperty(ref _cu, value);
        }

        public double I
        {
            get => _i;
            set => SetProperty(ref _i, value);
        }

        public double Fe
        {
            get => _fe;
            set => SetProperty(ref _fe, value);
        }

        public double Mn
        {
            get => _mn;
            set => SetProperty(ref _mn, value);
        }

        public double Se
        {
            get => _se;
            set => SetProperty(ref _se, value);
        }

        public double Zn
        {
            get => _zn;
            set => SetProperty(ref _zn, value);
        }

        public double VitA
        {
            get => _vitA;
            set => SetProperty(ref _vitA, value);
        }

        public double VitD
        {
            get => _vitD;
            set => SetProperty(ref _vitD, value);
        }

        public double VitE
        {
            get => _vitE;
            set => SetProperty(ref _vitE, value);
        }

        public double VitD3
        {
            get => _vitD3;
            set => SetProperty(ref _vitD3, value);
        }

        public double vitB1
        {
            get => _vitB1;
            set => SetProperty(ref _vitB1, value);
        }

        public double VitB2
        {
            get => _vitB2;
            set => SetProperty(ref _vitB2, value);
        }

        public double VitB3
        {
            get => _vitB3;
            set => SetProperty(ref _vitB3, value);
        }

        public double VitB5
        {
            get => _vitB5;
            set => SetProperty(ref _vitB5, value);
        }

        public double VitB6
        {
            get => _vitB6;
            set => SetProperty(ref _vitB6, value);
        }

        public double VitB7
        {
            get => _vitB7;
            set => SetProperty(ref _vitB7, value);
        }

        public double VitB12
        {
            get => _vitB12;
            set => SetProperty(ref _vitB12, value);
        }

        public double PercentageInDiet
        {
            get => _percentageInDiet;
            set => SetProperty(ref _percentageInDiet, value);
        }

        public bool IsCustomIngredient
        {
            get => _isCustomIngredient;
            set => SetProperty(ref _isCustomIngredient, value);
        }

        public string FeedNumber
        {
            get => _feedNumber;
            set => SetProperty(ref _feedNumber, value);
        }

        public DairyFeedClassType DairyFeedClass
        {
            get => _dairyFeedClass;
            set
            {
                SetProperty(ref _dairyFeedClass, value, () => { RaisePropertyChanged(nameof(IngredientTypeString)); });
            }
        }

        public double PAF
        {
            get => _paf;
            set => SetProperty(ref _paf, value);
        }

        [Units(MetricUnitsOfMeasurement.MegaCaloriePerKilogram)]
        public double NEL_ThreeX
        {
            get => _nel_threex;
            set => SetProperty(ref _nel_threex, value);
        }

        public double NEL_FourX
        {
            get => _nel_fourx;
            set => SetProperty(ref _nel_fourx, value);
        }

        public double NEM
        {
            get => _nem;
            set => SetProperty(ref _nem, value);
        }

        public double NEG
        {
            get => _neg;
            set => SetProperty(ref _neg, value);
        }

        public double NDICP
        {
            get => _ndicp;
            set => SetProperty(ref _ndicp, value);
        }

        public double EE
        {
            get => _ee;
            set => SetProperty(ref _ee, value);
        }

        /*
        private double _phytate;
        private double _phytatePhosphorus;
        private double _nonphytatePhosphorus;
        private double _betaCarotene;
        private double _choline;
        */
        public double Phytate
        {
            get => _phytate;
            set => SetProperty(ref _phytate, value);
        }

        public double PhytatePhosphorus
        {
            get => _phytatePhosphorus;
            set => SetProperty(ref _phytatePhosphorus, value);
        }

        public double NonPhytatePhosphorus
        {
            get => _nonphytatePhosphorus;
            set => SetProperty(ref _nonphytatePhosphorus, value);
        }

        public double BetaCarotene
        {
            get => _betaCarotene;
            set => SetProperty(ref _betaCarotene, value);
        }

        public double Choline
        {
            get => _choline;
            set => SetProperty(ref _choline, value);
        }

        [Units(MetricUnitsOfMeasurement.KiloCaloriePerKilogram)]
        public double NE
        {
            get => _ne;
            set => SetProperty(ref _ne, value);
        }

        // Digestability
        public double CpDigestAID
        {
            get => _cpDigestAID;
            set => SetProperty(ref _cpDigestAID, value);
        }

        public double ArgDigestAID
        {
            get => _argDigestAID;
            set => SetProperty(ref _argDigestAID, value);
        }

        public double HisDigestAID
        {
            get => _hisDigestAID;
            set => SetProperty(ref _hisDigestAID, value);
        }

        public double IleDigestAID
        {
            get => _ileDigestAID;
            set => SetProperty(ref _ileDigestAID, value);
        }

        public double LeuDigestAID
        {
            get => _leuDigestAID;
            set => SetProperty(ref _leuDigestAID, value);
        }

        public double LysDigestAID
        {
            get => _lysDigestAID;
            set => SetProperty(ref _lysDigestAID, value);
        }

        public double MetDigestAID
        {
            get => _metDigestAID;
            set => SetProperty(ref _metDigestAID, value);
        }

        public double PheDigestAID
        {
            get => _pheDigestAID;
            set => SetProperty(ref _pheDigestAID, value);
        }

        public double ThrDigestAID
        {
            get => _thrDigestAID;
            set => SetProperty(ref _thrDigestAID, value);
        }

        public double TrpDigestAID
        {
            get => _trpDigestAID;
            set => SetProperty(ref _trpDigestAID, value);
        }

        public double ValDigestAID
        {
            get => _valDigestAID;
            set => SetProperty(ref _valDigestAID, value);
        }

        public double AlaDigestAID
        {
            get => _alaDigestAID;
            set => SetProperty(ref _alaDigestAID, value);
        }

        public double AspDigestAID
        {
            get => _aspDigestAID;
            set => SetProperty(ref _aspDigestAID, value);
        }

        public double CysDigestAID
        {
            get => _cysDigestAID;
            set => SetProperty(ref _cysDigestAID, value);
        }

        public double GluDigestAID
        {
            get => _gluDigestAID;
            set => SetProperty(ref _gluDigestAID, value);
        }

        public double GlyDigestAID
        {
            get => _glyDigestAID;
            set => SetProperty(ref _glyDigestAID, value);
        }

        public double ProDigestAID
        {
            get => _proDigestAID;
            set => SetProperty(ref _proDigestAID, value);
        }

        public double SerDigestAID
        {
            get => _serDigestAID;
            set => SetProperty(ref _serDigestAID, value);
        }

        public double TyrDigestAID
        {
            get => _tyrDigestAID;
            set => SetProperty(ref _tyrDigestAID, value);
        }


        public double CpDigestSID
        {
            get => _cpDigestSID;
            set => SetProperty(ref _cpDigestSID, value);
        }

        public double ArgDigestSID
        {
            get => _argDigestSID;
            set => SetProperty(ref _argDigestSID, value);
        }

        public double HisDigestSID
        {
            get => _hisDigestSID;
            set => SetProperty(ref _hisDigestSID, value);
        }

        public double IleDigestSID
        {
            get => _ileDigestSID;
            set => SetProperty(ref _ileDigestSID, value);
        }

        public double LeuDigestSID
        {
            get => _leuDigestSID;
            set => SetProperty(ref _leuDigestSID, value);
        }

        public double LysDigestSID
        {
            get => _lysDigestSID;
            set => SetProperty(ref _lysDigestSID, value);
        }

        public double MetDigestSID
        {
            get => _metDigestSID;
            set => SetProperty(ref _metDigestSID, value);
        }

        public double PheDigestSID
        {
            get => _pheDigestSID;
            set => SetProperty(ref _pheDigestSID, value);
        }

        public double ThrDigestSID
        {
            get => _thrDigestSID;
            set => SetProperty(ref _thrDigestSID, value);
        }

        public double TrpDigestSID
        {
            get => _trpDigestSID;
            set => SetProperty(ref _trpDigestSID, value);
        }

        public double ValDigestSID
        {
            get => _valDigestSID;
            set => SetProperty(ref _valDigestSID, value);
        }

        public double AlaDigestSID
        {
            get => _alaDigestSID;
            set => SetProperty(ref _alaDigestSID, value);
        }

        public double AspDigestSID
        {
            get => _aspDigestSID;
            set => SetProperty(ref _aspDigestSID, value);
        }

        public double CysDigestSID
        {
            get => _cysDigestSID;
            set => SetProperty(ref _cysDigestSID, value);
        }

        public double GluDigestSID
        {
            get => _gluDigestSID;
            set => SetProperty(ref _gluDigestSID, value);
        }

        public double GlyDigestSID
        {
            get => _glyDigestSID;
            set => SetProperty(ref _glyDigestSID, value);
        }

        public double ProDigestSID
        {
            get => _proDigestSID;
            set => SetProperty(ref _proDigestSID, value);
        }

        public double SerDigestSID
        {
            get => _serDigestSID;
            set => SetProperty(ref _serDigestSID, value);
        }

        public double TyrDigestSID
        {
            get => _tyrDigestSID;
            set => SetProperty(ref _tyrDigestSID, value);
        }

        public string AAFCO
        {
            get => _aafco;
            set => SetProperty(ref _aafco, value);
        }

        public string AAFCO2010
        {
            get => _aafco2010;
            set => SetProperty(ref _aafco2010, value);
        }

        public double ATTDPhosphorus
        {
            get => _attdPhosphorous;
            set => SetProperty(ref _attdPhosphorous, value);
        }

        public double STTDPhosphorus
        {
            get => _sttdPhosphorous;
            set => SetProperty(ref _sttdPhosphorous, value);
        }

        public double Biotin
        {
            get => _biotin;
            set => SetProperty(ref _biotin, value);
        }

        public double Folacin
        {
            get => _folacin;
            set => SetProperty(ref _folacin, value);
        }

        public double Niacin
        {
            get => _niacin;
            set => SetProperty(ref _niacin, value);
        }

        public double PantothenicAcid
        {
            get => _pantothenicAcid;
            set => SetProperty(ref _pantothenicAcid, value);
        }

        public double Riboflavin
        {
            get => _riboflavin;
            set => SetProperty(ref _riboflavin, value);
        }

        public double Thiamin
        {
            get => _thiamin;
            set => SetProperty(ref _thiamin, value);
        }

        public double EtherExtractDup
        {
            get => _etherExtractDup;
            set => SetProperty(ref _etherExtractDup, value);
        }

        public double C120
        {
            get => _c120;
            set => SetProperty(ref _c120, value);
        }

        public double C140
        {
            get => _c140;
            set => SetProperty(ref _c140, value);
        }

        public double C160
        {
            get => _c160;
            set => SetProperty(ref _c160, value);
        }

        public double C161
        {
            get => _c161;
            set => SetProperty(ref _c161, value);
        }

        public double C180
        {
            get => _c180;
            set => SetProperty(ref _c180, value);
        }

        public double C181
        {
            get => _c181;
            set => SetProperty(ref _c181, value);
        }

        public double C182
        {
            get => _c182;
            set => SetProperty(ref _c182, value);
        }

        public double C183
        {
            get => _c183;
            set => SetProperty(ref _c183, value);
        }

        public double C184
        {
            get => _c184;
            set => SetProperty(ref _c184, value);
        }

        public double C200
        {
            get => _c200;
            set => SetProperty(ref _c200, value);
        }

        public double C201
        {
            get => _c201;
            set => SetProperty(ref _c201, value);
        }

        public double C204
        {
            get => _c204;
            set => SetProperty(ref _c204, value);
        }

        public double C205
        {
            get => _c205;
            set => SetProperty(ref _c205, value);
        }

        public double C220
        {
            get => _c220;
            set => SetProperty(ref _c220, value);
        }

        public double C221
        {
            get => _c221;
            set => SetProperty(ref _c221, value);
        }

        public double C225
        {
            get => _c225;
            set => SetProperty(ref _c225, value);
        }

        public double C226
        {
            get => _c226;
            set => SetProperty(ref _c226, value);
        }

        public double C240
        {
            get => _c240;
            set => SetProperty(ref _c240, value);
        }

        public double SFA
        {
            get => _sfa;
            set => SetProperty(ref _sfa, value);
        }

        public double MUFA
        {
            get => _mufa;
            set => SetProperty(ref _mufa, value);
        }

        public double PUFA
        {
            get => _pufa;
            set => SetProperty(ref _pufa, value);
        }

        public double IV
        {
            get => _iv;
            set => SetProperty(ref _iv, value);
        }

        public double IVP
        {
            get => _ivp;
            set => SetProperty(ref _ivp, value);
        }

        #endregion

        #region Public Methods

        public static FeedIngredient CopyFeedIngredient(FeedIngredient ingredient)
        {
            return _ingredientMapper.Map<FeedIngredient>(ingredient);
        }

        public static FeedIngredient ConvertFeedIngredientToImperial(FeedIngredient ingredient)
        {
            var copiedIngredient = CopyFeedIngredient(ingredient);

            var unitCalculator = new UnitsOfMeasurementCalculator();

            copiedIngredient.ME = Math.Round(
                unitCalculator.ConvertValueToImperialFromMetric(
                    MetricUnitsOfMeasurement.MegaCaloriePerKilogram, ingredient.ME), 1);

            copiedIngredient.GrossEnergy = Math.Round(
                unitCalculator.ConvertValueToImperialFromMetric(
                    MetricUnitsOfMeasurement.KiloCaloriePerKilogram, ingredient.GrossEnergy), 1);

            copiedIngredient.DeSwine = Math.Round(
                unitCalculator.ConvertValueToImperialFromMetric(
                    MetricUnitsOfMeasurement.KiloCaloriePerKilogram, ingredient.DeSwine), 1);

            copiedIngredient.DE = Math.Round(
                unitCalculator.ConvertValueToImperialFromMetric(MetricUnitsOfMeasurement.MegaCaloriePerKilogram,
                    ingredient.DE), 1);

            copiedIngredient.NE = Math.Round(
                unitCalculator.ConvertValueToImperialFromMetric(
                    MetricUnitsOfMeasurement.KiloCaloriePerKilogram, ingredient.NE), 1);

            copiedIngredient.NEL_ThreeX = Math.Round(
                unitCalculator.ConvertValueToImperialFromMetric(
                    MetricUnitsOfMeasurement.MegaCaloriePerKilogram, ingredient.NEL_ThreeX), 1);

            return copiedIngredient;
        }

        public override string ToString()
        {
            return $"{nameof(IngredientType)}: {IngredientType.GetDescription()}";
        }

        #endregion
    }
}