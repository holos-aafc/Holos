using H.Core.Models.LandManagement.Shelterbelt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Core.Services
{
    public class ShelterbeltComponentHelper : IShelterbeltComponentHelper
    {
        #region Public Methods
        
        /// <summary>
        /// Assigns each shelterbelt a unique name
        /// </summary>
        /// <param name="shelterbelts">All of the <see cref="ShelterbeltComponent"/>s in the farm</param>
        /// <returns>A unique name for the shelterbelts</returns>
        public string GetUniqueShelterbeltName(IEnumerable<ShelterbeltComponent> shelterbelts)
        {
            var increment = 1;
            var shelterbeltComponents = shelterbelts;
            var totalCount = shelterbeltComponents.Count();

            var proposedName = $"{Properties.Resources.LabelShelterbelt} #{increment}";
            while (shelterbeltComponents.Any(x => x.Name == proposedName))
            {
                proposedName = $"{Properties.Resources.LabelShelterbelt} #{++increment}";
            }
            return proposedName;
        } 

        #endregion
    }
}
