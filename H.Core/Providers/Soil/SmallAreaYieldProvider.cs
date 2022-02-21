using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Soil
{
    public class SmallAreaYieldProvider
    {
        #region Fields

        private readonly ProvinceStringConverter _provinceStringConverter = new ProvinceStringConverter();

        // Use a dictionary since there are > 1M records in the file
        private readonly Dictionary<(int year, int polygon, CropType cropType, Province province), SmallAreaYieldData> _cache = new Dictionary<(int year, int polygon, CropType cropType, Province province), SmallAreaYieldData>();

        #endregion

        #region Constructors

        public SmallAreaYieldProvider()
        {
        }

        #endregion

        #region Properties

        public event EventHandler FinishedReadingFile;

        #endregion

        #region Public Methods

        public void InitializeAsync()
        {
            // Read the file async since this is a large file
            Task.Run(this.ReadFile);
        }

        public IEnumerable<SmallAreaYieldData> GetData()
        {
            return _cache.Values.ToList();
        }

        public SmallAreaYieldData GetData(int year, int polygon, CropType cropType, Province province)
        {
            CropType lookupCropType = cropType;
            if (cropType.IsPerennial())
            {
                // Small area yield table only has one perennial type 'tame hay'. Had discussion with team on 8/17/2021  and it was agreed
                // that we would use tame hay yields as the default for all perennial types until better numbers were found
                lookupCropType = CropType.TamePasture;
            }

            if (_cache.ContainsKey((year: year, polygon: polygon, cropType: lookupCropType, province: province)))
            {
                return _cache[(year: year, polygon: polygon, cropType: lookupCropType, province: province)];
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Private Methods

        private void ReadFile()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.SmallAreaYields);

            foreach (var line in fileLines.Skip(1))
            {
                var id = int.Parse(line[0]);
                var year = int.Parse(line[1]);
                var province = _provinceStringConverter.Convert(line[2]);
                var polygon = int.Parse(line[3]);

                _cache.Add((year, polygon, CropType.Barley, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.Barley,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[5]) ? 0 : int.Parse(line[5]),
                });

                _cache.Add((year, polygon, CropType.Canola, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.Canola,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[6]) ? 0 : int.Parse(line[6]),
                });

                _cache.Add((year, polygon, CropType.GrainCorn, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.GrainCorn,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[7]) ? 0 : int.Parse(line[7]),
                });

                _cache.Add((year, polygon, CropType.FlaxSeed, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.FlaxSeed,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[8]) ? 0 : int.Parse(line[8]),
                });

                _cache.Add((year, polygon, CropType.Oats, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.Oats,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[9]) ? 0 : int.Parse(line[9]),
                });

                _cache.Add((year, polygon, CropType.Soybeans, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.Soybeans,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[10]) ? 0 : int.Parse(line[10]),
                });

                _cache.Add((year, polygon, CropType.Durum, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.Durum,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[11]) ? 0 : int.Parse(line[11]),
                });

                _cache.Add((year, polygon, CropType.SpringWheat, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.SpringWheat,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[12]) ? 0 : int.Parse(line[12]),
                });

                _cache.Add((year, polygon, CropType.WinterWheat, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.WinterWheat,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[13]) ? 0 : int.Parse(line[13]),
                });

                _cache.Add((year, polygon, CropType.Buckwheat, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.Buckwheat,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[16]) ? 0 : int.Parse(line[16]),
                });

                _cache.Add((year, polygon, CropType.CanarySeed, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.CanarySeed,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[17]) ? 0 : int.Parse(line[17]),
                });

                _cache.Add((year, polygon, CropType.Caraway, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.Caraway,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[18]) ? 0 : int.Parse(line[18]),
                });

                _cache.Add((year, polygon, CropType.SilageCorn, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.SilageCorn,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[21]) ? 0 : int.Parse(line[21]),
                });

                _cache.Add((year, polygon, CropType.FabaBeans, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.FabaBeans,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[22]) ? 0 : int.Parse(line[22]),
                });

                _cache.Add((year, polygon, CropType.Lentils, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.Lentils,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[23]) ? 0 : int.Parse(line[23]),
                });

                _cache.Add((year, polygon, CropType.MixedGrains, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.MixedGrains,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[24]) ? 0 : int.Parse(line[24]),
                });

                _cache.Add((year, polygon, CropType.Mustard, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.Mustard,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[25]) ? 0 : int.Parse(line[25]),
                });

                _cache.Add((year, polygon, CropType.DryPeas, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.DryPeas,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[26]) ? 0 : int.Parse(line[26]),
                });

                _cache.Add((year, polygon, CropType.Rye, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.Rye,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[27]) ? 0 : int.Parse(line[27]),
                });

                _cache.Add((year, polygon, CropType.FallRye, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.FallRye,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[28]) ? 0 : int.Parse(line[28]),
                });

                _cache.Add((year, polygon, CropType.SpringRye, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.SpringRye,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[29]) ? 0 : int.Parse(line[29]),
                });

                _cache.Add((year, polygon, CropType.Safflower, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.Safflower,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[30]) ? 0 : int.Parse(line[30]),
                });

                _cache.Add((year, polygon, CropType.SugarBeets, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.SugarBeets,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[31]) ? 0 : int.Parse(line[31]),
                });

                _cache.Add((year, polygon, CropType.Sunflower, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.Sunflower,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[32]) ? 0 : int.Parse(line[32]),
                });

                _cache.Add((year, polygon, CropType.TamePasture, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.TamePasture,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[33]) ? 0 : int.Parse(line[33]),
                });

                _cache.Add((year, polygon, CropType.Triticale, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.Triticale,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[34]) ? 0 : int.Parse(line[34]),
                });

                _cache.Add((year, polygon, CropType.Wheat, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.Wheat,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[35]) ? 0 : int.Parse(line[35]),
                });

                _cache.Add((year, polygon, CropType.Potatoes, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.Potatoes,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[36]) ? 0 : int.Parse(line[36]),
                });

                _cache.Add((year, polygon, CropType.BeansDryField, province), new SmallAreaYieldData()
                {
                    Id = id,
                    Polygon = polygon,
                    Province = province,
                    CropType = CropType.BeansDryField,
                    Year = year,
                    Yield = string.IsNullOrWhiteSpace(line[39]) ? 0 : int.Parse(line[39]),
                });
            }

            this.FinishedReadingFile?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}