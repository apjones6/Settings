using Settings;

namespace Examples
{
    [AppSettingKeys]
    public static class ExtraKeys
    {
        [Default("Administrators")]
        [Required]
        public const string OPTIONAL = "OPTIONAL";
    }
}
