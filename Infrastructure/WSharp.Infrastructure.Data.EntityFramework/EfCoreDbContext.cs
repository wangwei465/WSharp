using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WSharp.Core.Domain.Entities;
using WSharp.Infrastructure.Data;

namespace WSharp.Infrastructure.Data.EntityFramework;

/// <summary>
/// Entity Framework Core 数据库上下文基类
/// </summary>
public abstract class EfCoreDbContext : DbContext, IDbContext
{
    private IDbContextTransaction? _currentTransaction;

    /// <summary>
    /// 初始化数据库上下文
    /// </summary>
    /// <param name="options">配置选项</param>
    protected EfCoreDbContext(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    /// 保存更改前的处理
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>受影响的行数</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.ApplyAudit();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 开始事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    public virtual async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (this._currentTransaction != null)
        {
            return;
        }

        this._currentTransaction = await this.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// 提交事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    public virtual async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await this.SaveChangesAsync(cancellationToken);
            await (this._currentTransaction?.CommitAsync(cancellationToken) ?? Task.CompletedTask);
        }
        catch
        {
            await this.RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (this._currentTransaction != null)
            {
                this._currentTransaction.Dispose();
                this._currentTransaction = null;
            }
        }
    }

    /// <summary>
    /// 回滚事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    public virtual async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await (this._currentTransaction?.RollbackAsync(cancellationToken) ?? Task.CompletedTask);
        }
        finally
        {
            if (this._currentTransaction != null)
            {
                this._currentTransaction.Dispose();
                this._currentTransaction = null;
            }
        }
    }

    /// <summary>
    /// 应用审计信息
    /// </summary>
    private void ApplyAudit()
    {
        var entries = this.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

        var now = DateTime.UtcNow;
        var currentUser = this.GetCurrentUser();

        foreach (var entry in entries)
        {
            // 处理软删除
            if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete softDelete)
            {
                entry.State = EntityState.Modified;
                softDelete.IsDeleted = true;
                softDelete.DeletedAt = now;
                softDelete.DeletedBy = currentUser;
            }

            // 处理创建审计
            if (entry.State == EntityState.Added && entry.Entity is IAuditable auditable)
            {
                auditable.CreatedAt = now;
                auditable.CreatedBy = currentUser;
            }

            // 处理修改审计
            if (entry.State == EntityState.Modified && entry.Entity is IAuditable modifiedAuditable)
            {
                modifiedAuditable.UpdatedAt = now;
                modifiedAuditable.UpdatedBy = currentUser;
            }
        }
    }

    /// <summary>
    /// 获取当前用户（子类可重写）
    /// </summary>
    /// <returns>当前用户标识</returns>
    protected virtual string? GetCurrentUser()
    {
        // 子类应重写此方法以返回当前用户信息
        // 例如从 IHttpContextAccessor 或其他服务获取
        return null;
    }

    /// <summary>
    /// 配置模型
    /// </summary>
    /// <param name="modelBuilder">模型构建器</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置软删除全局查询过滤器
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(this.CreateSoftDeleteFilter(entityType.ClrType));
            }
        }
    }

    /// <summary>
    /// 创建软删除过滤器
    /// </summary>
    /// <param name="entityType">实体类型</param>
    /// <returns>过滤器表达式</returns>
    private LambdaExpression CreateSoftDeleteFilter(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
        var condition = Expression.Equal(property, Expression.Constant(false));
        return Expression.Lambda(condition, parameter);
    }
}
