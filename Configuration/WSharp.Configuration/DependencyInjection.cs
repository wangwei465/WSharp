using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace WSharp.Configuration;

/// <summary>
/// 配置的依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 WSharp 配置服务
    /// </summary>
    public static IServiceCollection AddWSharpConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IConfigurationValidator, ConfigurationValidator>();

        return services;
    }

    /// <summary>
    /// 添加并配置带验证的选项
    /// </summary>
    public static IServiceCollection AddValidatedOptions<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
        where TOptions : class
    {
        services.Configure<TOptions>(configuration.GetSection(sectionName));

        // 添加后置配置验证
        services.PostConfigure<TOptions>(options =>
        {
            var validator = new ConfigurationValidator();
            validator.ValidateAndThrow(options);
        });

        return services;
    }

    /// <summary>
    /// 添加并配置带变更通知的选项
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
    /// 添加并配置带验证和变更通知的选项
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
    /// 从多个来源添加配置
    /// </summary>
    public static IConfigurationBuilder AddWSharpConfiguration(
        this IConfigurationBuilder builder,
        string environmentName,
        bool reloadOnChange = true)
    {
        // 添加 appsettings.json
        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: reloadOnChange);

        // 添加特定环境的 appsettings
        builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: reloadOnChange);

        // 添加环境变量
        builder.AddEnvironmentVariables();

        return builder;
    }
}
