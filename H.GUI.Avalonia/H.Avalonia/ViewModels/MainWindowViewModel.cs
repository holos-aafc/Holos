using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using H.Avalonia.Core;
using H.Core;
using H.Core.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace H.Avalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region Fields        

    private readonly IFarmResultsService _farmResultsService;

    #endregion
    
    #region Properties

    public DelegateCommand ClosingCommand { get; set; }
    public DelegateCommand LoadedCommand { get; set; }

    /// <summary>
    /// Determines if data save has been completed after a quit event was triggered.
    /// </summary>
    public bool HasDataSaveCompletedOnQuit { get; set; }

    /// <summary>
    /// Set to true if the user has previously sent a close window command.
    /// </summary>
    public bool UserClosedWindow { get; set; }
    #endregion
    
    public MainWindowViewModel() { }
    public MainWindowViewModel(IRegionManager? regionManager, IEventAggregator? eventAggregator,
                                Storage? storage, IFarmResultsService farmResultsService) : base(regionManager, eventAggregator, storage)
    {
        this.ClosingCommand = new DelegateCommand(this.OnClosing);
    }
    
    private async void OnClosing()
    {
        await OnClosingSaveAsync();
        if (Application.Current?.ApplicationLifetime  is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow?.Close();
        }
    }
    
    /// <summary>
    /// Runs an async task to save the user's data. Sets <see cref="HasDataSaveCompletedOnQuit"/> to true when save is completed.
    /// </summary>
    /// <returns></returns>
    private async Task OnClosingSaveAsync()
    {
        Trace.TraceInformation($"{nameof(MainWindowViewModel)}.{nameof(OnClosingSaveAsync)}: saving farm to storage");
        await Storage.SaveAsync();
        HasDataSaveCompletedOnQuit = true;
        Trace.TraceInformation($"{nameof(MainWindowViewModel)}.{nameof(OnClosingSaveAsync)}: save complete.");
    }
}

