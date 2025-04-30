using System.Runtime.Caching;

namespace H.Core.Services
{
    public class DefaultCacheService : ICacheService
    {
        #region Fields

        private MemoryCache _cache = MemoryCache.Default;

        #endregion
    }
}