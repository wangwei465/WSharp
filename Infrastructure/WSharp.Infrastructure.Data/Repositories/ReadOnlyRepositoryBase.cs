using System.Linq.Expressions;
using WSharp.Core.Domain.Entities;
using WSharp.Core.Domain.Repositories;
using WSharp.Core.Domain.Specifications;

namespace WSharp.Infrastructure.Data.Repositories;

/// <summary>
/// 只读仓储基类
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
public abstract class ReadOnlyRepositoryBase<TEntity, TKey> : IReadOnlyRepository<TEntity, TKey>
    where TEntity : Entity<TKey>
    where TKey : notnull
{
    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    /// <param name="id">主键</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体</returns>
    public abstract Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件获取第一个实体
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体</returns>
    public abstract Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据规约获取第一个实体
    /// </summary>
    /// <param name="specification">规约</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体</returns>
    public abstract Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有实体
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体集合</returns>
    public abstract Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件获取实体列表
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体集合</returns>
    public abstract Task<IReadOnlyList<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据规约获取实体列表
    /// </summary>
    /// <param name="specification">规约</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体集合</returns>
    public abstract Task<IReadOnlyList<TEntity>> GetListAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取实体总数
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>总数</returns>
    public abstract Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件获取实体总数
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>总数</returns>
    public abstract Task<int> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断实体是否存在
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    public abstract Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
}
