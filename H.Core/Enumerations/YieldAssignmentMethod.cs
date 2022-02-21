using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum YieldAssignmentMethod
    {
        /// <summary>
        /// Takes the average of the yields from the crop view items of the field component
        /// </summary>
        [LocalizedDescription("EnumAverageYield", typeof(Resources))]
        Average,

        /// <summary>
        /// Uses the values entered for each year (by the user on the details screen)
        /// </summary>
        [LocalizedDescription("EnumCustomYield", typeof(Resources))]
        Custom,

        /// <summary>
        /// Uses CAR region values
        /// </summary>
        [LocalizedDescription("EnumCARValue", typeof(Resources))]
        CARValue,

        /// <summary>
        /// Reads a yield input file from the user
        /// </summary>
        [LocalizedDescription("EnumInputFile", typeof(Resources))]
        InputFile,

        /// <summary>
        /// Reads a yield input file from the user and the use the average of all crops in rotation
        /// </summary>
        [LocalizedDescription("EnumInputFileThenAverage", typeof(Resources))]
        InputFileThenAverage,

        /// <summary>
        /// Use Small Area Data (SAD) yields
        ///
        /// https://open.canada.ca/data/en/dataset/65f1cde1-95e0-4a1d-9a1a-c45b2f83a351
        /// </summary>
        [LocalizedDescription("EnumSmallAreaData", typeof(Resources))]
        SmallAreaData,
    }
}
