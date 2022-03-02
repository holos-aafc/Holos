using System;
using System.Diagnostics;
using H.Core.Enumerations;
using H.Core.Properties;

namespace H.Core.Converters
{
    public class AnimalTypeStringConverter : ConverterBase
    {
        public AnimalType Convert(string input)
        {
            switch (this.GetLettersAsLowerCase(input))
            {
                case "backgrounder":
                    return AnimalType.BeefBackgrounder;
                case "beef":
                    return AnimalType.Beef;
                case "beeffinisher":
                    return AnimalType.BeefFinisher;
                case "boar":
                case "swineboar":
                    return AnimalType.SwineBoar;
                case "chicken":
                    return AnimalType.Chicken;
                case "cowcalf":
                    return AnimalType.CowCalf;
                case "dairy":
                    return AnimalType.Dairy;
                case "dairybulls":
                    return AnimalType.DairyBulls;
                case "dairydry":
                case "dairydrycow":
                    return AnimalType.DairyDryCow;
                case "dairyheifers":
                    return AnimalType.DairyHeifers;
                case "dairylactating":
                    return AnimalType.DairyLactatingCow;
                case "drysow":
                    return AnimalType.SwineDrySow;
                case "ewe":
                case "ewes":
                    return AnimalType.Ewes;
                case "grower":
                case "swinegrower":
                    return AnimalType.SwineGrower;
                case "lactatingsow":
                    return AnimalType.SwineLactatingSow;
                case "ram":
                    return AnimalType.Ram;
                case "sheep":
                    return AnimalType.Sheep;
                case "stockers":
                    return AnimalType.Stockers;
                case "swine":
                    return AnimalType.Swine;
                case "swinefinisher":
                    return AnimalType.SwineFinisher;
                case "weanedlambs":
                    return AnimalType.Lambs;
                case "horse":
                    return AnimalType.Horses;
                case "turkey":
                    return AnimalType.Turkeys;
                case "goat":
                    return AnimalType.Goats;
                default:
                {
                    Trace.TraceError($"{nameof(AnimalTypeStringConverter)}.{nameof(AnimalTypeStringConverter.Convert)}: unknown animal type {input}. Returning {AnimalType.BeefBackgrounder}");

                    return AnimalType.BeefBackgrounder;
                }                    
            }
        }
    }
}