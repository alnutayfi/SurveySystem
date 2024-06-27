using Serilog;
using Serilog.Extensions.Hosting;

namespace SurveySystem;


public static class ProgramExtensions
{
    internal static void ConfigureCustomHost(this IHostBuilder builder, string[] args)
    {
        builder
            .UseSerilog(ConfigureReloadableLogger)
            .ConfigureAppConfiguration((hostingContext, config) =>
                AddConfiguration(config, hostingContext.HostingEnvironment, args))
            .UseDefaultServiceProvider(
                (context, options) =>
                {
                    var isDevelopment = context.HostingEnvironment.IsDevelopment();
                    options.ValidateScopes = isDevelopment;
                    options.ValidateOnBuild = isDevelopment;
                })
            .UseConsoleLifetime();
    }
    

    /// <summary>
    ///     Configures a logger used during the applications lifetime.
    ///     <see href="https://nblumhardt.com/2020/10/bootstrap-logger/" />.
    /// </summary>
    private static void ConfigureReloadableLogger(
        HostBuilderContext context,
        IServiceProvider services,
        LoggerConfiguration configuration)
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
            .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);
    }

    private static void AddConfiguration(
        IConfigurationBuilder configurationBuilder,
        IHostEnvironment hostEnvironment,
        string[] args)
    {
        Log.Information("appsettings binding started");
        string appSettingsPath = hostEnvironment.IsProduction() ?  ReadSettingsCmdArgs(args, "--appsettings") : Directory.GetCurrentDirectory() + "/appsettings.json";


        if (appSettingsPath is null || !File.Exists(appSettingsPath))
            throw new Exception(
                $"Please add correct path of appsettings.json file Using args --appsettings, the current provided path is '{appSettingsPath}'.");

        configurationBuilder
            .AddJsonFile(appSettingsPath, false, true)
            .Build();
    
        Log.Information("appsettings binding completed");

    }
    
    static string?  ReadSettingsCmdArgs(string[] args, string argName)
    {
        if (args.Length == 0)
            return default;

        var appSettingsIndex = Array.FindIndex(args, s => s.ToLower().Equals(argName.ToLower()));

        if (appSettingsIndex >= 0 && args.Length > appSettingsIndex + 1)
            return args[appSettingsIndex + 1].Trim();

        return default;
    }
}