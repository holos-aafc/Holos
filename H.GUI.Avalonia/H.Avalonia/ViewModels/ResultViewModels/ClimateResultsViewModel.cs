using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using H.Avalonia.Core;
using H.Avalonia.Core.Models.ClassMaps;
using H.Avalonia.Core.Models.Results;
using H.Avalonia.Infrastructure;
using H.Core;
using H.Core.Providers.Climate;
using Prism.Commands;
using Prism.Regions;

namespace H.Avalonia.ViewModels.ResultViewModels
{
    /// <summary>
    /// A viewmodel for the climate results view.
    /// </summary>
    public class ClimateResultsViewModel : ResultsViewModelBase
    {
        private readonly IRegionManager _regionManager;
        private IRegionNavigationJournal? _navigationJournal;
        private readonly NasaClimateProvider _nasaClimateProvider;
        private readonly ExportHelpers _exportHelpers;
        private readonly ClimateResultsViewItemMap _climateResultsViewItemMap;
        private CancellationTokenSource _cancellationTokenSource;
        private const int DefaultErrorNotificationTime = 10;

        
        
        /// <summary>
        /// A collection of <see cref="ClimateResultsViewItems"/> that are attached to the climate results page. Each viewitem denotes a row in the grid.
        /// </summary>
        public ObservableCollection<ClimateResultsViewItem> ClimateResultsViewItems { get; set; } = new();
        

        public ClimateResultsViewModel() { }

        public ClimateResultsViewModel(IRegionManager regionManager, PrototypeStorage prototypeStorage, ExportHelpers exportHelpers) : base(regionManager, prototypeStorage)
        {
            _regionManager = regionManager;
            _exportHelpers = exportHelpers;
            _climateResultsViewItemMap = new ClimateResultsViewItemMap();
            GoBackCommand = new DelegateCommand(OnGoBack, CanGoBack);
            ExportToCsvCommand = new DelegateCommand<object>(OnExportToCSV);
            _nasaClimateProvider = new NasaClimateProvider();
        }

        /// <summary>
        /// Triggered when a user navigates to this page.
        /// </summary>
        /// <param name="navigationContext">The navigation context of the user. Contains the navigation tree and journal</param>
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            // When we navigate to this view, we instantiate the journal property. This allows us to do navigation through journaling.
            _navigationJournal = navigationContext.NavigationService.Journal;
            GoBackCommand.RaiseCanExecuteChanged();
            AddViewItemsAsync();
        }

        /// <summary>
        /// Triggered when the user navigates from this page to a different page.
        /// </summary>
        /// <param name="navigationContext"></param>
        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            ClimateResultsViewItems.Clear();
        }

        /// <summary>
        /// Asynchronously adds viewitems to the grid displayed in the page. Also creates a cancellation token that is
        /// used to cancel the task of adding view items. This task is cancelled when the user goes back from this page to the previous page.
        /// </summary>
        private async void AddViewItemsAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            try
            {
                await AddViewItemsToCollectionAsync(cancellationToken);
            }
            catch (TaskCanceledException e)
            {
                Trace.TraceInformation($@"{e.Message} and TaskCanceledException thrown in method 
                                            {nameof(AddViewItemsAsync)} in class {nameof(ClimateResultsViewModel)}");
                _cancellationTokenSource.Dispose();
            }
        }


        private async Task AddViewItemsToCollectionAsync(CancellationToken cancellationToken)
        {
            IsProcessingData = true;
            foreach (var viewItem in PrototypeStorage.ClimateViewItems)
            {
                for (var currentYear = viewItem.StartYear; currentYear <= viewItem.EndYear; currentYear++)
                {
                    var resultItem = new ClimateResultsViewItem
                    {
                        Year = currentYear,
                        Latitude = viewItem.Latitude,
                        Longitude = viewItem.Longitude,
                        TotalPET = await Task.Run(() => GetTotalPETAsync(currentYear, viewItem.Latitude, viewItem.Longitude), cancellationToken),
                        TotalPPT = await Task.Run(() => GetTotalPPTAsync(currentYear, viewItem.Latitude, viewItem.Longitude), cancellationToken),
                    };

                    if (viewItem.ExtractMonthlyData)
                    {
                        resultItem.MonthlyPPT = await Task.Run(() => GetMonthlyPPTAsync(currentYear, viewItem.JulianStartDay,
                            viewItem.JulianEndDay, viewItem.Latitude, viewItem.Longitude), cancellationToken);
                    }
                    ClimateResultsViewItems.Add(resultItem);
                }
            }
            IsProcessingData = false;
        }

        /// <summary>
        /// Called when the user goes back to the previous page.
        /// </summary>
        private void OnGoBack()
        {
            if (_navigationJournal != null && _navigationJournal.CanGoBack)
            {
                _cancellationTokenSource.Cancel();
                _navigationJournal.GoBack();
            }
        }

        /// <summary>
        /// Returns whether the user can go back to the previous page or not. Returns true if a navigationJournal exists, return false otherwise.
        /// </summary>
        /// <returns></returns>
        private bool CanGoBack()
        {
            if (_navigationJournal == null) return false;
            return _navigationJournal.CanGoBack;
        }

        /// <summary>
        /// Called when the user exports the current grid contents to a csv.
        /// </summary>
        /// <param name="obj">A <see cref="IStorageFile"/> object. Contains the path where the user wants to export the csv.</param>
        private void OnExportToCSV(object? obj)
        {
            if (obj is not IStorageFile file) return;
            try
            {
                _exportHelpers.ExportPath = file.Path.AbsolutePath;
                _exportHelpers.ExportToCSV(ClimateResultsViewItems, _climateResultsViewItemMap);
            }
            catch (IOException e)
            {
                NotificationManager?.Show(new Notification("File being used.",
                    e.Message,
                    type: NotificationType.Error,
                    expiration: TimeSpan.FromSeconds(DefaultErrorNotificationTime))
                );
            }
        }

        /// <summary>
        /// Asynchronously obtains the total PET value of the year by calling the NASA POWER Api.
        /// </summary>
        /// <param name="year">The year for which the PET value is required.</param>
        /// <param name="latitude">The latitude coordinate of the location for which the PET value is required.</param>
        /// <param name="longitude">The latitude coordinate of the location for which the PET value is required.</param>
        /// <returns>A value that equals the total PET of the given year.</returns>
        private async Task<double> GetTotalPETAsync(int year, double latitude, double longitude)
        {
            var result = 0.0;
            var calculation = Task.Run(() =>
            {
                result = _nasaClimateProvider.GetTotalPET(year, latitude, longitude);
            });
            await calculation;
            return result;
        }

        /// <summary>
        /// Asynchronously obtains the total PPT value of the year by calling the NASA POWER Api.
        /// </summary>
        /// <param name="year">The year for which the PPT value is required.</param>
        /// <param name="latitude">The latitude coordinate of the location for which the PPT value is required.</param>
        /// <param name="longitude">The latitude coordinate of the location for which the PPT value is required.</param>
        /// <returns>A value that equals the total PET of the given year.</returns>
        private async Task<double> GetTotalPPTAsync(int year, double latitude, double longitude)
        {
            var result = 0.0;
            var calculation = Task.Run(() =>
            {
                result = _nasaClimateProvider.GetTotalPPT(year, latitude, longitude);
            });
            await calculation;
            return result;
        }

        /// <summary>
        /// Asynchronously obtains the total monthly PPT value by calling the NASA POWER Api. The starting and ending date
        /// will indicate which month's data is extracted.
        /// </summary>
        /// <param name="year">The year for which the PPT value is required.</param>
        /// <param name="startingDay">The starting day in julian for monthly data extraction.</param>
        /// <param name="endingDay">The ending day in julian for monthly data extraction.</param>
        /// <param name="latitude">The latitude coordinate of the location for which the PPT value is required.</param>
        /// <param name="longitude">The latitude coordinate of the location for which the PPT value is required.</param>
        /// <returns></returns>
        private async Task<double> GetMonthlyPPTAsync(int year, int startingDay, int endingDay, double latitude,
            double longitude)
        {
            var result = 0.0;
            var monthlyPPT = Task.Run(() =>
            {
                result = _nasaClimateProvider.GetMonthlyPPT(year, startingDay, endingDay, latitude, longitude);
            });
            await monthlyPPT;
            return result;
        }
    }
}
