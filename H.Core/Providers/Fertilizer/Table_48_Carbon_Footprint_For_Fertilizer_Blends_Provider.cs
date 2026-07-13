using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Core.Mappers;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Fertilizer
{
    /// <summary>
    /// Table 48. Carbon FootPrint at plant gate for different fertilizer blends (US numbers) based on Brentrup et al. 2016
    /// </summary>
    public class Table_48_Carbon_Footprint_For_Fertilizer_Blends_Provider : ProviderBase, IProvider<Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data>
    {
        #region Fields

        private readonly FertilizerBlendConverter _converter = new FertilizerBlendConverter();
        private readonly List<Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data> _cache = new List<Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data>();
        private readonly ModelMapper<Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data> _blendMapper;

        #endregion

        #region Constructors

        public Table_48_Carbon_Footprint_For_Fertilizer_Blends_Provider()
        {
            _blendMapper = new ModelMapper<Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data>(
                nameof(Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data.Guid));

            _cache.AddRange(this.BuildCache());
        }

        #endregion

        #region Public Methods

        public IEnumerable<Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data> GetData()
        {
            var result = new List<Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data>();

            foreach (var fertilizerBlendData in _cache)
            {
                result.Add(this.GetData(fertilizerBlendData.FertilizerBlend));
            }

            return result;
        }

        /// <summary>
        /// Return a copy of the system blends so that user will always get the original values when adding a new fertilizer application. User modifying system blends is not supported
        /// at this time.
        /// </summary>
        public Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data GetData(FertilizerBlends blend)
        {
            var cachedItem = _cache.SingleOrDefault(item => item.FertilizerBlend == blend);
            if (cachedItem != null)
            {
                return _blendMapper.Map(cachedItem);
            }
            else
            {
                Trace.Write($"{nameof(Table_48_Carbon_Footprint_For_Fertilizer_Blends_Provider)}.{nameof(GetData)}" +
                            $" - Unknown fertilizer blend type: {blend}");

                return new Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data();
            }
        }

        #endregion

        #region Private Methods

        private IEnumerable<Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.FertilizerBlends);
            var result = new List<Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data>();

            foreach (var line in fileLines.Skip(3))
            {
                if (string.IsNullOrWhiteSpace(line[0]))
                {
                    Trace.Write($"{nameof(Table_48_Carbon_Footprint_For_Fertilizer_Blends_Provider)}.{nameof(BuildCache)}" +
                                $" - File: {nameof(CsvResourceNames.FertilizerBlends)} : first cell of the line is empty. Exiting loop to stop reading more lines inside .csv file.");
                    break;
                }

                var entry = new Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data();

                entry.FertilizerBlend = _converter.Convert(line[0]);
                entry.Name = entry.FertilizerBlend.GetDescription();

                entry.PercentageNitrogen = double.Parse(line[2], cultureInfo);
                entry.PercentagePhosphorus = double.Parse(line[3], cultureInfo);
                entry.PercentagePotassium = double.Parse(line[4], cultureInfo);
                entry.PercentageSulphur = double.Parse(line[5], cultureInfo);

                if (string.IsNullOrWhiteSpace(line[6]) == false)
                {
                    entry.CarbonDioxideEmissionsAtTheGate = double.Parse(line[6], cultureInfo);
                }

                if (string.IsNullOrWhiteSpace(line[7]) == false)
                {
                    entry.ApplicationEmissions = double.Parse(line[7], cultureInfo);
                }

                result.Add(entry);
            }

            // Add in a blank custom type for the user
            result.Add(new Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data()
            {
                FertilizerBlend = FertilizerBlends.Custom,
            });

             return result;
        }

        #endregion
    }
}