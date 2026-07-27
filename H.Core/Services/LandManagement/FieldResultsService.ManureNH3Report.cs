using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using H.Core.Calculators.Nitrogen;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Infrastructure;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService
    {
        #region Manure land-application NH3-N emission factor report

        /// <summary>
        /// Builds a report of the final NH3-N emission factor used for each land application of manure, with one row per
        /// application. This is the per-application companion to the field results file: because a field-year can have
        /// several manure applications - each with its own animal type, date and emission factor - a single field-year
        /// row cannot represent them without ambiguity, so each application is given its own row here with the context
        /// needed to interpret its emission factor.
        ///
        /// The emission factor's basis differs by manure source (see
        /// <see cref="N2OEmissionFactorCalculator.AppliesEmissionFactorToTan"/>): beef, dairy and poultry factors are
        /// per kg TAN; sheep, swine, other livestock and imported manure factors are per kg N. The basis is written in
        /// its own column so the two are never confused.
        /// </summary>
        public string BuildManureLandApplicationEmissionFactorReport(
            IEnumerable<CropViewItem> viewItems,
            Farm farm,
            CultureInfo culture)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("sep =,");
            stringBuilder.AppendLine(string.Join(",",
                Properties.Resources.LabelFarm,
                Properties.Resources.LabelField,
                Properties.Resources.LabelTimePeriod,
                Properties.Resources.LabelYear,
                Properties.Resources.LabelCrop,
                Properties.Resources.LabelAnimalType,
                Properties.Resources.LabelManureSource,
                Properties.Resources.LabelManureType,
                Properties.Resources.LabelDateOfApplication,
                Properties.Resources.LabelTillageType,
                Properties.Resources.LabelAmountOfNitrogenApplied,
                Properties.Resources.LabelEmissionFactorBasis,
                Properties.Resources.LabelFinalAmmoniaEmissionFactorForLandApplication));

            foreach (var viewItem in viewItems)
            {
                var fieldName = string.IsNullOrWhiteSpace(viewItem.Name) ? viewItem.FieldName : viewItem.Name;

                foreach (var manureApplication in viewItem.ManureApplicationViewItems)
                {
                    var emissionFactor = _n2OEmissionFactorCalculator
                        .GetFinalAmmoniaEmissionFactorForLandApplication(farm, viewItem, manureApplication);

                    var basis = N2OEmissionFactorCalculator.AppliesEmissionFactorToTan(manureApplication)
                        ? Properties.Resources.LabelPerKilogramTan
                        : Properties.Resources.LabelPerKilogramNitrogen;

                    var source = manureApplication.IsImportedManure()
                        ? Properties.Resources.Imported
                        : Properties.Resources.LabelLivestock;

                    stringBuilder.AppendLine(string.Join(",",
                        Quote(farm.Name, culture),
                        Quote(fieldName, culture),
                        Quote(viewItem.TimePeriodCategoryString, culture),
                        Quote(viewItem.Year.ToString(culture), culture),
                        Quote(viewItem.CropTypeString, culture),
                        Quote(manureApplication.AnimalType.GetDescription(), culture),
                        Quote(source, culture),
                        Quote(manureApplication.ManureStateType.GetDescription(), culture),
                        Quote(manureApplication.DateOfApplication.ToString("yyyy-MM-dd", culture), culture),
                        Quote(viewItem.TillageType.GetDescription(), culture),
                        Quote(manureApplication.AmountOfNitrogenAppliedPerHectare.ToString("F4", culture), culture),
                        Quote(basis, culture),
                        Quote(emissionFactor.ToString("F4", culture), culture)));
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Writes the report from <see cref="BuildManureLandApplicationEmissionFactorReport"/> to
        /// <paramref name="path"/>. Returns true on success.
        /// </summary>
        public bool ExportManureLandApplicationEmissionFactorReportToFile(
            IEnumerable<CropViewItem> viewItems,
            Farm farm,
            string path,
            CultureInfo culture)
        {
            var contents = this.BuildManureLandApplicationEmissionFactorReport(viewItems, farm, culture);

            try
            {
                File.WriteAllText(path, contents, Encoding.UTF8);
            }
            catch (IOException exception)
            {
                Trace.TraceInformation(
                    $"{nameof(FieldResultsService)}.{nameof(this.ExportManureLandApplicationEmissionFactorReportToFile)}: " +
                    $"error writing data to csv file: '{exception.Message}'.");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Wraps a value in double quotes for CSV output, escaping any embedded quotes.
        /// </summary>
        private static string Quote(string value, CultureInfo culture)
        {
            var text = value ?? string.Empty;
            return "\"" + text.Replace("\"", "\"\"") + "\"";
        }

        #endregion
    }
}
