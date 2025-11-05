using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WSharp.Infrastructure.Data.EntityFramework;

namespace WSharp.Infrastructure.Data.MySQL;

/// <summary>
/// MySQL 依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 MySQL 数据访问
    /// </summary>
    /// <typeparam name="TContext">数据库上下文类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="serverVersion">MySQL 服务器版本（可选，自动检测）</param>
    /// <param name="optionsAction">额外配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddMySqlData<TContext>(
        this IServiceCollection services,
        string connectionString,
        ServerVersion? serverVersion = null,
        Action<DbContextOptionsBuilder>? optionsAction = null)
        where TContext : EfCoreDbContext
    {
        // 如果未指定版本，自动检测
        var mySqlVersion = serverVersion ?? ServerVersion.AutoDetect(connectionString);

        services.AddEfCoreData<TContext>(options =>
        {
            options.UseMySql(connectionString, mySqlVersion, mySqlOptions =>
            {
                // 启用重试策略
                mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);

                // 设置命令超时
                mySqlOptions.CommandTimeout(30);

                // 启用查询拆分行为
                mySqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });

            // 应用额外配置
            optionsAction?.Invoke(options);
        });

        return services;
    }

    /// <summary>
    /// 添加 MySQL 数据访问（带迁移程序集）
    /// </summary>
    /// <typeparam name="TContext">数据库上下文类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="migrationsAssembly">迁移程序集名称</param>
    /// <param name="serverVersion">MySQL 服务器版本（可选，自动检测）</param>
    /// <param name="optionsAction">额外配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddMySqlDataWithMigrations<TContext>(
        this IServiceCollection services,
        string connectionString,
        string migrationsAssembly,
        ServerVersion? serverVersion = null,
        Action<DbContextOptionsBuilder>? optionsAction = null)
        where TContext : EfCoreDbContext
    {
        // 如果未指定版本，自动检测
        var mySqlVersion = serverVersion ?? ServerVersion.AutoDetect(connectionString);

        services.AddEfCoreData<TContext>(options =>
        {
            options.UseMySql(connectionString, mySqlVersion, mySqlOptions =>
            {
                // 设置迁移程序集
                mySqlOptions.MigrationsAssembly(migrationsAssembly);

                // 启用重试策略
                mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);

                // 设置命令超时
                mySqlOptions.CommandTimeout(30);

                // 启用查询拆分行为
                mySqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });

            // 应用额外配置
            optionsAction?.Invoke(options);
        });

        return services;
    }
}
