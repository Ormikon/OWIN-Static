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

        public string Path { get; set; }

        public string Sources { get; set; }

        public bool Cached { get; set; }

        public DateTimeOffset Expires { get; set; }

        public string MaxAge { get; set; } = "0";

        public string Default { get; set; } = StaticSettings.DefaultFileValue;

        public bool RedirectIfFolder { get; set; } = true;

        public string Include { get; set; }

        public string Exclude { get; set; }

        public bool AllowHidden { get; set; }
    }
}
