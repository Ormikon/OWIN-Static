using System.Configuration;

namespace Ormikon.Owin.Static.Config
{
    /// <summary>
    /// OWIN Static configuration element collection
    /// <example>
    /// <maps>
    ///   <map path="/scripts" sources="..\..\Scripts" cached="false" expires="2020-01-01" include="*.js" exclude="**\*1.6.4.js" />
    ///   <map path="/styles" sources="..\..\Styles" cached="false" expires="2020-01-01" include="*.css" exclude="**\*debug.css" />
    ///   <map path="/home" sources="..\..\Index.html" />
    /// </maps>
    /// </example>
    /// </summary>
    [ConfigurationCollection(typeof(MapElement), AddItemName = "map", RemoveItemName = "removeMap", ClearItemsName = "clear")]
    public class MapElementCollection : ConfigurationElementCollection
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            var el = element as MapElement;
            if (el != null)
            {
                string key = "";
                if (!string.IsNullOrEmpty(el.Path))
                {
                    key = "[" + el.Path + "]-";
                }
                key += "[" + el.Sources + "]";
                return key;
            }
            return null;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new MapElement();
        }
    }
}
