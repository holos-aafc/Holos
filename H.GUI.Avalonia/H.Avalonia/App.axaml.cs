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
using H.Avalonia.ViewModels.ResultViewModels;
using H.Avalonia.ViewModels.SupportingViewModels;
using H.Avalonia.Views.SupportingViews;
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
            containerRegistry.RegisterForNavigation<FooterView, FooterViewModel>();
            containerRegistry.RegisterForNavigation<ClimateDataView, ClimateDataViewModel>();
            containerRegistry.RegisterForNavigation<SoilDataView, SoilDataViewModel>();
            containerRegistry.RegisterForNavigation<AboutPageView, AboutPageViewModel>();
            containerRegistry.RegisterForNavigation<ClimateResultsView, ClimateResultsViewModel>();
            containerRegistry.RegisterForNavigation<SoilResultsView, SoilResultsViewModel>();

            // 
            //containerRegistry.RegisterSingleton<ResultsViewModelBase>();
            containerRegistry.RegisterSingleton<Storage>();

            // Providers
            containerRegistry.RegisterSingleton<GeographicDataProvider>();
            containerRegistry.RegisterSingleton<ExportHelpers>();
            containerRegistry.RegisterSingleton<ImportHelpers>();
            containerRegistry.RegisterSingleton<KmlHelpers>();

            // Dialogs
            containerRegistry.RegisterDialog<DeleteRowDialog, DeleteRowDialogViewModel>();
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

            regionManager.RegisterViewWithRegion(UiRegions.ToolbarRegion, typeof(ToolbarView));
            regionManager.RegisterViewWithRegion(UiRegions.SidebarRegion, typeof(SidebarView));
            regionManager.RegisterViewWithRegion(UiRegions.FooterRegion, typeof(FooterView));
            regionManager.RegisterViewWithRegion(UiRegions.ContentRegion, typeof(DisclaimerView));

            var geographicProvider = Container.Resolve<GeographicDataProvider>();
            geographicProvider.Initialize();
            Container.Resolve<KmlHelpers>();

        }
    }
}