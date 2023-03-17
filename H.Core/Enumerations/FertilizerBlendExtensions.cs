namespace H.Core.Enumerations
{
    public static class FertilizerBlendExtensions
    {
        #region Public Methods

        public static bool IsNitrogenFertilizer(this FertilizerBlends fertilizerBlend)
        {
            switch (fertilizerBlend)
            {
                case FertilizerBlends.Ammonia:
                case FertilizerBlends.Urea:
                case FertilizerBlends.UreaAmmoniumNitrate:
                case FertilizerBlends.AmmoniumNitrate:
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

        public static bool IsPhosphorusFertilizer(this FertilizerBlends fertilizerBlend)
        {
            switch (fertilizerBlend)
            {
                case FertilizerBlends.DiAmmoniumPhosphate:
                case FertilizerBlends.TripleSuperPhosphate:
                {
                    return true;
                }

                default:
                    return false;
            }
        }

        public static bool IsPotassiumFertilizer(this FertilizerBlends fertilizerBlend)
        {
            switch (fertilizerBlend)
            {
                case FertilizerBlends.Potash:
                case FertilizerBlends.Npk:
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