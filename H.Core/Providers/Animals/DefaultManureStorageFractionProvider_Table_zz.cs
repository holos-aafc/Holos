using H.Core.Enumerations;
using H.Core.Tools;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table_zz:Fraction of organic N mineralized as TAN, and fraction of TAN immobilized to organic N and nitrified during manure storage (Chai et al., 2014)  
    /// </summary>
    public class DefaultManureStorageFractionProvider_Table_zz
    {
        private List<DefaultManureStorageFractionData> Data { get; set; } = new List<DefaultManureStorageFractionData>();
        public DefaultManureStorageFractionProvider_Table_zz()
        {
            HTraceListener.AddTraceListener(); 
            #region Compost
            this.Data.Add(new DefaultManureStorageFractionData()
            {
                StorageType = ManureStateType.Composted,
                FractionMineralizedAsTan = 0.46,
                FractionImmobilized = 0,
                FractionNitrified = 0.250

            });
            #endregion
          
            #region Stockpile/Pack
            this.Data.Add(new DefaultManureStorageFractionData()
            {
                StorageType = ManureStateType.SolidStorage,
                FractionMineralizedAsTan = 0.28,
                FractionImmobilized = 0,
                FractionNitrified = 0.145,
            }); 
            #endregion

        }

        public DefaultManureStorageFractionData GetDataByManureStorageType(ManureStateType manureStateType)
        {
            var dataByManureStorageType = this.Data.SingleOrDefault(x => x.StorageType == manureStateType);
            if (dataByManureStorageType == null)
            {
                var defaultValue = new DefaultManureStorageFractionData();
                Trace.TraceError($"{nameof(DefaultManureStorageFractionProvider_Table_zz)}.{nameof(DefaultManureStorageFractionProvider_Table_zz.GetDataByManureStorageType)}" +
                    $" unable to get data for manure state type: {manureStateType}." +
                    $" Returning default value of {defaultValue}.");
                return defaultValue;
            }
            return dataByManureStorageType;
        }
    }
}
