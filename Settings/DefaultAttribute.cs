using System;

namespace Settings
{
    /// <summary>
    /// When applied to a const string field in a class marked with [AppSettingKeys], it provides 
    /// the default value to use for an application setting with a key matching the field value, if
    /// no explicit setting is provided.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class DefaultAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAttribute"/> class with a default string value.
        /// </summary>
        /// <param name="value">The default string value.</param>
        public DefaultAttribute(string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAttribute"/> class with a default integer value.
        /// </summary>
        /// <param name="value">The default integer value.</param>
        public DefaultAttribute(int value)
            : this(value.ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAttribute"/> class with a default boolean value.
        /// </summary>
        /// <param name="value">The default boolean value.</param>
        public DefaultAttribute(bool value)
            : this(value.ToString())
        {
        }

        /// <summary>
        /// Gets the default value this attribute indicates as a string.
        /// </summary>
        public string Value { get; private set; }
    }
}
