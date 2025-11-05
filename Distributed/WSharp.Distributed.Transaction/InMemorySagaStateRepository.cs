using System.Collections.Concurrent;
using System.Text.Json;

namespace WSharp.Distributed.Transaction;

/// <summary>
/// In-memory implementation of saga state repository (for development/testing)
/// </summary>
public class InMemorySagaStateRepository : ISagaStateRepository
{
    private readonly ConcurrentDictionary<string, string> _storage = new();

    public Task SaveAsync(SagaContext context, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(context);
        _storage[context.SagaId] = json;
        return Task.CompletedTask;
    }

    public Task<SagaContext?> GetAsync(string sagaId, CancellationToken cancellationToken = default)
    {
        if (_storage.TryGetValue(sagaId, out var json))
        {
            var context = JsonSerializer.Deserialize<SagaContext>(json);
            return Task.FromResult(context);
        }
        return Task.FromResult<SagaContext?>(null);
    }

    public Task UpdateAsync(SagaContext context, CancellationToken cancellationToken = default)
    {
        return SaveAsync(context, cancellationToken);
    }

    public Task DeleteAsync(string sagaId, CancellationToken cancellationToken = default)
    {
        _storage.TryRemove(sagaId, out _);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<SagaContext>> GetByStateAsync(SagaState state, CancellationToken cancellationToken = default)
    {
        var contexts = _storage.Values
            .Select(json => JsonSerializer.Deserialize<SagaContext>(json))
            .Where(c => c != null && c.State == state)
            .Cast<SagaContext>();

        return Task.FromResult(contexts);
    }
}
