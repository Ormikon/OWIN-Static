using System.Reflection;

namespace Ormikon.AspNetCore.Static
{
    /// <summary>
    /// Configuration settings for Assembly resources middleware
    /// </summary>
    public class StaticResourcesSettings : StaticSettingsBase
    {
        /// <summary>
        /// Creates default settings with Entry assembly as a source for resources
        /// </summary>
        public StaticResourcesSettings()
        {
        }

        /// <summary>
        /// Creates default settings with <paramref name="assembly"/> as a source for resources
        /// </summary>
        /// <param name="assembly">Assembly with static resources</param>
        public StaticResourcesSettings(Assembly assembly)
        {
            Assembly = assembly;
        }

        /// <summary>
        /// Assembly with static resources
        /// </summary>
        public Assembly Assembly { get; set; } = Assembly.GetEntryAssembly();

        /// <summary>
        /// Filter for resources in assembly. Should be a part of name space ending with dot symbol
        /// or with concrete resource name
        /// </summary>
        public string Resources { get; set; }
    }
}
