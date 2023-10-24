using System.ComponentModel;
using Avalonia.Controls;
using H.Avalonia.ViewModels;

namespace H.Avalonia
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;

        public MainWindow()
        {
            
        }

        public MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            InitializeComponent();
            Closing += OnClosing;
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

            //ShowSavingDataWindow();
            this._mainWindowViewModel.ClosingCommand.Execute();
            // Disable clicks on main window so that user does not change settings while application is closing and saving data.
            this.IsHitTestVisible = false;
            _mainWindowViewModel.UserClosedWindow = true;
        }
    }
}