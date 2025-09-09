using System.Collections.Generic;
using System.Diagnostics;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Converters
{
    public class ProductionStageStringConverter : ConverterBase
    {
        #region Properties

        public static Dictionary<string, ProductionStages> Cache { get; set; } =
            new Dictionary<string, ProductionStages>();

        #endregion

        public ProductionStages Convert(string input)
        {
            ProductionStages result;

            if (Cache.ContainsKey(input)) return Cache[input];

            var cleanedInput = GetLettersAsLowerCase(input);
            switch (cleanedInput)
            {
                case "gestating":
                    result = ProductionStages.Gestating;
                    break;

                case "lactating":
                    result = ProductionStages.Lactating;
                    break;

                case "open":
                    result = ProductionStages.Open;
                    break;

                case "weaning":
                    result = ProductionStages.Weaning;
                    break;

                case "growingandfinishing":
                    result = ProductionStages.GrowingAndFinishing;
                    break;

                case "breedingstock":
                    result = ProductionStages.BreedingStock;
                    break;

                case "weaned":
                    result = ProductionStages.Weaned;
                    break;

                default:
                {
                    var notFound = ProductionStages.Gestating;

                    Trace.TraceError(
                        $"{nameof(ProductionStageStringConverter)}.{nameof(Convert)}: unknown production stage '{input}'. result = ing {notFound.GetDescription()}");

                    result = notFound;

                    break;
                }
            }

            Cache.Add(input, result);

            return result;
        }
    }
}