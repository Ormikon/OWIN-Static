namespace Ormikon.Owin.Static.Config
{
    /// <summary>
    /// Helper class to better operate with StaticSettings and values from the application config
    /// </summary>
    public class Settings
    {
        private readonly string path;
        private readonly StaticSettings val;

        /// <summary>
        /// Creates settings from the path and static settings
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="settings">Static settings</param>
        public Settings(string path, StaticSettings settings)
        {
            this.path = path;
            val = settings;
        }

        /// <summary>
        /// Creates settings from the configuration file value
        /// </summary>
        /// <param name="map"></param>
        public Settings(MapElement map) :
            this(map.Path, CreateSettigsFromConfig(map))
        {
        }

        private static StaticSettings CreateSettigsFromConfig(MapElement map)
        {
            return new StaticSettings(map.Sources)
            {
                Cached = map.Cached,
                Expires = map.Expires,
                MaxAgeExpression = map.MaxAge,
                DefaultFile = map.DefaultFile,
                RedirectIfFolderFound = map.RedirectIfFolder,
                Include = map.Include,
                Exclude = map.Exclude,
                AllowHidden = map.AllowHidden
            };
        }

        /// <summary>
        /// The OWIN matches path
        /// </summary>
        public string Path
        {
            get
            {
                return path;
            }
        }

        /// <summary>
        /// Is path assigned to the static settings
        /// </summary>
        public bool HasPath
        {
            get
            {
                return !string.IsNullOrEmpty(path);
            }
        }

        /// <summary>
        /// Static settings value
        /// </summary>
        public StaticSettings Value
        {
            get
            {
                return val;
            }
        }
    }
}
