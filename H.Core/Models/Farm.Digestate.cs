using System.Linq;
using H.Core.Models.Infrastructure;

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
