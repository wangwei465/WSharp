using System.Data;
using WSharp.Infrastructure.Data;

namespace WSharp.Infrastructure.Data.Dapper;

/// <summary>
/// Dapper 数据库上下文
/// </summary>
public class DapperDbContext : IDbContext
{
    private readonly IDbConnection _connection;
    private IDbTransaction? _transaction;
    private bool _disposed;

    /// <summary>
    /// 初始化 Dapper 数据库上下文
    /// </summary>
    /// <param name="connection">数据库连接</param>
    public DapperDbContext(IDbConnection connection)
    {
        this._connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    /// <summary>
    /// 获取数据库连接
    /// </summary>
    public IDbConnection Connection => this._connection;

    /// <summary>
    /// 获取当前事务
    /// </summary>
    public IDbTransaction? Transaction => this._transaction;

    /// <summary>
    /// 保存更改（Dapper 不需要显式保存）
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>受影响的行数</returns>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dapper 是直接执行 SQL，不需要显式保存更改
        return Task.FromResult(0);
    }

    /// <summary>
    /// 开始事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (this._transaction != null)
        {
            return Task.CompletedTask;
        }

        if (this._connection.State != ConnectionState.Open)
        {
            this._connection.Open();
        }

        this._transaction = this._connection.BeginTransaction();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 提交事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (this._transaction == null)
        {
            throw new InvalidOperationException("没有活动的事务可以提交");
        }

        try
        {
            this._transaction.Commit();
        }
        finally
        {
            this._transaction.Dispose();
            this._transaction = null;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 回滚事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (this._transaction == null)
        {
            return Task.CompletedTask;
        }

        try
        {
            this._transaction.Rollback();
        }
        finally
        {
            this._transaction.Dispose();
            this._transaction = null;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="disposing">是否释放托管资源</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                this._transaction?.Dispose();
                this._connection?.Dispose();
            }

            this._disposed = true;
        }
    }
}
