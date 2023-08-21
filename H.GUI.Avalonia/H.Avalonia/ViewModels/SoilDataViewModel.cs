using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using H.Avalonia.Infrastructure;
using H.Avalonia.Infrastructure.Dialogs;
using H.Avalonia.Models;
using H.Avalonia.Models.ClassMaps;
using H.Avalonia.Views;
using H.Avalonia.Views.Results;
using H.Core.Enumerations;
using Mapsui;
using Mapsui.Extensions;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using SharpKml.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Avalonia.ViewModels
{
    public class SoilDataViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;
        private IRegionNavigationJournal? _navigationJournal;
        private readonly MapHelpers _mapHelpers;
        private readonly IDialogService _dialogService;
        private double _longitude;
        private double _latitude;
        private string _address = string.Empty;
        private MPoint _navigationPoint;
        private ImportHelpers _importHelper;
        private SoilViewItemMap _soilViewItemMap;
        private const int DefaultErrorNotificationTime = 10;
        private const int DefaultInformationNotificationTime = 5;

        private bool _hasSoilViewItems => Storage?.SoilViewItems != null && Storage.SoilViewItems.Any();

        private bool _anySoilViewItemsSelected => Storage?.SoilViewItems != null &&
                                                  Storage.SoilViewItems.Any(item => item.IsSelected);
        private bool _allSoilViewItemsSelected;

        public readonly KmlHelpers KmlHelpers;

        public readonly Dictionary<Province, List<Polygon>> WktPolygonMap = new();
        private bool _isDataProcessing;
        private bool _showPolygonsOnMap;
        private Province _selectedProvince = Province.SelectProvince;

        /// <summary>
        /// The longitude value of a coordinate
        /// </summary>
        public double Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        /// <summary>
        /// The latitude value of a coordinate
        /// </summary>
        public double Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        /// <summary>
        /// An address field that signifies a location.
        /// </summary>
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        /// <summary>
        /// A point that the user wants to navigate to. This point is what the user selects when they specify a location in the <see cref="SoilDataView"/>
        /// </summary>
        public MPoint NavigationPoint
        {
            get => _navigationPoint;
            set => SetProperty(ref _navigationPoint, value);
        }

        /// <summary>
        /// A bool that indicates if data is still processing in the background. If data is still loading/processing, then return true. Returns false if all
        /// data is loaded and ready to use.
        /// </summary>
        public bool IsDataProcessing
        {
            get => _isDataProcessing;
            set => SetProperty(ref _isDataProcessing, value);
        }

        /// <summary>
        /// A bool that indicates whether to show SLC polygons on the map displayed to the user.
        /// </summary>
        public bool ShowPolygonsOnMap
        {
            get => _showPolygonsOnMap;
            set => SetProperty(ref _showPolygonsOnMap, value);
        }

        /// <summary>
        /// A collection of provinces for which SLC polygon data is available.
        /// </summary>
        public ObservableCollection<Province> Provinces { get; set; } = new(Enum.GetValues(typeof(Province))
            .Cast<Province>()
            .Except(new[] { Province.NorthwestTerritories, Province.Nunavut, Province.Yukon }));

        public SoilDataViewModel() { }

        /// <summary>
        /// A constructor that uses dependency injection to pass various objects into the class.
        /// </summary>
        /// <param name="regionManager">The region manager object controls the navigation of our view.</param>
        /// <param name="storage">The storage object contains various items that are passed between different viewmodels</param>
        /// <param name="importHelper">A set of methods that help with importing data from an external file.</param>
        /// <param name="kmlHelpers">A set of methods that help us process .kml files.</param>
        /// <param name="dialogService">An Prism dialogService object that helps us display dialogs to the user.</param>
        public SoilDataViewModel(IRegionManager regionManager, Storage storage, ImportHelpers importHelper, KmlHelpers kmlHelpers, IDialogService dialogService) : base(regionManager, storage)
        {
            _regionManager = regionManager;
            _mapHelpers = new MapHelpers();
            _importHelper = importHelper;
            _dialogService = dialogService;
            _soilViewItemMap = new SoilViewItemMap();
            KmlHelpers = kmlHelpers;
            SwitchToResultsViewFromSingleCoordinateCommand = new DelegateCommand(SwitchToSoilResultsViewFromSingleCoordinate);
            SwitchToResultsViewFromMultiCoordinateCommand =
                new DelegateCommand(SwitchToSoilResultsViewFromMultiCoordinate).ObservesCanExecute(() => _hasSoilViewItems);
            ImportFromCsvCommand = new DelegateCommand<object>(OnImportCsv);
            ToggleSelectAllRowsCommand = new DelegateCommand(OnToggleSelectAllRows).ObservesCanExecute(() => _hasSoilViewItems);
            AddRowCommand = new DelegateCommand(OnAddRow);
            DeleteRowCommand = new DelegateCommand<object>(OnDeleteRow);
            DeleteSelectedRowsCommand = new DelegateCommand(OnDeleteSelectedRows).ObservesCanExecute(() => _anySoilViewItemsSelected);
            GetCoordinatesFromAddressCommand = new DelegateCommand(OnGetCoordinates);
            GetAddressFromCoordinateCommand = new DelegateCommand(OnGetAddress);
            UpdateNavigationPointCommand = new DelegateCommand<MPoint>(OnUpdateNavigationPoint);
            UpdateInformationFromNavigationPointCommand = new DelegateCommand(OnUpdateInformationFromNavPoint);
            CreateWktPolygons();
        }

        /// <summary>
        /// Triggered when a user navigates to this page.
        /// </summary>
        /// <param name="navigationContext">The navigation context of the user. Contains the navigation tree and journal</param>
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            // When we navigate to this view, we instantiate the journal property. This allows us to do navigation through journaling.
            _navigationJournal = navigationContext.NavigationService.Journal;
            if (Storage?.SoilViewItems != null)
            {
                Storage.SoilViewItems.CollectionChanged += OnSoilViewItemsCollectionChanged;
            }
        }

        /// <summary>
        /// The province currently selected by the user.
        /// </summary>
        public Province SelectedProvince
        {
            get => _selectedProvince;
            set => SetProperty(ref _selectedProvince, value);
        }

        /// <summary>
        /// Command switches the current <see cref="SoilDataView"/> from the single coordinate tab to the results section.
        /// </summary>
        public DelegateCommand SwitchToResultsViewFromSingleCoordinateCommand { get; set; }

        /// <summary>
        /// Command switches the current <see cref="SoilDataView"/> from the multiple coordinate tab to the results section.
        /// </summary>
        public DelegateCommand SwitchToResultsViewFromMultiCoordinateCommand { get; set; }

        /// <summary>
        /// Selects or deselects all rows added to the grid by the user.
        /// </summary>
        public DelegateCommand ToggleSelectAllRowsCommand { get; set; }

        /// <summary>
        /// Adds a row to the grid displayed to the user.
        /// </summary>
        public DelegateCommand AddRowCommand { get; set; }

        /// <summary>
        /// Triggered by the user when they click the delete icon next to a row. Deletes that specific row only.
        /// </summary>
        public DelegateCommand<object> DeleteRowCommand { get; set; }

        /// <summary>
        /// Imports inputs from a csv file for which soil data is required. This csv file must have the following headers:
        /// Longitude, Latitude
        /// </summary>
        public DelegateCommand<object> ImportFromCsvCommand { get; }

        /// <summary>
        /// Deletes a selection of rows that are marked as selected by the user.
        /// </summary>
        public DelegateCommand DeleteSelectedRowsCommand { get; set; }

        /// <summary>
        /// Get the coordinates of a location based on the address specified by the user.
        /// </summary>
        public DelegateCommand GetCoordinatesFromAddressCommand { get; set; }

        /// <summary>
        /// Get the address of a location based on the coordinates specified by the user.
        /// </summary>
        public DelegateCommand GetAddressFromCoordinateCommand { get; set; }

        /// <summary>
        /// Updates the point that the user wants to navigate to when the user clicks a specific area on the world map.
        /// </summary>
        public DelegateCommand<MPoint> UpdateNavigationPointCommand { get; }

        /// <summary>
        /// Updates the <see cref="Latitude"/>, <see cref="Longitude"/> and <see cref="Address"/> fields when the user selects
        /// a new navigation point.
        /// </summary>
        public DelegateCommand UpdateInformationFromNavigationPointCommand { get; }

        /// <summary>
        /// Handles displaying of notifications to the user.
        /// </summary>
        public WindowNotificationManager NotificationManager { get; set; }

        /// <summary>
        /// Triggered when the <see cref="Storage.SoilViewItems"/> changes. This method raises CanExecuteChanged events for the various
        /// buttons on the page and also attaches/detaches PropertyChanged events to individual properties inside the collection so that
        /// we can be notified when an internal property changes in the collection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSoilViewItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ToggleSelectAllRowsCommand.RaiseCanExecuteChanged();
            DeleteSelectedRowsCommand.RaiseCanExecuteChanged();
            SwitchToResultsViewFromMultiCoordinateCommand.RaiseCanExecuteChanged();
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    if (item != null)
                        item.PropertyChanged += CollectionItemOnPropertyChanged;
                }
                _allSoilViewItemsSelected = false;
            }

            if (e.OldItems == null) return;
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    if (item != null)
                        item.PropertyChanged -= CollectionItemOnPropertyChanged;
                }
            }
        }

        /// <summary>
        /// A property changed event that is attached to each property of the <see cref="Storage.ClimateViewItems"/> collection.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event that was triggered.</param>
        private void CollectionItemOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SoilViewItem.IsSelected))
            {
                DeleteSelectedRowsCommand.RaiseCanExecuteChanged();

                if (sender is not SoilViewItem viewItem) return;
                if (!viewItem.IsSelected)
                {
                    _allSoilViewItemsSelected = false;
                }
            }
        }

        /// <summary>
        /// Helps select all rows that are currently added to the grid.
        /// </summary>
        private void OnToggleSelectAllRows()
        {
            if (Storage?.SoilViewItems == null) return;
            if (_allSoilViewItemsSelected)
            {
                foreach (var item in Storage.SoilViewItems)
                {
                    item.IsSelected = false;
                }
                _allSoilViewItemsSelected = false;
            }
            else
            {
                foreach (var item in Storage.SoilViewItems)
                {
                    item.IsSelected = true;
                }
                _allSoilViewItemsSelected = true;
            }
        }

        /// <summary>
        /// Adds a row to the grid.
        /// </summary>
        private void OnAddRow()
        {
            Storage?.SoilViewItems?.Add(new SoilViewItem());
        }

        /// <summary>
        /// Deletes a row from a grid.
        /// </summary>
        /// <param name="obj"></param>
        private void OnDeleteRow(object obj)
        {
            if (obj is not SoilViewItem viewItem) return;
            var message = Core.Properties.Resources.RowDeleteMessage;
            _dialogService.ShowMessageDialog(nameof(DeleteRowDialog), message, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    Storage?.SoilViewItems?.Remove(viewItem);
                }
            });
        }

        /// <summary>
        /// Called when the user imports a csv file. The imported csv file must have the following column headers:
        /// Latitude, Longitude (respectively).
        /// </summary>
        /// <param name="obj">The <see cref="IStorageItem"/> object passed to the method containing the file path where the csv is located.</param>
        private void OnImportCsv(object obj)
        {
            var item = obj as IReadOnlyCollection<IStorageItem>;
            var file = item?.FirstOrDefault();

            if (file == null) return;

            _importHelper.ImportPath = file.Path.AbsolutePath;
            try
            {
                Storage?.SoilViewItems.AddRange(_importHelper.ImportFromCsv(_soilViewItemMap));

            }
            catch (HeaderValidationException e)
            {
                NotificationManager?.Show(new Notification("Invalid Header",
                    e.Message,
                    type: NotificationType.Error,
                    expiration: TimeSpan.FromSeconds(DefaultErrorNotificationTime))
                );
            }
            catch (TypeConverterException e)
            {
                NotificationManager?.Show(new Notification("Invalid CSV Content",
                    e.Message,
                    type: NotificationType.Error,
                    expiration: TimeSpan.FromSeconds(DefaultErrorNotificationTime))
                );
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
        /// Deletes the rows marked as selected by the user
        /// </summary>
        private void OnDeleteSelectedRows()
        {
            if (!Storage.SoilViewItems.Any()) return;
            var message = Core.Properties.Resources.RowDeleteMessage;
            _dialogService.ShowMessageDialog(nameof(DeleteRowDialog), message, r =>
            {
                if (r.Result != ButtonResult.OK) return;
                var currentItems = Storage.SoilViewItems.ToList();
                foreach (var item in currentItems.Where(item => item.IsSelected))
                {
                    Storage?.SoilViewItems?.Remove(item);
                }

                if (!_hasSoilViewItems)
                {
                    _allSoilViewItemsSelected = false;
                }
            });
        }

        /// <summary>
        /// Switch to <see cref="SoilResultsView"/> from the current single page coordinate tab.
        /// </summary>
        private void SwitchToSoilResultsViewFromSingleCoordinate()
        {
            Storage.SingleSoilViewItem.Latitude = Latitude;
            Storage.SingleSoilViewItem.Longitude = Longitude;
            Storage.ShowSingleCoordinateResults = true;
            Storage.ShowMultipleCoordinateResults = false;
            _regionManager.RequestNavigate(UiRegions.ContentRegion, nameof(SoilResultsView));
        }

        /// <summary>
        /// Switch to <see cref="SoilResultsView"/> from the current multiple page coordinate tab.
        /// </summary>
        private void SwitchToSoilResultsViewFromMultiCoordinate()
        {
            Storage.ShowMultipleCoordinateResults = true;
            Storage.ShowSingleCoordinateResults = false;
            _regionManager.RequestNavigate(UiRegions.ContentRegion, nameof(SoilResultsView));
        }

        /// <summary>
        /// Gets the new navigation point based on the update latitude and longitude values.
        /// </summary>
        /// <returns></returns>
        private MPoint GetNavigationPoint()
        {
            var coordinate = _mapHelpers.ConvertLatLongtToSphericalMercator(Latitude, Longitude);
            return (coordinate.x, coordinate.y).ToMPoint();
        }

        /// <summary>
        /// Gets the new coordinate values based on the address provided by the user.
        /// </summary>
        private async void OnGetCoordinates()
        {
            if (string.IsNullOrEmpty(Address))
            {
                Trace.TraceInformation($@"Cannot find location as an empty address was entered.");
                NotificationManager?.Show(new Notification("Address field empty",
                    Core.Properties.Resources.MessageEmptyAddress,
                    type: NotificationType.Information,
                    expiration: TimeSpan.FromSeconds(DefaultInformationNotificationTime))
                );
                return;
            }
            try
            {
                var point = await _mapHelpers.GetLocationFromAddressAsync(Address);
                Latitude = point.latitude;
                Longitude = point.longitude;
                NavigationPoint = GetNavigationPoint();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Trace.TraceInformation($@"{e.Message}. Exception thrown in {nameof(OnGetCoordinates)} by class {nameof(SoilDataViewModel)}");
                NotificationManager?.Show(new Notification("Invalid Address",
                    Core.Properties.Resources.MessageIncorrectAddress,
                    type: NotificationType.Error,
                    expiration: TimeSpan.FromSeconds(DefaultErrorNotificationTime))
                );
            }
        }

        /// <summary>
        /// Gets the new address values based on the coordinates provided by the user.
        /// </summary>
        private async void OnGetAddress()
        {
            var address = await _mapHelpers.GetAddressFromLocationAsync(Latitude, Longitude);
            if (string.IsNullOrEmpty(address))
            {
                Trace.TraceInformation($@"Cannot find the coordinate. Please enter correct latitude and longitude values");
                NotificationManager?.Show(new Notification("Incorrect Coordinate",
                    Core.Properties.Resources.MessageInValidCoordinateEntered,
                    type: NotificationType.Information,
                    expiration: TimeSpan.FromSeconds(DefaultInformationNotificationTime))
                );
                return;
            }

            Address = address;
            NavigationPoint = GetNavigationPoint();
        }

        /// <summary>
        /// Updates the navigation point based on user choice.
        /// </summary>
        /// <param name="point">The new point that is to be set as the navigation point.</param>
        private void OnUpdateNavigationPoint(MPoint? point)
        {
            if (point is null) return;

            NavigationPoint = point;
        }

        /// <summary>
        /// Updates various information displayed to the user when the navigation point updates.
        /// </summary>
        private async void OnUpdateInformationFromNavPoint()
        {
            var point = _mapHelpers.ConvertSphericalMercatorToCoordinate(NavigationPoint);
            Latitude = point.latitude;
            Longitude = point.longitude;

            var address = await _mapHelpers.GetAddressFromLocationAsync(Latitude, Longitude);
            if (string.IsNullOrEmpty(address))
            {
                Trace.TraceInformation($@"Incorrect coordinate location cannot find matching address.");
                NotificationManager?.Show(new Notification("Cannot find address",
                    Core.Properties.Resources.MessageIncorrectLocationSelected,
                    type: NotificationType.Information,
                    expiration: TimeSpan.FromSeconds(DefaultInformationNotificationTime))
                );
                return;
            }
            Address = address;

        }

        /// <summary>
        /// Calls the <see cref="CreateWktPolygonsAsync"/> method to create a WKT (Well-known-text) representation of all the SLC polygons.
        /// </summary>
        private async void CreateWktPolygons()
        {
            IsDataProcessing = true;
            if (KmlHelpers.LoadPolygonsAsync != null) await KmlHelpers.LoadPolygonsAsync;
            await CreateWktPolygonsAsync();
            IsDataProcessing = false;
        }

        /// <summary>
        /// Creates a WKT (Well-known-text) representation of all the SLC polygons in the province KML files.
        /// </summary>
        private async Task CreateWktPolygonsAsync()
        {
            await Task.Run(() =>
            {
                foreach (var (province, polygons) in KmlHelpers.PolygonMap)
                {
                    var result = new List<Polygon>();
                    foreach (var polygonItem in polygons)
                    {
                        var wktPolygon = polygonItem.sharpKmlPolygon.ToWkt();
                        var polygon = (Polygon)new WKTReader().Read(wktPolygon);
                        result.Add(polygon);
                    }
                    WktPolygonMap.TryAdd(province, result);
                }
            });
        }
    }
}
