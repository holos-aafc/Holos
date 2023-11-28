using System.Collections.Generic;
using System.Linq;
using H.Infrastructure;

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

        #endregion
	}
}