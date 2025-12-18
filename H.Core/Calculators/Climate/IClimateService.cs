using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Climate
{
    public interface IClimateService
    {
        /// <summary>
        /// Calculate climate parameter. Will use custom climate data if it exists for the farm, otherwise will use SLC normals
        /// for climate data.
        /// </summary>
        double CalculateClimateParameter(CropViewItem viewItem, Farm farm);
    }
}