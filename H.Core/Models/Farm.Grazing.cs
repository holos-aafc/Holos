using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Core.Models
{
    public partial class Farm
    {
        #region Public Methods

        public List<int> GetYearsWithGrazingAnimals()
        {
            var list = new List<int>();

            foreach (var fieldSystemComponent in this.FieldSystemComponents)
            {
                foreach (var cropViewItem in fieldSystemComponent.CropViewItems)
                {
                    foreach (var grazingViewItem in cropViewItem.GrazingViewItems)
                    {
                        if (list.Contains(grazingViewItem.Start.Year) == false)
                        {
                            list.Add(grazingViewItem.Start.Year);
                        }
                    }
                }
            }

            return list;
        }

        #endregion
    }
}
