using System.Collections.Generic;
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

        public void InitializeNitrogenContent(List<CropViewItem> viewItem, Farm farm)
        {
            foreach (var cropViewItem in viewItem)
            {
                this.InitializeNitrogenContent(cropViewItem, farm);
            }
        }

        public void InitializeNitrogenContentInProduct(CropViewItem viewItem, Farm farm)
        {
            var residueData = this.GetResidueData(farm, viewItem);
            if (residueData != null)
            {
                // Table has values in grams but unit of display is kg
                viewItem.NitrogenContentInProduct = residueData.NitrogenContentProduct / 1000;
            }
        }

        public void InitializeNitrogenContentInStraw(CropViewItem viewItem, Farm farm)
        {
            var residueData = this.GetResidueData(farm, viewItem);
            if (residueData != null)
            {
                // Table has values in grams but unit of display is kg
                viewItem.NitrogenContentInStraw = residueData.NitrogenContentStraw / 1000;

                if (viewItem.CropType.IsPerennial())
                {
                    viewItem.NitrogenContentInStraw = 0;
                }
            }
        }

        public void InitializeNitrogenContentInRoots(CropViewItem viewItem, Farm farm)
        {
            var residueData = this.GetResidueData(farm, viewItem);
            if (residueData != null)
            {
                // Table has values in grams but unit of display is kg
                viewItem.NitrogenContentInRoots = residueData.NitrogenContentRoot / 1000;
            }
        }

        public void InitializeNitrogenContentInExtraroots(CropViewItem viewItem, Farm farm)
        {
            var residueData = this.GetResidueData(farm, viewItem);
            if (residueData != null)
            {
                // Table has values in grams but unit of display is kg
                viewItem.NitrogenContentInExtraroot = residueData.NitrogenContentExtraroot / 1000;
            }
        }

        public void InitializeNitrogenContent(CropViewItem viewItem, Farm farm)
        {
            // Assign N content values used for the ICBM methodology
            var residueData = this.GetResidueData(farm, viewItem);
            if (residueData != null)
            {
                this.InitializeNitrogenContentInProduct(viewItem, farm);
                this.InitializeNitrogenContentInStraw(viewItem, farm);
                this.InitializeNitrogenContentInRoots(viewItem, farm);
                this.InitializeNitrogenContentInExtraroots(viewItem, farm);
            }

            // Assign N content values used for IPCC Tier 2
            this.InitializeIPCCNitrogenContent(viewItem, farm);
        }

        public void InitializeIPCCNitrogenContent(CropViewItem viewItem, Farm farm)
        {
            // Assign N content values used for IPCC Tier 2
            var cropData = _slopeProviderTable.GetDataByCropType(viewItem.CropType);

            viewItem.NitrogenContent = cropData.NitrogenContentResidues;
        }

        #endregion
    }
}