using Prism.Services.Dialogs;

namespace H.Avalonia.Infrastructure
{
    /// <summary>
    /// Extension methods for the dialog service.
    /// </summary>
    public static class DialogServiceExtensions
    {
        /// <summary>
        /// An extension method that shows the dialog to the user.
        /// </summary>
        /// <param name="dialogService"></param>
        /// <param name="dialogName">The name/type of the dialog</param>
        /// <param name="message">The message that needs to be shown to the user</param>
        /// <param name="callback">A callback function that includes the behaviour that allows actions to be taken
        /// based on the result of the dialog.</param>
        public static void ShowMessageDialog(this IDialogService dialogService, string dialogName, string message,
            Action<IDialogResult> callback)
        {
            dialogService.ShowDialog(dialogName, new DialogParameters($"message={message}"), callback);
        }
    }
}