using System;

namespace Settings
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AppSettingAttribute : Attribute
    {
        public AppSettingAttribute()
        {
        }

        public AppSettingAttribute(string @default)
        {
            this.Default = @default;
        }

        public AppSettingAttribute(int @default)
            : this(@default.ToString())
        {
        }

        public AppSettingAttribute(bool @default)
            : this(@default.ToString())
        {
        }

        public string Default { get; set; }

        public bool Required { get; set; }
    }
}
