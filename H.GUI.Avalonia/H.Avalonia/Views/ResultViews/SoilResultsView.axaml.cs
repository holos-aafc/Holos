using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using H.Avalonia.ViewModels.ResultViewModels;

namespace H.Avalonia.Views.ResultViews
{
    public partial class SoilResultsView : UserControl
    {
        private SoilResultsViewModel? ViewModel => DataContext as SoilResultsViewModel;
        
        /// <summary>
        /// The TopLevel act as the visual root, and is the base class for all top level controls, eg. Window.
        /// It handles scheduling layout, styling and rendering as well as keeping track of the client size.
        /// Most services are accessed through the TopLevel.
        /// </summary>
        /// <returns>The TopLevel of the visual root.</returns>
        /// <exception cref="NullReferenceException"></exception>
        private TopLevel GetTopLevel() => TopLevel.GetTopLevel(this) ?? throw new NullReferenceException("Invalid Owner");


        public SoilResultsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
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
                SuggestedFileName = Core.Properties.Resources.SoilDataExportDefaultName,
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