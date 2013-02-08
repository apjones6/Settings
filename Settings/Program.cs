using System;
using System.Configuration;

namespace Settings
{
    class Program
    {
        static void Main(string[] args)
        {
            // The code here is less than flattering, as it makes it look like there's a lot of writing involved in using this class, whereas nearly
            // all the code here is for writing to console. The actual code used is:

            // var setting = AppSettings.Bool(AppSettingKeys.CULTURE_SWITCHING);



            // No default settings, neither in app settings nor configured otherwise
            Header("No Defaults");
            Console.WriteLine("{0}: {1}", AppSettingKeys.CULTURE_SWITCHING, AppSettings.Bool(AppSettingKeys.CULTURE_SWITCHING));
            Console.WriteLine("{0}: {1}", AppSettingKeys.IMAGE_TYPES, AppSettings.FileExtensions(AppSettingKeys.IMAGE_TYPES));
            Console.WriteLine("{0}: {1}", AppSettingKeys.PAGE_SIZE, AppSettings.Int(AppSettingKeys.PAGE_SIZE));

            // Explicitly set default settings
            Header("Explicit Defaults");
            AppSettings.Defaults[AppSettingKeys.CULTURE_SWITCHING] = "True";
            AppSettings.Defaults[AppSettingKeys.IMAGE_TYPES] = "gif;png";
            AppSettings.Defaults[AppSettingKeys.PAGE_SIZE] = "50";
            AppSettings.Required.Add(AppSettingKeys.IMAGE_PATH);
            Console.WriteLine("{0}: {1}", AppSettingKeys.CULTURE_SWITCHING, AppSettings.Bool(AppSettingKeys.CULTURE_SWITCHING));
            Console.WriteLine("{0}: {1}", AppSettingKeys.IMAGE_TYPES, AppSettings.FileExtensions(AppSettingKeys.IMAGE_TYPES));
            Console.WriteLine("{0}: {1}", AppSettingKeys.PAGE_SIZE, AppSettings.Int(AppSettingKeys.PAGE_SIZE));

            // AppSettings driven (set directly rather than through appSettings.config file)
            Header("Settings");
            ConfigurationManager.AppSettings[AppSettingKeys.CULTURE_SWITCHING] = "False";
            ConfigurationManager.AppSettings[AppSettingKeys.IMAGE_PATH] = "~/Content/Images/V2/";
            ConfigurationManager.AppSettings[AppSettingKeys.IMAGE_TYPES] = "JPG;JPEG;BMP;JPG;;";
            ConfigurationManager.AppSettings[AppSettingKeys.PAGE_SIZE] = "25";
            Console.WriteLine("{0}: {1}", AppSettingKeys.CULTURE_SWITCHING, AppSettings.Bool(AppSettingKeys.CULTURE_SWITCHING));
            Console.WriteLine("{0}: {1}", AppSettingKeys.IMAGE_PATH, AppSettings.String(AppSettingKeys.IMAGE_PATH));
            Console.WriteLine("{0}: {1}", AppSettingKeys.IMAGE_TYPES, AppSettings.FileExtensions(AppSettingKeys.IMAGE_TYPES));
            Console.WriteLine("{0}: {1}", AppSettingKeys.PAGE_SIZE, AppSettings.Int(AppSettingKeys.PAGE_SIZE));

            // Test FileExtensionSet class
            Header("File Extensions");
            Console.WriteLine("Contains JPG: {0}", AppSettings.FileExtensions(AppSettingKeys.IMAGE_TYPES).Contains("Jpg"));
            Console.WriteLine("Contains PNG: {0}", AppSettings.FileExtensions(AppSettingKeys.IMAGE_TYPES).Contains(".png"));
            Console.WriteLine("Contains BMP: {0}", AppSettings.FileExtensions(AppSettingKeys.IMAGE_TYPES).Contains(".BMP"));

            // Reflection based configuration
            Header("Reflection Configuration");
            AppSettings.ConfigureByReflection();
            ConfigurationManager.AppSettings[AppSettingKeys.CULTURE_SWITCHING] = null;
            ConfigurationManager.AppSettings[AppSettingKeys.IMAGE_TYPES] = null;
            ConfigurationManager.AppSettings[AppSettingKeys.PAGE_SIZE] = null;
            Console.WriteLine("{0}: {1}", AppSettingKeys.CULTURE_SWITCHING, AppSettings.Bool(AppSettingKeys.CULTURE_SWITCHING));
            Console.WriteLine("{0}: {1}", AppSettingKeys.IMAGE_TYPES, AppSettings.FileExtensions(AppSettingKeys.IMAGE_TYPES));
            Console.WriteLine("{0}: {1}", AppSettingKeys.PAGE_SIZE, AppSettings.Int(AppSettingKeys.PAGE_SIZE));

            // Listing missing required settings
            Header("Not Found");
            ConfigurationManager.AppSettings[AppSettingKeys.IMAGE_PATH] = null;
            Console.WriteLine("Required: {0}", string.Join(", ", AppSettings.NotFound()));

            Console.ReadKey();
        }

        static void Header(string text)
        {
            using (ColorBlock.For(ConsoleColor.Green))
            {
                Console.WriteLine("--------------------");
                Console.WriteLine(text);
            }
        }
    }

    class ColorBlock : IDisposable
    {
        private readonly ConsoleColor color;

        public ColorBlock(ConsoleColor color)
        {
            this.color = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }

        public static ColorBlock For(ConsoleColor color)
        {
            return new ColorBlock(color);
        }

        public void Dispose()
        {
            Console.ForegroundColor = color;
        }
    }
}
