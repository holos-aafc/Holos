using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using Avalonia.Controls.Notifications;
using H.Avalonia.Views;
using H.Core.Enumerations;
using H.Core.Properties;
using H.Infrastructure;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace H.Avalonia.ViewModels.SupportingViewModels;

public class DisclaimerViewModel : ViewModelBase
    {
        #region Fields
        private const int DefaultNotificationTime = 15;
        private Languages _selectedLanguage;
        private readonly UserRegion _startupRegion;
        private UserRegion _selectedUserRegion;
        private readonly IRegionManager _regionManager = null!;

        private bool _showLanguageBox;
        private string _aboutHolosString;
        private string _toBeKeptInformedString;
        private string _disclaimerRtfString;
        private string _versionString;
        private string _disclaimerWordString;
        private string _selectLanguageString;
        private string _selectRegionString;

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
            SelectedLanguage = Settings.Default.DisplayLanguage;
            SelectedUserRegion = _startupRegion = Settings.Default.UserRegion;
            _regionManager = regionManager;
            SwitchToLandingPageCommand = new DelegateCommand(OnSwitchToLandingPage);
            this.Construct();
        }

        #endregion

        #region Properties

        public ObservableCollection<Languages> Languages { get; set; } = new (Enum.GetValues(typeof(Languages)).Cast<Languages>());

        public ObservableCollection<UserRegion> UserRegions { get; set; } = new (Enum.GetValues(typeof(UserRegion)).Cast<UserRegion>());

        public Languages SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value, OnSelectedLanguageChanged);
        }

        public UserRegion SelectedUserRegion
        {
            get => _selectedUserRegion;
            set => SetProperty(ref _selectedUserRegion, value, OnSelectedRegionChanged);
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

        public string SelectLanguageString
        {
            get => _selectLanguageString;
            set => SetProperty(ref _selectLanguageString, value);
        }
        
        public string SelectRegionString
        {
            get => _selectRegionString;
            set => SetProperty(ref _selectRegionString, value);
        }
        
        public bool ShowLanguageBox
        {
            get => _showLanguageBox;
            set => SetProperty(ref _showLanguageBox, value);
        }
        
        /// <summary>
        /// A command that helps switch view to <see cref="SoilDataView"/>
        /// </summary>
        public DelegateCommand SwitchToLandingPageCommand { get; set; } = null!;

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
            CultureInfo cultureInfo;
            if (SelectedLanguage == H.Core.Enumerations.Languages.English)
            {
                cultureInfo = InfrastructureConstants.EnglishCultureInfo;
                DisclaimerRtfString = Core.Properties.FileResources.Disclaimer_English;
            }
            else
            {
                cultureInfo = InfrastructureConstants.FrenchCultureInfo;
                DisclaimerRtfString = Core.Properties.FileResources.Disclaimer_French;
            }
            Settings.Default.DisplayLanguage = SelectedLanguage;
            Settings.Default.Save();
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;

            SelectRegionString = Core.Properties.Resources.SelectRegion;
            SelectLanguageString = Core.Properties.Resources.SelectYourLanguage;
            AboutHolosString = Core.Properties.Resources.AboutHolosMessage;
            ToBeKeptInformedString = Core.Properties.Resources.ToBeKeptInformedMessage;
            DisclaimerWordString = Core.Properties.Resources.LabelDisclaimer;
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

        private void OnSelectedRegionChanged()
        {
            Settings.Default.UserRegion = SelectedUserRegion;
            Settings.Default.Save();

            if (SelectedUserRegion != _startupRegion)
            {
                NotificationManager?.Show(new Notification(
                    Core.Properties.Resources.HeadingRestartRequired,
                    Core.Properties.Resources.MessageRestartRequired,
                    type: NotificationType.Information,
                    expiration: TimeSpan.FromSeconds(DefaultNotificationTime))
                );
            }
        }

        private void OnSwitchToLandingPage()
        {
            _regionManager.RequestNavigate(UiRegions.ContentRegion, nameof(SoilDataView));
            _regionManager.RequestNavigate(UiRegions.ToolbarRegion, nameof(ToolbarView));
        }

        #endregion
    }