using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Settings
{
    public static class AppSettings
    {
        #region Properties

        private static readonly Dictionary<string, string> defaults = new Dictionary<string, string>();
        public static Dictionary<string, string> Defaults
        {
            get { return defaults; }
        }

        private static readonly HashSet<string> required = new HashSet<string>();
        public static HashSet<string> Required
        {
            get { return required; }
        }

        #endregion

        #region Retrieval

        public static string String(string key)
        {
            return Get(key);
        }

        public static bool Bool(string key)
        {
            var value = Get(key);
            return value != null ? bool.Parse(value) : default(bool);
        }

        public static int Int(string key)
        {
            var value = Get(key);
            return value != null ? int.Parse(value) : default(int);
        }

        public static Guid Guid(string key)
        {
            var value = Get(key);
            return value != null ? System.Guid.Parse(value) : default(Guid);
        }

        public static FileExtensionSet FileExtensions(string key)
        {
            var value = Get(key);
            return value != null ? new FileExtensionSet(value) : new FileExtensionSet();
        }

        #endregion

        #region Configuration

        // You only want to call this overload once on site startup, as it's quite heavy, but it nicely deals with component based applications
        public static void ConfigureByReflection()
        {
            // Find AppSettingKeys types - We use an attribute to select the classes, but we probably get better performance and
            // definitely cleaner code if we simply require all keys defined in a class called 'AppSettingKeys'. This however might
            // limit our future usage, and it's not a particularly easy thing to make clear to clients.
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .AsParallel()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsAbstract && x.GetCustomAttributes(typeof(AppSettingKeysAttribute), true).Any())
                .ToArray();

            // Clear defaults and required so we can populate them again
            defaults.Clear();
            required.Clear();

            // Set keys from these types
            foreach (var type in types)
            {
                var keys = type.GetFields(BindingFlags.Static | BindingFlags.Public)
                    .Where(x => x.FieldType == typeof(string))
                    .Select(x => new
                        {
                            Type = x,
                            Default = x.GetCustomAttributes(typeof(DefaultAttribute), false).SingleOrDefault(),
                            Required = x.GetCustomAttributes(typeof(RequiredAttribute), false).Any()
                        })
                    .Where(x => x.Default != null || x.Required)
                    .Select(x => new
                        {
                            x.Type,
                            Value = x.Type.GetRawConstantValue().ToString(),
                            Default = x.Default as DefaultAttribute,
                            x.Required
                        })
                    .ToArray();

                foreach (var key in keys)
                {
                    if (key.Required)
                    {
                        required.Add(key.Value);
                    }

                    if (key.Default != null)
                    {
                        defaults[key.Value] = key.Default.Value;
                    }
                }
            }
        }

        public static string[] NotFound()
        {
            return required.Where(key => ConfigurationManager.AppSettings[key] == null).ToArray();
        }

        // Can call this from startup, after calling Configure, to ensure all your required settings are available.
        public static void Verify()
        {
            var notFound = NotFound();
            if (notFound.Any())
            {
                var message = string.Concat("Required settings not found: ", string.Join(", ", notFound));
                throw new ConfigurationErrorsException(message);
            }
        }

        #endregion

        #region Private

        private static string Get(string key)
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

            return value;
        }

        #endregion
    }
}
