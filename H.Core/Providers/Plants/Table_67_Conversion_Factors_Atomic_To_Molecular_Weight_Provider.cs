using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Plants
{
    /// <summary>
    /// Table 67
    /// 
    /// A revised utilization rate table for grazed systems, where the utilization rate depends on the type of grazed perennial
    /// forage (rather than the grazing regime or stocking density), and this in turn can be used to back-calculate aboveground pasture biomass.
    /// </summary>
    public class Table_67_Conversion_Factors_Atomic_To_Molecular_Weight_Provider
    {
        public double GetUtilizationRate(CropType cropType)
        {
            switch (cropType)
            {
                case CropType.RangelandNative:
                    return 45;
                case CropType.SeededGrassland:
                    return 50;

                case CropType.TameGrass:
                case CropType.TameLegume:
                case CropType.TameMixed:
                    return 60;

                default:
                {
                    Trace.TraceError($"No data found for '{cropType.GetDescription()}'");

                    return 0;
                }
            }
        }
    }
}
