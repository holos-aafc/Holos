using System.Collections.ObjectModel;
using H.Core.Enumerations;
using System.Diagnostics;
using System.Linq;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;

namespace H.Core.Models
{
    public partial class Farm
    {
        #region Public Methods

        public AnaerobicDigestionComponent GetAnaerobicDigestionComponent()
        {
            if (this.AnaerobicDigestionComponents.Any())
            {
                return this.AnaerobicDigestionComponents.First();
            }
            else
            {
                return new AnaerobicDigestionComponent();
            }
        }

        #endregion
    }
}
