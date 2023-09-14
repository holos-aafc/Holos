#region Imports
#endregion

namespace H.Core.Enumerations
{
    /// <summary>
    /// </summary>
    public static class ProvinceExtension
    {
        #region Constructors

        #endregion

        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the province neighbour if possible.
        /// </summary>
        /// <param name="province">Province to get the neighbour for</param>
        /// <returns>Either the neighbouring province if it exists or the original province if no neighbour exists</returns>
        public static Province GetNeigbouringProvince(this Province province)
        {
            if (province.IsPrairieProvince())
            {
                switch (province)
                {
                    case Province.Alberta:
                        return Province.Saskatchewan;
                    case Province.Saskatchewan:
                        return Province.Manitoba;
                    case Province.Manitoba:
                        return Province.Saskatchewan;
                }
            }

            return province;
        }
        public static Region GetRegion(this Province province)
        {
            if (province == Province.Alberta ||
                province == Province.BritishColumbia ||
                province == Province.Manitoba ||
                province == Province.Saskatchewan ||
                province == Province.NorthwestTerritories ||
                province == Province.Nunavut)
            {
                return Region.WesternCanada;
            }

            return Region.EasternCanada;
        }

        public static bool IsPrairieProvince(this Province province)
        {
            return province == Province.Alberta ||
                   province == Province.Saskatchewan ||
                   province == Province.Manitoba;
        }

        public static bool IsEconomicProvince(this Province province)
        {
            return province.IsPrairieProvince() || province == Province.Ontario;
        }

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}