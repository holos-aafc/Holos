using System.Collections.Generic;
using System.Globalization;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models.Results;
using H.Core.Providers.Carbon;

namespace H.Core.Services.LandManagement
{
    public interface IFieldResultsService 
    { 
        double CalculateTillageFactor(CropViewItem viewItem, Farm farm);
        double CalculateManagementFactor(double climateParameter, double tillageFactor);
        void CreateDetailViewItems(Farm farm);

        
        List<CropViewItem> CalculateFinalResults(IEnumerable<Farm> farms);

        bool ExportResultsToFile(IEnumerable<CropViewItem> results,
                                            object path,
                                            CultureInfo cultureInfo,
                                            MeasurementSystemType measurementSystemType,
                                            string languageAddOn,
                                            bool exportedFromGui,
                                            Farm farm);
        double CalculateHarvest(CropViewItem viewItem);

        FieldSystemDetailsStageState GetStageState(Farm farm);
        CropViewItem MapDetailsScreenViewItemFromComponentScreenViewItem(CropViewItem viewItem, int year);
        void InitializeStageState(Farm farm);
        List<CropViewItem> CalculateFinalResults(Farm farm);
        List<AnimalComponentEmissionsResults> AnimalResults { get; set; }
    }
}