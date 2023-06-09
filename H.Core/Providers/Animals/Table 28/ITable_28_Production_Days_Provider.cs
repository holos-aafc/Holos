using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Providers.Animals.Table_28
{
    public interface ITable_28_Production_Days_Provider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="managementPeriod"></param>
        /// <param name="componentType"></param>
        /// <returns></returns>
        ProductionDaysData GetData(ManagementPeriod managementPeriod, ComponentType? componentType = null);
    }
}