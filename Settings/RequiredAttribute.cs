using System;

namespace Settings
{
    /// <summary>
    /// When applied to a const string field in a class marked with [AppSettingKeys], it indicates
    /// an application setting with a key matching the field value is required by the application
    /// to run correctly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class RequiredAttribute : Attribute
    {
    }
}
