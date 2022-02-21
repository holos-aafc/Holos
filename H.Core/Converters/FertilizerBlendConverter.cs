using System.Diagnostics;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Converters
{
    public class FertilizerBlendConverter : ConverterBase
    {
        public FertilizerBlends Convert(string input)
        {
            var lowerCase = input.ToLowerInvariant();
            switch (lowerCase)
            {
                case "urea":
                    return FertilizerBlends.Urea;

                case "ammonia":
                    return FertilizerBlends.Ammonia;

                case "urea ammonium nitrate":
                    return FertilizerBlends.UreaAmmoniumNitrate;

                case "ammonium nitrate":
                    return FertilizerBlends.AmmoniumNitrate;

                case "calcium ammonium nitrate":
                    return FertilizerBlends.CalciumAmmoniumNitrate;

                case "ammonium sulphate":
                    return FertilizerBlends.AmmoniumSulphate;

                case "mes":
                    return FertilizerBlends.MesS15;

                case "monoammonium phosphate":
                    return FertilizerBlends.MonoAmmoniumPhosphate;

                case "diammonium phosphate":
                    return FertilizerBlends.DiAmmoniumPhosphate;

                case "triple superphosphate":
                    return FertilizerBlends.TripleSuperPhosphate;

                case "potash":
                    return FertilizerBlends.Potash;

                case "npk":
                    return FertilizerBlends.Npk;

                case "calcium nitrate":
                    return FertilizerBlends.CalciumNitrate;

                case "ammonium nitrosulphate":
                    return FertilizerBlends.AmmoniumNitroSulphate;

                default:
                {
                    Trace.TraceError($"{nameof(FertilizerBlendConverter)}.{nameof(FertilizerBlendConverter.Convert)}: unknown input '{input}'. Returning default value of {FertilizerBlends.Urea.GetDescription()}");

                    return FertilizerBlends.Urea;
                }
            }
        }
    }
}