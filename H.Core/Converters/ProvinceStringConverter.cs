using System;
using System.Diagnostics;
using H.Core.Enumerations;
using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Converters
{
    public class ProvinceStringConverter : ConverterBase
    {
        public Province Convert(string input)
        {
            switch (this.GetLettersAsLowerCase(input))
            {
                case "alberta":
                case "ab":
                case "alta":
                case "alb":
                    return Province.Alberta;
                case "britishcolumbia":
                case "colombiebritannique":
                case "bc":
                case "cb":
                    return Province.BritishColumbia;
                case "saskatchewan":
                case "sk":
                case "sask":
                    return Province.Saskatchewan;
                case "manitoba":
                case "mb":
                case "man":
                    return Province.Manitoba;
                case "ontario":
                case "on":
                case "ont":
                    return Province.Ontario;
                case "quebec":
                case "québec":
                case "qc":
                case "que":
                    return Province.Quebec;
                case "newbrunswick":
                case "nouveaubrunswick":
                case "nb":
                    return Province.NewBrunswick;
                case "novascotia":
                case "nouvelleécosse":
                case "nouvelleecosse":
                case "ns":
                case "né":
                case "ne":
                    return Province.NovaScotia;
                case "princeedwardisland":
                case "îleduprinceédouard":
                case "îleduprinceedouard":
                case "ileduprinceédouard":
                case "ileduprinceedouard":
                case "pe":
                case "pei":
                case "ipe":
                case "ipé":
                case "îpe":
                case "îpé":
                    return Province.PrinceEdwardIsland;
                case "newfoundlandandlabrador":
                case "terreneuveetlabrador":
                case "nl":
                case "nf":
                case "tnl":
                case "nfld":
                    return Province.Newfoundland;
                case "yukon":
                case "yt":
                case "yk":
                case "yuk":
                case "yn":
                    return Province.Yukon;
                case "northwestterritories":
                case "territoiresdunordouest":
                case "nt":
                case "tno":
                    return Province.NorthwestTerritories;
                case "nunavut":
                case "nu":
                case "nvt":
                    return Province.Nunavut;
                default:
                {
                    Trace.TraceError($"{nameof(ProvinceStringConverter)}.{nameof(ProvinceStringConverter.Convert)}: unknown input '{input}'. Returning default value of {Province.Alberta.GetDescription()}");

                    return Province.Alberta;
                }                    
            }            
        }
    }
}