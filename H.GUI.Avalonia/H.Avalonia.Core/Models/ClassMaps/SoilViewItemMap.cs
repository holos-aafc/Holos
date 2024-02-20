using CsvHelper.Configuration;
using H.Avalonia.Infrastructure;
using H.Common.Models;

namespace H.Avalonia.Core.Models.ClassMaps
{
    /// <summary>
    /// A mapping class for CsvReader extension. The class maps properties in <see cref="H.Common.Models.SoilViewItem"/> class
    /// so that CsvReader is able to identify these properties and handle them when exporting/importing data.
    /// </summary>
    public class SoilViewItemMap : ClassMap<SoilViewItem>
    {
        public SoilViewItemMap()
        {
            Map(map => map.Latitude).Index(0).Name(Core.Properties.Resources.LabelLatitude.ConvertToImportFormat());
            Map(map => map.Longitude).Index(1).Name(Core.Properties.Resources.LabelLongitude.ConvertToImportFormat());
        }
    }
}