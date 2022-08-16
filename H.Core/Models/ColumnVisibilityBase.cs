using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Core.Models
{
    /// <summary>
    /// base class for ColumnVisibility classes
    /// </summary>
    public abstract class ColumnVisibilityBase : BindableBase
    {
        /// <summary>
        /// this method to be called when ever we want to restore back to default columns
        /// </summary>
        public void SetAllColumnsInvisible()
        {
            foreach(var item in this.GetType().GetProperties())
            {
                item.SetValue(this, false);
            }
        }        
    }
}
