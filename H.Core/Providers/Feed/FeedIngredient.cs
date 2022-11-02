#region Imports

using System;
using AutoMapper;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Infrastructure;

#endregion

namespace H.Core.Providers.Feed
{
    /// <summary>
    /// </summary>
    public class FeedIngredient : ModelBase
    {
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

        #region Constructors

        static FeedIngredient()
        {
            var mapperConfig = new MapperConfiguration(x =>
            {
                x.CreateMap<FeedIngredient, FeedIngredient>().ForMember(prop => prop.Guid, opt => opt.Ignore());
            });
            _ingredientMapper = mapperConfig.CreateMapper();

        }
        #endregion

        #region Properties

        /// <summary>
        /// Used when display system (default) ingredients that should not be modifiable by user.
        /// </summary>
        public bool IsReadonly { get; set; }

        public string IngredientTypeString
        {
            get { return _ingredientStringType; }
            set { this.SetProperty(ref _ingredientStringType, value); }
        }

        public IngredientType IngredientType
        {
            get { return _ingredientType; }
            set { this.SetProperty(ref _ingredientType, value, () => { this.RaisePropertyChanged(nameof(this.IngredientTypeString)); }); }
        }

        public double Sugars
        {
            get { return _sugars; }
            set { this.SetProperty(ref _sugars, value); }
        }

        public double SolubleCrudeProtein
        {
            get { return _solubleCrudeProtein; }
            set { this.SetProperty(ref _solubleCrudeProtein, value); }
        }

        public double DryMatter
        {
            get { return _dryMatter; }
            set { this.SetProperty(ref _dryMatter, value); }
        }

        public double CrudeProtein
        {
            get { return _crudeProtein; }
            set { this.SetProperty(ref _crudeProtein, value); }
        }

        public double CrudeFiber
        {
            get { return _crudeFiber; }
            set { this.SetProperty(ref _crudeFiber, value); }
        }

        public double AcidEtherExtract
        {
            get { return _acidEtherExtract; }
            set { this.SetProperty(ref _acidEtherExtract, value); }
        }

        [Units(MetricUnitsOfMeasurement.KiloCaloriePerKilogram)]
        public double GrossEnergy
        {
            get { return _grossEnergy; }
            set { this.SetProperty(ref _grossEnergy, value); }
        }

        public double Fat
        {
            get { return _fat; }
            set { this.SetProperty(ref _fat, value); }
        }

        public double TotalDigestibleNutrient
        {
            get { return _totalDigestibleNutrient; }
            set { this.SetProperty(ref _totalDigestibleNutrient, value); }
        }

        public double Starch
        {
            get { return _starch; }
            set { this.SetProperty(ref _starch, value); }
        }

        public string IFN
        {
            get { return _ifn; }
            set { this.SetProperty(ref _ifn, value); }
        }

        public double Cost
        {
            get { return _cost; }
            set { this.SetProperty(ref _cost, value); }
        }

        public double Forage
        {
            get { return _forage; }
            set { this.SetProperty(ref _forage, value); }
        }

        public double RDP
        {
            get { return _rdp; }
            set { this.SetProperty(ref _rdp, value); }
        }

        public double ADF
        {
            get { return _adf; }
            set { this.SetProperty(ref _adf, value); }
        }

        public double ADL
        {
            get { return _adl; }
            set { this.SetProperty(ref _adl, value); }
        }

        public double Hemicellulose
        {
            get { return _hemicellulose; }
            set { this.SetProperty(ref _hemicellulose, value); }
        }

        public double TDF
        {
            get { return _tdf; }
            set { this.SetProperty(ref _tdf, value); }
        }

        public double IDF
        {
            get { return _idf; }
            set { this.SetProperty(ref _idf, value); }
        }

        public double SDF
        {
            get { return _sdf; }
            set { this.SetProperty(ref _sdf, value); }
        }

        public double Sucrose
        {
            get { return _sucrose; }
            set { this.SetProperty(ref _sucrose, value); }
        }

        public double Lactose
        {
            get { return _lactose; }
            set { this.SetProperty(ref _lactose, value); }
        }

        public double Glucose
        {
            get { return _glucose; }
            set { this.SetProperty(ref _glucose, value); }
        }

        public double Oligosaccharides
        {
            get { return _oligosaccharides; }
            set { this.SetProperty(ref _oligosaccharides, value); }
        }

        public double Raffinose
        {
            get { return _raffinose; }
            set { this.SetProperty(ref _raffinose, value); }
        }

        public double Stachyose
        {
            get { return _stachyose; }
            set { this.SetProperty(ref _stachyose, value); }
        }

        public double Verbascose
        {
            get { return _verbascose; }
            set { this.SetProperty(ref _verbascose, value); }
        }

        public double Mo
        {
            get { return _mo; }
            set { this.SetProperty(ref _mo, value); }
        }

        /// <summary>
        /// used for cattle diets
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaCaloriePerKilogram)]
        public double DE
        {
            get { return _de; }
            set { this.SetProperty(ref _de, value); }
        }

        /// <summary>
        /// used only for swine diets
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KiloCaloriePerKilogram)]
        public double DeSwine
        {
            get => _deSwine;
            set => SetProperty(ref _deSwine, value);
        }


        public double SP
        {
            get { return _sp; }
            set { this.SetProperty(ref _sp, value); }
        }

        public double ADICP
        {
            get { return _adicp; }
            set { this.SetProperty(ref _adicp, value); }
        }

        public double OA
        {
            get { return _oa; }
            set { this.SetProperty(ref _oa, value); }
        }

        public double Ash
        {
            get { return _ash; }
            set { this.SetProperty(ref _ash, value); }
        }

        public double NDF
        {
            get { return _ndf; }
            set { this.SetProperty(ref _ndf, value); }
        }

        public double Lignin
        {
            get { return _lignin; }
            set { this.SetProperty(ref _lignin, value); }
        }

        public double Pef
        {
            get { return _pef; }
            set { this.SetProperty(ref _pef, value); }
        }

        [Units(MetricUnitsOfMeasurement.MegaCaloriePerKilogram)]
        public double ME
        {
            get { return _me; }
            set { this.SetProperty(ref _me, value); }
        }

        public double NEma
        {
            get { return _nEma; }
            set { this.SetProperty(ref _nEma, value, () => base.RaisePropertyChanged(nameof(this.Nemf))); }
        }

        public double NEga
        {
            get { return _nEga; }
            set { this.SetProperty(ref _nEga, value, () => base.RaisePropertyChanged(nameof(this.Nemf))); }
        }

        public double RUP
        {
            get { return _rup; }
            set { this.SetProperty(ref _rup, value); }
        }

        public double kdPB
        {
            get { return _kdPb; }
            set { this.SetProperty(ref _kdPb, value); }
        }

        public double KdCB1
        {
            get { return _kdCb1; }
            set { this.SetProperty(ref _kdCb1, value); }
        }

        public double KdCB2
        {
            get { return _kdCb2; }
            set { this.SetProperty(ref _kdCb2, value); }
        }

        public double KdCB3
        {
            get { return _kdCb3; }
            set { this.SetProperty(ref _kdCb3, value); }
        }

        public double PBID
        {
            get { return _pbid; }
            set { this.SetProperty(ref _pbid, value); }
        }

        public double CB1ID
        {
            get { return _cb1id; }
            set { this.SetProperty(ref _cb1id, value); }
        }

        public double CB2ID
        {
            get { return _cb2id; }
            set { this.SetProperty(ref _cb2id, value); }
        }

        public double ARG
        {
            get { return _arg; }
            set { this.SetProperty(ref _arg, value); }
        }

        public double HIS
        {
            get { return _his; }
            set { this.SetProperty(ref _his, value); }
        }

        public double ILE
        {
            get { return _ile; }
            set { this.SetProperty(ref _ile, value); }
        }

        public double LEU
        {
            get { return _leu; }
            set { this.SetProperty(ref _leu, value); }
        }

        public double LYS
        {
            get { return _lys; }
            set { this.SetProperty(ref _lys, value); }
        }

        public double MET
        {
            get { return _met; }
            set { this.SetProperty(ref _met, value); }
        }

        public double CYS
        {
            get { return _cys; }
            set { this.SetProperty(ref _cys, value); }
        }

        public double PHE
        {
            get { return _phe; }
            set { this.SetProperty(ref _phe, value); }
        }

        public double TYR
        {
            get { return _tyr; }
            set { this.SetProperty(ref _tyr, value); }
        }

        public double THR
        {
            get { return _thr; }
            set { this.SetProperty(ref _thr, value); }
        }

        public double TRP
        {
            get { return _trp; }
            set { this.SetProperty(ref _trp, value); }
        }

        public double VAL
        {
            get { return _val; }
            set { this.SetProperty(ref _val, value); }
        }

        public double ALA
        {
            get { return _ala; }
            set { this.SetProperty(ref _ala, value); }
        }

        public double ASP
        {
            get { return _asp; }
            set { this.SetProperty(ref _asp, value); }
        }

        public double GLU
        {
            get { return _glu; }
            set { this.SetProperty(ref _glu, value); }
        }

        public double GLY
        {
            get { return _gly; }
            set { this.SetProperty(ref _gly, value); }
        }

        public double PRO
        {
            get { return _pro; }
            set { this.SetProperty(ref _pro, value); }
        }

        public double SER
        {
            get { return _ser; }
            set { this.SetProperty(ref _ser, value); }
        }

        public double CR
        {
            get { return _cr; }
            set { this.SetProperty(ref _cr, value); }
        }

        public double Ca
        {
            get { return _ca; }
            set { this.SetProperty(ref _ca, value); }
        }

        public double P
        {
            get { return _p; }
            set { this.SetProperty(ref _p, value); }
        }

        public double Mg
        {
            get { return _mg; }
            set { this.SetProperty(ref _mg, value); }
        }

        public double Cl
        {
            get { return _cl; }
            set { this.SetProperty(ref _cl, value); }
        }

        public double K
        {
            get { return _k; }
            set { this.SetProperty(ref _k, value); }
        }

        public double Na
        {
            get { return _na; }
            set { this.SetProperty(ref _na, value); }
        }

        public double S
        {
            get { return _s; }
            set { this.SetProperty(ref _s, value); }
        }

        public double Co
        {
            get { return _co; }
            set { this.SetProperty(ref _co, value); }
        }

        public double Cu
        {
            get { return _cu; }
            set { this.SetProperty(ref _cu, value); }
        }

        public double I
        {
            get { return _i; }
            set { this.SetProperty(ref _i, value); }
        }

        public double Fe
        {
            get { return _fe; }
            set { this.SetProperty(ref _fe, value); }
        }

        public double Mn
        {
            get { return _mn; }
            set { this.SetProperty(ref _mn, value); }
        }

        public double Se
        {
            get { return _se; }
            set { this.SetProperty(ref _se, value); }
        }

        public double Zn
        {
            get { return _zn; }
            set { this.SetProperty(ref _zn, value); }
        }

        public double VitA
        {
            get { return _vitA; }
            set { this.SetProperty(ref _vitA, value); }
        }

        public double VitD
        {
            get { return _vitD; }
            set { this.SetProperty(ref _vitD, value); }
        }

        public double VitE
        {
            get { return _vitE; }
            set { this.SetProperty(ref _vitE, value); }
        }

        public double VitD3
        {
            get { return _vitD3; }
            set { this.SetProperty(ref _vitD3, value); }
        }

        public double vitB1
        {
            get { return _vitB1; }
            set { this.SetProperty(ref _vitB1, value); }
        }

        public double VitB2
        {
            get { return _vitB2; }
            set { this.SetProperty(ref _vitB2, value); }
        }

        public double VitB3
        {
            get { return _vitB3; }
            set { this.SetProperty(ref _vitB3, value); }
        }

        public double VitB5
        {
            get { return _vitB5; }
            set { this.SetProperty(ref _vitB5, value); }
        }

        public double VitB6
        {
            get { return _vitB6; }
            set { this.SetProperty(ref _vitB6, value); }
        }

        public double VitB7
        {
            get { return _vitB7; }
            set { this.SetProperty(ref _vitB7, value); }
        }

        public double VitB12
        {
            get { return _vitB12; }
            set { this.SetProperty(ref _vitB12, value); }
        }

        public double PercentageInDiet
        {
            get { return _percentageInDiet; }
            set { this.SetProperty(ref _percentageInDiet, value); }
        }

        public bool IsCustomIngredient
        {
            get { return _isCustomIngredient; }
            set { this.SetProperty(ref _isCustomIngredient, value); }
        }

        public string FeedNumber
        {
            get { return _feedNumber; }
            set { this.SetProperty(ref _feedNumber, value); }
        }

        public DairyFeedClassType DairyFeedClass
        {
            get { return _dairyFeedClass; }
            set { this.SetProperty(ref _dairyFeedClass, value, () => { this.RaisePropertyChanged(nameof(this.IngredientTypeString)); }); }
        }

        public double PAF
        {
            get { return _paf; }
            set { this.SetProperty(ref _paf, value); }
        }

        [Units(MetricUnitsOfMeasurement.MegaCaloriePerKilogram)]
        public double NEL_ThreeX
        {
            get { return _nel_threex; }
            set { this.SetProperty(ref _nel_threex, value); }
        }

        public double NEL_FourX
        {
            get { return _nel_fourx; }
            set { this.SetProperty(ref _nel_fourx, value); }
        }

        public double NEM
        {
            get { return _nem; }
            set { this.SetProperty(ref _nem, value); }
        }

        public double NEG
        {
            get { return _neg; }
            set { this.SetProperty(ref _neg, value); }
        }

        public double NDICP
        {
            get { return _ndicp; }
            set { this.SetProperty(ref _ndicp, value); }
        }

        public double EE
        {
            get { return _ee; }
            set { this.SetProperty(ref _ee, value); }
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
            get { return _phytate; }
            set { this.SetProperty(ref _phytate, value); }
        }

        public double PhytatePhosphorus
        {
            get { return _phytatePhosphorus; }
            set { this.SetProperty(ref _phytatePhosphorus, value); }
        }

        public double NonPhytatePhosphorus
        {
            get { return _nonphytatePhosphorus; }
            set { this.SetProperty(ref _nonphytatePhosphorus, value); }
        }

        public double BetaCarotene
        {
            get { return _betaCarotene; }
            set { this.SetProperty(ref _betaCarotene, value); }
        }

        public double Choline
        {
            get { return _choline; }
            set { this.SetProperty(ref _choline, value); }
        }

        [Units(MetricUnitsOfMeasurement.KiloCaloriePerKilogram)]
        public double NE
        {
            get { return _ne; }
            set { this.SetProperty(ref _ne, value); }
        }
        // Digestability
        public double CpDigestAID
        {
            get { return _cpDigestAID; }
            set { this.SetProperty(ref _cpDigestAID, value); }
        }

        public double ArgDigestAID
        {
            get { return _argDigestAID; }
            set { this.SetProperty(ref _argDigestAID, value); }
        }

        public double HisDigestAID
        {
            get { return _hisDigestAID; }
            set { this.SetProperty(ref _hisDigestAID, value); }
        }

        public double IleDigestAID
        {
            get { return _ileDigestAID; }
            set { this.SetProperty(ref _ileDigestAID, value); }
        }

        public double LeuDigestAID
        {
            get { return _leuDigestAID; }
            set { this.SetProperty(ref _leuDigestAID, value); }
        }

        public double LysDigestAID
        {
            get { return _lysDigestAID; }
            set { this.SetProperty(ref _lysDigestAID, value); }
        }

        public double MetDigestAID
        {
            get { return _metDigestAID; }
            set { this.SetProperty(ref _metDigestAID, value); }
        }

        public double PheDigestAID
        {
            get { return _pheDigestAID; }
            set { this.SetProperty(ref _pheDigestAID, value); }
        }

        public double ThrDigestAID
        {
            get { return _thrDigestAID; }
            set { this.SetProperty(ref _thrDigestAID, value); }
        }

        public double TrpDigestAID
        {
            get { return _trpDigestAID; }
            set { this.SetProperty(ref _trpDigestAID, value); }
        }

        public double ValDigestAID
        {
            get { return _valDigestAID; }
            set { this.SetProperty(ref _valDigestAID, value); }
        }

        public double AlaDigestAID
        {
            get { return _alaDigestAID; }
            set { this.SetProperty(ref _alaDigestAID, value); }
        }

        public double AspDigestAID
        {
            get { return _aspDigestAID; }
            set { this.SetProperty(ref _aspDigestAID, value); }
        }

        public double CysDigestAID
        {
            get { return _cysDigestAID; }
            set { this.SetProperty(ref _cysDigestAID, value); }
        }

        public double GluDigestAID
        {
            get { return _gluDigestAID; }
            set { this.SetProperty(ref _gluDigestAID, value); }
        }

        public double GlyDigestAID
        {
            get { return _glyDigestAID; }
            set { this.SetProperty(ref _glyDigestAID, value); }
        }

        public double ProDigestAID
        {
            get { return _proDigestAID; }
            set { this.SetProperty(ref _proDigestAID, value); }
        }

        public double SerDigestAID
        {
            get { return _serDigestAID; }
            set { this.SetProperty(ref _serDigestAID, value); }
        }

        public double TyrDigestAID
        {
            get { return _tyrDigestAID; }
            set { this.SetProperty(ref _tyrDigestAID, value); }
        }


        public double CpDigestSID
        {
            get { return _cpDigestSID; }
            set { this.SetProperty(ref _cpDigestSID, value); }
        }

        public double ArgDigestSID
        {
            get { return _argDigestSID; }
            set { this.SetProperty(ref _argDigestSID, value); }
        }

        public double HisDigestSID
        {
            get { return _hisDigestSID; }
            set { this.SetProperty(ref _hisDigestSID, value); }
        }

        public double IleDigestSID
        {
            get { return _ileDigestSID; }
            set { this.SetProperty(ref _ileDigestSID, value); }
        }

        public double LeuDigestSID
        {
            get { return _leuDigestSID; }
            set { this.SetProperty(ref _leuDigestSID, value); }
        }

        public double LysDigestSID
        {
            get { return _lysDigestSID; }
            set { this.SetProperty(ref _lysDigestSID, value); }
        }

        public double MetDigestSID
        {
            get { return _metDigestSID; }
            set { this.SetProperty(ref _metDigestSID, value); }
        }

        public double PheDigestSID
        {
            get { return _pheDigestSID; }
            set { this.SetProperty(ref _pheDigestSID, value); }
        }

        public double ThrDigestSID
        {
            get { return _thrDigestSID; }
            set { this.SetProperty(ref _thrDigestSID, value); }
        }

        public double TrpDigestSID
        {
            get { return _trpDigestSID; }
            set { this.SetProperty(ref _trpDigestSID, value); }
        }

        public double ValDigestSID
        {
            get { return _valDigestSID; }
            set { this.SetProperty(ref _valDigestSID, value); }
        }

        public double AlaDigestSID
        {
            get { return _alaDigestSID; }
            set { this.SetProperty(ref _alaDigestSID, value); }
        }

        public double AspDigestSID
        {
            get { return _aspDigestSID; }
            set { this.SetProperty(ref _aspDigestSID, value); }
        }

        public double CysDigestSID
        {
            get { return _cysDigestSID; }
            set { this.SetProperty(ref _cysDigestSID, value); }
        }

        public double GluDigestSID
        {
            get { return _gluDigestSID; }
            set { this.SetProperty(ref _gluDigestSID, value); }
        }

        public double GlyDigestSID
        {
            get { return _glyDigestSID; }
            set { this.SetProperty(ref _glyDigestSID, value); }
        }

        public double ProDigestSID
        {
            get { return _proDigestSID; }
            set { this.SetProperty(ref _proDigestSID, value); }
        }

        public double SerDigestSID
        {
            get { return _serDigestSID; }
            set { this.SetProperty(ref _serDigestSID, value); }
        }

        public double TyrDigestSID
        {
            get { return _tyrDigestSID; }
            set { this.SetProperty(ref _tyrDigestSID, value); }
        }
        public string AAFCO
        {
            get { return _aafco; }
            set { this.SetProperty(ref _aafco, value); }
        }
        public string AAFCO2010
        {
            get { return _aafco2010; }
            set { this.SetProperty(ref _aafco2010, value); }
        }

        public double ATTDPhosphorus
        {
            get { return _attdPhosphorous; }
            set { this.SetProperty(ref _attdPhosphorous, value); }
        }
        public double STTDPhosphorus
        {
            get { return _sttdPhosphorous; }
            set { this.SetProperty(ref _sttdPhosphorous, value); }
        }
        public double Biotin
        {
            get { return _biotin; }
            set { this.SetProperty(ref _biotin, value); }
        }
        public double Folacin
        {
            get { return _folacin; }
            set { this.SetProperty(ref _folacin, value); }
        }
        public double Niacin
        {
            get { return _niacin; }
            set { this.SetProperty(ref _niacin, value); }
        }
        public double PantothenicAcid
        {
            get { return _pantothenicAcid; }
            set { this.SetProperty(ref _pantothenicAcid, value); }
        }
        public double Riboflavin
        {
            get { return _riboflavin; }
            set { this.SetProperty(ref _riboflavin, value); }
        }
        public double Thiamin
        {
            get { return _thiamin; }
            set { this.SetProperty(ref _thiamin, value); }
        }
        public double EtherExtractDup
        {
            get { return _etherExtractDup; }
            set { this.SetProperty(ref _etherExtractDup, value); }
        }

        public double C120
        {
            get { return _c120; }
            set { this.SetProperty(ref _c120, value); }
        }
        public double C140
        {
            get { return _c140; }
            set { this.SetProperty(ref _c140, value); }
        }
        public double C160
        {
            get { return _c160; }
            set { this.SetProperty(ref _c160, value); }
        }
        public double C161
        {
            get { return _c161; }
            set { this.SetProperty(ref _c161, value); }
        }
        public double C180
        {
            get { return _c180; }
            set { this.SetProperty(ref _c180, value); }
        }
        public double C181
        {
            get { return _c181; }
            set { this.SetProperty(ref _c181, value); }
        }
        public double C182
        {
            get { return _c182; }
            set { this.SetProperty(ref _c182, value); }
        }
        public double C183
        {
            get { return _c183; }
            set { this.SetProperty(ref _c183, value); }
        }
        public double C184
        {
            get { return _c184; }
            set { this.SetProperty(ref _c184, value); }
        }
        public double C200
        {
            get { return _c200; }
            set { this.SetProperty(ref _c200, value); }
        }
        public double C201
        {
            get { return _c201; }
            set { this.SetProperty(ref _c201, value); }
        }
        public double C204
        {
            get { return _c204; }
            set { this.SetProperty(ref _c204, value); }
        }
        public double C205
        {
            get { return _c205; }
            set { this.SetProperty(ref _c205, value); }
        }
        public double C220
        {
            get { return _c220; }
            set { this.SetProperty(ref _c220, value); }
        }
        public double C221
        {
            get { return _c221; }
            set { this.SetProperty(ref _c221, value); }
        }
        public double C225
        {
            get { return _c225; }
            set { this.SetProperty(ref _c225, value); }
        }
        public double C226
        {
            get { return _c226; }
            set { this.SetProperty(ref _c226, value); }
        }
        public double C240
        {
            get { return _c240; }
            set { this.SetProperty(ref _c240, value); }
        }
        public double SFA
        {
            get { return _sfa; }
            set { this.SetProperty(ref _sfa, value); }
        }
        public double MUFA
        {
            get { return _mufa; }
            set { this.SetProperty(ref _mufa, value); }
        }
        public double PUFA
        {
            get { return _pufa; }
            set { this.SetProperty(ref _pufa, value); }
        }
        public double IV
        {
            get { return _iv; }
            set { this.SetProperty(ref _iv, value); }
        }
        public double IVP
        {
            get { return _ivp; }
            set { this.SetProperty(ref _ivp, value); }
        }

        #endregion

        #region Calculated Properties

        public double Nemf
        {
            get
            {
                // NEmf (MJ/kg DM) = [NEma (Mcal/kg DM) + NEga (Mcal/kg DM)] * 4.184 (conversion factor for Mcal to MJ)
                return (this.NEga + this.NEma) * 4.184;
            }
        }

        #endregion

        #region Public Methods

        public static FeedIngredient CopyFeedIngredient(FeedIngredient ingredient)
        {
            return _ingredientMapper.Map<FeedIngredient>(ingredient);
        }

        public static FeedIngredient ConvertFeedIngredientToImperial(FeedIngredient ingredient)
        {
            var copiedIngredient = FeedIngredient.CopyFeedIngredient(ingredient);

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
            return $"{nameof(this.IngredientType)}: {this.IngredientType.GetDescription()}";
        }

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}


