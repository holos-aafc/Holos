using H.Core.Enumerations;

namespace H.Core.Models.Infrastructure
{
    public class FarmResiduesSubstrateViewItem : SubstrateViewItemBase
    {
        #region Fields
        
        private FarmResidueType _farmResidueType;

        #endregion

        #region Constructors

        public FarmResiduesSubstrateViewItem()
        {
        }

        #endregion

        #region Properties

        public FarmResidueType FarmResidueType
        {
            get => _farmResidueType;
            set => this.SetProperty(ref _farmResidueType, value);
        }

        #endregion
    }
}
