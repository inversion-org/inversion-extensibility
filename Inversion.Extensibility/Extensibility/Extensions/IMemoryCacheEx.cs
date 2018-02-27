using System;
using Microsoft.Extensions.Caching.Memory;

namespace Inversion.Extensibility.Extensions
{
    public static class IMemoryCacheEx
    {
        public static T GetWithAssert<T>(this IMemoryCache self, string key) where T : class
        {
            if (!self.Contains(key))
            {
                throw new ArgumentException(String.Format("Cache member '{0}' was missing.", key));
            }

            object o = self.Get(key);

            T candidate = o as T;

            if (candidate == null)
            {
                throw new ArgumentException(
                    String.Format("Cache member '{0}' was invalid. Expected: {1} Actual: {2}", key,
                        typeof(T).FullName, o.GetType().FullName));
            }

            return candidate;
        }

        public static void Add(this IMemoryCache self, string name, object obj)
        {
            ICacheEntry cacheEntry = self.CreateEntry(name);
            cacheEntry.Value = obj;
        }

        public static void Add(this IMemoryCache self, string name, object obj, DateTimeOffset expiry)
        {
            ICacheEntry cacheEntry = self.CreateEntry(name);
            cacheEntry.Value = obj;
            cacheEntry.AbsoluteExpiration = expiry;
        }

        public static object Get(this IMemoryCache self, string key)
        {
            object result = null;
            if(self.TryGetValue(key, out result))
            {
                return result;
            }

            return null;
        }

        public static bool Contains(this IMemoryCache self, string key)
        {
            object result = null;
            if(self.TryGetValue(key, out result))
            {
                return true;
            }

            return false;
        }
    }
}