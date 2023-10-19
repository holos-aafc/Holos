using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using H.Avalonia.Models;
using H.Avalonia.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace H.Avalonia.Views
{
    public partial class ClimateDataView : UserControl
    {
        private ObservableCollection<ClimateViewItem>? ClimateDataGridItems =>
            ClimateDataGrid.ItemsSource as ObservableCollection<ClimateViewItem>;
        
        /// <summary>
        /// The TopLevel act as the visual root, and is the base class for all top level controls, eg. Window.
        /// It handles scheduling layout, styling and rendering as well as keeping track of the client size.
        /// Most services are accessed through the TopLevel.
        /// </summary>
        /// <returns>The TopLevel of the visual root.</returns>
        /// <exception cref="NullReferenceException"></exception>
        TopLevel GetTopLevel() => TopLevel.GetTopLevel(this) ?? throw new NullReferenceException("Invalid Owner");

        /// <summary>
        /// Get the viewmodel associated with the view.
        /// </summary>
        private ClimateDataViewModel? ViewModel => DataContext as ClimateDataViewModel;

        public ClimateDataView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Is used to attach the windows manager for displaying notifications.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            ViewModel.NotificationManager = new WindowNotificationManager(GetTopLevel());
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
    }
}