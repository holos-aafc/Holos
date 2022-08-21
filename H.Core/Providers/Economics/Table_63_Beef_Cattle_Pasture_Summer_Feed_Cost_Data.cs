using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Core.Providers.Economics
{
    public class Table_63_Beef_Cattle_Pasture_Summer_Feed_Cost_Data : BeefCattleFeedCostData
    {
        /// <summary>
        /// The quality of the pasture grass
        /// </summary>
        public PastureType PastureType { get; set; }

        /// <summary>
        /// Feed related variable cost
        /// </summary>
        public double VariableCostFeed { get; set; }
    }
}
