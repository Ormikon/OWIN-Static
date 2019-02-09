namespace Ormikon.AspNetCore.Static.Options
{
    /// <summary>
    /// Ormikon Static configuration options
    /// <example>
    /// "OrmikonStatic" : {
    ///     "Maps" : [
    ///         { "Path": "/scripts", "Sources": "Scripts", "Cached": false, "Expires": "2025-01-01", "Include": "*.js", "Exclude": "**\\*1.6.4.js" },
    ///         { "Path": "/styles", "Sources": "Styles", "Cached": false, "Expires": "2025-01-01", "Include": "*.css", "Exclude": "**\\*debug.css" },
    ///         { "Path": "/home", "Sources": "Index.html" }
    ///     ]
    /// }
    /// </example>
    /// </summary>
    public class StaticOptions
    {
        /// <summary>
        /// Is any mapping found in the configuration
        /// </summary>
        /// <returns>True if mapping configuration has map options inside</returns>
        public bool HasMappings()
        {
            return Maps != null && Maps.Length > 0;
        }

        /// <summary>
        /// Map options collection
        /// </summary>
        public MapOptions[] Maps { get; set; }
    }
}
