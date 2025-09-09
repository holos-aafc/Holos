﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Economics
{
    /// <summary>
    ///     Table 58. Beef cattle – Fed/Winter feed
    /// </summary>
    public class Beef_Cattle_Winter_Feed_Cost_Provider
    {
        #region Constructors

        public Beef_Cattle_Winter_Feed_Cost_Provider()
        {
            _dietTypeStringConverter = new DietTypeStringConverter();
            _animalTypeStringConverter = new AnimalTypeStringConverter();
            BeefCattleWinterFeedData = ReadFile();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     A list of all data values from the .csv file. Each entry in the list corresponds to a row in the .csv file.
        /// </summary>
        public List<Beef_Cattle_Winter_Feed_Cost_Data> BeefCattleWinterFeedData { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Finds an instance of <see cref="Beef_Cattle_Winter_Feed_Cost_Data" /> based on the AnimalType and DietType. Each
        ///     instance
        ///     represents a row in the .csv file.
        /// </summary>
        /// <param name="animalType">The animal type for which the cost information is needed</param>
        /// <param name="dietType">The diet type of the animal for which cost information is needed.</param>
        /// <returns>Returns an instance of <see cref="Beef_Cattle_Winter_Feed_Cost_Data" />. If nothing is found, returns null.</returns>
        public Beef_Cattle_Winter_Feed_Cost_Data GetBeefCattleWinterFeedCost(AnimalType animalType,
            DietType dietType)
        {
            var data = BeefCattleWinterFeedData.Find(x =>
                x.AnimalType == animalType &&
                x.DietType == dietType);

            if (data != null) return data;

            data = BeefCattleWinterFeedData.Find(x => x.AnimalType == animalType);

            if (data != null)
                Trace.TraceError(
                    $"{nameof(Beef_Cattle_Winter_Feed_Cost_Provider)}.{nameof(GetBeefCattleWinterFeedCost)} " +
                    $": could not find DietType: {dietType} in the .csv file for the AnimalType:{animalType}, returning null.");
            else
                Trace.TraceError(
                    $"{nameof(Beef_Cattle_Winter_Feed_Cost_Provider)}.{nameof(GetBeefCattleWinterFeedCost)} " +
                    $": could not find AnimalType: {animalType} in the .csv file, returning null.");

            return null;
        }

        #endregion

        #region Private Methods

        private List<Beef_Cattle_Winter_Feed_Cost_Data> ReadFile()
        {
            var fileData = new List<Beef_Cattle_Winter_Feed_Cost_Data>();
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.BeefCattleFedWinterFeedCost);

            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;

            foreach (var line in fileLines.Skip(1))
            {
                var animalType = _animalTypeStringConverter.Convert(line[0]);
                var dietType = _dietTypeStringConverter.Convert(line[1]);
                var otherVariableCosts = double.Parse(line[3], cultureInfo);
                var fixedCosts = double.Parse(line[4], cultureInfo);
                var labourCosts = double.Parse(line[5], cultureInfo);

                fileData.Add(new Beef_Cattle_Winter_Feed_Cost_Data
                {
                    AnimalType = animalType,
                    DietType = dietType,
                    VariableCostsNonFeed = otherVariableCosts,
                    FixedCosts = fixedCosts,
                    LabourCosts = labourCosts
                });
            }

            return fileData;
        }

        #endregion

        #region Fields

        private readonly AnimalTypeStringConverter _animalTypeStringConverter;
        private readonly DietTypeStringConverter _dietTypeStringConverter;

        #endregion
    }
}