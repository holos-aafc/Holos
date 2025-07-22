namespace H.Core.Models.Infrastructure
{
    public static class SubstrateTypeExtensions
    {
        public static bool IsNonManureSubstrate(this SubstrateType substrateType)
        {
            return substrateType == SubstrateType.CropResidues || substrateType == SubstrateType.FarmResidues;
        }
    }
}