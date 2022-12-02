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
                    return ManureStateType.Pasture;
                    
                case "deepbedding":
                    return ManureStateType.DeepBedding;

                case "solidstorage":
                    return ManureStateType.SolidStorage;

                case "compostedpassive":
                case "compostpassive":
                    return ManureStateType.CompostPassive;

                case "compostedintensive":
                case "compostintensive":
                    return ManureStateType.CompostIntensive;

                case "composted":
                    return ManureStateType.Composted;

                case "liquid":
                    return ManureStateType.Liquid;

                case "slurry":
                    return ManureStateType.Slurry;

                case "liquidseparated":
                    return ManureStateType.LiquidSeparated;

                case "anaerobicdigestion":
                case "anaerobicdigestor":
                    return ManureStateType.AnaerobicDigester;

                case "deeppit":
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
