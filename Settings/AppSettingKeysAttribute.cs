using System;

namespace Settings
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class AppSettingKeysAttribute : Attribute
    {
    }
}
