using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Fertilizer
{
    public class FertilizerBlendProvider : ProviderBase, IProvider<FertilizerBlendData>
    {
        #region Fields

        private readonly FertilizerBlendConverter _converter = new FertilizerBlendConverter();
        private readonly List<FertilizerBlendData> _cache = new List<FertilizerBlendData>();
        private readonly IMapper _blendMapper;

        #endregion

        #region Constructors

        public FertilizerBlendProvider()
        {
            var configuration = new MapperConfiguration(expression =>
            {
                expression.CreateMap<FertilizerBlendData, FertilizerBlendData>()
                    .ForMember(property => property.Guid, options => options.Ignore());
            });

            _blendMapper = configuration.CreateMapper();

            _cache.AddRange(this.BuildCache());
        }

        #endregion

        #region Public Methods

        public IEnumerable<FertilizerBlendData> GetData()
        {
            var result = new List<FertilizerBlendData>();

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
        public FertilizerBlendData GetData(FertilizerBlends blend)
        {
            var cachedItem = _cache.SingleOrDefault(item => item.FertilizerBlend == blend);
            if (cachedItem != null)
            {
                return _blendMapper.Map<FertilizerBlendData>(cachedItem);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Private Methods

        private IEnumerable<FertilizerBlendData> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.FertilizerBlends);
            var result = new List<FertilizerBlendData>();

            foreach (var line in fileLines.Skip(3))
            {
                var entry = new FertilizerBlendData();

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
            result.Add(new FertilizerBlendData()
            {
                FertilizerBlend = FertilizerBlends.Custom,
            });

            return result;
        }

        #endregion
    }
}