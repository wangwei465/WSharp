using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using WSharp.Core.Application.Behaviors;

namespace WSharp.Core.Application;

/// <summary>
/// 应用层依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加应用层服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="assemblies">包含命令/查询处理器的程序集</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        // 注册 MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
        });

        // 注册 FluentValidation
        services.AddValidatorsFromAssemblies(assemblies);

        // 注册管道行为
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

        // 注册 AutoMapper
        services.AddAutoMapper(assemblies);

        return services;
    }
}
