using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    public interface INitrogenService
    {
        double CalculateAboveGroundResidueNitrogen(CropViewItem cropViewItem);
        double CalculateBelowGroundResidueNitrogen(CropViewItem cropViewItem);
        double CalculateCropResidueExportNitrogen(CropViewItem cropViewItem);
    }
}