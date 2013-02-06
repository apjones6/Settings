using System;

namespace Settings
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class RequiredAttribute : Attribute
    {
    }
}
