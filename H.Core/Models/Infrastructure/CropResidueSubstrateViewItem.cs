using H.Core.Enumerations;

namespace H.Core.Models.Infrastructure
{
    public class CropResidueSubstrateViewItem : SubstrateViewItemBase
    {
        #region Fields

        private CropType _cropType;

        #endregion

        #region Properties
        
        public CropType CropType
        {
            get => _cropType;
            set => SetProperty(ref _cropType, value);
        } 

        #endregion
    }
}