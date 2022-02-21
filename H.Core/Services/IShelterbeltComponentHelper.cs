using H.Core.Models.LandManagement.Shelterbelt;
using System.Collections.Generic;

namespace H.Core.Services
{
    public interface IShelterbeltComponentHelper
    {
        string GetUniqueShelterbeltName(IEnumerable<ShelterbeltComponent> shelterbelts);
    }
}
