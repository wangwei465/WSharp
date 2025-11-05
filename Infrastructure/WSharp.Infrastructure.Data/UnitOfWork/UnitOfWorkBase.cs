using WSharp.Core.Domain.Repositories;

namespace WSharp.Infrastructure.Data.UnitOfWork;

/// <summary>
/// 工作单元基类
/// </summary>
public abstract class UnitOfWorkBase : IUnitOfWork
{
    private readonly IDbContext _dbContext;
    private bool _disposed;

    /// <summary>
    /// 初始化工作单元
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    protected UnitOfWorkBase(IDbContext dbContext)
    {
        this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// 保存更改
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>受影响的行数</returns>
    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await this._dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 开始事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    public virtual async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await this._dbContext.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// 提交事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    public virtual async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await this._dbContext.CommitTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// 回滚事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    public virtual async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await this._dbContext.RollbackTransactionAsync(cancellationToken);
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
                this._dbContext?.Dispose();
            }

            this._disposed = true;
        }
    }
}
