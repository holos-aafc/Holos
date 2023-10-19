using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using H.Avalonia.ViewModels;
using H.Avalonia.ViewModels.SupportingViewModels;

namespace H.Avalonia.Views.SupportingViews;

public partial class DisclaimerView : UserControl
{
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
    private DisclaimerViewModel? ViewModel => DataContext as DisclaimerViewModel;
    
    public DisclaimerView()
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
}