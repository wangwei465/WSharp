using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WSharp.Core.Domain.Repositories;
using WSharp.Infrastructure.Data.UnitOfWork;

namespace WSharp.Infrastructure.Data.MongoDB;

/// <summary>
/// MongoDB 依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 MongoDB 数据访问
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="databaseName">数据库名称</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddMongoDbData(
        this IServiceCollection services,
        string connectionString,
        string databaseName)
    {
        // 注册配置
        services.Configure<MongoDbOptions>(options =>
        {
            options.ConnectionString = connectionString;
            options.DatabaseName = databaseName;
        });

        // 注册 MongoDB 客户端
        services.AddSingleton<IMongoClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            var settings = MongoClientSettings.FromConnectionString(options.ConnectionString);

            // 配置连接池和超时
            settings.MaxConnectionPoolSize = 100;
            settings.MinConnectionPoolSize = 10;
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(30);
            settings.ConnectTimeout = TimeSpan.FromSeconds(30);

            return new MongoClient(settings);
        });

        // 注册 DbContext
        services.AddScoped<MongoDbContext>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var options = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            return new MongoDbContext(client, options.DatabaseName);
        });

        services.AddScoped<IDbContext>(sp => sp.GetRequiredService<MongoDbContext>());

        // 注册工作单元
        services.AddScoped<IUnitOfWork, MongoDbUnitOfWork>();

        // 注册仓储
        services.AddScoped(typeof(IRepository<,>), typeof(MongoDbRepository<,>));
        services.AddScoped(typeof(IReadOnlyRepository<,>), typeof(MongoDbRepository<,>));

        return services;
    }

    /// <summary>
    /// 添加 MongoDB 数据访问（使用选项配置）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddMongoDbData(
        this IServiceCollection services,
        Action<MongoDbOptions> configureOptions)
    {
        services.Configure(configureOptions);

        // 注册 MongoDB 客户端
        services.AddSingleton<IMongoClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            var settings = MongoClientSettings.FromConnectionString(options.ConnectionString);

            // 配置连接池和超时
            settings.MaxConnectionPoolSize = 100;
            settings.MinConnectionPoolSize = 10;
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(30);
            settings.ConnectTimeout = TimeSpan.FromSeconds(30);

            return new MongoClient(settings);
        });

        // 注册 DbContext
        services.AddScoped<MongoDbContext>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var options = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            return new MongoDbContext(client, options.DatabaseName);
        });

        services.AddScoped<IDbContext>(sp => sp.GetRequiredService<MongoDbContext>());

        // 注册工作单元
        services.AddScoped<IUnitOfWork, MongoDbUnitOfWork>();

        // 注册仓储
        services.AddScoped(typeof(IRepository<,>), typeof(MongoDbRepository<,>));
        services.AddScoped(typeof(IReadOnlyRepository<,>), typeof(MongoDbRepository<,>));

        return services;
    }
}
