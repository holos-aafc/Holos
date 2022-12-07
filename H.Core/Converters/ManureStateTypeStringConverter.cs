using System.Diagnostics;
using H.Core.Enumerations;

namespace H.Core.Converters
{
    public class ManureStateTypeStringConverter : ConverterBase
    {
        public ManureStateType Convert(string input)
        {
            switch (GetLettersAsLowerCase(input))
            {
                case "pasture":
                case "pasturerangepaddock":
                    return ManureStateType.Pasture;
                    
                case "deepbedding":
                    return ManureStateType.DeepBedding;

                case "solidstorage":
                case "solidstoragestockpiled":
                    return ManureStateType.SolidStorage;

                case "solidstoragewithorwithoutlitter":
                    return ManureStateType.SolidStorageWithOrWithoutLitter;

                case "compostedpassive":
                case "compostpassive":
                case "compostpassivewindrow":
                    return ManureStateType.CompostPassive;

                case "compostedintensive":
                case "compostintensive":
                case "compostintensivewindrow":
                    return ManureStateType.CompostIntensive;

                case "compostedinvessel":
                    return ManureStateType.CompostedInVessel;

                case "composted":
                    return ManureStateType.Composted;

                case "anaerobicdigestion":
                case "anaerobicdigestor":
                    return ManureStateType.AnaerobicDigester;

                case "deeppit":
                case "deeppitunderbarn":
                    return ManureStateType.DeepPit;

                case "liquidsolidcover":
                case "liquidwithsolidcover":
                case "liquidslurrywithsolidcover":
                    return ManureStateType.LiquidWithSolidCover;

                case "liquidnaturalcrust":
                case "liquidwithnaturalcrust":
                case "liquidslurrywithnaturalcrust":
                    return ManureStateType.LiquidWithNaturalCrust;

                case "liquidnocrust":
                case "liquidwithnocrust":
                case "liquidslurrywithnonaturalcrust":
                    return ManureStateType.LiquidNoCrust;

                case "dailyspread":
                    return ManureStateType.DailySpread;

                default:
                    Trace.TraceError($"{nameof(ManureStateTypeStringConverter)}.{nameof(Convert)} was not able to convert" +
                                     $" ManureStateType: {input}. Returning {ManureStateType.NotSelected}");
                    return ManureStateType.NotSelected;
            }
        }
    }
}
