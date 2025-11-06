using Microsoft.Extensions.DependencyInjection;

namespace WSharp.Distributed.Transaction;

/// <summary>
/// 分布式事务的依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加带内存状态存储的 Saga 模式支持
    /// </summary>
    public static IServiceCollection AddWSharpSaga(this IServiceCollection services)
    {
        services.AddSingleton<ISagaStateRepository, InMemorySagaStateRepository>();
        services.AddScoped<ISagaOrchestrator, SagaOrchestrator>();

        return services;
    }

    /// <summary>
    /// 添加带自定义状态仓储的 Saga 模式支持
    /// </summary>
    public static IServiceCollection AddWSharpSaga<TRepository>(this IServiceCollection services)
        where TRepository : class, ISagaStateRepository
    {
        services.AddSingleton<ISagaStateRepository, TRepository>();
        services.AddScoped<ISagaOrchestrator, SagaOrchestrator>();

        return services;
    }

    /// <summary>
    /// 添加带自定义状态仓储工厂的 Saga 模式支持
    /// </summary>
    public static IServiceCollection AddWSharpSaga(
        this IServiceCollection services,
        Func<IServiceProvider, ISagaStateRepository> repositoryFactory)
    {
        services.AddSingleton(repositoryFactory);
        services.AddScoped<ISagaOrchestrator, SagaOrchestrator>();

        return services;
    }
}
