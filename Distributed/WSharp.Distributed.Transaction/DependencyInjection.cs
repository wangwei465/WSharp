using Microsoft.Extensions.DependencyInjection;

namespace WSharp.Distributed.Transaction;

/// <summary>
/// Dependency injection extensions for distributed transactions
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add Saga pattern support with in-memory state storage
    /// </summary>
    public static IServiceCollection AddWSharpSaga(this IServiceCollection services)
    {
        services.AddSingleton<ISagaStateRepository, InMemorySagaStateRepository>();
        services.AddScoped<ISagaOrchestrator, SagaOrchestrator>();

        return services;
    }

    /// <summary>
    /// Add Saga pattern support with custom state repository
    /// </summary>
    public static IServiceCollection AddWSharpSaga<TRepository>(this IServiceCollection services)
        where TRepository : class, ISagaStateRepository
    {
        services.AddSingleton<ISagaStateRepository, TRepository>();
        services.AddScoped<ISagaOrchestrator, SagaOrchestrator>();

        return services;
    }

    /// <summary>
    /// Add Saga pattern support with custom state repository factory
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
