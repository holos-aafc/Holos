using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Economics
{
    /// <summary>
    /// Table 62. Feed costs for beef
    /// </summary>
    public class Table_62_Feed_Costs_For_Beef_Provider
    {
        #region Fields

        private readonly DietTypeStringConverter _dietTypeStringConverter;

        #endregion

        #region Properties

        /// <summary>
        /// Contains a list of <see cref="Table_62_Feed_Costs_For_Beef_Data"/> as read from the .csv file. Each item in the list represents a row in the .csv file.
        /// </summary>
        public List<Table_62_Feed_Costs_For_Beef_Data> FeedCostsForBeefData { get; private set; }

        #endregion

        #region Constructor

        public Table_62_Feed_Costs_For_Beef_Provider()
        {
            _dietTypeStringConverter = new DietTypeStringConverter();
            FeedCostsForBeefData = ReadFile();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get a single instance of <see cref="Table_62_Feed_Costs_For_Beef_Data"/> based on a specific Diet Type.
        /// </summary>
        /// <param name="dietType">The Diet Type for which we need the beef feed cost.</param>
        /// <returns>An instance of <see cref="Table_62_Feed_Costs_For_Beef_Data"/> representing a row in the .csv file. If nothing is found, returns null.</returns>
        public Table_62_Feed_Costs_For_Beef_Data GetFeedCostByDietType(DietType dietType)
        {
            var data = FeedCostsForBeefData.Find(diet => diet.DietType == dietType);

            if (data == null)
            {
                Trace.TraceError($"{nameof(Table_62_Feed_Costs_For_Beef_Provider)}.{nameof(GetFeedCostByDietType)} " +
                                 $": could not find DietType: {dietType} data in the csv file. Returning null.");
                return null;
            }

            return data;
        }

        #endregion

        #region Private Methods

        private List<Table_62_Feed_Costs_For_Beef_Data> ReadFile()
        {
            var fileData = new List<Table_62_Feed_Costs_For_Beef_Data>();
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            IEnumerable<string[]> fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.BeefFeedCost);

            foreach (var line in fileLines.Skip(1))
            {
                var dietType = _dietTypeStringConverter.Convert(line[0]);
                var cost = double.Parse(line[1], cultureInfo);

                fileData.Add(new Table_62_Feed_Costs_For_Beef_Data
                {
                    DietType = dietType,
                    Cost = cost,
                });
            }

            return fileData;
        }

        #endregion
    }
}
