using H.Avalonia.Models;
using H.Avalonia.Views;
using Prism.Commands;

namespace H.Avalonia.ViewModels;

public interface IDataGridFeatures
{
    /// <summary>
    /// A command that adds rows to the grid displayed to the user. Each row indicates a viewitem of a relevant type.
    /// </summary>
    DelegateCommand AddRowCommand { get; set; }

    /// <summary>
    /// A command that removes rows to the grid displayed to the user.. Each row indicates a viewitem of a relevant type.
    /// </summary>
    DelegateCommand<object> DeleteRowCommand { get; set; }

    /// <summary>
    /// Deletes a row that is currently marked as selected by the user.
    /// </summary>
    DelegateCommand DeleteSelectedRowsCommand { get; set; }

    /// <summary>
    /// Import data from a csv file. The csv file must have the following columns:
    /// </summary>
    DelegateCommand<object> ImportFromCsvCommand { get; set; }

    /// <summary>
    /// Toggles the select all row command. This command either selects or deselects all the rows currently displayed in the grid.
    /// </summary>
    DelegateCommand ToggleSelectAllRowsCommand { get; set; }
    
    /// <summary>
    /// Check if the grid contains any view items.
    /// </summary>
    public bool HasViewItems { get;}

    /// <summary>
    /// Check if the grid has any view items selected by the user.
    /// </summary>
    public bool AnyViewItemsSelected { get;}
    
    /// <summary>
    /// Check if the all the viewitems are selected in the grid.
    /// </summary>
    public bool AllViewItemsSelected { get; set; }
    
}