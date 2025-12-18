using System.Collections.Generic;
using H.Core.Models;
using H.Infrastructure;

namespace H.Core.Emissions.Results
{
    public class LandUseChangeResults : ModelBase
    {
        #region Fields

        private SoilCarbonEmissionResult _carbonDioxideFromTillageChange;
        private SoilCarbonEmissionResult _carbonDioxideFromFallowChange;
        private SoilCarbonEmissionResult _carbonDioxideFromPastPerennials;
        private SoilCarbonEmissionResult _carbonDioxideFromCurrentPerennials;
        private SoilCarbonEmissionResult _carbonDioxideFromSeedGrassland;
        private SoilCarbonEmissionResult _carbonDioxideFromBrokenGrassland;

        #endregion

        public LandUseChangeResults()
        {
            this.CarbonDioxideFromTillageChange = new SoilCarbonEmissionResult();
            this.CarbonDioxideFromFallowChange = new SoilCarbonEmissionResult();
            this.CarbonDioxideFromPastPerennials = new SoilCarbonEmissionResult();
            this.CarbonDioxideFromCurrentPerennials = new SoilCarbonEmissionResult();
            this.CarbonDioxideFromSeedGrassland = new SoilCarbonEmissionResult();
            this.CarbonDioxideFromBrokenGrassland = new SoilCarbonEmissionResult();
        }

        #region Properties

        /// <summary>
        /// Returns all land use change results for the field
        /// </summary>
        public List<SoilCarbonEmissionResult> GetAllLandUseChangeResultsForField
        {
            get
            {
                return new List<SoilCarbonEmissionResult>()
                {
                    this.CarbonDioxideFromTillageChange,
                    this.CarbonDioxideFromFallowChange,
                    this.CarbonDioxideFromPastPerennials,
                    this.CarbonDioxideFromCurrentPerennials,
                    this.CarbonDioxideFromSeedGrassland,
                    this.CarbonDioxideFromBrokenGrassland,
                };
            }
        }

        public SoilCarbonEmissionResult CarbonDioxideFromTillageChange
        {
            get => _carbonDioxideFromTillageChange;
            set => SetProperty(ref _carbonDioxideFromTillageChange, value);
        }

        public SoilCarbonEmissionResult CarbonDioxideFromFallowChange
        {
            get => _carbonDioxideFromFallowChange;
            set => SetProperty(ref _carbonDioxideFromFallowChange, value);
        }

        public SoilCarbonEmissionResult CarbonDioxideFromPastPerennials
        {
            get => _carbonDioxideFromPastPerennials;
            set => SetProperty(ref _carbonDioxideFromPastPerennials, value);
        }

        public SoilCarbonEmissionResult CarbonDioxideFromCurrentPerennials
        {
            get => _carbonDioxideFromCurrentPerennials;
            set => SetProperty(ref _carbonDioxideFromCurrentPerennials, value);
        }

        public SoilCarbonEmissionResult CarbonDioxideFromSeedGrassland
        {
            get => _carbonDioxideFromSeedGrassland;
            set => SetProperty(ref _carbonDioxideFromSeedGrassland, value);
        }

        public SoilCarbonEmissionResult CarbonDioxideFromBrokenGrassland
        {
            get => _carbonDioxideFromBrokenGrassland;
            set => SetProperty(ref _carbonDioxideFromBrokenGrassland, value);
        }

        #endregion
    }
}