using System.Linq;
using H.Core.Models.Infrastructure;

namespace H.Core.Models
{
    public partial class Farm
    {
        #region Public Methods

        public AnaerobicDigestionComponent GetAnaerobicDigestionComponent()
        {
            if (AnaerobicDigestionComponents.Any()) return AnaerobicDigestionComponents.First();

            return new AnaerobicDigestionComponent();
        }

        #endregion
    }
}