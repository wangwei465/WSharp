using System.Linq.Expressions;
using MongoDB.Driver;
using WSharp.Core.Domain.Entities;
using WSharp.Core.Domain.Specifications;
using WSharp.Infrastructure.Data.Repositories;

namespace WSharp.Infrastructure.Data.MongoDB;

/// <summary>
/// MongoDB 仓储实现
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
public class MongoDbRepository<TEntity, TKey> : RepositoryBase<TEntity, TKey>
    where TEntity : Entity<TKey>
    where TKey : notnull
{
    private readonly MongoDbContext _context;
    private readonly IMongoCollection<TEntity> _collection;

    /// <summary>
    /// 初始化仓储
    /// </summary>
    /// <param name="context">MongoDB 上下文</param>
    /// <param name="collectionName">集合名称（可选）</param>
    public MongoDbRepository(MongoDbContext context, string? collectionName = null)
    {
        this._context = context ?? throw new ArgumentNullException(nameof(context));
        this._collection = context.GetCollection<TEntity>(collectionName);
    }

    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    public override async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id);
        return await this._collection.Find(this._context.Session, filter).FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 根据条件获取第一个实体
    /// </summary>
    public override async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await this._collection.Find(this._context.Session, predicate).FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 根据规约获取第一个实体
    /// </summary>
    public override async Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = this.ApplySpecification(specification);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 获取所有实体
    /// </summary>
    public override async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await this._collection.Find(this._context.Session, FilterDefinition<TEntity>.Empty)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 根据条件获取实体列表
    /// </summary>
    public override async Task<IReadOnlyList<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await this._collection.Find(this._context.Session, predicate).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 根据规约获取实体列表
    /// </summary>
    public override async Task<IReadOnlyList<TEntity>> GetListAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = this.ApplySpecification(specification);
        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 获取实体总数
    /// </summary>
    public override async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var count = await this._collection.CountDocumentsAsync(
            this._context.Session,
            FilterDefinition<TEntity>.Empty,
            cancellationToken: cancellationToken);
        return (int)count;
    }

    /// <summary>
    /// 根据条件获取实体总数
    /// </summary>
    public override async Task<int> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var count = await this._collection.CountDocumentsAsync(
            this._context.Session,
            predicate,
            cancellationToken: cancellationToken);
        return (int)count;
    }

    /// <summary>
    /// 判断实体是否存在
    /// </summary>
    public override async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var count = await this._collection.CountDocumentsAsync(
            this._context.Session,
            predicate,
            new CountOptions { Limit = 1 },
            cancellationToken);
        return count > 0;
    }

    /// <summary>
    /// 添加实体
    /// </summary>
    public override async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await this._collection.InsertOneAsync(this._context.Session, entity, cancellationToken: cancellationToken);
        return entity;
    }

    /// <summary>
    /// 批量添加实体
    /// </summary>
    public override async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await this._collection.InsertManyAsync(this._context.Session, entities, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 更新实体
    /// </summary>
    public override async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);
        await this._collection.ReplaceOneAsync(this._context.Session, filter, entity, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 批量更新实体
    /// </summary>
    public override async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await this.UpdateAsync(entity, cancellationToken);
        }
    }

    /// <summary>
    /// 删除实体
    /// </summary>
    public override async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);
        await this._collection.DeleteOneAsync(filter, cancellationToken);
    }

    /// <summary>
    /// 根据ID删除实体
    /// </summary>
    public override async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id);
        await this._collection.DeleteOneAsync(filter, cancellationToken);
    }

    /// <summary>
    /// 批量删除实体
    /// </summary>
    public override async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var ids = entities.Select(e => e.Id).ToList();
        var filter = Builders<TEntity>.Filter.In(e => e.Id, ids);
        await this._collection.DeleteManyAsync(filter, cancellationToken);
    }

    /// <summary>
    /// 应用规约到查询
    /// </summary>
    private IFindFluent<TEntity, TEntity> ApplySpecification(ISpecification<TEntity> specification)
    {
        var filter = specification.Criteria != null
            ? Builders<TEntity>.Filter.Where(specification.Criteria)
            : FilterDefinition<TEntity>.Empty;

        var query = this._collection.Find(this._context.Session, filter);

        // 应用排序
        if (specification.OrderBy != null)
        {
            query = query.SortBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.SortByDescending(specification.OrderByDescending);
        }

        // 应用分页
        if (specification.IsPagingEnabled)
        {
            if (specification.Skip.HasValue)
            {
                query = query.Skip(specification.Skip.Value);
            }

            if (specification.Take.HasValue)
            {
                query = query.Limit(specification.Take.Value);
            }
        }

        return query;
    }
}
