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

        public string Default { get; set; }

        public bool Required { get; set; }
    }
}
