using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Economics
{
    /// <summary>
    /// Table_63_Beef_Cattle_Pasture_Summer_Feed_Cost_Data
    /// </summary>
    public class Table_63_Beef_Cattle_Pasture_Summer_Feed_Cost_Provider
    {
        #region Fields

        private readonly AnimalTypeStringConverter _animalTypeStringConverter;
        private readonly DietTypeStringConverter _dietTypeStringConverter;
        private readonly PastureTypeStringConverter _pastureTypeStringConverter;

        #endregion


        #region Properties

        /// <summary>
        /// A list of all data values from the .csv file. Each entry in the list corresponds to a row in the .csv file.
        /// </summary>
        public List<Table_63_Beef_Cattle_Pasture_Summer_Feed_Cost_Data> BeefCattlePastureSummerFeedCostData { get; private set; }

        #endregion


        #region Constructors

        public Table_63_Beef_Cattle_Pasture_Summer_Feed_Cost_Provider()
        {
            _animalTypeStringConverter = new AnimalTypeStringConverter();
            _dietTypeStringConverter = new DietTypeStringConverter();
            _pastureTypeStringConverter = new PastureTypeStringConverter();
            BeefCattlePastureSummerFeedCostData = ReadFile();
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Finds an instance of <see cref="Table_63_Beef_Cattle_Pasture_Summer_Feed_Cost_Data"/> based on the AnimalType and DietType. Each instance
        /// represents a row in the .csv file.
        /// </summary>
        /// <param name="animalType">The animal type for which the cost information is needed</param>
        /// <param name="dietType">The diet type of the animal for which cost information is needed.</param>
        /// <returns>Returns an instance of <see cref="Table_63_Beef_Cattle_Pasture_Summer_Feed_Cost_Data"/>. If nothing is found, returns null.</returns>
        public Table_63_Beef_Cattle_Pasture_Summer_Feed_Cost_Data GetBeefCattlePastureSummerFeedCostData(AnimalType animalType, DietType dietType)
        {
            var costData = BeefCattlePastureSummerFeedCostData.Find(x =>
                                                                        x.AnimalType == animalType &&
                                                                        x.DietType == dietType);

            if (costData != null)
            {
                return costData;
            }

            costData = BeefCattlePastureSummerFeedCostData.Find(x => x.AnimalType == animalType);

            if (costData != null)
            {
                Trace.TraceError($"{nameof(Table_63_Beef_Cattle_Pasture_Summer_Feed_Cost_Data)}.{nameof(BeefCattlePastureSummerFeedCostData)} " +
                                 $": could not find DietType: {dietType} in the .csv file for the AnimalType:{animalType}, returning null.");
            }
            else
            {
                Trace.TraceError($"{nameof(Table_63_Beef_Cattle_Pasture_Summer_Feed_Cost_Data)}.{nameof(BeefCattlePastureSummerFeedCostData)} " +
                                 $": could not find AnimalType: {animalType} in the .csv file, returning null.");
            }

            return null;
        }


        #endregion

        #region Private Methods

        private List<Table_63_Beef_Cattle_Pasture_Summer_Feed_Cost_Data> ReadFile()
        {
            var fileData = new List<Table_63_Beef_Cattle_Pasture_Summer_Feed_Cost_Data>();
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.BeefCattlePastureSummerFeedCost);
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;

            foreach (var line in fileLines.Skip(1))
            {
                AnimalType animalType = _animalTypeStringConverter.Convert(line[0]);
                DietType dietType = _dietTypeStringConverter.Convert(line[1]);
                PastureType pastureType = _pastureTypeStringConverter.Convert(line[2]);

                var feedVariableCost = double.Parse(line[3], cultureInfo);
                var variableCostOther = double.Parse(line[4], cultureInfo);
                var fixedCosts = double.Parse(line[5], cultureInfo);
                var labourCosts = double.Parse(line[6], cultureInfo);


                fileData.Add(new Table_63_Beef_Cattle_Pasture_Summer_Feed_Cost_Data
                {
                    AnimalType = animalType,
                    DietType = dietType,
                    PastureType = pastureType,
                    VariableCostFeed = feedVariableCost,
                    VariableCostsNonFeed = variableCostOther,
                    FixedCosts = fixedCosts,
                    LabourCosts = labourCosts,

                });
            }


            return fileData;
        }

        #endregion
    }
}
