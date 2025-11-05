using MongoDB.Driver;
using WSharp.Infrastructure.Data;

namespace WSharp.Infrastructure.Data.MongoDB;

/// <summary>
/// MongoDB 数据库上下文
/// </summary>
public class MongoDbContext : IDbContext
{
    private readonly IMongoDatabase _database;
    private IClientSessionHandle? _session;

    /// <summary>
    /// 初始化 MongoDB 上下文
    /// </summary>
    /// <param name="client">MongoDB 客户端</param>
    /// <param name="databaseName">数据库名称</param>
    public MongoDbContext(IMongoClient client, string databaseName)
    {
        this._database = client.GetDatabase(databaseName);
    }

    /// <summary>
    /// 获取数据库
    /// </summary>
    public IMongoDatabase Database => this._database;

    /// <summary>
    /// 获取当前会话
    /// </summary>
    public IClientSessionHandle? Session => this._session;

    /// <summary>
    /// 获取集合
    /// </summary>
    /// <typeparam name="T">文档类型</typeparam>
    /// <param name="name">集合名称</param>
    /// <returns>集合</returns>
    public IMongoCollection<T> GetCollection<T>(string? name = null)
    {
        return this._database.GetCollection<T>(name ?? typeof(T).Name);
    }

    /// <summary>
    /// 保存更改（MongoDB 自动保存）
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>受影响的行数</returns>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // MongoDB 是即时写入的，无需显式保存
        return Task.FromResult(0);
    }

    /// <summary>
    /// 开始事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (this._session != null)
        {
            return;
        }

        this._session = await this._database.Client.StartSessionAsync(cancellationToken: cancellationToken);
        this._session.StartTransaction();
    }

    /// <summary>
    /// 提交事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (this._session == null)
        {
            throw new InvalidOperationException("没有活动的事务可以提交");
        }

        await this._session.CommitTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// 回滚事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (this._session == null)
        {
            return;
        }

        await this._session.AbortTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        this._session?.Dispose();
        GC.SuppressFinalize(this);
    }
}
