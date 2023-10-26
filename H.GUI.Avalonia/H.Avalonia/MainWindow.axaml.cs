using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using H.Avalonia.ViewModels;

namespace H.Avalonia
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        
        /// <summary>
        /// The TopLevel act as the visual root, and is the base class for all top level controls, eg. Window.
        /// It handles scheduling layout, styling and rendering as well as keeping track of the client size.
        /// Most services are accessed through the TopLevel.
        /// </summary>
        /// <returns>The TopLevel of the visual root.</returns>
        /// <exception cref="NullReferenceException"></exception>
        TopLevel GetTopLevel() => TopLevel.GetTopLevel(this) ?? throw new NullReferenceException("Invalid Owner");

        public MainWindow()
        {
        }

        public MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            InitializeComponent();
            Loaded += OnLoaded;
            Closing += OnClosing;
        }
        
        private void OnLoaded(object? sender, RoutedEventArgs e)
        {
            _mainWindowViewModel.NotificationManager = new WindowNotificationManager(GetTopLevel());
        }
        
        private void OnClosing(object? sender, CancelEventArgs e)
        {
            if (_mainWindowViewModel is null) return;
            // If data has been saved already on a previous quit event, we do an early return and close the window.
            if (_mainWindowViewModel.HasDataSaveCompletedOnQuit) return;

            // Otherwise we cancel the close window event, show the saving data window and save user data.
            e.Cancel = true;

            // If the user clicked on the close window button already, do an early return as we don't want to start
            // another save task.
            if (_mainWindowViewModel.UserClosedWindow) return;
            
            _mainWindowViewModel.ClosingCommand.Execute();
            // Disable clicks on main window so that user does not change settings while application is closing and saving data.
            IsHitTestVisible = false;
            _mainWindowViewModel.UserClosedWindow = true;
        }
    }
}