using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    public interface INitrogenService
    {
        double CalculateAboveGroundResidueNitrogen(CropViewItem currentYearViewItem, CropViewItem previousYearViewItem);
        double CalculateBelowGroundResidueNitrogen(CropViewItem currentYearViewItem, CropViewItem previousYearViewItem);
        double CalculateCropResidueExportNitrogen(CropViewItem cropViewItem);
    }
}