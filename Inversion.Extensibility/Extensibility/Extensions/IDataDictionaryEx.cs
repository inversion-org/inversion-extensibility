using System;
using Inversion.Collections;

namespace Inversion.Extensibility.Extensions
{
    public static class IDataDictionaryEx
    {
        public static string GetWithAssert(this IDataDictionary<string> self, string key, bool assert = true, string defaultValue = "")
        {
            if (!self.ContainsKey(key))
            {
                if (assert)
                {
                    throw new ArgumentException(String.Format("Expected '{0}' in dictionary.", key));
                }
                return defaultValue;
            }
            return self[key];
        }

        public static string GetParameterValue(this IDataDictionary<string> self, string keyAndPath)
        {
            string[] splitlist = keyAndPath.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (!self.ContainsKey(splitlist[1]) && splitlist.Length > 2)
            {
                return splitlist[2];
            }
            return self.GetWithAssert(splitlist[1]);
        }
    }
}