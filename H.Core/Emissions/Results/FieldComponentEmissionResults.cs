using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models.Results;
using H.Core.Services.LandManagement;
using H.Infrastructure;

namespace H.Core.Emissions.Results
{
    public class FieldComponentEmissionResults : ModelBase
    {
        #region Fields

        private FieldSystemComponent _fieldSystemComponent;
        private LandUseChangeResults _landUseChangeResults;
        private CropEnergyResults _cropEnergyResults;
        private SoilN2OEmissionsResults _cropN2OEmissionsResults;
        private ObservableCollection<EstimatesOfProductionResultsViewItem> _harvestViewItems;
        

        #endregion

        #region Constructors

        public FieldComponentEmissionResults()
        {
            this.CropEnergyResults = new CropEnergyResults();
            this.LandUseChangeResults = new LandUseChangeResults();
            this.CropN2OEmissionsResults = new SoilN2OEmissionsResults();

            this.HarvestViewItems = new ObservableCollection<EstimatesOfProductionResultsViewItem>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// A collection of harvest view items for one field. A view item will exist for each year that there is yield data available.
        /// </summary>
        public ObservableCollection<EstimatesOfProductionResultsViewItem> HarvestViewItems
        {
            get => _harvestViewItems;
            set => SetProperty(ref _harvestViewItems, value);
        }

        public double SingleYearHarvest
        {
            get
            {
                if (this.HarvestViewItems.Any() == false)
                {
                    return 0;
                }
                else
                {
                    return this.HarvestViewItems.OrderByDescending(viewItem => viewItem.Year).First().Harvest;
                }
            }
        }

        public CropEnergyResults CropEnergyResults
        {
            get => _cropEnergyResults;
            set => SetProperty(ref _cropEnergyResults, value);
        }

        public LandUseChangeResults LandUseChangeResults
        {
            get => _landUseChangeResults;
            set => SetProperty(ref _landUseChangeResults, value);
        }

        public SoilN2OEmissionsResults CropN2OEmissionsResults
        {
            get => _cropN2OEmissionsResults;
            set => SetProperty(ref _cropN2OEmissionsResults, value);
        }

        public FieldSystemComponent FieldSystemComponent
        {
            get => _fieldSystemComponent;
            set => SetProperty( ref _fieldSystemComponent, value);
        }



        #endregion
    }
}