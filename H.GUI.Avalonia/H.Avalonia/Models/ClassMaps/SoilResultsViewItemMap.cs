using CsvHelper.Configuration;
using H.Avalonia.Models.Results;
using H.Infrastructure;

namespace H.Avalonia.Models.ClassMaps
{
    /// <summary>
    /// A mapping class for CsvReader extension. The class maps properties in <see cref="SoilResultsViewItem"/> class
    /// so that CsvReader is able to identify these properties and handle them when exporting/importing data.
    /// </summary>
    public sealed class SoilResultsViewItemMap : ClassMap<SoilResultsViewItem>
    {
        public SoilResultsViewItemMap()
        {
            Map(map => map.Latitude).Index(0).Name(Core.Properties.Resources.LabelLatitude);
            Map(map => map.Longitude).Index(1).Name(Core.Properties.Resources.LabelLongitude);
            Map(map => map.Province).Index(2).Name(Core.Properties.Resources.LabelProvince);
            Map(map => map.SoilGreatGroup).Index(3).Name(Core.Properties.Resources.LabelSoilGreatGroup).Convert(o => o.Value.SoilGreatGroup.GetDescription());
            Map(map => map.SoilTexture).Index(4).Name(Core.Properties.Resources.LabelSoilTexture).Convert(o => o.Value.SoilTexture.GetDescription()); ;
            Map(map => map.PercentClayInSoil).Index(5).Name(Core.Properties.Resources.LabelPercentClayInSoil);
            Map(map => map.SoilPh).Index(5).Name(Core.Properties.Resources.LabelSoilPh);
            Map(map => map.PercentOrganicMatterInSoil).Index(5).Name(Core.Properties.Resources.LabelOrganicMatterSoil);
        }
    }
}