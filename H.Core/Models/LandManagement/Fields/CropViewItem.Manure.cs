using System.Collections.Generic;
using System.Linq;

namespace H.Core.Models.LandManagement.Fields
{
    public partial class CropViewItem
    {
        #region Public Methods

        public List<ManureApplicationViewItem> GetManureImportsByYear(int year)
        {
            var imports = ManureApplicationViewItems
                .Where(x => x.IsImportedManure() && x.DateOfApplication.Year == year).ToList();

            return new List<ManureApplicationViewItem>(imports);
        }

        public List<ManureApplicationViewItem> GetLocalSourcedApplications(int year)
        {
            var imports = ManureApplicationViewItems
                .Where(x => x.IsImportedManure() == false && x.DateOfApplication.Year == Year).ToList();

            return new List<ManureApplicationViewItem>(imports);
        }

        public double GetSumOfManureNitrogenApplied()
        {
            return ManureApplicationViewItems.Sum(x => x.AmountOfManureAppliedPerHectare) * Area;
        }

        #endregion
    }
}