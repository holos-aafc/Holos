using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.Infrastructure
{
    public class ManureSubstrateViewItem : ModelBase
    {
        private AnimalType _animalType;
        private BeddingMaterialType _beddingMaterialType;
        private double _biomethanePotential;
        private double _methaneFraction;
        private double _volatileSolids;
        private double _totalSolids;
        private double _totalNitrogen;
        private double _dailyManureAddedToDigester;
        private double _flowRate;

        public AnimalType AnimalType
        {
            get => _animalType;
            set => this.SetProperty(ref _animalType, value);
        }

        public BeddingMaterialType BeddingMaterialType
        {
            get => _beddingMaterialType;
            set => this.SetProperty(ref _beddingMaterialType, value);
        }

        public double BiomethanePotential
        {
            get => _biomethanePotential;
            set => this.SetProperty(ref _biomethanePotential, value);
        }

        public double MethaneFraction
        {
            get => _methaneFraction;
            set => this.SetProperty(ref _methaneFraction, value);
        }

        public double VolatileSolids
        {
            get => _volatileSolids;
            set => this.SetProperty(ref _volatileSolids, value);
        }

        /// <summary>
        /// (kg t^-1)
        /// </summary>
        public double TotalSolids
        {
            get => _totalSolids;
            set => this.SetProperty(ref _totalSolids, value);
        }

        public double TotalNitrogen
        {
            get => _totalNitrogen;
            set => this.SetProperty(ref _totalNitrogen, value);
        }

        public double DailyManureAddedToDigester
        {
            get => _dailyManureAddedToDigester;
            set => this.SetProperty(ref _dailyManureAddedToDigester, value);
        }

        /// <summary>
        /// (kg or L day^-1)
        /// </summary>
        public double FlowRate
        {
            get => _flowRate;
            set => SetProperty(ref _flowRate, value);
        }
    }
}
