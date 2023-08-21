using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace H.Avalonia.Infrastructure.Dialogs
{
    /// <summary>
    /// A ViewModel for the <see cref="DeleteRowDialog"/> view. Handles backend login and button behaviour for the dialog shown to the user.
    /// </summary>
    public class DeleteRowDialogViewModel : BindableBase, IDialogAware
    {
        private string _message;

        public DeleteRowDialogViewModel()
        {
            CancelButtonCommand = new DelegateCommand(OnClickCancelButton);
            DeleteButtonCommand = new DelegateCommand(OnClickDeleteButton);
        }

        /// <summary>
        /// The message string that will appear in the dialog shown to the user.
        /// </summary>
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        /// <summary>
        /// The title of the dialog shown to the user.
        /// </summary>
        public string Title =>  H.Avalonia.Core.Properties.Resources.TitleDeleteRowDialog;

        /// <summary>
        /// A <see cref="DelegateCommand"/> that handles behaviour when the user cancels the dialog by clicking a cancel button.
        /// </summary>
        public DelegateCommand CancelButtonCommand { get; set; }

        /// <summary>
        /// A <see cref="DelegateCommand"/> that handles behaviour when the user selects the delete button in the dialog.
        /// </summary>
        public DelegateCommand DeleteButtonCommand { get; set; }

        /// <summary>
        /// An action that allows us to invoke commands by passing in a <see cref="DialogResult"/> that dictates the type
        /// of action being taken.
        /// </summary>
        public event Action<IDialogResult>? RequestClose;

        /// <summary>
        /// Indicates whether the dialog can be closed or not by the user.
        /// </summary>
        /// <returns></returns>
        public bool CanCloseDialog()
        {
            return true;
        }

        /// <summary>
        /// Method that indicates the actions to take after the dialog is closed by the user.
        /// </summary>
        public void OnDialogClosed()
        {
        }

        /// <summary>
        /// Triggered when the dialogue is opened. Allows us to display items in the dialogue using the paramaters passed
        /// to the dialogue window.
        /// </summary>
        /// <param name="parameters">The parameters passed to the dialog service.</param>
        public void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
        }

        /// <summary>
        /// A function that handles behaviour when the delete button is clicked.
        /// </summary>
        private void OnClickDeleteButton()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        /// <summary>
        /// A function that handles behaviour when the cancel button is clicked.
        /// </summary>
        private void OnClickCancelButton()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }
    }
}