#region Imports

using H.Core.Calculators.Carbon;
using H.Core.Calculators.Climate;
using H.Core.Calculators.Nitrogen;
using H.Core.Calculators.Tillage;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Providers;
using H.Core.Providers.Carbon;
using H.Core.Providers.Climate;
using H.Core.Providers.Evapotranspiration;
using H.Core.Providers.Feed;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Soil;
using H.Core.Providers.Temperature;
using H.Core.Services;
using H.Core.Services.LandManagement;
using Prism.Ioc;
using Prism.Modularity;

#endregion

namespace H.Core
{
    /// <summary>
    /// Load services from the project into the container.
    /// </summary>
    public class CoreModule : IModule
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IClimateParameterCalculator, ClimateParameterCalculator>();
            containerRegistry.RegisterSingleton<ITillageFactorCalculator, TillageFactorCalculator>();
            containerRegistry.RegisterSingleton<IICBMSoilCarbonCalculator, ICBMSoilCarbonCalculator>();            

            containerRegistry.RegisterSingleton<ICustomFileClimateDataProvider, CustomFileClimateDataProvider>();
            containerRegistry.RegisterSingleton<IDietProvider, DietProvider>();
            containerRegistry.RegisterSingleton<IResidueDataProvider, Table_10_Relative_Biomass_Provider>();
            containerRegistry.RegisterSingleton<ISoilDataProvider, NationalSoilDataBaseProvider>();
            containerRegistry.RegisterSingleton<IGeographicDataProvider, GeographicDataProvider>();
            containerRegistry.RegisterSingleton<IFeedIngredientProvider, FeedIngredientProvider>();

            containerRegistry.RegisterSingleton<IUnitsOfMeasurementCalculator, UnitsOfMeasurementCalculator>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}