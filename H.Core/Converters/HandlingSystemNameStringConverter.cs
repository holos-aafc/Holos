using System;
using H.Core.Enumerations;
using H.Core.Properties;

namespace H.Core.Converters
{
    public class HandlingSystemNameStringConverter : ConverterBase
    {
        public ManureStateType Convert(string input)
        {
            switch (this.GetLettersAsLowerCase(input))
            {
                case "anaerobicdigester":
                    return ManureStateType.AnaerobicDigester;
                case "compostintensive":
                    return ManureStateType.CompostIntensive;
                case "compostpassive":
                    return ManureStateType.CompostPassive;
                case "dailyspread":
                    return ManureStateType.DailySpread;
                case "deepbedding":
                    return ManureStateType.DeepBedding;
                case "deeppit":
                    return ManureStateType.DeepPit;
                case "liquidcrust":
                    return ManureStateType.LiquidCrust;
                case "liquidnocrust":
                    return ManureStateType.LiquidNoCrust;
                case "pasture":
                    return ManureStateType.Pasture;
                case "solidstorage":
                    return ManureStateType.SolidStorage;
                default:
                    throw new Exception(string.Format(Resources.ExceptionUnknownHandlingSystemName, input));
            }
        }
    }
}