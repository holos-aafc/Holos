using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using H.Avalonia.ViewModels.Results;
using System;

namespace H.Avalonia.Views.Results
{
    public partial class ClimateResultsView : UserControl
    {
        public ClimateResultsView()
        {
            InitializeComponent();
        }
        TopLevel GetTopLevel() => TopLevel.GetTopLevel(this) ?? throw new NullReferenceException("Invalid Owner");


        private ClimateResultsViewModel? ViewModel
        {
            get => DataContext as ClimateResultsViewModel;
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            ViewModel.NotificationManager = new WindowNotificationManager(GetTopLevel());
        }

        private void GoBackPanel_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (sender is not StackPanel || ViewModel is null) return;
            if (ViewModel.GoBackCommand.CanExecute())
            {
                ViewModel.GoBackCommand.Execute();
            }
        }

        private async void ExportDataButton_OnClick(object? sender, RoutedEventArgs e)
        {
            if (ViewModel is null) return;
            var storageProvider = GetTopLevel().StorageProvider;
            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                Title = Core.Properties.Resources.ExportDefaultName,
                SuggestedFileName = Core.Properties.Resources.ClimateDataExportDefaultName,
                DefaultExtension = "csv",
                ShowOverwritePrompt = true,
            });
            if (file is not null && ViewModel.ExportToCsvCommand.CanExecute(file))
            {
                ViewModel.ExportToCsvCommand.Execute(file);
            }
        }
    }
}