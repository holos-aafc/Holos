using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;
using System.Diagnostics;

namespace H.Core.Converters
{
    class FarmResidueTypeStringConverter : ConverterBase
    {
        public FarmResidueType Convert(string input)
        {
            switch (this.GetLettersAsLowerCase(input))
            {
                case "barleystraw":
                    return FarmResidueType.BarleyStraw;
                case "cornsilage":
                    return FarmResidueType.CornSilage;
                case "cornstover":
                    return FarmResidueType.CornStover;
                case "grassclippings":
                    return FarmResidueType.GrassClippings;
                case "grasssilage":
                    return FarmResidueType.GrassSilage;
                case "maizegrain":
                case "grainmaize":
                case "maize":
                    return FarmResidueType.MaizeGrain;
                case "maizesilage":
                case "silagemaize":
                    return FarmResidueType.MaizeSilage;
                case "oatstraw":
                    return FarmResidueType.OatStraw;
                case "oatsilage":
                    return FarmResidueType.OatSilage;
                case "paddystraw":
                    return FarmResidueType.PaddyStraw;
                case "ryestraw":
                    return FarmResidueType.RyeStraw;
                case "strawpellets":
                    return FarmResidueType.StrawPellets;
                case "strawsample":
                    return FarmResidueType.StrawSample;
                case "sunflowerresidues":
                    return FarmResidueType.SunflowerResidues;
                case "triticalestraw":
                    return FarmResidueType.TriticaleStraw;
                case "wheatstraw":
                    return FarmResidueType.WheatStraw;
                case "sweagesludge":
                    return FarmResidueType.SweageSludge;
                case "foodwaste":
                    return FarmResidueType.FoodWaste;
                case "vegetableoil":
                    return FarmResidueType.VegetableOil;

                default:
                {
                        Trace.TraceError($"{nameof(FarmResidueTypeStringConverter)}.{nameof(FarmResidueTypeStringConverter.Convert)} unknown residue type {input}. Returning {FarmResidueType.BarleyStraw}");
                        return FarmResidueType.BarleyStraw;
                }
            }
        }
    }
}
