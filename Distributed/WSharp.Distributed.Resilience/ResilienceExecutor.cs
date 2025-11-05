using Polly;

namespace WSharp.Distributed.Resilience;

/// <summary>
/// 弹性执行器
/// </summary>
public class ResilienceExecutor
{
    private readonly IAsyncPolicy _asyncPolicy;
    private readonly ISyncPolicy _syncPolicy;

    public ResilienceExecutor(IAsyncPolicy asyncPolicy, ISyncPolicy syncPolicy)
    {
        _asyncPolicy = asyncPolicy;
        _syncPolicy = syncPolicy;
    }

    /// <summary>
    /// 执行异步操作
    /// </summary>
    /// <param name="action">要执行的操作</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        await _asyncPolicy.ExecuteAsync(action);
    }

    /// <summary>
    /// 执行异步操作并返回结果
    /// </summary>
    /// <typeparam name="T">结果类型</typeparam>
    /// <param name="func">要执行的函数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>执行结果</returns>
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> func, CancellationToken cancellationToken = default)
    {
        return await _asyncPolicy.ExecuteAsync(func);
    }

    /// <summary>
    /// 执行同步操作
    /// </summary>
    /// <param name="action">要执行的操作</param>
    public void Execute(Action action)
    {
        _syncPolicy.Execute(action);
    }

    /// <summary>
    /// 执行同步操作并返回结果
    /// </summary>
    /// <typeparam name="T">结果类型</typeparam>
    /// <param name="func">要执行的函数</param>
    /// <returns>执行结果</returns>
    public T Execute<T>(Func<T> func)
    {
        return _syncPolicy.Execute(func);
    }
}
