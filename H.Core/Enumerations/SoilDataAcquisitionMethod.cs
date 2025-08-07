namespace H.Core.Enumerations
{
    public enum SoilDataAcquisitionMethod
    {
        // Use .NET 9

        /// <summary>
        /// Default (currently uses SLC method)
        /// </summary>
        Default = 0,

        /// <summary>
        /// Get data from SLC database according to polygon ID
        /// </summary>
        SLC,

        /// <summary>
        /// Read data from settings file
        /// </summary>
        Custom,
    }
}