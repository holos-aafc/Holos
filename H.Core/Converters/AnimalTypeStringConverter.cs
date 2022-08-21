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
                case "beefcattle":
                    return AnimalType.Beef;
                case "beeffinisher":
                case "finisher":
                    return AnimalType.BeefFinisher;
                case "boar":
                case "swineboar":
                    return AnimalType.SwineBoar;
                case "chicken":
                    return AnimalType.Chicken;
                case "cowcalf":
                    return AnimalType.CowCalf;
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
                case "horses":
                    return AnimalType.Horses;
                case "turkey":
                    return AnimalType.Turkeys;
                case "goat":
                case "goats":
                    return AnimalType.Goats;
                case "mules":
                case "mule":
                    return AnimalType.Mules;
                case "poultry":
                    return AnimalType.Poultry;
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
                case "beefcalves":
                case "beefcalf":
                    return AnimalType.BeefCalf;

                case "pullets":
                    return AnimalType.ChickenPullets;
                case "broilers":
                    return AnimalType.Broilers;
                case "cockerels":
                    return AnimalType.ChickenCockerels;
                case "roasters":
                    return AnimalType.ChickenRoosters;
                case "hens":
                    return AnimalType.ChickenHens;
                case "layersdry":
                    return AnimalType.LayersDryPoultry;
                case "layerswet":
                    return AnimalType.LayersWetPoultry;
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