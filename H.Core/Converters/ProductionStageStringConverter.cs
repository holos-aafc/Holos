using System.Diagnostics;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Converters
{
    public class ProductionStageStringConverter : ConverterBase
    {
        public ProductionStages Convert(string input)
        {
            var cleanedInput = base.GetLettersAsLowerCase(input);
            switch (cleanedInput)
            {
                case "gestating":
                    return ProductionStages.Gestating;

                case "lactating":
                    return ProductionStages.Lactating;

                case "open":
                    return ProductionStages.Open;

                case "weaning":
                    return ProductionStages.Weaning;

                case "growingandfinishing":
                    return ProductionStages.GrowingAndFinishing;

                case "breedingstock":
                    return ProductionStages.BreedingStock;

                case "weaned":
                    return ProductionStages.Weaned;

                default:
                {
                        ProductionStages notFound = ProductionStages.Gestating;

                    Trace.TraceError($"{nameof(ProductionStageStringConverter)}.{nameof(ProductionStageStringConverter.Convert)}: unknown production stage '{input}'. Returning {notFound.GetDescription()}");

                    return notFound;
                }
            }
        }
    }
}