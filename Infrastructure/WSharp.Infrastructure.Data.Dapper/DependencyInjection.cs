using System.Data;
using Microsoft.Extensions.DependencyInjection;
using WSharp.Core.Domain.Repositories;

namespace WSharp.Infrastructure.Data.Dapper;

/// <summary>
/// Dapper 依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 Dapper 数据访问
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="connectionFactory">数据库连接工厂</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDapperData(
        this IServiceCollection services,
        Func<IServiceProvider, IDbConnection> connectionFactory)
    {
        // 注册数据库连接
        services.AddScoped(connectionFactory);

        // 注册 DbContext
        services.AddScoped<DapperDbContext>();
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<DapperDbContext>());

        // 注册工作单元
        services.AddScoped<IUnitOfWork, DapperUnitOfWork>();

        return services;
    }
}
