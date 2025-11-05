using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WSharp.Core.Domain.Entities;
using WSharp.Core.Domain.Specifications;
using WSharp.Infrastructure.Data.Repositories;

namespace WSharp.Infrastructure.Data.EntityFramework;

/// <summary>
/// Entity Framework Core 仓储实现
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">主键类��</typeparam>
public class EfCoreRepository<TEntity, TKey> : RepositoryBase<TEntity, TKey>
    where TEntity : Entity<TKey>
    where TKey : notnull
{
    private readonly EfCoreDbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    /// <summary>
    /// 初始化仓储
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    public EfCoreRepository(EfCoreDbContext dbContext)
    {
        this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this._dbSet = this._dbContext.Set<TEntity>();
    }

    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    /// <param name="id">主键</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体</returns>
    public override async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await this._dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <summary>
    /// 根据条件获取第一个实体
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体</returns>
    public override async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await this._dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// 根据规约获取第一个实体
    /// </summary>
    /// <param name="specification">规约</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体</returns>
    public override async Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = EfCoreSpecificationEvaluator.GetQuery(this._dbSet.AsQueryable(), specification);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 获取所有实体
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体集合</returns>
    public override async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await this._dbSet.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 根据条件获取实体列表
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体集合</returns>
    public override async Task<IReadOnlyList<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await this._dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 根据规约获取实体列表
    /// </summary>
    /// <param name="specification">规约</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体集合</returns>
    public override async Task<IReadOnlyList<TEntity>> GetListAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = EfCoreSpecificationEvaluator.GetQuery(this._dbSet.AsQueryable(), specification);
        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 获取实体总数
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>总数</returns>
    public override async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await this._dbSet.CountAsync(cancellationToken);
    }

    /// <summary>
    /// 根据条件获取实体总数
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>总数</returns>
    public override async Task<int> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await this._dbSet.CountAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// 判断实体是否存在
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    public override async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await this._dbSet.AnyAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// 添加实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>添加的实体</returns>
    public override async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await this._dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <summary>
    /// 批量添加实体
    /// </summary>
    /// <param name="entities">实体集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    public override async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await this._dbSet.AddRangeAsync(entities, cancellationToken);
    }

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    public override Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        this._dbSet.Update(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 批量更新实体
    /// </summary>
    /// <param name="entities">实体集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    public override Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        this._dbSet.UpdateRange(entities);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    public override Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        this._dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 根据ID删除实体
    /// </summary>
    /// <param name="id">主键</param>
    /// <param name="cancellationToken">取消令牌</param>
    public override async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await this.GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            await this.DeleteAsync(entity, cancellationToken);
        }
    }

    /// <summary>
    /// 批量删除实体
    /// </summary>
    /// <param name="entities">实体集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    public override Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        this._dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }
}
