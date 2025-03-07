using H.Core.Models.LandManagement.Fields;

namespace H.Core.Models
{
    public class AdjoiningYears
    {
        public CropViewItem PreviousYearViewItem { get; set; }
        public CropViewItem CurrentYearViewItem { get; set; }
        public CropViewItem NextYearViewItem { get; set; }
    }
}