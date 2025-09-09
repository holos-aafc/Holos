using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.AnaerobicDigestion
{
    public class EmissionFactorsForDigestateStorageProvider
    {
        #region Constructors

        public EmissionFactorsForDigestateStorageProvider()
        {
            _digestateStateStringConverter = new DigestateStateStringConverter();
            _emissionTypeStringConverter = new EmissionTypeStringConverter();

            Data = ReadFile();
        }

        #endregion

        #region Properties

        private List<EmissionFactorsForDigestateStorageData> Data { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Takes an emission type and emission origin (digestate state) and returns a single instance of
        ///     <see cref="EmissionFactorsForDigestateStorageData" /> corresponding to the parameter.
        /// </summary>
        /// <param name="emissionType">The type of emission from the digestate during storage.</param>
        /// <param name="emissionOrigin">The state of the digestate during storage e.g. Raw, Liquid or Solid.</param>
        /// <returns>
        ///     Returns a single instance of <see cref="EmissionFactorsForDigestateStorageData" /> . If nothing found, returns
        ///     an empty instance.
        /// </returns>
        public EmissionFactorsForDigestateStorageData GetEmissionFactorInstance(EmissionTypes emissionType,
            DigestateState emissionOrigin)
        {
            var data = Data.Find(x => x.EmissionType == emissionType && x.EmissionOrigin == emissionOrigin);

            if (data != null) return data;

            data = Data.Find(x => x.EmissionType == emissionType);

            if (data != null)
                Trace.TraceError(
                    $"{nameof(EmissionFactorsForDigestateStorageProvider)}.{nameof(GetEmissionFactorInstance)}: " +
                    $"cannot find Emission Origin: {emissionOrigin}. Returning an empty instance of EmissionFactorsForDigestateStorageData.");
            else
                Trace.TraceError(
                    $"{nameof(EmissionFactorsForDigestateStorageProvider)}.{nameof(GetEmissionFactorInstance)}: " +
                    $"cannot find Emission Type: {emissionType}. Returning an empty instance of EmissionFactorsForDigestateStorageData.");

            return new EmissionFactorsForDigestateStorageData();
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Reads the csv file containing data for emission factors.
        /// </summary>
        /// <returns>
        ///     Returns a list of <see cref="EmissionFactorsForDigestateStorageData" />. Each entry in the list corresponds to
        ///     a single row in the csv.
        /// </returns>
        private List<EmissionFactorsForDigestateStorageData> ReadFile()
        {
            var results = new List<EmissionFactorsForDigestateStorageData>();

            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.EmissionFactorsForDigestateStorage);

            foreach (var line in fileLines.Skip(1))
            {
                var emissionType = _emissionTypeStringConverter.Convert(line[0]);
                var emissionOrigin = _digestateStateStringConverter.Convert(line[1]);
                var emissionFactor = double.Parse(line[2], cultureInfo);
                var description = line[3];

                results.Add(new EmissionFactorsForDigestateStorageData
                {
                    EmissionType = emissionType,
                    EmissionOrigin = emissionOrigin,
                    EmissionFactor = emissionFactor,
                    Description = description
                });
            }

            return results;
        }

        #endregion

        #region Fields

        private readonly DigestateStateStringConverter _digestateStateStringConverter;
        private readonly EmissionTypeStringConverter _emissionTypeStringConverter;

        #endregion
    }
}