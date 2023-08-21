using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using H.Avalonia.Infrastructure;
using H.Avalonia.ViewModels;
using H.Core.Enumerations;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Extensions.Cache;
using Mapsui.Layers;
using Mapsui.Limiting;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Providers;
using Mapsui.Rendering;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.Tiling.Fetcher;
using Mapsui.Tiling.Layers;
using Mapsui.Tiling.Rendering;
using Mapsui.Widgets.Zoom;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Point = NetTopologySuite.Geometries.Point;

namespace H.Avalonia.Views
{
    public partial class SoilDataView : UserControl
    {

        private SoilDataViewModel? ViewModel => DataContext as SoilDataViewModel;

        private TopLevel GetTopLevel() => TopLevel.GetTopLevel(this) ?? throw new NullReferenceException("Invalid Owner");

        /// <summary>
        /// A GenericCollectionLayer that goes on top of the map and holds the points that are displayed to indicate location.
        /// </summary>
        private GenericCollectionLayer<List<IFeature>> _pointsLayer = new()
        {
            Style = SymbolStyles.CreatePinStyle()
        };

        /// <summary>
        /// A RasterizingTileLayer that goes on top of the map to display the polygons for a specific province.
        /// </summary>
        private RasterizingTileLayer? _polygonLayer;

        public SoilDataView()
        {
            InitializeComponent();
            InitializeMap();
        }

        /// <summary>
        /// Is used to attach the windows manager for displaying notifications.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            ViewModel.NotificationManager = new WindowNotificationManager(GetTopLevel());
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        /// <summary>
        /// Handles certain behaviour related to the map and how it is affected based on user interaction.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs? args)
        {
            switch (args?.PropertyName)
            {
                case nameof(ViewModel.NavigationPoint):
                    {
                        // Navigate to the specific point on the map based on longitude and lat values.
                        NavigateToPoint();

                        // Call method to add point to map
                        AddPointToMap();
                        break;
                    }
                case nameof(ViewModel.ShowPolygonsOnMap):
                    {
                        ViewModel.SelectedProvince = Province.SelectProvince;
                        break;
                    }
                case nameof(ViewModel.SelectedProvince):
                    {
                        if (ViewModel.ShowPolygonsOnMap)
                        {
                            if (_polygonLayer != null) SoilTabMap.Map.Layers.Remove(_polygonLayer);
                            if (ViewModel.SelectedProvince != Province.SelectProvince)
                            {
                                var sqlitePersistentCache = new SqlitePersistentCache("polygonsMap");
                                sqlitePersistentCache.Clear();
                                _polygonLayer = new RasterizingTileLayer(CreateLayer(ViewModel.SelectedProvince), persistentCache: sqlitePersistentCache, minTiles: 400, maxTiles: 800, renderFormat: RenderFormat.WebP);
                                SoilTabMap.Map.Layers.Add(_polygonLayer);
                            }
                        }
                        else
                        {
                            SoilTabMap.Map.Layers.Remove(_polygonLayer);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Initialize the map displayed on the <see cref="SoilDataView"/>'s single coordinate tab.
        /// </summary>
        private void InitializeMap()
        {
            SoilTabMap.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
            SoilTabMap.Map.Navigator.Limiter = new ViewportLimiterKeepWithinExtent();
            //SoilMap.Map.Navigator.OverridePanBounds = panBounds;
            SoilTabMap.Map.Widgets.Add(new ZoomInOutWidget
            {
                MarginX = 10,
                MarginY = 20,
                Size = 25,
                TextColor = Color.Black,
                BackColor = Color.White,
                Opacity = 1,
            });
        }

        /// <summary>
        /// Navigate to a specific point and zoom into that location.
        /// </summary>
        private void NavigateToPoint()
        {
            SoilTabMap.Map.Navigator.CenterOnAndZoomTo(ViewModel.NavigationPoint, resolution: 9);
        }

        /// <summary>
        /// Adds a point to the map displayed to the user.
        /// </summary>
        private void AddPointToMap()
        {
            // Add the points layer to our current map
            SoilTabMap.Map.Layers.Add(_pointsLayer);
            // Clear the features collection of the points layer so that any previous points are removed
            _pointsLayer?.Features.Clear();

            // Add a new point to the map as a GeometryFeature
            _pointsLayer?.Features.Add(new GeometryFeature
            {
                Geometry = new Point(ViewModel.NavigationPoint.X, ViewModel.NavigationPoint.Y)
            });
            // To notify the map that a redraw is needed.
            _pointsLayer?.DataHasChanged();
        }

        /// <summary>
        /// Handles behaviour related to user mouse clicks on the map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SoilMap_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            // Get the properties of the pointer event so that we can determine the type of click.
            var properties = e.GetCurrentPoint(this).Properties;
            if (!properties.IsRightButtonPressed) return;

            // Get the screen position and world position of the clicked point
            var screenPosition = e.GetPosition(SoilTabMap);
            var worldPosition = SoilTabMap.Map.Navigator.Viewport.ScreenToWorld(screenPosition.X, screenPosition.Y);

            // Update the navigation point based on the new world position
            ViewModel?.UpdateNavigationPointCommand.Execute(worldPosition);

            // Navigate to point and create marker for point
            NavigateToPoint();
            AddPointToMap();

            // Update the Address and Long/Lat values shown to user in the UI
            ViewModel?.UpdateInformationFromNavigationPointCommand.Execute();
        }

        /// <summary>
        /// Calls the appropriate command in the viewmodel when the user clicks the import data button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ImportDataButton_OnClick(object? sender, RoutedEventArgs e)
        {
            if (ViewModel is null) return;
            var storageProvider = GetTopLevel().StorageProvider;
            var item = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = Core.Properties.Resources.ImportDefaultName,
                AllowMultiple = false,
            });
            if (ViewModel.ImportFromCsvCommand.CanExecute(item))
            {
                ViewModel.ImportFromCsvCommand.Execute(item);
            }
        }

        /// <summary>
        /// Creates a layer for a polygon that will be placed on top of the map. We must have <see cref="ViewModel.WktPolygonMap"/> ready before using this method, otherwise an exception might be thrown.
        /// </summary>
        /// <param name="province"></param>
        /// <returns></returns>
        private ILayer CreateLayer(Province province)
        {
            var polygons = ViewModel.WktPolygonMap[province];
            return new Layer("Polygons")
            {
                DataSource = new MemoryProvider(polygons.ToFeatures()),
                Style = new VectorStyle
                {
                    Fill = new Brush(Color.Orange),
                    Opacity = 0.20f,
                    Outline = new Pen
                    {
                        Color = Color.Brown,
                        Width = 2,
                        PenStyle = PenStyle.Solid,
                        PenStrokeCap = PenStrokeCap.Round
                    }
                }
            };
        }
    }
}