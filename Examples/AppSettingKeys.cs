using Settings;

namespace Examples
{
    [AppSettingKeys]
    public static class AppSettingKeys
    {
        [Default(".jpg;.jpeg")]
        public const string IMAGE_TYPES = "IMAGE_TYPES";
        [Default(true)]
        public const string CULTURE_SWITCHING = "CULTURE_SWITCHING";
        [Required]
        public const string IMAGE_PATH = "IMAGE_PATH";
        [Default(50)]
        public const string PAGE_SIZE = "PAGE_SIZE";
    }
}
