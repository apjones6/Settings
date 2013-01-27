using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Settings
{
    public static class AppSettings
    {
        #region Fields

        // These SHOULD be hard coded, as they are defaults for where no settings or explicit defaults are provided, but we may need a clean way of
        // dealing with default settings for numerous modules, built after the core library.
        // My solution would be to allow setting this dictionary through a static method, and set it on application start-up
        private static readonly Dictionary<string, string> DEFAULTS = new Dictionary<string, string>
            {
                { AppSettingKeys.CULTURE_SWITCHING, "False" },
                { AppSettingKeys.IMAGE_TYPES, ".jpg;.jpeg;.png;.gif" },
                { AppSettingKeys.PAGE_SIZE, "10" }
            };

        private static readonly HashSet<string> REQUIRED = new HashSet<string>
            {
                AppSettingKeys.IMAGE_PATH
            };

        #endregion

        #region Retrieval Methods

        public static string String(string key, string @default = null)
        {
            return Get(key, @default);
        }

        public static bool Bool(string key)
        {
            return bool.Parse(Get(key));
        }

        public static bool Bool(string key, bool @default)
        {
            return bool.Parse(Get(key, @default.ToString()));
        }

        public static int Int(string key)
        {
            return int.Parse(Get(key));
        }

        public static int Int(string key, int @default)
        {
            return int.Parse(Get(key, @default.ToString()));
        }

        public static Guid Guid(string key)
        {
            return System.Guid.Parse(Get(key));
        }

        public static Guid Guid(string key, Guid @default)
        {
            return System.Guid.Parse(Get(key, @default.ToString()));
        }

        // I really hate this name, and want a much shorter one if we can think of it. Also note no @default version; not useful
        public static FileExtensionSet FileExtensions(string key)
        {
            return new FileExtensionSet(Get(key));
        }

        #endregion

        #region Configuration Methods

        public static void Configure(Dictionary<string, string> defaults, List<string> required = null)
        {
            // Adds new DEFAULTS and REQUIRED items, or overwrites existing DEFAULTS, but leaves existing ones intact
            // Case sensitivity could be more robust, but not likely worth it
            if (defaults != null)
            {
                foreach (var pair in defaults)
                {
                    DEFAULTS[pair.Key] = pair.Value;
                }
            }

            if (required != null)
            {
                foreach (var key in required)
                {
                    REQUIRED.Add(key);
                }
            }
        }

        // Can call this from Global.asax or an administration URL to ensure app settings set
        public static string[] NotFound()
        {
            return REQUIRED.Where(key => ConfigurationManager.AppSettings[key] == null).ToArray();
        }

        #endregion

        #region Private

        private static string Get(string key, string @default = null)
        {
            var value = ConfigurationManager.AppSettings[key];

            // Throw if required and not set BEFORE? attempting to apply defaults
            if (value == null && REQUIRED.Contains(key))
            {
                throw new ConfigurationErrorsException(string.Format("Required setting {0} was not found.", key));
            }

            // Set explicit default if not set
            if (value == null && @default != null)
            {
                value = @default;
            }

            // Set global default if not set
            if (value == null && DEFAULTS.ContainsKey(key))
            {
                value = DEFAULTS[key];
            }

            return value;
        }

        #endregion
    }
}
