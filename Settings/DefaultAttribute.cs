using System;

namespace Settings
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DefaultAttribute : Attribute
    {
        public DefaultAttribute(string value)
        {
            if (value == null) throw new ArgumentNullException("@value");
            this.Value = value;
        }

        public DefaultAttribute(int value)
            : this(value.ToString())
        {
        }

        public DefaultAttribute(bool value)
            : this(value.ToString())
        {
        }

        public string Value { get; set; }
    }
}
