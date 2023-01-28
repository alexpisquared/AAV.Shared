# AppSettings vs. User Secrets
## 2023-01-28

C# code for using visual studio secrets feature together with dependency injection.
Here is some sample C# code that demonstrates using the "secrets" feature in Visual Studio together with dependency injection:

c#

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add the configuration to the service collection
        services.AddSingleton(Configuration);

        // Add other services here
    }

    public void Configure(IApplicationBuilder app)
    {
        // Configure the app here
    }
}
In this example, the IConfiguration object is being injected into the Startup class constructor, and the services.AddSingleton(Configuration) line is registering the configuration object as a singleton that can be injected into other parts of the application.

To use the Visual Studio "secrets" feature, you can add a secrets.json file to your project, and then use the AddUserSecrets method to add the secrets to the configuration:

c#

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add the user secrets to the configuration
        Configuration = new ConfigurationBuilder()
            .AddUserSecrets<Startup>()
            .Build();

        // Add the configuration to the service collection
        services.AddSingleton(Configuration);

        // Add other services here
    }

    public void Configure(IApplicationBuilder app)
    {
        // Configure the app here
    }
}
In this way, the secrets can be easily accessed by the application via dependency injection.




Alex Pigida
Add also utilization of the local AppSettings.json file to store and retrieve application settings.
Sure, here's an example of how you can use both the Visual Studio "secrets" feature and a local appsettings.json file to store and retrieve application settings in a C# application:

c#

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add the user secrets to the configuration
        Configuration = new ConfigurationBuilder()
            .AddUserSecrets<Startup>()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        // Add the configuration to the service collection
        services.AddSingleton(Configuration);

        // Add other services here
    }

    public void Configure(IApplicationBuilder app)
    {
        // Configure the app here
    }
}
In this example, AddUserSecrets<Startup>() is used to add the user secrets to the configuration, and AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) is used to add the settings from the appsettings.json file. The optional: true parameter means that the file is not required and the application will still start without it, and the reloadOnChange: true parameter means that the configuration will automatically reload if the file is changed.
Then the configuration object is registered as a singleton in the service collection to be injected as needed.

You can then access the settings from the appsettings.json file and the secrets in the same way, by using the Configuration object:

c#

string mySetting = Configuration["MySetting"];
## It's worth noting that when retrieving the settings, the order of how the configuration is added matters, the last added will overwrite the previous one with the same key.