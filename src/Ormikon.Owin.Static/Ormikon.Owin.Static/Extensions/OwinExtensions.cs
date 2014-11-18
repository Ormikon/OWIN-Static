using System.Collections.Generic;

namespace Ormikon.Owin.Static.Extensions
{
    internal static class OwinExtensions
    {
        public static object Get(this IDictionary<string, object> owinData, string parameter, object defaultValue = null)
        {
            object result;
            return owinData.TryGetValue(parameter, out result) ? result : defaultValue;
        }

        public static T Get<T>(this IDictionary<string, object> owinData, string parameter, T defaultValue = default (T))
        {
            var result = owinData.Get(parameter, (object) defaultValue);
            return result is T ? (T) result : defaultValue;
        }
    }
}