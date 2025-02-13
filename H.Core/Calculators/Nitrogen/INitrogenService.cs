using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Nitrogen
{
    public interface INitrogenService
    {
        double CalculateAboveGroundResidueNitrogen(Farm farm, CropViewItem currentYearViewItem,
            CropViewItem previousYearViewItem);
        double CalculateBelowGroundResidueNitrogen(Farm farm, CropViewItem currentYearViewItem,
            CropViewItem previousYearViewItem);
        double CalculateCropResidueExportNitrogen(CropViewItem cropViewItem);
    }
}