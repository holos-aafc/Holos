using System.Collections.ObjectModel;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Services.LandManagement;
using H.Infrastructure;

namespace H.Core.Emissions.Results
{
    public class CropEnergyResults : ModelBase
    {
        #region Constructors

        public CropEnergyResults()
        {
            ManureSpreadingResults = new ObservableCollection<MonthlyManureSpreadingEmissions>();
        }

        #endregion

        #region Public Methods

        public double TotalManureSpreadingEmissionsForMonth(Months month)
        {
            return ManureSpreadingResults.Where(x => x.Month == (int)month).Sum(y => y.TotalEmissions);
        }

        #endregion

        #region Fields

        private double _energyCarbonDioxideFromFuelUse;
        private double _energyCarbonDioxideFromFertilizerUse;
        private double _energyCarbonDioxideFromPhosphorusFertilizer;
        private double _energyCarbonDioxideFromPotassiumFertilizer;
        private double _energyCarbonDioxideFromSulphurFertilizer;
        private double _energyCarbonDioxideFromIrrigation;
        private double _energyCarbonDioxideFromLime;

        private double _upstreamEnergyCarbonDioxideFromHerbicideUse;
        private double _upstreamEnergyFromFertilizerProduction;

        private ObservableCollection<MonthlyManureSpreadingEmissions> _manureSpreadingResults;

        #endregion

        #region Properties

        /// <summary>
        ///     Returns the sum of energy emissions from fuel use, fertilizer (N & P) application, irrigation, and manure spreading
        ///     operations
        ///     (kg CO2)
        /// </summary>
        public double TotalOnFarmCroppingEnergyEmissions =>
            EnergyCarbonDioxideFromFuelUse +
            EnergyCarbonDioxideFromFertilizerUse +
            EnergyCarbonDioxideFromPhosphorusFertilizer +
            EnergyCarbonDioxideFromPotassiumFertilizer +
            EnergyCarbonDioxideFromSulphurFertilizer +
            EnergyCarbonDioxideFromIrrigation +
            EnergyCarbonDioxideFromLimeUse +
            EnergyCarbonDioxideFromManureSpreading;

        /// <summary>
        ///     Returns the sum of upstream emissions resulting from herbicide and fertilizer production
        ///     (kg CO2)
        /// </summary>
        public double TotalUpstreamCroppingEnergyEmissions =>
            UpstreamEnergyCarbonDioxideFromHerbicideUse +
            UpstreamEnergyFromFertilizerProduction;

        public ObservableCollection<MonthlyManureSpreadingEmissions> ManureSpreadingResults
        {
            get => _manureSpreadingResults;
            set => SetProperty(ref _manureSpreadingResults, value);
        }

        /// <summary>
        ///     (kg CO2)
        /// </summary>
        public double EnergyCarbonDioxideFromFuelUse
        {
            get => _energyCarbonDioxideFromFuelUse;
            set => SetProperty(ref _energyCarbonDioxideFromFuelUse, value);
        }

        /// <summary>
        ///     This is an upstream emission
        ///     (kg CO2)
        /// </summary>
        public double UpstreamEnergyCarbonDioxideFromHerbicideUse
        {
            get => _upstreamEnergyCarbonDioxideFromHerbicideUse;
            set => SetProperty(ref _upstreamEnergyCarbonDioxideFromHerbicideUse, value);
        }

        /// <summary>
        ///     (kg CO2)
        /// </summary>
        public double EnergyCarbonDioxideFromFertilizerUse
        {
            get => _energyCarbonDioxideFromFertilizerUse;
            set => SetProperty(ref _energyCarbonDioxideFromFertilizerUse, value);
        }

        /// <summary>
        ///     (kg CO2)
        /// </summary>
        public double EnergyCarbonDioxideFromPhosphorusFertilizer
        {
            get => _energyCarbonDioxideFromPhosphorusFertilizer;
            set => SetProperty(ref _energyCarbonDioxideFromPhosphorusFertilizer, value);
        }

        /// <summary>
        ///     (kg CO2)
        /// </summary>
        public double EnergyCarbonDioxideFromIrrigation
        {
            get => _energyCarbonDioxideFromIrrigation;
            set => SetProperty(ref _energyCarbonDioxideFromIrrigation, value);
        }

        /// <summary>
        ///     (kg CO2)
        /// </summary>
        public double EnergyCarbonDioxideFromPotassiumFertilizer
        {
            get => _energyCarbonDioxideFromPotassiumFertilizer;
            set => SetProperty(ref _energyCarbonDioxideFromPotassiumFertilizer, value);
        }

        /// <summary>
        ///     (kg CO2)
        /// </summary>
        public double EnergyCarbonDioxideFromLimeUse
        {
            get => _energyCarbonDioxideFromLime;
            set => SetProperty(ref _energyCarbonDioxideFromLime, value);
        }

        /// <summary>
        ///     (kg CO2)
        /// </summary>
        public double EnergyCarbonDioxideFromSulphurFertilizer
        {
            get => _energyCarbonDioxideFromSulphurFertilizer;
            set => SetProperty(ref _energyCarbonDioxideFromSulphurFertilizer, value);
        }

        /// <summary>
        ///     (kg CO2)
        /// </summary>
        public double EnergyCarbonDioxideFromManureSpreading
        {
            get { return ManureSpreadingResults.Sum(x => x.TotalEmissions); }
        }

        public double UpstreamEnergyFromFertilizerProduction
        {
            get => _upstreamEnergyFromFertilizerProduction;
            set => SetProperty(ref _upstreamEnergyFromFertilizerProduction, value);
        }

        #endregion
    }
}