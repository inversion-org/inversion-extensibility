using System;
using Newtonsoft.Json.Linq;

namespace Inversion.Extensibility.Extensions
{
    public static class ControlStateEx
    {
        public static string GetEffectiveStringResult(this Inversion.Collections.IDataDictionary<object> controlState, string key)
        {
            string path = String.Empty;

            if (key.Contains("."))
            {
                path = key.Substring(key.IndexOf(".", StringComparison.Ordinal) + 1);
                key = key.Substring(0, key.IndexOf(".", StringComparison.Ordinal));
            }

            if (!controlState.ContainsKey(key))
            {
                return null;
            }

            IData controlStateObject = controlState[key] as IData;

            if (controlStateObject == null)
            {
                return null;
            }

            if (String.IsNullOrEmpty(path))
            {
                TextData o = controlStateObject as TextData;
                return o != null ? o.Value : null;
            }

            try
            {
                JToken resultToken = controlStateObject.Data.SelectToken(path);
                if (resultToken is JObject)
                {
                    return resultToken.ToString();
                }
                return resultToken != null ? resultToken.Value<string>() : null;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(String.Format("Problem while evaluating {0} path {1}: {2}", key, path,
                    ex.ToString()));
            }
        }

        public static T GetWithAssert<T>(this Inversion.Collections.IDataDictionary<object> controlState, string key) where T : class
        {
            if (!controlState.ContainsKey(key))
            {
                throw new ArgumentException(String.Format("Control state member '{0}' was missing.", key));
            }

            object o = controlState[key];

            T candidate = o as T;

            if (candidate == null)
            {
                throw new ArgumentException(
                    String.Format("Control state member '{0}' was invalid. Expected: {1} Actual: {2}", key,
                        typeof (T).FullName, o.GetType().FullName));
            }

            return candidate;
        }
    }
}