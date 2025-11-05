using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WSharp.Core.Domain.Repositories;

namespace WSharp.Infrastructure.Data.EntityFramework;

/// <summary>
/// Entity Framework Core 依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 Entity Framework Core 数据访问
    /// </summary>
    /// <typeparam name="TContext">数据库上下文类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="optionsAction">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddEfCoreData<TContext>(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction)
        where TContext : EfCoreDbContext
    {
        // 注册 DbContext
        services.AddDbContext<TContext>(optionsAction);

        // 注册 IDbContext
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<TContext>());

        // 注册工作单元
        services.AddScoped<IUnitOfWork, EfCoreUnitOfWork>();

        // 注册仓储
        services.AddScoped(typeof(IRepository<,>), typeof(EfCoreRepository<,>));
        services.AddScoped(typeof(IReadOnlyRepository<,>), typeof(EfCoreRepository<,>));

        return services;
    }
}
