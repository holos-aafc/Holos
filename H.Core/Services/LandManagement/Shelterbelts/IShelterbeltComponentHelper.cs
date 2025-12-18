using System.Collections.Generic;
using H.Core.Models.LandManagement.Shelterbelt;

namespace H.Core.Services.LandManagement.Shelterbelts
{
    public interface IShelterbeltComponentHelper
    {
        string GetUniqueShelterbeltName(IEnumerable<ShelterbeltComponent> shelterbelts);
    }
}