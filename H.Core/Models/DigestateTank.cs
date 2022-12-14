using H.Core.Enumerations;

namespace H.Core.Models
{
    /// <summary>
    /// A storage tank for digestate resulting from anaerobic digestion
    /// </summary>
    public class DigestateTank : StorageTankBase
    {
        #region Fields

        private DigestateState _digestateState;

        #endregion

        #region Properties
        
        /// <summary>
        /// A tank of digestate can store whole (raw) digestate or separated digestate
        /// </summary>
        public DigestateState DigestateState
        {
            get => _digestateState;
            set => SetProperty(ref _digestateState, value);
        } 

        #endregion
    }
}