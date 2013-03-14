using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Settings
{
    /// <summary>
    /// A class providing static access to application settings, with features supporting simple and 
    /// robust use such as type conversion and requirements checking.
    /// </summary>
    public static class AppSettings
    {
        private static readonly Dictionary<string, string> defaults = new Dictionary<string, string>();
        private static readonly HashSet<string> required = new HashSet<string>();

        #region Properties

        /// <summary>
        /// Gets the dictionary of default values to application setting keys.
        /// </summary>
        /// <remarks>
        /// The values are stored as strings, which are then converted to other object types as
        /// necessary. It may be more logical to store objects to represent this.
        /// </remarks>
        public static Dictionary<string, string> Defaults
        {
            get { return defaults; }
        }

        /// <summary>
        /// Gets the set of required application setting keys, which must exist and cannot be 
        /// defaulted or otherwise ignored.
        /// </summary>
        public static HashSet<string> Required
        {
            get { return required; }
        }

        #endregion

        #region Retrieval

        /// <summary>
        /// Returns a string value for the provided application settings key, or default or null if
        /// not available.
        /// </summary>
        /// <param name="key">The string key to get.</param>
        /// <returns>A string value, or null.</returns>
        /// <exception cref="ConfigurationErrorsException">
        /// The setting was marked required and not found.
        /// </exception>
        public static string String(string key)
        {
            return Get(key);
        }

        /// <summary>
        /// Returns a boolean value for the provided application settings key, or default if not set
        /// and a default is provided.
        /// </summary>
        /// <param name="key">The string key to get.</param>
        /// <returns>A boolean value.</returns>
        /// <exception cref="ConfigurationErrorsException">
        /// The setting was marked required and not found, or could not be converted to a bool.
        /// </exception>
        public static bool Bool(string key)
        {
            var value = Get(key);
            return value != null ? bool.Parse(value) : default(bool);
        }

        /// <summary>
        /// Returns an int value for the provided application settings key, or default if not set
        /// and a default is provided.
        /// </summary>
        /// <param name="key">The string key to get.</param>
        /// <returns>An integer value.</returns>
        /// <exception cref="ConfigurationErrorsException">
        /// The setting was marked required and not found, or could not be converted to an int.
        /// </exception>
        public static int Int(string key)
        {
            var value = Get(key);
            return value != null ? int.Parse(value) : default(int);
        }

        /// <summary>
        /// Returns a Guid value for the provided application settings key, or default if not set
        /// and a default is provided.
        /// </summary>
        /// <param name="key">The string key to get.</param>
        /// <returns>A Guid value.</returns>
        /// <exception cref="ConfigurationErrorsException">
        /// The setting was marked required and not found, or could not be converted to a Guid.
        /// </exception>
        public static Guid Guid(string key)
        {
            var value = Get(key);
            return value != null ? System.Guid.Parse(value) : default(Guid);
        }

        /// <summary>
        /// Returns a FileExtensionSet value for the provided application settings key, or default 
        /// if not set and a default is provided.
        /// </summary>
        /// <param name="key">The string key to get.</param>
        /// <returns>A FileExtensionSet value.</returns>
        /// <exception cref="ConfigurationErrorsException">
        /// The setting was marked required and not found, or could not be converted to a
        /// FileExtensionSet.
        /// </exception>
        public static FileExtensionSet FileExtensions(string key)
        {
            var value = Get(key);
            return value != null ? new FileExtensionSet(value) : new FileExtensionSet();
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Populates the Defaults and Required properties by using reflection in the current execution
        /// folder and searching for types marked with the [AppSettingKeys] attribute.
        /// </summary>
        /// <remarks>
        /// You only want to call this overload once on site startup, as it's quite heavy, but it 
        /// nicely deals with component based applications
        /// </remarks>
        /// <exception cref="ConfigurationErrorsException">
        /// One or more string keys was found duplicated but with conflicting default or required 
        /// attribute values.
        /// </exception>
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

            // Get keys from these types
            var keys = types.SelectMany(t =>
                t.GetFields(BindingFlags.Static | BindingFlags.Public)
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
                            Value = x.Type.GetRawConstantValue().ToString(),
                            Default = x.Default as DefaultAttribute,
                            x.Required
                        }))
                .ToArray();

            // Check for conflicts
            var conflicts = keys.GroupBy(x => x.Value)
                .Where(x => x.Count() > 1)
                .Where(x => x.GroupBy(y => new { Default = y.Default != null ? y.Default.Value : null, y.Required }).Count() > 1)
                .Select(x => x.FirstOrDefault().Value)
                .ToArray();
            if (conflicts.Any())
            {
                var message = string.Concat("Found conflicts for keys: ", string.Join(", ", conflicts));
                throw new ConfigurationErrorsException(message);
            }

            // Add the keys to dictionary
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

        /// <summary>
        /// Returns an array of string keys which exist in the Required set but were not found in the
        /// application settings configuration file.
        /// </summary>
        /// <returns>An array of missing keys.</returns>
        public static string[] NotFound()
        {
            return required.Where(key => ConfigurationManager.AppSettings[key] == null).ToArray();
        }

        /// <summary>
        /// A convenience method which checks if <c>NotFound()</c> returns any keys, and if any throws
        /// a descriptive <see cref="ConfigurationErrorsException"/> listing these keys.
        /// </summary>
        /// <remarks>
        /// Can call this from startup, after calling Configure, to ensure all your required
        /// settings are available.
        /// </remarks>
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

        /// <summary>
        /// Returns the string value for the specified key, falling back to the <c>Defaults</c> property
        /// dictionary if not found, and returning null if still not found.
        /// </summary>
        /// <param name="key">The string key to get.</param>
        /// <returns>The string value for the key.</returns>
        /// <exception cref="ConfigurationErrorsException">
        /// The setting was marked as required and not found.
        /// </exception>
        private static string Get(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            // Throw if required and not set BEFORE attempting to apply defaults
            if (value == null && required.Contains(key))
            {
                throw new ConfigurationErrorsException(string.Format("Required setting {0} was not found.", key));
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
