using H.Core.Enumerations;

namespace H.Core.Models.Infrastructure
{
    public class FarmResiduesSubstrateViewItem : SubstrateViewItemBase
    {
        #region Fields

        private FarmResidueType _farmResidueType;

        #endregion

        #region Properties

        public FarmResidueType FarmResidueType
        {
            get => _farmResidueType;
            set => SetProperty(ref _farmResidueType, value);
        }

        #endregion
    }
}