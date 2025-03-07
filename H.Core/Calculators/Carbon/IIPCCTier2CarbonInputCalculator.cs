using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Carbon
{
    public interface IIPCCTier2CarbonInputCalculator : ICarbonInputCalculator
    {
        bool CanCalculateInputsForCrop(CropViewItem cropViewItem);
        void AssignInputs(CropViewItem viewItem, Farm farm);
    }
}