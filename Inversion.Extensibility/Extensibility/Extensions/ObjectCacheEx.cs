using System;
using System.Runtime.Caching;

namespace Inversion.Extensibility.Extensions
{
    public static class ObjectCacheEx
    {
        public static T GetWithAssert<T>(this ObjectCache self, string key) where T : class
        {
            if (!self.Contains(key))
            {
                throw new ArgumentException(String.Format("Object cache member '{0}' was missing.", key));
            }

            object o = self[key];

            T candidate = o as T;

            if (candidate == null)
            {
                throw new ArgumentException(
                    String.Format("Object cache member '{0}' was invalid. Expected: {1} Actual: {2}", key,
                        typeof(T).FullName, o.GetType().FullName));
            }

            return candidate;
        }
    }
}