Settings project readme
=======================

Objectives:
 - Simple, reusable access to the AppSettings configuration section
 - Streamlined default values and enforcing 'required' app settings.
 - No need to create separate settings providers
 - Simple management of file extensions white/black lists from settings
 - Simple unit testing

Example Usage
=============

Day to day usage
----------------

Most of the time we use app settings to store string paths, boolean flags, GUID ids and integer 
values. These scenarios are simple:

```C#
    string path = AppSettings.String(Keys.EXAMPLE_PATH);
    bool flag = AppSettings.Bool(Keys.EXAMPLE_FLAG);
    int count = AppSettings.Int(Keys.EXAMPLE_COUNT);
    Guid id = AppSettings.Guid(Keys.EXAMPLE_ID);
```

The final example which we use quite often is to store white/black lists of file extensions. This 
is specially handled by AppSettings for convenience of use and robustness:

```C#
    string extension = Path.GetExtension("example file.jpg");
    if (AppSettings.FileExtensions(Keys.EXAMPLE_FILE_TYPES).Contains(extension))
    {
        // Do something...
    }
```

The AppSettings class has a knowledge of settings which are required, and default values for 
certain keys. If a setting is not in the configuration but has been marked as required, a 
ConfigurationErrorsException is automatically thrown when it is requested. If a setting is not 
required, but no value is in the settings then a dictionary of default values is checked. If found 
this value is used, otherwise the default value for the type is returned, except for FileExtensions
which returns an empty set.

Initialization
--------------

There are two ways to configure the AppSettings known default values and required keys; explicitly 
and by reflection. The first is simpler and more efficient, but requires up front knowledge of all 
the settings. The second is more suited to a large modular application.

Configuration Explicitly:

```C#
    // Called on startup, such as in Global.asax
    AppSettings.Defaults["default-key"] = "default-value";
    AppSettings.Required.Add("required-key");
```

Configuration by Reflection:

```C#
    // This class could be anywhere, including another DLL, as long as it's part of the final 
    // executing applications BIN
    [AppSettingKeys]
    public static class ExampleKeys
    {
        [Default(".jpg;.jpeg")]
        [Required]
        public const string IMAGE_TYPES = "IMAGE_TYPES";
    }
    
    // Called on startup, such as in Global.asax
    AppSettings.ConfigureByReflection();
```

Another useful method returns any keys which have been configured as required, but are not in the 
settings file. This allows early detection of configuration errors.

```C#
    AppSettings.ConfigureByReflection();
    var notFound = AppSettings.NotFound();
    if (notFound.Any())
    {
        var message = string.Concat("Required settings not found: ", string.Join(", ", missing));
        throw new ConfigurationErrorsException(message);
    }
```

There's even a method built in which does this exception throwing for you, for convenience.

```C#
    AppSettings.ConfigureByReflection();
    AppSettings.Verify();
```

Testing With AppSettings
------------------------

Unit testing code which calls the AppSettings retrieval methods relies on a pass through approach.
This is so the code can remain so much simpler than if we had to introduce layers of indirection 
(interfaces) just to support testing dependent code.

In your SetUp or directly in your Tests set the defaults you expect. As there shouldn't be actual 
app settings for a test, these values will be returned.

```C#
    [SetUp]
    public void SetUp()
    {
        AppSettings.Defaults[Keys.ROOT_PATH] = "TestData/Root";
        AppSettings.Defaults[Keys.PAGE_SIZE] = "10";
    }
```

Then just remember to clean up after yourself in TearDown, to ensure tests don't affect eachother.

```C#
    [TearDown]
    public void TearDown()
    {
        AppSettings.Defaults.Clear();
    }
```
