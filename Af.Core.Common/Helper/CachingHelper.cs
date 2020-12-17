using Microsoft.Extensions.Caching.Memory;
using System;

namespace Af.Core.Common.Helper
{
    public interface ICaching
    {
        object Get(string cacheKey);
        void Set(string cacheKey, object cacheValue);
    }

    public class MemoryCaching : ICaching
    {
        private readonly IMemoryCache _cache;
        public object Get(string cacheKey)
        {
            return _cache.Get(cacheKey);
        }

        public void Set(string cacheKey, object cacheValue)
        {
            _cache.Set(cacheKey, cacheValue, TimeSpan.FromSeconds(7200));
        }
    }
}
