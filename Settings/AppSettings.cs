﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Settings
{
    public static class AppSettings
    {
        #region Fields

        // These SHOULD be hard coded, as they are defaults for where no settings or explicit defaults are provided, but we may need a clean way of
        // dealing with default settings for numerous modules, built after the core library.
        // My solution would be to allow setting this dictionary through a static method, and set it on application start-up
        private static readonly Dictionary<string, string> defaults = new Dictionary<string, string>
            {
                { AppSettingKeys.CULTURE_SWITCHING, "False" },
                { AppSettingKeys.IMAGE_TYPES, ".jpg;.jpeg;.png;.gif" },
                { AppSettingKeys.PAGE_SIZE, "10" }
            };

        private static readonly HashSet<string> required = new HashSet<string>
            {
                AppSettingKeys.IMAGE_PATH
            };

        #endregion

        #region Retrieval Methods

        public static string String(string key)
        {
            return Get(key, false);
        }

        public static bool Bool(string key)
        {
            return bool.Parse(Get(key));
        }

        public static int Int(string key)
        {
            return int.Parse(Get(key));
        }

        public static Guid Guid(string key)
        {
            return System.Guid.Parse(Get(key));
        }

        public static FileExtensionSet FileExtensions(string key)
        {
            return new FileExtensionSet(Get(key));
        }

        #endregion

        #region Configuration Methods

        // Good for maintaining compile time settings, and yet ugly to call
        public static void Configure(Dictionary<string, string> defaults, List<string> required = null, bool replace = false)
        {
            // Adds new defaults and required items, or overwrites existing defaults
            // Optionally replaces all items (clears first)
            if (defaults != null)
            {
                if (replace)
                {
                    defaults.Clear();
                }

                foreach (var pair in defaults)
                {
                    defaults[pair.Key] = pair.Value;
                }
            }

            if (required != null)
            {
                if (replace)
                {
                    required.Clear();
                }

                foreach (var key in required)
                {
                    required.Add(key);
                }
            }
        }

        // This is probably very inefficient in a large application, and yet in many ways its almost the ideal solution
        public static void Configure()
        {
            // Find AppSettingKeys types - We could use an attribute to select the classes, but we probably get better performance and
            // definitely cleaner code if we simply require all keys defined in a class called 'AppSettingKeys'
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .AsParallel()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsAbstract && x.Name == "AppSettingKeys")
                .ToArray();

            // Clear defaults and required so we can populate them again
            defaults.Clear();
            required.Clear();

            // Set keys from these types
            foreach (var type in types)
            {
                var keys = type.GetFields(BindingFlags.Static | BindingFlags.Public)
                    .Where(x => x.FieldType == typeof(string))
                    .Select(x => new { Type = x, Attribute = x.GetCustomAttributes(typeof(AppSettingAttribute), false).FirstOrDefault() })
                    .Where(x => x.Attribute != null)
                    .Select(x => new { x.Type, Value = x.Type.GetRawConstantValue().ToString(), ((AppSettingAttribute)x.Attribute).Default, ((AppSettingAttribute)x.Attribute).Required })
                    .ToArray();

                foreach (var key in keys)
                {
                    if (key.Required)
                    {
                        required.Add(key.Value);
                    }

                    if (key.Default != null)
                    {
                        defaults[key.Value] = key.Default;
                    }
                }
            }
        }

        // Can call this from Global.asax or an administration URL to ensure app settings set
        public static string[] NotFound()
        {
            return required.Where(key => ConfigurationManager.AppSettings[key] == null).ToArray();
        }

        #endregion

        #region Private

        private static string Get(string key, bool errorIfNull = true)
        {
            var value = ConfigurationManager.AppSettings[key];

            // Throw if required and not set BEFORE attempting to apply defaults
            if (value == null && required.Contains(key))
            {
                throw new ConfigurationErrorsException(string.Format("required setting {0} was not found.", key));
            }

            // Set global default if not set
            if (value == null && defaults.ContainsKey(key))
            {
                value = defaults[key];
            }

            // Throw if we can't let a null through (i.e. we're going to try and parse it)
            if (value == null && errorIfNull)
            {
                throw new ConfigurationErrorsException(string.Format("required setting {0} was not found.", key));
            }

            return value;
        }

        #endregion
    }
}
