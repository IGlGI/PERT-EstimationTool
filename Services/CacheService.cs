using PertEstimationTool.Services.Interfaces;
using System;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Unity;

namespace PertEstimationTool.Services
{
    public class CacheService : ICacheService
    {
        private MemoryCache _memoryCache;

        private CacheItemPolicy _cacheItemPolicy;

        public CacheService(IUnityContainer container)
        {
            _memoryCache = container.Resolve<MemoryCache>();
            _cacheItemPolicy = container.Resolve<CacheItemPolicy>();
        }

        public async Task Add<T>(string key, T obj)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("The key cannot be empty or null");

            if (obj == null)
                throw new NullReferenceException("The object cannot be null");

            _memoryCache.Add(key, obj, _cacheItemPolicy);
        }

        public async Task Clear()
        {
            foreach (var element in MemoryCache.Default)
            {
                MemoryCache.Default.Remove(element.Key);
            }
        }

        public async Task<T> Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("The key cannot be empty or null");

            try
            {
                return (T)_memoryCache.Get(key);
            }
            catch
            {
                throw;
            }
        }

        public async Task Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("The key cannot be empty or null");

            _memoryCache.Remove(key);
        }

        public async Task Upadate<T>(string key, T obj)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("The key cannot be empty or null");

            if (obj == null)
                throw new NullReferenceException("The object cannot be null");

            _memoryCache.Remove(key);
            _memoryCache.Add(key, obj, _cacheItemPolicy);
        }
    }
}
