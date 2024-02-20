using CsvHelper.Configuration;
using H.Avalonia.Core.Models.Results;

namespace H.Avalonia.Core.Models.ClassMaps
{
    /// <summary>
    /// A mapping class for CsvReader extension. The class maps properties in <see cref="ClimateResultsViewItem"/> class
    /// so that CsvReader is able to identify these properties and handle them when exporting/importing data.
    /// </summary>
    public sealed class ClimateResultsViewItemMap : ClassMap<ClimateResultsViewItem>
    {
        public ClimateResultsViewItemMap()
        {
            Map(map => map.Latitude).Index(0).Name(H.Avalonia.Core.Properties.Resources.LabelLatitude);
            Map(map => map.Longitude).Index(1).Name(H.Avalonia.Core.Properties.Resources.LabelLongitude);
            Map(map => map.Year).Index(2).Name(H.Avalonia.Core.Properties.Resources.LabelYear);
            Map(map => map.TotalPET).Index(3).Name(H.Avalonia.Core.Properties.Resources.LabelTotalPET);
            Map(map => map.TotalPPT).Index(4).Name(H.Avalonia.Core.Properties.Resources.LabelTotalPPT);
            Map(map => map.MonthlyPPT).Index(5).Name(H.Avalonia.Core.Properties.Resources.LabelMonthlyPPT);
        }
    }
}
