using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using H.Core.Enumerations;
using H.Core.Properties;
using H.Infrastructure;
using Prism.Events;
using Prism.Regions;

namespace H.Avalonia.ViewModels.SupportingViewModels;

public class DisclaimerViewModel : ViewModelBase
    {
        #region Fields

        private Languages _selectedLanguage;

        private bool _showLanguageBox;
        private string _aboutHolosString;
        private string _toBeKeptInformedString;
        private string _disclaimerRtfString;
        private string _versionString;
        private string _disclaimerWordString;

        #endregion

        #region Constructors

        public DisclaimerViewModel()
        {
            this.Construct();
        }

        public DisclaimerViewModel(IRegionManager regionManager,
                                   IEventAggregator eventAggregator,
                                   Storage storage) : base(regionManager, eventAggregator, storage)
        {
            this.Construct();
        }

        #endregion

        #region Properties

        public ObservableCollection<Languages> Languages { get; set; } = new ObservableCollection<Languages>(Enum.GetValues(typeof(Languages)).Cast<Languages>());

        public Languages SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value, OnSelectedLanguageChanged);
        }

        public string AboutHolosString
        {
            get => _aboutHolosString;
            set => SetProperty(ref _aboutHolosString, value);
        }

        public string ToBeKeptInformedString
        {
            get => _toBeKeptInformedString;
            set => SetProperty(ref _toBeKeptInformedString, value);
        }

        public string DisclaimerRtfString
        {
            get => _disclaimerRtfString;
            set => SetProperty(ref _disclaimerRtfString, value);
        }
        public string DisclaimerWordString
        {
            get => _disclaimerWordString;
            set => SetProperty(ref _disclaimerWordString, value);
        }

        public string VersionString
        {
            get => _versionString;
            set => _versionString = value;
        }
        public bool ShowLanguageBox
        {
            get => _showLanguageBox;
            set => SetProperty(ref _showLanguageBox, value);
        }

        #endregion

        #region Public Methods

        public new void Construct()
        {
            this.SetHolosLanguageToSystemLanguage();
            this.UpdateDisplayBasedOnLanguage();
            this.VersionString = GuiConstants.GetVersionString();
        }

        #endregion

        #region Private Methods

        private void UpdateDisplayBasedOnLanguage()
        {
            if (this.SelectedLanguage == H.Core.Enumerations.Languages.English)
            {
                this.AboutHolosString = "Holos - a tool to estimate and reduce greenhouse gas emissions from farms";
                this.ToBeKeptInformedString = "To be kept informed about  future versions, please send your contact information (including email address) to holos@canada.ca";
                this.DisclaimerRtfString = Core.Properties.FileResources.Disclaimer_English;

                this.DisclaimerWordString = "Disclaimer";
                //Settings.Default.DisplayLanguage = H.Core.Enumerations.Languages.English.GetDescription();
            }
            else
            {
                this.AboutHolosString = "Holos - outil d'évaluation et de réduction des émissions de gaz à effet de serre des fermes agricoles";
                this.ToBeKeptInformedString = "Pour être informé de la publication des prochaines versions du logiciel, faites parvenir vos coordonnées (y compris votre adresse électronique) à holos@canada.ca";
                this.DisclaimerRtfString = Core.Properties.FileResources.Disclaimer_French;
                this.DisclaimerWordString = "Avis de non-responsabilité";

                //Settings.Default.DisplayLanguage = H.Core.Enumerations.Languages.French.GetDescription();
            }
        }

        private void SetHolosLanguageToSystemLanguage()
        {
            const string usEnglish = "en-US";
            var culture = Thread.CurrentThread.CurrentCulture;

            //some computers might have their region set to united states english best to keep their gui in english
            if (culture.Name == InfrastructureConstants.EnglishCultureInfo.Name || culture.Name == usEnglish)
            {
                this.SelectedLanguage = H.Core.Enumerations.Languages.English;
            }
            else
            {
                this.SelectedLanguage = H.Core.Enumerations.Languages.French;
            }
        }

        #endregion

        #region Event Handlers

        private void OnSelectedLanguageChanged()
        {
            this.UpdateDisplayBasedOnLanguage();
        }

        #endregion
    }