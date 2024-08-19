using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using H.Core.Providers.Nitrogen;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        /// <summary>
        /// Initialize the nitrogen fixation for each <see cref="CropViewItem"/> within a <see cref="Farm"/>
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="CropViewItem"/>'s that will have their nitrogen fixation reinitialized</param>
        public void InitializeNitrogenFixation(Farm farm)
        {
            var viewItems = farm.GetAllCropViewItems();
            foreach (var viewItem in viewItems)
            {
                InitializeNitrogenFixation(viewItem);
            }
        }

        /// <summary>
        /// Initialize the nitrogen fixation within a <see cref="CropViewItem"/>
        /// </summary>
        /// <param name="viewItem">The <see cref="CropViewItem"/> having its nitrogen fixation reinitialized</param>
        public void InitializeNitrogenFixation(CropViewItem viewItem)
        {
            viewItem.NitrogenFixationPercentage = _nitrogenFixationProvider.GetNitrogenFixationResult(viewItem.CropType).Fixation * 100;
        }

        public void InitializeNitrogenContent(Farm farm)
        {
            foreach (var viewItem in farm.GetAllCropViewItems())
            {
                this.InitializeNitrogenContent(viewItem, farm);
            }
        }

        public void InitializeNitrogenContent(CropViewItem viewItem, Farm farm)
        {
            // Assign N content values used for the ICBM methodology
            var residueData = this.GetResidueData(farm, viewItem);
            if (residueData != null)
            {
                // Table has values in grams but unit of display is kg
                viewItem.NitrogenContentInProduct = residueData.NitrogenContentProduct / 1000;
                viewItem.NitrogenContentInStraw = residueData.NitrogenContentStraw / 1000;
                viewItem.NitrogenContentInRoots = residueData.NitrogenContentRoot / 1000;
                viewItem.NitrogenContentInExtraroot = residueData.NitrogenContentExtraroot / 1000;

                if (viewItem.CropType.IsPerennial())
                {
                    viewItem.NitrogenContentInStraw = 0;
                }
            }

            // Assign N content values used for IPCC Tier 2
            var cropData = _slopeProviderTable.GetDataByCropType(viewItem.CropType);

            viewItem.NitrogenContent = cropData.NitrogenContentResidues;
        }

        #endregion
    }
}