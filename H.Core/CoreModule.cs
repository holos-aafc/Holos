#region Imports

using H.Core.Calculators.Carbon;
using H.Core.Calculators.Climate;
using H.Core.Calculators.Tillage;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Providers;
using H.Core.Providers.Carbon;
using H.Core.Providers.Climate;
using H.Core.Providers.Feed;
using H.Core.Providers.Soil;
using Prism.Ioc;
using Prism.Modularity;

#endregion

namespace H.Core
{
    /// <summary>
    ///     Load services from the project into the container.
    /// </summary>
    public class CoreModule : IModule
    {
        #region Public Methods

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IClimateParameterCalculator, ClimateParameterCalculator>();
            containerRegistry.RegisterSingleton<ITillageFactorCalculator, TillageFactorCalculator>();
            containerRegistry.RegisterSingleton<IICBMSoilCarbonCalculator, ICBMSoilCarbonCalculator>();

            containerRegistry.RegisterSingleton<ICustomFileClimateDataProvider, CustomFileClimateDataProvider>();
            containerRegistry.RegisterSingleton<IDietProvider, DietProvider>();
            containerRegistry.RegisterSingleton<IResidueDataProvider, Table_7_Relative_Biomass_Information_Provider>();
            containerRegistry.RegisterSingleton<ISoilDataProvider, NationalSoilDataBaseProvider>();
            containerRegistry.RegisterSingleton<IGeographicDataProvider, GeographicDataProvider>();
            containerRegistry.RegisterSingleton<IFeedIngredientProvider, FeedIngredientProvider>();

            containerRegistry.RegisterSingleton<IUnitsOfMeasurementCalculator, UnitsOfMeasurementCalculator>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        #endregion
    }
}