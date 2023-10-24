using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using CsvHelper;
using CsvHelper.TypeConversion;
using H.Avalonia.Infrastructure;
using H.Avalonia.Views;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using H.Avalonia.Core;
using H.Avalonia.Core.Models;
using H.Avalonia.Core.Models.ClassMaps;
using H.Avalonia.Dialogs;
using H.Avalonia.Views.ResultViews;
using ClimateResultsView = H.Avalonia.Views.ResultViews.ClimateResultsView;

namespace H.Avalonia.ViewModels
{
    public class ClimateDataViewModel : ViewModelBase, IDataGridFeatures

    {
        private readonly IRegionManager _regionManager;
        private IRegionNavigationJournal? _navigationJournal;
        private readonly IDialogService _dialogService;
        private readonly ImportHelpers _importHelper;
        private readonly ClimateViewItemMap _climateViewItemMap;
        private const int DefaultNotificationTime = 10;

        /// <summary>
        /// Allows navigation from the current view to the <see cref="SoilResultsView"/>.
        /// </summary>
        public DelegateCommand NavigateToResultsView { get; set; }

        /// <summary>
        /// A command that adds rows to the grid displayed on <see cref="ClimateDataView"/>. Each row indicates <see cref="ClimateViewItem"/>.
        /// </summary>
        public DelegateCommand AddRowCommand { get; set; }

        /// <summary>
        /// A command that removes rows to the grid displayed on <see cref="ClimateDataView"/>. Each row indicates <see cref="ClimateViewItem"/>.
        /// </summary>
        public DelegateCommand<object> DeleteRowCommand { get; set; }

        /// <summary>
        /// Deletes a row that is currently marked as selected by the user.
        /// </summary>
        public DelegateCommand DeleteSelectedRowsCommand { get; set; }

        /// <summary>
        /// Import climate data from a csv file. The csv file must have the following columns:
        /// Latitude, Longitude, Start Year, End Year, Julian day start, Julian day end (respectively).
        /// </summary>
        public DelegateCommand<object> ImportFromCsvCommand { get; set; }

        /// <summary>
        /// Toggles the select all row command. This command either selects or deselects all the rows currently displayed in the grid.
        /// </summary>
        public DelegateCommand ToggleSelectAllRowsCommand { get; set; }

        /// <summary>
        /// A bool that indicates if all climate data items are selected in the grid. Returns true if all items are selected, returns false otherwise.
        /// </summary>
        public bool AllViewItemsSelected { get; set; }

        /// <summary>
        /// A bool that indicates if the grid has any climate view items currently added to it. Returns true if Any view items exist, returns false otherwise.
        /// </summary>
        public bool HasViewItems => PrototypeStorage?.ClimateViewItems != null && PrototypeStorage.ClimateViewItems.Any();

        /// <summary>
        /// A bool that indicates if any climate view items are selected or not. Returns true if at least one view item is selected, returns false if none are selected.
        /// </summary>
        public bool AnyViewItemsSelected => PrototypeStorage?.ClimateViewItems != null &&
                                                   PrototypeStorage.ClimateViewItems.Any(item => item.IsSelected);

        public ClimateDataViewModel()
        {
        }

        public ClimateDataViewModel(IRegionManager regionManager, PrototypeStorage prototypeStorage, ImportHelpers importHelper,
            IDialogService dialogService) : base(regionManager, prototypeStorage)
        {
            _regionManager = regionManager;
            _importHelper = importHelper;
            _dialogService = dialogService;
            InitializeCommands();
            _climateViewItemMap = new ClimateViewItemMap();
        }
        
        /// <summary>
        /// Initializes the various commands used by the related view.
        /// </summary>
        private void InitializeCommands()
        {
            NavigateToResultsView = new DelegateCommand(SwitchToResultsView).ObservesCanExecute(() => HasViewItems);
            AddRowCommand = new DelegateCommand(OnAddRow);
            ImportFromCsvCommand = new DelegateCommand<object>(OnImportCsv);
            DeleteRowCommand = new DelegateCommand<object>(OnDeleteRow);
            DeleteSelectedRowsCommand =
                new DelegateCommand(OnDeleteSelectedRows).ObservesCanExecute(() => AnyViewItemsSelected);
            ToggleSelectAllRowsCommand =
                new DelegateCommand(OnToggleSelectAllRows).ObservesCanExecute(() => HasViewItems);
        }

        /// <summary>
        /// Triggered when the <see cref="PrototypeStorage.ClimateViewItems"/> changes. This method raises CanExecuteChanged events for the various
        /// buttons on the page and also attaches/detaches PropertyChanged events to individual properties inside the collection so that
        /// we can be notified when an internal property changes in the collection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClimateViewItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ToggleSelectAllRowsCommand.RaiseCanExecuteChanged();
            DeleteSelectedRowsCommand.RaiseCanExecuteChanged();
            NavigateToResultsView.RaiseCanExecuteChanged();
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    if (item != null)
                        item.PropertyChanged += CollectionItemOnPropertyChanged;
                }

                AllViewItemsSelected = false;
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
        /// A property changed event that is attached to each property of the <see cref="PrototypeStorage.ClimateViewItems"/> collection.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event that was triggered.</param>
        private void CollectionItemOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ClimateViewItem.IsSelected))
            {
                DeleteSelectedRowsCommand.RaiseCanExecuteChanged();

                if (sender is not ClimateViewItem viewItem) return;
                if (!viewItem.IsSelected)
                {
                    AllViewItemsSelected = false;
                }
            }
        }

        /// <summary>
        /// Triggered when a user navigates to this page.
        /// </summary>
        /// <param name="navigationContext">The navigation context of the user. Contains the navigation tree and journal</param>
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            // When we navigate to this view, we instantiate the journal property. This allows us to do navigation through journaling.
            _navigationJournal = navigationContext.NavigationService.Journal;
            if (PrototypeStorage?.ClimateViewItems != null)
            {
                PrototypeStorage.ClimateViewItems.CollectionChanged += OnClimateViewItemsCollectionChanged;
            }
        }

        /// <summary>
        /// Uses Prism framework to switch from <see cref="ClimateDataView"/> to <see cref="ClimateResultsView"/>.
        /// </summary>
        private void SwitchToResultsView()
        {
            _regionManager.RequestNavigate(UiRegions.ContentRegion, nameof(ClimateResultsView));
        }

        /// <summary>
        /// Add a row to the grid on  the <see cref="ClimateDataView"/>
        /// </summary>
        private void OnAddRow()
        {
            PrototypeStorage?.ClimateViewItems?.Add(new ClimateViewItem());
        }

        /// <summary>
        /// Deletes a row from the grid on <see cref="ClimateDataView"/>
        /// </summary>
        /// <param name="obj">The <see cref="ClimateViewItem"/> that needs to be deleted.</param>
        private void OnDeleteRow(object obj)
        {
            if (obj is not ClimateViewItem viewItem) return;

            var message = Core.Properties.Resources.RowDeleteMessage;
            _dialogService.ShowMessageDialog(nameof(DeleteRowDialog), message, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    PrototypeStorage?.ClimateViewItems?.Remove(viewItem);
                }
            });
        }

        /// <summary>
        /// Deletes a group of <see cref="ClimateViewItem"/> that are selected by the user.
        /// </summary>
        private void OnDeleteSelectedRows()
        {
            if (!PrototypeStorage.ClimateViewItems.Any()) return;
            var message = Core.Properties.Resources.RowDeleteMessage;
            _dialogService.ShowMessageDialog(nameof(DeleteRowDialog), message, r =>
            {
                if (r.Result != ButtonResult.OK) return;
                var currentItems = PrototypeStorage.ClimateViewItems.ToList();
                foreach (var item in currentItems.Where(item => item.IsSelected))
                {
                    PrototypeStorage?.ClimateViewItems?.Remove(item);
                }

                if (!HasViewItems)
                {
                    AllViewItemsSelected = false;
                }
            });
        }

        /// <summary>
        /// Called when the user imports a csv file. The imported csv file must have the following column headers:
        /// Latitude, Longitude, Start Year, End Year, Julian day start, Julian day end (respectively).
        /// </summary>
        /// <param name="obj">The <see cref="IStorageItem"/> object passed to the method containing the file path where the csv is located.</param>
        private void OnImportCsv(object? obj)
        {
            var item = obj as IReadOnlyCollection<IStorageItem>;
            var file = item?.FirstOrDefault();

            if (file == null) return;

            _importHelper.ImportPath = file.Path.AbsolutePath;
            try
            {
                PrototypeStorage?.ClimateViewItems.AddRange(_importHelper.ImportFromCsv(_climateViewItemMap));

            }
            catch (HeaderValidationException e)
            {
                NotificationManager?.Show(new Notification("Invalid Header",
                    e.Message,
                    type: NotificationType.Error,
                    expiration: TimeSpan.FromSeconds(DefaultNotificationTime))
                );
            }
            catch (TypeConverterException e)
            {
                NotificationManager?.Show(new Notification("Invalid CSV Content",
                    e.Message,
                    type: NotificationType.Error,
                    expiration: TimeSpan.FromSeconds(DefaultNotificationTime))
                );
            }
            catch (IOException e)
            {
                NotificationManager?.Show(new Notification("File being used.",
                    e.Message,
                    type: NotificationType.Error,
                    expiration: TimeSpan.FromSeconds(DefaultNotificationTime))
                );
            }
        }

        /// <summary>
        /// Helps select all rows that are currently added to the grid.
        /// </summary>
        private void OnToggleSelectAllRows()
        {
            if (PrototypeStorage?.ClimateViewItems == null) return;
            if (AllViewItemsSelected)
            {
                foreach (var item in PrototypeStorage.ClimateViewItems)
                {
                    item.IsSelected = false;
                }

                AllViewItemsSelected = false;
            }
            else
            {
                foreach (var item in PrototypeStorage.ClimateViewItems)
                {
                    item.IsSelected = true;
                }

                AllViewItemsSelected = true;
            }
        }
    }
}
