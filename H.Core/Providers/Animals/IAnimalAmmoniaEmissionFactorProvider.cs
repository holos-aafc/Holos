using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Providers.Animals
{
    public interface IAnimalAmmoniaEmissionFactorProvider
    {
        /// <summary>
        /// Provides the default ammonia emission factor (kg NH3-N kg^-1 TAN) for the given housing type
        /// </summary>
        /// <param name="housingType">The housing type</param>
        /// <returns>The ammonia emission factor for the particular housing type</returns>
        double GetEmissionFactorByHousing(HousingType housingType);

        /// <summary>
        /// Provides the default ammonia emission factor (kg NH3-N kg^-1 TAN) for the given manure storage type
        /// </summary>
        /// <param name="storageType">The manure storage type</param>
        /// <returns>The ammonia emission factor for the particular storage type (kg NH3-N kg^-1 TAN)</returns>
        double GetByManureStorageType(ManureStateType storageType);

        double GetEmissionFactorForLandAppliedManure(ManureApplicationViewItem manureApplicationViewItem);

        /// <summary>
        /// For liquid manure types, the emission factor will depend on the type of manure application method being used.
        /// </summary>
        /// <param name="manureApplicationType">The manure application method</param>
        /// <returns>The ammonia emission factor for the particular application method (kg NH3-N kg^-1 TAN)</returns>
        double GetAmmoniaEmissionFactorForLandAppliedManure(ManureApplicationTypes manureApplicationType);
    }
}