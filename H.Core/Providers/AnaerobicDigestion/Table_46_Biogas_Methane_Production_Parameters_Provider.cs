using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Providers.Carbon;
using H.Core.Providers.Climate;
using H.Infrastructure;

namespace H.Core.Providers.AnaerobicDigestion
{
    /// <summary>
    /// Table 46
    ///
    /// Parameters used for the calculation of biogas and methane production using an anaerobic digestion system
    /// </summary>
    public class Table_46_Biogas_Methane_Production_Parameters_Provider
    {
        #region Fields

        private readonly FarmResidueTypeStringConverter _farmResidueTypeStringConverter;
        private readonly BeddingMaterialTypeStringConverter _beddingMaterialTypeStringConverter;
        private readonly AnimalTypeStringConverter _animalTypeStringConverter;
        private readonly Table_7_Relative_Biomass_Information_Provider _table7RelativeBiomassInformationProvider;

        #endregion

        #region Constructors

        public Table_46_Biogas_Methane_Production_Parameters_Provider()
        {
            _farmResidueTypeStringConverter = new FarmResidueTypeStringConverter();
            _beddingMaterialTypeStringConverter = new BeddingMaterialTypeStringConverter();
            _animalTypeStringConverter = new AnimalTypeStringConverter();
            _table7RelativeBiomassInformationProvider = new Table_7_Relative_Biomass_Information_Provider();
            
            this.ReadFile();
        }

        #endregion

        #region Properties
        private List<Table_46_Biogas_Methane_Production_Manure_Data> ManureData { get; set; }
        private List<Table_46_Biogas_Methane_Production_FarmResidue_Data> FarmResiduesData { get; set; }
        private List<Table_46_Biogas_Methane_Production_CropResidue_Data> CropResiduesData { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds a manure substrate type by taking in an <see cref="AnimalType" /> and a <see cref="BeddingMaterialType"/>.
        /// </summary>
        /// <param name="animalType">The AnimalType whose manure we need the data for.</param>
        /// <param name="beddingMaterial">The bedding material for the animal. Specify 'None' for no bedding material.</param>
        /// <returns>Returns a single instance of <see cref="BiogasAndMethaneProductionParametersData"/> based on the parameters specified.</returns>
        public BiogasAndMethaneProductionParametersData GetBiogasMethaneProductionInstance(AnimalType animalType, BeddingMaterialType beddingMaterial)
        {
            AnimalType lookupAnimalType;
            if (animalType.IsBeefCattleType())
            {
                lookupAnimalType = AnimalType.Beef;
            }
            else if (animalType.IsDairyCattleType())
            {
                lookupAnimalType = AnimalType.Dairy;
            }
            else if (animalType.IsSwineType())
            {
                lookupAnimalType = AnimalType.Swine;
            }
            else if (animalType.IsChickenType())
            {
                lookupAnimalType = AnimalType.Chicken;
            }
            else if (animalType.IsTurkeyType())
            {
                lookupAnimalType = AnimalType.Turkeys;
            }
            else
            {
                // Horses, and goats are only other values in table
                lookupAnimalType = animalType;
            }

            BiogasAndMethaneProductionParametersData data = this.ManureData.Find(x => (x.AnimalType == lookupAnimalType) && (x.BeddingType == beddingMaterial));

            if (data != null)
            {
                return data;
            }

            data = this.ManureData.Find(x => x.AnimalType == lookupAnimalType);

            if (data != null)
            {
                return data;
            }
            else
            {
                Trace.TraceError($"{nameof(Table_46_Biogas_Methane_Production_Parameters_Provider)}.{nameof(Table_46_Biogas_Methane_Production_Parameters_Provider.GetBiogasMethaneProductionInstance)}" +
                    $" does not contain AnimalType of {lookupAnimalType}. Returning an empty instance of {nameof(Table_46_Biogas_Methane_Production_Parameters_Provider)}");
            }

            return new BiogasAndMethaneProductionParametersData();
        }

        /// <summary>
        /// Finds a farm residues substrate type by taking in a <see cref="FarmResidueType"/> as the parameter.
        /// Unit of measurement: Biomethane potential = Nm3 ton-1 VS
        /// </summary>
        /// <param name="residueType">The farm residue type for which we need the required data values.</param>
        /// <returns>Returns a single instance of <see cref="BiogasAndMethaneProductionParametersData"/> based on the parameters specified. Returns an empty instance otherwise.</returns>
        public BiogasAndMethaneProductionParametersData GetBiogasMethaneProductionInstance(FarmResidueType residueType)
        {
            BiogasAndMethaneProductionParametersData data = this.FarmResiduesData.Find(x => x.ResidueType == residueType);

            if (data != null)
            {
                return data;
            }

            Trace.TraceError($"{nameof(Table_46_Biogas_Methane_Production_Parameters_Provider)}.{nameof(Table_46_Biogas_Methane_Production_Parameters_Provider.GetBiogasMethaneProductionInstance)}" +
             $" does not contain FarmResidueType of {residueType}. Returning an empty instance of {nameof(Table_46_Biogas_Methane_Production_Parameters_Provider)}");

            return new BiogasAndMethaneProductionParametersData();

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the csv file and stores data into two lists. Each list corresponds to Manure Type and Farm Residue Type substrates entries in the csv respectively.
        /// </summary>
        private void ReadFile()
        {
            // If more manure type substrates are added to csv file, increase the value of int const LastManureRow accordingly.
            // If LastManureRow is changed, also change the lines the foreach loop skips to get to farm residue substrate types.

            List<Table_46_Biogas_Methane_Production_Manure_Data> manureData = new List<Table_46_Biogas_Methane_Production_Manure_Data>();
            List<Table_46_Biogas_Methane_Production_FarmResidue_Data> residueData = new List<Table_46_Biogas_Methane_Production_FarmResidue_Data>();

            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            IEnumerable<string[]> fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.ParametersBiogasMethaneProduction);

            double biomethanePotential, methaneFraction, volatileSolids, totalSolids, totalNitrogen;

            // LastManureRow indicates the last row with a manure type substrate in the csv file.
            const int LastManureRow = 10;
            // Store the first half of the file into a manure type data list. Row's value is based on the location of the first row containing manure data in the csv.
            for (int row = 2; row < LastManureRow; row++)
            {
                string[] line = fileLines.ElementAt(row);

                AnimalType animalType = _animalTypeStringConverter.Convert(line[0]);
                BeddingMaterialType beddingType = _beddingMaterialTypeStringConverter.Convert(line[1]);
                biomethanePotential = double.Parse(line[2], cultureInfo);
                methaneFraction = double.Parse(line[3], cultureInfo);
                volatileSolids = double.Parse(line[4], cultureInfo);
                totalSolids = double.Parse(line[5], cultureInfo);
                totalNitrogen = double.Parse(line[6], cultureInfo);

                manureData.Add(new Table_46_Biogas_Methane_Production_Manure_Data
                {
                    AnimalType = animalType,
                    BeddingType = beddingType,
                    BioMethanePotential = biomethanePotential,
                    MethaneFraction = methaneFraction,
                    VolatileSolids = volatileSolids,
                    TotalSolids = totalSolids,
                    TotalNitrogen = totalNitrogen,
                });
            }

            // Store the second half of the file into a farm residue type data list.
            foreach (string[] line in fileLines.Skip(11).Take(6))
            {
                FarmResidueType residueType = _farmResidueTypeStringConverter.Convert(line[0]);
                biomethanePotential = double.Parse(line[2], cultureInfo);
                methaneFraction = double.Parse(line[3], cultureInfo);
                volatileSolids = double.Parse(line[4], cultureInfo);
                totalSolids = double.Parse(line[5], cultureInfo);
                totalNitrogen = double.Parse(line[6], cultureInfo);

                residueData.Add(new Table_46_Biogas_Methane_Production_FarmResidue_Data
                {
                    ResidueType = residueType,
                    BioMethanePotential = biomethanePotential,
                    MethaneFraction = methaneFraction,
                    VolatileSolids = volatileSolids,
                    TotalSolids = totalSolids,
                    TotalNitrogen = totalNitrogen,
                });
            }
            
            ManureData = manureData;
            FarmResiduesData = residueData;
        }

        #endregion
    }
}
