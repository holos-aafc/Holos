using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Providers.AnaerobicDigestion;
using H.Infrastructure;

namespace H.Core.Providers.Soil
{
    public class SmallAreaYieldProvider
    {
        #region Fields

        private readonly ProvinceStringConverter _provinceStringConverter = new ProvinceStringConverter();
        private readonly CropTypeStringConverter _cropStringConverter = new CropTypeStringConverter();

        // Use a dictionary since there are > 1M records in the file.
        //
        // These are static, and the reads below are guarded, because the yield file is ~31 MB and
        // parsing it produces millions of entries. The data is immutable once parsed (the provider
        // exposes no mutators), so it is loaded once per process and shared by every instance.
        // Previously each instance re-read the whole file - and FieldResultsService creates a
        // provider per instance in its constructor, so the parse was repeated on every construction.
        // Only the yield is stored. Every other field of SmallAreaYieldData is already in the key
        // (year/polygon/cropType/province) or is the CSV line number, so storing the object cost
        // ~98 bytes per entry across ~4.85M entries (~453 MB). Storing the int and rebuilding the
        // (small, short-lived) object on lookup keeps the public API identical for ~1/3 the memory.
        private static readonly Dictionary<(int year, int polygon, CropType cropType, Province province), int> _cache = new Dictionary<(int year, int polygon, CropType cropType, Province province), int>();

        private static readonly Dictionary<(int year, int polygon, CropType cropType, Province province), int> _updatedYields = new Dictionary<(int year, int polygon, CropType cropType, Province province), int>();

        private static readonly object _loadLock = new object();

        // Tracked separately: Initialize() loads only the main file, InitializeAsync() also loads
        // the updated yields, so a single flag would let one path suppress the other's load.
        private static bool _cacheLoaded;
        private static bool _updatedYieldsLoaded;


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

        public async Task InitializeAsync()
        {
            // Read the file async since this is a large file
            await Task.Run(this.ReadFile);
            await Task.Run(ReadUpdatedYields);
        }

        public void Initialize()
        {
            this.ReadFile();
        }

        public IEnumerable<SmallAreaYieldData> GetData()
        {
            return _cache.Select(kvp => Build(kvp.Key, kvp.Value)).ToList();
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

            if (cropType == CropType.GrassSilage)
            {
                lookupCropType = CropType.TamePasture;
            }

            if (cropType == CropType.Flax)
            {
                lookupCropType = CropType.FlaxSeed;
            }

            if (cropType == CropType.FieldPeas)
            {
                lookupCropType = CropType.DryPeas;
            }

            var key = (year: year, polygon: polygon, cropType: lookupCropType, province: province);
            if (_cache.TryGetValue(key, out var yield))
            {
                return Build(key, yield);
            }
            else
            {
                return null;
            }
        }

        public SmallAreaYieldData GetUpdatedData(int year, int polygon, CropType cropType, Province province)
        {
            var key = (year: year, polygon: polygon, cropType: cropType, province: province);
            if (_updatedYields.TryGetValue(key, out var yield))
            {
                return Build(key, yield);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Rebuilds the data object from the key plus the stored yield. Note <see cref="SmallAreaYieldData.Id"/>
        /// (the CSV line number) is not retained - it was never read by any consumer.
        /// </summary>
        private static SmallAreaYieldData Build((int year, int polygon, CropType cropType, Province province) key, int yield)
        {
            return new SmallAreaYieldData()
            {
                Year = key.year,
                Polygon = key.polygon,
                CropType = key.cropType,
                Province = key.province,
                Yield = yield,
            };
        }

        #endregion

        #region Private Methods

        private void ReadFile()
        {
            if (!_cacheLoaded)
            {
                lock (_loadLock)
                {
                    if (!_cacheLoaded)
                    {
                        this.ReadFileCore();
                        _cacheLoaded = true;
                    }
                }
            }

            // Raised on every call (not just the first) so subscribers still get notified even when
            // the data was already loaded by an earlier instance.
            this.FinishedReadingFile?.Invoke(this, EventArgs.Empty);
        }

        private void ReadFileCore()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.SmallAreaYields);

            foreach (var line in fileLines.Skip(1))
            {
                var year = int.Parse(line[1]);
                var province = _provinceStringConverter.Convert(line[2]);
                var polygon = int.Parse(line[3]);

                _cache.Add((year, polygon, CropType.Barley, province), string.IsNullOrWhiteSpace(line[5]) ? 0 : int.Parse(line[5]));

                _cache.Add((year, polygon, CropType.Canola, province), string.IsNullOrWhiteSpace(line[6]) ? 0 : int.Parse(line[6]));

                _cache.Add((year, polygon, CropType.GrainCorn, province), string.IsNullOrWhiteSpace(line[7]) ? 0 : int.Parse(line[7]));

                _cache.Add((year, polygon, CropType.FlaxSeed, province), string.IsNullOrWhiteSpace(line[8]) ? 0 : int.Parse(line[8]));

                _cache.Add((year, polygon, CropType.Oats, province), string.IsNullOrWhiteSpace(line[9]) ? 0 : int.Parse(line[9]));

                _cache.Add((year, polygon, CropType.Soybeans, province), string.IsNullOrWhiteSpace(line[10]) ? 0 : int.Parse(line[10]));

                _cache.Add((year, polygon, CropType.Durum, province), string.IsNullOrWhiteSpace(line[11]) ? 0 : int.Parse(line[11]));

                _cache.Add((year, polygon, CropType.SpringWheat, province), string.IsNullOrWhiteSpace(line[12]) ? 0 : int.Parse(line[12]));

                _cache.Add((year, polygon, CropType.WinterWheat, province), string.IsNullOrWhiteSpace(line[13]) ? 0 : int.Parse(line[13]));

                _cache.Add((year, polygon, CropType.Buckwheat, province), string.IsNullOrWhiteSpace(line[16]) ? 0 : int.Parse(line[16]));

                _cache.Add((year, polygon, CropType.CanarySeed, province), string.IsNullOrWhiteSpace(line[17]) ? 0 : int.Parse(line[17]));

                _cache.Add((year, polygon, CropType.Caraway, province), string.IsNullOrWhiteSpace(line[18]) ? 0 : int.Parse(line[18]));

                _cache.Add((year, polygon, CropType.SilageCorn, province), string.IsNullOrWhiteSpace(line[21]) ? 0 : int.Parse(line[21]));

                _cache.Add((year, polygon, CropType.FabaBeans, province), string.IsNullOrWhiteSpace(line[22]) ? 0 : int.Parse(line[22]));

                _cache.Add((year, polygon, CropType.Lentils, province), string.IsNullOrWhiteSpace(line[23]) ? 0 : int.Parse(line[23]));

                _cache.Add((year, polygon, CropType.MixedGrains, province), string.IsNullOrWhiteSpace(line[24]) ? 0 : int.Parse(line[24]));

                _cache.Add((year, polygon, CropType.Mustard, province), string.IsNullOrWhiteSpace(line[25]) ? 0 : int.Parse(line[25]));

                _cache.Add((year, polygon, CropType.DryPeas, province), string.IsNullOrWhiteSpace(line[26]) ? 0 : int.Parse(line[26]));

                _cache.Add((year, polygon, CropType.Rye, province), string.IsNullOrWhiteSpace(line[27]) ? 0 : int.Parse(line[27]));

                _cache.Add((year, polygon, CropType.FallRye, province), string.IsNullOrWhiteSpace(line[28]) ? 0 : int.Parse(line[28]));

                _cache.Add((year, polygon, CropType.SpringRye, province), string.IsNullOrWhiteSpace(line[29]) ? 0 : int.Parse(line[29]));

                _cache.Add((year, polygon, CropType.Safflower, province), string.IsNullOrWhiteSpace(line[30]) ? 0 : int.Parse(line[30]));

                _cache.Add((year, polygon, CropType.SugarBeets, province), string.IsNullOrWhiteSpace(line[31]) ? 0 : int.Parse(line[31]));

                _cache.Add((year, polygon, CropType.Sunflower, province), string.IsNullOrWhiteSpace(line[32]) ? 0 : int.Parse(line[32]));

                _cache.Add((year, polygon, CropType.TamePasture, province), string.IsNullOrWhiteSpace(line[33]) ? 0 : int.Parse(line[33]));

                _cache.Add((year, polygon, CropType.Triticale, province), string.IsNullOrWhiteSpace(line[34]) ? 0 : int.Parse(line[34]));

                _cache.Add((year, polygon, CropType.Wheat, province), string.IsNullOrWhiteSpace(line[35]) ? 0 : int.Parse(line[35]));

                _cache.Add((year, polygon, CropType.Potatoes, province), string.IsNullOrWhiteSpace(line[36]) ? 0 : int.Parse(line[36]));

                _cache.Add((year, polygon, CropType.BeansDryField, province), string.IsNullOrWhiteSpace(line[39]) ? 0 : int.Parse(line[39]));
            }
        }

        /// <summary>
        /// Reads the csv file containing data for emission factors.
        /// </summary>
        private void ReadUpdatedYields()
        {
            if (_updatedYieldsLoaded)
            {
                return;
            }

            lock (_loadLock)
            {
                if (_updatedYieldsLoaded)
                {
                    return;
                }

                this.ReadUpdatedYieldsCore();
                _updatedYieldsLoaded = true;
            }
        }

        private void ReadUpdatedYieldsCore()
        {
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.UpdatedSmallAreaYields).ToList();

            var crops = fileLines.ElementAt(0).Skip(5).ToList();

            foreach (string[] line in fileLines.Skip(1))
            {
                var year = int.Parse(line[1]);
                Province province = _provinceStringConverter.Convert(line[2]);
                var polygon = int.Parse(line[3]);

                var yieldLocationInFile = 5;
                foreach (var entry in crops)
                {
                    var cropType = _cropStringConverter.Convert(entry);
                    var yield = string.IsNullOrWhiteSpace(line[yieldLocationInFile])
                        ? 0
                        : int.Parse(line[yieldLocationInFile]);

                    _updatedYields.Add((year, polygon, cropType, province), yield);
                    yieldLocationInFile++;
                }
            }
        }
        #endregion
    }
}