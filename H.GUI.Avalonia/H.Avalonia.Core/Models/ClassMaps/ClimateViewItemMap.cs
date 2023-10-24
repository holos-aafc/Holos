using CsvHelper.Configuration;
using H.Avalonia.Infrastructure;

namespace H.Avalonia.Core.Models.ClassMaps
{
    /// <summary>
    /// A mapping class for CsvReader extension. The class maps properties in <see cref="ClimateViewItem"/> class
    /// so that CsvReader is able to identify these properties and handle them when exporting/importing data.
    /// </summary>
    public sealed class ClimateViewItemMap : ClassMap<ClimateViewItem>
    {
        public ClimateViewItemMap()
        {
            Map(map => map.Latitude).Index(0).Name(Core.Properties.Resources.LabelLatitude.ConvertToImportFormat());
            Map(map => map.Longitude).Index(1).Name(Core.Properties.Resources.LabelLongitude.ConvertToImportFormat());
            Map(map => map.StartYear).Index(2).Name(Core.Properties.Resources.LabelStartYear.ConvertToImportFormat());
            Map(map => map.EndYear).Index(3).Name(Core.Properties.Resources.LabelEndYear.ConvertToImportFormat());
            Map(map => map.JulianStartDay).Index(4).Name(Core.Properties.Resources.LabelJulianStartDay.ConvertToImportFormat());
            Map(map => map.JulianEndDay).Index(5).Name(Core.Properties.Resources.LabelJulianEndDay.ConvertToImportFormat());

        }
    }
}