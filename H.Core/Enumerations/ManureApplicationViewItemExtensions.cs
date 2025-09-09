using System.Collections.Generic;
using System.Linq;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Enumerations
{
    public static class ManureApplicationViewItemExtensions
    {
        public static List<ManureApplicationViewItem> Get(this IEnumerable<ManureApplicationViewItem> viewItems,
            int year, ManureStateType manureStateType, AnimalType animalType, bool includeImports)
        {
            if (animalType.IsOtherAnimalType())
                // Manure tanks will use 'AnimalType.OtherLivestock' enum value but the manure application will have a specific animal type from the other livestock category (i.e. sheep)
                return viewItems.Where(
                        manureApplicationViewItem => manureApplicationViewItem.DateOfApplication.Year == year &&
                                                     manureApplicationViewItem.ManureStateType == manureStateType &&
                                                     manureApplicationViewItem.AnimalType.IsOtherAnimalType() &&
                                                     manureApplicationViewItem.IsImportedManure() == includeImports)
                    .ToList();

            return viewItems.Where(
                manureApplicationViewItem => manureApplicationViewItem.DateOfApplication.Year == year &&
                                             manureApplicationViewItem.ManureStateType == manureStateType &&
                                             manureApplicationViewItem.AnimalType == animalType &&
                                             manureApplicationViewItem.IsImportedManure() == includeImports).ToList();
        }
    }
}