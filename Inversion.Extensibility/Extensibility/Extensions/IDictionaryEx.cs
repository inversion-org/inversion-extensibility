using System;
using System.Collections.Generic;

namespace Inversion.Extensibility.Extensions
{
    public static class IDictionaryEx
    {
        public static string GetEvalData(this IDictionary<string, string> self, string key, string defaultValue = "")
        {
            return self.GetEvalDataWithAssert(key, assert: true, defaultValue: defaultValue);
        }

        public static string GetEvalDataWithAssert(this IDictionary<string, string> self, string key, bool assert = true, string defaultValue = "")
        {
            if (!self.ContainsKey(key))
            {
                if (assert)
                {
                    throw new ArgumentException(String.Format("Expected '{0}' in Eval data.", key));
                }
                return defaultValue;
            }
            return self[key];
        }
    }
}