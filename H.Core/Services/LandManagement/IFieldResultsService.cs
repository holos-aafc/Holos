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

        /// <summary>
        /// Writes the per-application manure NH3-N emission factor report to <paramref name="path"/>. Returns true on
        /// success. See <see cref="FieldResultsService.BuildManureLandApplicationEmissionFactorReport"/> for the report.
        /// </summary>
        bool ExportManureLandApplicationEmissionFactorReportToFile(IEnumerable<CropViewItem> viewItems,
                                            Farm farm,
                                            string path,
                                            CultureInfo culture);
        double CalculateHarvest(CropViewItem viewItem);

        FieldSystemDetailsStageState GetStageState(Farm farm);
        CropViewItem MapDetailsScreenViewItemFromComponentScreenViewItem(CropViewItem viewItem, int year);
        void InitializeStageState(Farm farm);
        List<CropViewItem> CalculateFinalResults(Farm farm);
        List<AnimalComponentEmissionsResults> AnimalResults { get; set; }

        /// <summary>
        /// When set (per farm run), the carbon calculators build their manure tanks into this shared per-run store
        /// instead of private ones (issue #451 follow-up). Forwarded to the carbon service. Byte-identical.
        /// </summary>
        ManureTankStore SharedManureTankStore { get; set; }
    }
}