using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Previously Table 32. Default manure volume excreted per day for sheep, poultry and other livestock.
    /// </summary>
    public class Table_29_Default_Manure_Excreted_Provider
    {
        #region Inner Classes

        public class Table_29_Default_Manure_Excreted_Provider_Data
        {
            public double ManureExcretionRate { get; set; }
            public ManureStateType ManureStateType { get; set; }
            public AnimalType AnimalType { get; set; }
        }

        #endregion

        #region Fields

        private readonly List<ManureStateType> _availableStateTypes = new List<ManureStateType>()
        {
            ManureStateType.Liquid,
            ManureStateType.Solid,
            ManureStateType.Pasture,
            ManureStateType.Custom,
        };

        private AnimalTypeStringConverter _animalTypeStringConverter = new AnimalTypeStringConverter();

        #endregion

        #region Properties

        #endregion

        #region Constructors

        public Table_29_Default_Manure_Excreted_Provider()
        {
            this.ReadFile();
        }

        public MultiKeyDictionary<AnimalType, ManureStateType, double> Data { get; } = new MultiKeyDictionary<AnimalType, ManureStateType, double>();

        #endregion

        #region Private Methods

        private void ReadFile()
        {
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.PercentageTotalManureProduced).ToList();
            foreach (var line in fileLines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line[0]) || line.Any(string.IsNullOrWhiteSpace))
                {
                    continue;
                }

                var animalType = _animalTypeStringConverter.Convert(line[0]);
                var column = 1;

                foreach (var manureStateType in _availableStateTypes)
                {
                    this.Data[animalType][manureStateType] = double.Parse(line[column], InfrastructureConstants.EnglishCultureInfo);

                    column++;
                }
            }
        }

        #endregion

        public double GetManureExcretionRate(AnimalType animalType, ManureStateType manureStateType)
        {
            ManureStateType manureStateLookup;
            if (manureStateType.IsGrazingArea())
            {
                manureStateLookup = ManureStateType.Pasture;
            }
            else if (manureStateType.IsLiquidManure())
            {
                manureStateLookup = ManureStateType.Liquid;
            }
            else if(manureStateType.IsSolidManure())
            {
                manureStateLookup = ManureStateType.Solid;
            }
            else
            {
                manureStateLookup = ManureStateType.Custom;
            }

            AnimalType animalTypeLookup = animalType;
            if (animalType.IsBeefCattleType())
            {
                animalTypeLookup = AnimalType.Beef;
            }
            else if(animalType.IsDairyCattleType())
            {
                animalTypeLookup = AnimalType.Dairy;
            }
            else if (animalType.IsSheepType())
            {
                animalTypeLookup = AnimalType.Sheep;
            }
            else if (animalType.IsSwineType())
            {
                animalTypeLookup = AnimalType.Swine;
            }

            return 1;

            var result = this.Data[animalTypeLookup][manureStateLookup];


            Trace.TraceError($"{nameof(Table_29_Default_Manure_Excreted_Provider)}.{nameof(GetManureExcretionRate)}" +
                             $" unable to get data for manure excretion rate for animal type: {animalType} and manure type: {manureStateType}." +
                             $" Returning default value of 0.");

            return 0;
        }

        #region Footnotes

        /*
         *
           Footnote 1: Calculated from Hofmann and Beaulieu (2006), Table A1 (for all values in Table A1, total manure production consists of feces and urine.
           Bedding and other types of material such as feather, unused feed, etc. are not included)
           Footnote 2: Lorimor et al. (2004), Table 6
           Footnote 3: Value for ducks used from Lorimor et al., 2004), Table 6
           Footnote 4: Value for sheep and lambs used from Hofmann and Beaulieu (2006), Table A1
           Footnote 5: Value for horses and ponies used from Hofmann and Beaulieu (2006), Table A1
           Footnote 6: Value for dairy cows used from Hofmann and Beaulieu (2006), Table A1
         */

        #endregion
    }
}