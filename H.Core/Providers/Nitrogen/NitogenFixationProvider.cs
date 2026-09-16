using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Nitrogen
{
    /// <summary>
    /// %NDFA - the share of a crop's nitrogen derived from the atmosphere - for the crops that fix nitrogen, taken
    /// from the Nfixation definition in the algorithm document (Eq. 2.5.5-6, Eq. 2.6.8-12 and Eq. 2.7.7-11).
    ///
    /// The values live in Resources\NitrogenFixationByCropType.csv so they can be revised without a code change.
    /// </summary>
    public class NitogenFixationProvider : ProviderBase
    {
        #region Fields

        private readonly CropTypeStringConverter _cropTypeStringConverter;
        private readonly List<NitrogenFixationResult> _table;

        #endregion

        #region Constructors

        public NitogenFixationProvider()
        {
            HTraceListener.AddTraceListener();

            _cropTypeStringConverter = new CropTypeStringConverter();
            _table = this.ReadFile();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The share of the crop's nitrogen fixed from the atmosphere, as a fraction. A crop the file does not list
        /// fixes none, which includes legumes the algorithm document gives no value for - faba beans and white beans
        /// among them.
        /// </summary>
        public NitrogenFixationResult GetNitrogenFixationResult(CropType cropType)
        {
            var result = _table.Find(entry => entry.CropType == cropType);

            return result ?? new NitrogenFixationResult() { CropType = cropType, Fixation = 0 };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The file states each value as a percentage, the way the algorithm document does; it is held here as a
        /// fraction, which is what every caller expects.
        /// </summary>
        private List<NitrogenFixationResult> ReadFile()
        {
            var results = new List<NitrogenFixationResult>();

            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.NitrogenFixationByCropType);

            // The file leads with a comment block, so the header is the first line that is neither blank nor a
            // comment - it cannot be found by position.
            var rows = fileLines
                .Where(line => base.IsBlank(line) == false && base.IsComment(line) == false)
                .Skip(1);

            foreach (var line in rows)
            {
                var cropType = _cropTypeStringConverter.Convert(line[0]);
                if (cropType == CropType.NotSelected)
                {
                    Trace.TraceError($"{nameof(NitogenFixationProvider)}.{nameof(ReadFile)}: could not parse '{line[0]}' as a crop type. This crop will fix no nitrogen.");

                    continue;
                }

                results.Add(new NitrogenFixationResult()
                {
                    CropType = cropType,
                    Fixation = base.ParseDouble(line[1]) / 100.0,
                });
            }

            return results;
        }

        #endregion
    }
}
