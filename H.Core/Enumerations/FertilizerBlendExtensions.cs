namespace H.Core.Enumerations
{
    public static class FertilizerBlendExtensions
    {
        #region Public Methods

        /// <summary>
        /// (N)
        /// </summary>
        public static bool IsNitrogenFertilizer(this FertilizerBlends fertilizerBlend)
        {
            switch (fertilizerBlend)
            {
                case FertilizerBlends.Ammonia:
                case FertilizerBlends.Urea:
                case FertilizerBlends.UreaAmmoniumNitrate:
                case FertilizerBlends.AmmoniumNitrate:
                case FertilizerBlends.AmmoniumNitratePrilled:
                case FertilizerBlends.AmmoniumNitrateGranulated:
                case FertilizerBlends.CalciumAmmoniumNitrate:
                case FertilizerBlends.AmmoniumSulphate:
                case FertilizerBlends.CalciumNitrate:
                case FertilizerBlends.AmmoniumNitroSulphate:
                case FertilizerBlends.MesS15:
                case FertilizerBlends.MonoAmmoniumPhosphate:
                {
                    return true;
                }

                default:
                    return false;
            }
        }

        /// <summary>
        /// (P2O5)
        /// </summary>
        public static bool IsPhosphorusFertilizer(this FertilizerBlends fertilizerBlend)
        {
            switch (fertilizerBlend)
            {
                case FertilizerBlends.DiAmmoniumPhosphate:
                case FertilizerBlends.TripleSuperPhosphate:
                case FertilizerBlends.SuperPhosphate:
                    {
                    return true;
                }

                default:
                    return false;
            }
        }

        /// <summary>
        /// (K2O)
        /// </summary>
        public static bool IsPotassiumFertilizer(this FertilizerBlends fertilizerBlend)
        {
            switch (fertilizerBlend)
            {
                case FertilizerBlends.Potash:
                case FertilizerBlends.Npk:
                case FertilizerBlends.NpkMixedAcid:
                case FertilizerBlends.NpkNitrophosphate:
                case FertilizerBlends.PotassiumSulphate:
                    {
                    return true;
                }

                default:
                    return false;
            }
        }

        #endregion
    }
}
