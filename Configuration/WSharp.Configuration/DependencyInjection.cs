using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace WSharp.Configuration;

/// <summary>
/// Dependency injection extensions for configuration
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add WSharp configuration services
    /// </summary>
    public static IServiceCollection AddWSharpConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IConfigurationValidator, ConfigurationValidator>();

        return services;
    }

    /// <summary>
    /// Add and configure options with validation
    /// </summary>
    public static IServiceCollection AddValidatedOptions<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
        where TOptions : class
    {
        services.Configure<TOptions>(configuration.GetSection(sectionName));

        // Add post-configure validation
        services.PostConfigure<TOptions>(options =>
        {
            var validator = new ConfigurationValidator();
            validator.ValidateAndThrow(options);
        });

        return services;
    }

    /// <summary>
    /// Add and configure options with change notification
    /// </summary>
    public static IServiceCollection AddNotifiableOptions<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
        where TOptions : class
    {
        services.Configure<TOptions>(configuration.GetSection(sectionName));
        services.AddSingleton<IConfigurationChangeNotifier<TOptions>, ConfigurationChangeNotifier<TOptions>>();

        return services;
    }

    /// <summary>
    /// Add and configure options with validation and change notification
    /// </summary>
    public static IServiceCollection AddValidatedNotifiableOptions<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
        where TOptions : class
    {
        services.AddValidatedOptions<TOptions>(configuration, sectionName);
        services.AddSingleton<IConfigurationChangeNotifier<TOptions>, ConfigurationChangeNotifier<TOptions>>();

        return services;
    }

    /// <summary>
    /// Add configuration from multiple sources
    /// </summary>
    public static IConfigurationBuilder AddWSharpConfiguration(
        this IConfigurationBuilder builder,
        string environmentName,
        bool reloadOnChange = true)
    {
        // Add appsettings.json
        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: reloadOnChange);

        // Add environment-specific appsettings
        builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: reloadOnChange);

        // Add environment variables
        builder.AddEnvironmentVariables();

        return builder;
    }
}
