using System.Linq;
using H.Core.Mappers;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods


        public void InitializeUserDefaults(CropViewItem viewItem, GlobalSettings globalSettings)
        {
            // Check if user has defaults defined for the type of crop
            var cropDefaults = globalSettings.CropDefaults.SingleOrDefault(x => x.CropType == viewItem.CropType);
            if (cropDefaults == null)
            {
                return;
            }

            if (cropDefaults.EnableCustomUserDefaultsForThisCrop == false)
            {
                // User did not specify defaults for this crop (or just wants to use system defaults) so return from here without modifying the view item further

                return;
            }

            // Don't copy the GUID, and do not overwrite the year, name, or area, on the crop
            var mapper = new ModelMapper<CropViewItem>(
                nameof(CropViewItem.Guid),
                nameof(CropViewItem.Year),
                nameof(CropViewItem.Name),
                nameof(CropViewItem.Area));

            mapper.Map(cropDefaults, viewItem);
        }

        #endregion
    }
}