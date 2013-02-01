namespace Settings
{
    public static class AppSettingKeys
    {
        [AppSetting(".jpg;.jpeg")]
        public const string IMAGE_TYPES = "IMAGE_TYPES";
        [AppSetting(false)]
        public const string CULTURE_SWITCHING = "CULTURE_SWITCHING";
        [AppSetting(Required = true)]
        public const string IMAGE_PATH = "IMAGE_PATH";
        [AppSetting(50)]
        public const string PAGE_SIZE = "PAGE_SIZE";
        public const string OPTIONAL = "OPTIONAL";
    }
}
