using System.Collections.Generic;
using System.Linq;

namespace H.Core.Models.LandManagement.Fields
{
	public partial class CropViewItem
	{
        #region Public Methods

        public List<ManureApplicationViewItem> GetManureImportsByYear(int year)
        {
            var imports = this.ManureApplicationViewItems.Where(x => x.IsImportedManure() && x.DateOfApplication.Year == year).ToList();

            return new List<ManureApplicationViewItem>(imports);
        }

        public List<ManureApplicationViewItem> GetLocalSourcedApplications(int year)
        {
            var imports = this.ManureApplicationViewItems.Where(x => x.IsImportedManure() == false && x.DateOfApplication.Year == this.Year).ToList();

            return new List<ManureApplicationViewItem>(imports);
        }

        public double GetSumOfManureNitrogenApplied()
        {
            return this.ManureApplicationViewItems.Sum(x => x.AmountOfManureAppliedPerHectare) * this.Area;
        }

        #endregion
	}
}