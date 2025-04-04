using System.Collections.Generic;
using System.Linq;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Enumerations
{
    public static class ManureApplicationViewItemExtensions
    {
        public static List<ManureApplicationViewItem> Get(this IEnumerable<ManureApplicationViewItem> viewItems, int year, ManureStateType manureStateType, AnimalType animalType, bool includeImports)
        {
            return viewItems.Where(
                manureApplicationViewItem => manureApplicationViewItem.DateOfApplication.Year == year &&
                                             manureApplicationViewItem.ManureStateType == manureStateType &&
                                             manureApplicationViewItem.AnimalType == animalType &&
                                             manureApplicationViewItem.IsImportedManure() == includeImports).ToList();
        }
    }
}