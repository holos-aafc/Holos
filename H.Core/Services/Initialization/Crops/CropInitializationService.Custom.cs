using System.Linq;
using AutoMapper;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using Microsoft.Extensions.Logging.Abstractions;

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

            var customCropDefaultsMapperConfiguration = new MapperConfiguration(configuration =>
            {
                // Don't copy the GUID, and do not overwrite the year, name, or area, on the crop
                configuration.CreateMap<CropViewItem, CropViewItem>()
                    .ForMember(x => x.Guid, options => options.Ignore())
                    .ForMember(x => x.Year, options => options.Ignore())
                    .ForMember(x => x.Name, options => options.Ignore())
                    .ForMember(x => x.Area, options => options.Ignore());
            }, new NullLoggerFactory());

            var mapper = customCropDefaultsMapperConfiguration.CreateMapper();

            mapper.Map(cropDefaults, viewItem);
        }

        #endregion
    }
}