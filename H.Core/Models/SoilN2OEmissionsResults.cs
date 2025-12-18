using H.Core.Models.LandManagement.Fields;
using H.Infrastructure;

namespace H.Core.Models
{
    public class SoilN2OEmissionsResults : ModelBase
    {
        #region Fields

        private FieldSystemComponent _fieldSystemComponent;

        private double _directN2OEmissions;
        private double _indirectN2OEmissions;

        private LandEmissionSourceType _landEmissionSource;

        #endregion

        #region Properties

        public FieldSystemComponent FieldSystemComponent
        {
            get
            {
                return _fieldSystemComponent;
            }
            set
            {
                SetProperty(ref _fieldSystemComponent, value);
            }
        }

        /// <summary>
        /// Direct N20 emissions
        ///
        /// (kg N20)
        /// </summary>
        public double DirectN2OEmissions
        {
            get
            {
                return _directN2OEmissions;
            }
            set
            {
                SetProperty(ref _directN2OEmissions, value);
            }
        }

        /// <summary>
        /// Indirect N₂0 emissions (kg N₂0 year⁻¹)
        /// </summary>
        public double IndirectN2OEmissions
        {
            get
            {
                return _indirectN2OEmissions;
            }
            set
            {
                SetProperty(ref _indirectN2OEmissions, value);
            }
        }

        public LandEmissionSourceType LandEmissionSource
        {
            get
            {
                return _landEmissionSource;
            }
            set
            {
                SetProperty(ref _landEmissionSource, value);
            }
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(DirectN2OEmissions)}: {DirectN2OEmissions}, {nameof(IndirectN2OEmissions)}: {IndirectN2OEmissions}, {nameof(LandEmissionSource)}: {LandEmissionSource}";
        }

        #endregion
    }
}