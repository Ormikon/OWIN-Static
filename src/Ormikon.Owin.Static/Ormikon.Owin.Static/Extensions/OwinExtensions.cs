using System.Collections.Generic;

namespace Ormikon.Owin.Static.Extensions
{
    internal static class OwinExtensions
    {
        public static object Get(this IDictionary<string, object> owinData, string parameter)
        {
            object result;
            return owinData.TryGetValue(parameter, out result) ? result : null;
        }

        public static T Get<T>(this IDictionary<string, object> owinData, string parameter)
        {
            var result = owinData.Get(parameter);
            return result == null || !(result is T) ? default(T) : (T) result;
        }
    }
}
