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
            var cleanedInput = this.GetLettersAsLowerCase(input);

            switch (cleanedInput)
            {
                /*
                 * Beef cattle
                 */

                case "backgrounding":
                case "backgrounder":
                    return AnimalType.BeefBackgrounder;
                case "beef":
                case "nondairycattle":
                case "beefcattle":
                    return AnimalType.Beef;
                case "beeffinisher":
                case "finisher":
                    return AnimalType.BeefFinisher;
                case "cowcalf":
                    return AnimalType.CowCalf;
                case "stockers":
                    return AnimalType.Stockers;
                case "beefcalves":
                case "beefcalf":
                    return AnimalType.BeefCalf;

                /*
                 * Dairy
                 */

                case "dairy":
                case "dairycattle":
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

                /*
                 * Swine
                 */

                case "boar":
                case "swineboar":
                    return AnimalType.SwineBoar;
                case "piglets":
                    return AnimalType.SwinePiglets;
                case "drysow":
                    return AnimalType.SwineDrySow;
                case "sow":
                    return AnimalType.SwineSows;
                case "grower":
                case "hogs":
                case "swinegrower":
                    return AnimalType.SwineGrower;
                case "lactatingsow":
                    return AnimalType.SwineLactatingSow;
                case "swine":
                    return AnimalType.Swine;
                case "swinefinisher":
                    return AnimalType.SwineFinisher;

                /*
                 * Sheep
                 */

                case "ewe":
                case "ewes":
                    return AnimalType.Ewes;
                case "ram":
                    return AnimalType.Ram;
                case "sheep":
                case "sheepandlambs":
                    return AnimalType.Sheep;
                case "weanedlambs":
                    return AnimalType.Lambs;

                /*
                 * Other livestock
                 */

                case "horse":
                case "horses":
                    return AnimalType.Horses;
                case "goat":
                case "goats":
                    return AnimalType.Goats;
                case "mules":
                case "mule":
                    return AnimalType.Mules;
                case "bull":
                    return AnimalType.BeefBulls;
                case "llamas":
                    return AnimalType.Llamas;
                case "alpacas":
                    return AnimalType.Alpacas;
                case "deer":
                    return AnimalType.Deer;
                case "elk":
                    return AnimalType.Elk;
                case "bison":
                    return AnimalType.Bison;

                /*
                 * Poultry
                 */

                case "poultry":
                    return AnimalType.Poultry;
                case "poultrypulletsbroilers":
                case "broilers":
                    return AnimalType.Broilers;
                case "pullets":
                    return AnimalType.ChickenPullets;
                case "chicken":
                    return AnimalType.Chicken;
                case "cockerels":
                    return AnimalType.ChickenCockerels;
                case "roasters":
                    return AnimalType.ChickenRoosters;
                case "hens":
                    return AnimalType.ChickenHens;
                case "poultryturkeys":
                case "turkey":
                    return AnimalType.Turkeys;
                case "layersdry":
                    return AnimalType.LayersDryPoultry;
                case "layerswet":
                    return AnimalType.LayersWetPoultry;
                case "poultrylayers":
                case "layers":
                    return AnimalType.Layers;

                default:
                {
                    Trace.TraceError($"{nameof(AnimalTypeStringConverter)}.{nameof(AnimalTypeStringConverter.Convert)}: unknown animal type {input}. Returning {AnimalType.BeefBackgrounder}");

                    return AnimalType.BeefBackgrounder;
                }                    
            }
        }
    }
}