using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using H.Avalonia.Infrastructure.Dialogs;
using H.Avalonia.ViewModels;
using H.Avalonia.Views;
using H.Avalonia.Views.ResultViews;
using H.Avalonia.Infrastructure;
using H.Core.Providers;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Regions;
using System;
using H.Avalonia.Core;
using H.Avalonia.Core.Services;
using H.Avalonia.Core.Services.Ireland;
using H.Avalonia.Dialogs;
using H.Avalonia.ViewModels.ResultViewModels;
using H.Avalonia.ViewModels.SupportingViewModels;
using H.Avalonia.ViewModels.SupportingViewModels.Ireland;
using H.Avalonia.Views.SupportingViews;
using H.Avalonia.Views.SupportingViews.Ireland;
using H.Core.Calculators.Infrastructure;
using H.Core.Enumerations;
using H.Core.Services;
using H.Core.Services.Animals;
using ClimateResultsView = H.Avalonia.Views.ResultViews.ClimateResultsView;
using SoilResultsView = H.Avalonia.Views.ResultViews.SoilResultsView;

namespace H.Avalonia
{
    public partial class App : PrismApplication
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
        }

        /// <summary>Register Services and Views.</summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Views - Region Navigation
            containerRegistry.RegisterForNavigation<DisclaimerView, DisclaimerViewModel>();
            containerRegistry.RegisterForNavigation<ToolbarView, ToolbarViewModel>();
            containerRegistry.RegisterForNavigation<SidebarView, SidebarViewModel>();
            containerRegistry.RegisterForNavigation<ClimateDataView, ClimateDataViewModel>();
            containerRegistry.RegisterForNavigation<SoilDataView, SoilDataViewModel>();
            containerRegistry.RegisterForNavigation<AboutPageView, AboutPageViewModel>();
            containerRegistry.RegisterForNavigation<ClimateResultsView, ClimateResultsViewModel>();
            containerRegistry.RegisterForNavigation<SoilResultsView, SoilResultsViewModel>();
            
            // Miscellaneous
            containerRegistry.RegisterSingleton<PrototypeStorage>();
            containerRegistry.RegisterSingleton<Storage>();

            // Providers
            containerRegistry.RegisterSingleton<GeographicDataProvider>();
            containerRegistry.RegisterSingleton<ExportHelpers>();
            containerRegistry.RegisterSingleton<ImportHelpers>();
            containerRegistry.RegisterSingleton<KmlHelpers>();

            // Dialogs
            containerRegistry.RegisterDialog<DeleteRowDialog, DeleteRowDialogViewModel>();
            
            //Services
            containerRegistry.RegisterSingleton<IFarmResultsService, FarmResultsService>();
            containerRegistry.RegisterSingleton<IADCalculator, ADCalculator>();
            containerRegistry.RegisterSingleton<IDigestateService, DigestateService>();
            containerRegistry.RegisterSingleton<IManureService, ManureService>();
            containerRegistry.RegisterSingleton<IAnimalService, AnimalResultsService>();

            // Geographic region Based container registration
            if (Settings.Default.UserRegion == UserRegion.Canada)
            {
                // Views - Region Navigation
                containerRegistry.RegisterForNavigation<FooterView, FooterViewModel>();
                
                // Services
                containerRegistry.RegisterSingleton<IDisclaimerService, DisclaimerService>();
            }
            else
            {
                // Views - Region Navigation
                containerRegistry.RegisterForNavigation<IrishFooterView, IrishFooterViewModel>();
                
                // Services
                containerRegistry.RegisterSingleton<IDisclaimerService, IrishDisclaimerService>();
            }
        }

        /// <summary>User interface entry point, called after Register and ConfigureModules.</summary>
        /// <returns>Startup View.</returns>
        protected override AvaloniaObject CreateShell()
        {
             var mainWindow = Container.Resolve<MainWindow>();
             return mainWindow;
        }

        /// <summary>Called after Initialize.</summary>
        protected override void OnInitialized()
        {
            // Register Views to the Region it will appear in. Don't register them in the ViewModel.
            var regionManager = Container.Resolve<IRegionManager>();
            
            // Register views based on user's region
            if (Settings.Default.UserRegion == UserRegion.Canada)
            {
                regionManager.RegisterViewWithRegion(UiRegions.FooterRegion, typeof(FooterView));
            }
            else
            {
                regionManager.RegisterViewWithRegion(UiRegions.FooterRegion, typeof(IrishFooterView));
            }
            regionManager.RegisterViewWithRegion(UiRegions.FooterRegion, typeof(FooterView));
            regionManager.RegisterViewWithRegion(UiRegions.ContentRegion, typeof(DisclaimerView));

            var geographicProvider = Container.Resolve<GeographicDataProvider>();
            geographicProvider.Initialize();
            Container.Resolve<KmlHelpers>();

        }
    }
}