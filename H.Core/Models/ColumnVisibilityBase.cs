using Prism.Mvvm;

namespace H.Core.Models
{
    /// <summary>
    ///     base class for ColumnVisibility classes
    /// </summary>
    public abstract class ColumnVisibilityBase : BindableBase
    {
        /// <summary>
        ///     this method to be called when ever we want to restore back to default columns
        /// </summary>
        public void SetAllColumnsInvisible()
        {
            foreach (var item in GetType().GetProperties()) item.SetValue(this, false);
        }
    }
}