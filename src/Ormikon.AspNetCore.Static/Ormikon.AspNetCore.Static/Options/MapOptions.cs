using System;

namespace Ormikon.AspNetCore.Static.Options
{
    /// <summary>
    /// Helper class to better operate with StaticSettings and values from the application configuration
    /// </summary>
    public class MapOptions
    {
        internal StaticSettings CreateSettings()
        {
            return new StaticSettings(Sources)
            {
                Cached = Cached,
                Expires = Expires,
                MaxAgeExpression = MaxAge,
                DefaultFile = Default,
                RedirectIfFolderFound = RedirectIfFolder,
                Include = Include,
                Exclude = Exclude,
                AllowHidden = AllowHidden
            };
        }

        /// <summary>
        /// Map path. Hosting default used if none set
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Sources separated by semicolon
        /// </summary>
        public string Sources { get; set; }

        /// <summary>
        /// Use content caching
        /// </summary>
        public bool Cached { get; set; }

        /// <summary>
        /// Content cache expires
        /// </summary>
        public DateTimeOffset Expires { get; set; }

        /// <summary>
        /// Content max age
        /// </summary>
        public string MaxAge { get; set; } = "0";

        /// <summary>
        /// Content default file names
        /// </summary>
        public string Default { get; set; } = StaticSettingsBase.DefaultFileValue;

        /// <summary>
        /// Redirect if path is folder
        /// </summary>
        public bool RedirectIfFolder { get; set; } = true;

        /// <summary>
        /// Include pattern (can be several separated by semicolon)
        /// </summary>
        public string Include { get; set; }

        /// <summary>
        /// Exclude pattern (can be several separated by semicolon)
        /// </summary>
        public string Exclude { get; set; }

        /// <summary>
        /// Allow hidden files
        /// </summary>
        public bool AllowHidden { get; set; }
    }
}
