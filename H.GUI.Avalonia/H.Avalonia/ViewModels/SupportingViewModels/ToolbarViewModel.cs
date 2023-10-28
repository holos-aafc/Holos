using System;
using Avalonia.Controls.Notifications;
using H.Core;
using Prism.Commands;
using Prism.Regions;

namespace H.Avalonia.ViewModels.SupportingViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        private readonly Storage _storage;
        
        public DelegateCommand SaveCommand { get; set; }
        public ToolbarViewModel()
        {
            
        }
        public ToolbarViewModel(IRegionManager regionManager, Storage storage) : base(regionManager, storage)
        {
            _storage = storage;
            SaveCommand = new DelegateCommand(OnClickSave);
        }
        
        
        private async void OnClickSave()
        {
            var notification = new Notification(
                title: Core.Properties.Resources.HeadingSavingData,
                message: Core.Properties.Resources.MessageSavingData,
                type: NotificationType.Information
            );
            NotificationManager.Show(notification);
            _storage.HasSaveCompleted = false;
            await _storage.SaveAsync();
            CloseNotification(notification);
        }
    }
}
