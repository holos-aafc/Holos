#region Imports

using Prism.Mvvm;

#endregion

namespace H.Core.Models
{
    public class StageStateBase : BindableBase
    {
        #region Fields

        private bool _isInitialized;

        #endregion

        #region Properties

        public bool IsInitialized
        {
            get { return _isInitialized; }
            set { this.SetProperty(ref _isInitialized, value); }
        }

        #endregion

        #region Constructors

        #endregion

        #region Public Methods

        public virtual void ClearState() {}

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}