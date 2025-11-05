using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WSharp.Infrastructure.Data.EntityFramework;

namespace WSharp.Infrastructure.Data.SqlServer;

/// <summary>
/// SQL Server 依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 SQL Server 数据访问
    /// </summary>
    /// <typeparam name="TContext">数据库上下文类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="optionsAction">额外配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddSqlServerData<TContext>(
        this IServiceCollection services,
        string connectionString,
        Action<DbContextOptionsBuilder>? optionsAction = null)
        where TContext : EfCoreDbContext
    {
        services.AddEfCoreData<TContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                // 启用重试策略
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);

                // 设置命令超时
                sqlOptions.CommandTimeout(30);

                // 启用查询拆分行为（避免笛卡尔爆炸）
                sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });

            // 应用额外配置
            optionsAction?.Invoke(options);
        });

        return services;
    }

    /// <summary>
    /// 添加 SQL Server 数据访问（带迁移程序集）
    /// </summary>
    /// <typeparam name="TContext">数据库上下文类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="migrationsAssembly">迁移程序集名称</param>
    /// <param name="optionsAction">额外配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddSqlServerDataWithMigrations<TContext>(
        this IServiceCollection services,
        string connectionString,
        string migrationsAssembly,
        Action<DbContextOptionsBuilder>? optionsAction = null)
        where TContext : EfCoreDbContext
    {
        services.AddEfCoreData<TContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                // 设置迁移程序集
                sqlOptions.MigrationsAssembly(migrationsAssembly);

                // 启用重试策略
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);

                // 设置命令超时
                sqlOptions.CommandTimeout(30);

                // 启用查询拆分行为
                sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });

            // 应用额外配置
            optionsAction?.Invoke(options);
        });

        return services;
    }
}
