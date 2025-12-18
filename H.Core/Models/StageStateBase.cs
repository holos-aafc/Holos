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
            get => _isInitialized;
            set => SetProperty(ref _isInitialized, value);
        }

        #endregion

        #region Public Methods

        public virtual void ClearState()
        {
        }

        #endregion
    }
}