using System.Collections.Generic;

namespace H.Core.Providers
{
    public interface IProvider<T> where T : class
    {
        bool IsInitialized { get; set; }
        IEnumerable<T> GetData();
    }
}