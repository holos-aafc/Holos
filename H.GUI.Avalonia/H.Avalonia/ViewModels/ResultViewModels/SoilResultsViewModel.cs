using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using H.Avalonia.Infrastructure;
using H.Avalonia.Models;
using H.Avalonia.Models.ClassMaps;
using H.Avalonia.Models.Results;
using H.Core.Providers;
using Prism.Commands;
using Prism.Regions;

namespace H.Avalonia.ViewModels.ResultViewModels
{
    public class SoilResultsViewModel : ResultsViewModelBase
    {
        private readonly IRegionManager? _regionManager;
        private IRegionNavigationJournal? _navigationJournal;
        private readonly ExportHelpers _exportHelpers;
        private readonly KmlHelpers _kmlHelpers;
        private readonly GeographicDataProvider _geographicDataProvider;
        private readonly SoilResultsViewItemMap _soilResultsViewItemMap;
        private CancellationTokenSource _cancellationTokenSource;
        private const double DefaultErrorNotificationTime = 10;
        
        /// <summary>
        /// A collection of <see cref="SoilResultsViewItem"/> that are attached to the climate results page. Each viewitem denotes a row in the grid. This collection
        /// is populated using the coordinates entered in the multi-coordinate page.
        /// </summary>
        public ObservableCollection<SoilResultsViewItem> SoilResultsViewItems { get; set; } = new();

        /// <summary>
        /// A collection of <see cref="SoilResultsViewItem"/> that are attached to the climate results page. Each viewitem denotes a row in the grid. This collection
        /// is populated using the coordinates entered in the single-coordinate page.
        /// </summary>
        public ObservableCollection<SoilResultsViewItem> SingleSoilResultsViewItems { get; set; } = new();
        

        public SoilResultsViewModel(IRegionManager regionManager, PrototypeStorage prototypeStorage, ExportHelpers exportHelpers, KmlHelpers kmlHelpers, GeographicDataProvider geographicDataProvider) : base(regionManager, prototypeStorage)
        {
            _regionManager = regionManager;
            _exportHelpers = exportHelpers;
            _geographicDataProvider = geographicDataProvider;
            _kmlHelpers = kmlHelpers;
            GoBackCommand = new DelegateCommand(OnGoBack);
            ExportToCsvCommand = new DelegateCommand<object>(OnExportToCsv);
            _soilResultsViewItemMap = new SoilResultsViewItemMap();
        }

        /// <summary>
        /// Triggered when a user navigates to this page.
        /// </summary>
        /// <param name="navigationContext">The navigation context of the user. Contains the navigation tree and journal</param>
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
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
            SoilResultsViewItems.Clear();
            SingleSoilResultsViewItems.Clear();
        }

        /// <summary>
        /// Called when the user goes back to the previous page.
        /// </summary>
        private void OnGoBack()
        {
            if (_navigationJournal is not null && _navigationJournal.CanGoBack)
            {
                _cancellationTokenSource.Cancel();
                _navigationJournal.GoBack();
            }
        }

        /// <summary>
        /// Called when the user exports the current grid contents to a csv.
        /// </summary>
        /// <param name="obj">A <see cref="IStorageFile"/> object. Contains the path where the user wants to export the csv.</param>
        private void OnExportToCsv(object obj)
        {
            if (obj is not IStorageFile file) return;
            try
            {
                _exportHelpers.ExportPath = file.Path.AbsolutePath;
                if (PrototypeStorage.ShowSingleCoordinateResults)
                {
                    _exportHelpers.ExportToCSV(SingleSoilResultsViewItems, _soilResultsViewItemMap);
                }
                else
                {
                    _exportHelpers.ExportToCSV(SoilResultsViewItems, _soilResultsViewItemMap);
                }
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
        /// Asynchronously adds viewitems to the grid displayed in the page. Also creates a cancellation token that is
        /// used to cancel the task of adding view items. This task is cancelled when the user goes back from this page to the previous page.
        /// </summary>
        private async void AddViewItemsAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            try
            {
                if (PrototypeStorage.ShowSingleCoordinateResults)
                {
                    var sourceCollection = new ObservableCollection<SoilViewItem>
                {
                    PrototypeStorage.SingleSoilViewItem
                };
                    await AddViewItemsToCollection(cancellationToken, sourceCollection, SingleSoilResultsViewItems);
                }
                else
                {
                    await AddViewItemsToCollection(cancellationToken, PrototypeStorage.SoilViewItems, SoilResultsViewItems);
                }
            }
            catch (TaskCanceledException e)
            {
                Trace.TraceInformation($@"{e.Message} and TaskCanceledException thrown in method 
                                            {nameof(AddViewItemsAsync)} in class {nameof(SoilResultsViewModel)}");
                _cancellationTokenSource.Dispose();
            }
        }

        private async Task AddViewItemsToCollection(CancellationToken cancellationToken,
                                                    ObservableCollection<SoilViewItem> sourceCollection,
                                                    ObservableCollection<SoilResultsViewItem> resultsCollection)
        {
            IsProcessingData = true;

            foreach (var viewItem in sourceCollection)
            {
                var polygon = await Task.Run(() => _kmlHelpers.GetPolygonFromCoordinateAsync(viewItem.Latitude, viewItem.Longitude), cancellationToken);
                var polygonData = _geographicDataProvider.GetGeographicalData(polygon);
                var soilData = polygonData.SoilDataForAllComponentsWithinPolygon;
                foreach (var soil in soilData)
                {
                    resultsCollection.Add(new SoilResultsViewItem
                    {
                        Latitude = viewItem.Latitude,
                        Longitude = viewItem.Longitude,
                        SoilGreatGroup = soil.SoilGreatGroup,
                        SoilTexture = soil.SoilTexture,
                        Province = soil.Province,
                        SoilPh = soil.SoilPh,
                        PercentClayInSoil = soil.ProportionOfClayInSoilAsPercentage,
                        PercentOrganicMatterInSoil = soil.ProportionOfSoilOrganicCarbon,
                    });
                }
            }

            IsProcessingData = false;
        }
    }
}