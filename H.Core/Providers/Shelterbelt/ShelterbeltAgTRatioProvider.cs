using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Enumerations;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Shelterbelt
{
    public class ShelterbeltAgTRatioProvider
    {
        #region Constructors

        public ShelterbeltAgTRatioProvider()
        {
            HTraceListener.AddTraceListener();
            this.GetLines();
        }

        #endregion

        #region Properties

        private List<ShelterbeltRatioTableData> Table { get; set; } = new List<ShelterbeltRatioTableData>();

        #endregion

        #region Methods

        private void GetLines()
        {
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.AboveGroundBiomassTotalTreeBiomassRatios);
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;

            foreach (var line in fileLines.Skip(2))
            {
                var age = int.Parse(line[0], cultureInfo);

                var hybridPoplar = new ShelterbeltRatioTableData()
                {
                    TreeSpecies = TreeSpecies.HybridPoplar,
                    Age = age,
                    Ratio = double.Parse(line[1], cultureInfo),
                };

                var whiteSpruce = new ShelterbeltRatioTableData()
                {
                    TreeSpecies = TreeSpecies.WhiteSpruce,
                    Age = age,
                    Ratio = double.Parse(line[2], cultureInfo),
                };

                var scotsPine = new ShelterbeltRatioTableData()
                {
                    TreeSpecies = TreeSpecies.ScotsPine,
                    Age = age,
                    Ratio = double.Parse(line[3], cultureInfo),
                };

                var manitobaMaple = new ShelterbeltRatioTableData()
                {
                    TreeSpecies = TreeSpecies.ManitobaMaple,
                    Age = age,
                    Ratio = double.Parse(line[4], cultureInfo),
                };

                var greenAsh = new ShelterbeltRatioTableData()
                {
                    TreeSpecies =  TreeSpecies.GreenAsh,
                    Age = age,
                    Ratio = double.Parse(line[5], cultureInfo),
                };

                var caragana = new ShelterbeltRatioTableData()
                {
                    TreeSpecies = TreeSpecies.Caragana,
                    Age = age,
                    Ratio = double.Parse(line[6], cultureInfo),
                };

                Table.Add(hybridPoplar);
                Table.Add(whiteSpruce);
                Table.Add(scotsPine);
                Table.Add(manitobaMaple);
                Table.Add(greenAsh);
                Table.Add(caragana);
            }
        }

        /// <summary>
        /// Get the ratio (AgT) that will be used to calculate the total biomass of a tree
        /// </summary>
        /// <param name="treeSpecies">The tree species</param>
        /// <param name="age">The age of the tree (years)</param>
        /// <returns>Fraction of aboveground over total biomass (total biomass = aboveground + belowground)</returns>
        public double GetAboveGroundBiomassTotalTreeBiomassRatio(
            TreeSpecies treeSpecies, 
            int age)
        {
            if (age > 60)
            {
                // Table only has data up to 60 years of age
                age = 60;
            }

            var aboveGroundBiomassTotalTreeBiomassRatio = Table.Single(x => x.TreeSpecies == treeSpecies && x.Age == age).Ratio;
            if (aboveGroundBiomassTotalTreeBiomassRatio == 0)
            {
                var defaultValue = 0;
                Trace.TraceError($"{nameof(ShelterbeltAgTRatioProvider)}.{nameof(this.GetAboveGroundBiomassTotalTreeBiomassRatio)}" +
                    $" unable to get data for the tree species: {treeSpecies} and age: {age}." +
                    $" Returning default value of {defaultValue}.");
                return defaultValue;
            }
            return aboveGroundBiomassTotalTreeBiomassRatio;
        }

        #endregion
    }
}