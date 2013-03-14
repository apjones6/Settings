using System;

namespace Settings
{
    /// <summary>
    /// When applied to a static class containing one or more const string fields, it indicates those
    /// fields should be bound as required and defaulted application setting keys.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class AppSettingKeysAttribute : Attribute
    {
    }
}
