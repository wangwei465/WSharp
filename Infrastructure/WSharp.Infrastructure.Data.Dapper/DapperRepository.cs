using System.Data;
using System.Linq.Expressions;
using System.Text;
using Dapper;
using WSharp.Core.Domain.Entities;
using WSharp.Core.Domain.Specifications;
using WSharp.Infrastructure.Data.Repositories;

namespace WSharp.Infrastructure.Data.Dapper;

/// <summary>
/// Dapper 仓储实现
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
public abstract class DapperRepository<TEntity, TKey> : RepositoryBase<TEntity, TKey>
    where TEntity : Entity<TKey>
    where TKey : notnull
{
    private readonly DapperDbContext _dbContext;

    /// <summary>
    /// 初始化仓储
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    protected DapperRepository(DapperDbContext dbContext)
    {
        this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// 获取数据库连接
    /// </summary>
    protected IDbConnection Connection => this._dbContext.Connection;

    /// <summary>
    /// 获取当前事务
    /// </summary>
    protected IDbTransaction? Transaction => this._dbContext.Transaction;

    /// <summary>
    /// 获取表名（子类必须实现）
    /// </summary>
    protected abstract string TableName { get; }

    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    /// <param name="id">主键</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体</returns>
    public override async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT * FROM {this.TableName} WHERE Id = @Id";
        return await this.Connection.QueryFirstOrDefaultAsync<TEntity>(
            new CommandDefinition(sql, new { Id = id }, this.Transaction, cancellationToken: cancellationToken));
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
        var (whereClause, parameters) = this.BuildWhereClause(predicate);
        var sql = $"SELECT TOP 1 * FROM {this.TableName} WHERE {whereClause}";
        return await this.Connection.QueryFirstOrDefaultAsync<TEntity>(
            new CommandDefinition(sql, parameters, this.Transaction, cancellationToken: cancellationToken));
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
        var (sql, parameters) = this.BuildQuery(specification, true);
        return await this.Connection.QueryFirstOrDefaultAsync<TEntity>(
            new CommandDefinition(sql, parameters, this.Transaction, cancellationToken: cancellationToken));
    }

    /// <summary>
    /// 获取所有实体
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体集合</returns>
    public override async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT * FROM {this.TableName}";
        var result = await this.Connection.QueryAsync<TEntity>(
            new CommandDefinition(sql, transaction: this.Transaction, cancellationToken: cancellationToken));
        return result.ToList();
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
        var (whereClause, parameters) = this.BuildWhereClause(predicate);
        var sql = $"SELECT * FROM {this.TableName} WHERE {whereClause}";
        var result = await this.Connection.QueryAsync<TEntity>(
            new CommandDefinition(sql, parameters, this.Transaction, cancellationToken: cancellationToken));
        return result.ToList();
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
        var (sql, parameters) = this.BuildQuery(specification);
        var result = await this.Connection.QueryAsync<TEntity>(
            new CommandDefinition(sql, parameters, this.Transaction, cancellationToken: cancellationToken));
        return result.ToList();
    }

    /// <summary>
    /// 获取实体总数
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>总数</returns>
    public override async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT COUNT(*) FROM {this.TableName}";
        return await this.Connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, transaction: this.Transaction, cancellationToken: cancellationToken));
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
        var (whereClause, parameters) = this.BuildWhereClause(predicate);
        var sql = $"SELECT COUNT(*) FROM {this.TableName} WHERE {whereClause}";
        return await this.Connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, parameters, this.Transaction, cancellationToken: cancellationToken));
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
        var (whereClause, parameters) = this.BuildWhereClause(predicate);
        var sql = $"SELECT CASE WHEN EXISTS(SELECT 1 FROM {this.TableName} WHERE {whereClause}) THEN 1 ELSE 0 END";
        var result = await this.Connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, parameters, this.Transaction, cancellationToken: cancellationToken));
        return result == 1;
    }

    /// <summary>
    /// 添加实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>添加的实体</returns>
    public override async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var (sql, parameters) = this.BuildInsertCommand(entity);
        await this.Connection.ExecuteAsync(
            new CommandDefinition(sql, parameters, this.Transaction, cancellationToken: cancellationToken));
        return entity;
    }

    /// <summary>
    /// 批量添加实体
    /// </summary>
    /// <param name="entities">实体集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    public override async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await this.AddAsync(entity, cancellationToken);
        }
    }

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    public override async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var (sql, parameters) = this.BuildUpdateCommand(entity);
        await this.Connection.ExecuteAsync(
            new CommandDefinition(sql, parameters, this.Transaction, cancellationToken: cancellationToken));
    }

    /// <summary>
    /// 批量更新实体
    /// </summary>
    /// <param name="entities">实体集合</param>
    /// <param name="cancellationToken">取消令牌</param>
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
    /// <param name="entity">实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    public override async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var sql = $"DELETE FROM {this.TableName} WHERE Id = @Id";
        await this.Connection.ExecuteAsync(
            new CommandDefinition(sql, new { entity.Id }, this.Transaction, cancellationToken: cancellationToken));
    }

    /// <summary>
    /// 根据ID删除实体
    /// </summary>
    /// <param name="id">主键</param>
    /// <param name="cancellationToken">取消令牌</param>
    public override async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var sql = $"DELETE FROM {this.TableName} WHERE Id = @Id";
        await this.Connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, this.Transaction, cancellationToken: cancellationToken));
    }

    /// <summary>
    /// 批量删除实体
    /// </summary>
    /// <param name="entities">实体集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    public override async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await this.DeleteAsync(entity, cancellationToken);
        }
    }

    /// <summary>
    /// 构建 WHERE 子句（简化版本，子类应根据需要重写）
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <returns>WHERE 子句和参数</returns>
    protected virtual (string whereClause, object parameters) BuildWhereClause(Expression<Func<TEntity, bool>> predicate)
    {
        // 简化实现：仅支持基本的相等比较
        // 实际项目中应使用 SqlKata 或类似的 SQL 构建器
        throw new NotImplementedException("子类应实现 BuildWhereClause 方法以支持动态查询");
    }

    /// <summary>
    /// 构建查询 SQL（简化版本）
    /// </summary>
    /// <param name="specification">规约</param>
    /// <param name="firstOnly">是否只查询第一条</param>
    /// <returns>SQL 和参数</returns>
    protected virtual (string sql, object? parameters) BuildQuery(ISpecification<TEntity> specification, bool firstOnly = false)
    {
        var sql = new StringBuilder($"SELECT {(firstOnly ? "TOP 1" : "")} * FROM {this.TableName}");

        // 简化实现：实际项目中应根据 specification 构建完整的 SQL
        // 包括 WHERE、ORDER BY、OFFSET/FETCH 等

        return (sql.ToString(), null);
    }

    /// <summary>
    /// 构建 INSERT 命令（子类应重写以指定列）
    /// </summary>
    /// <param name="entity">实体</param>
    /// <returns>SQL 和参数</returns>
    protected abstract (string sql, object parameters) BuildInsertCommand(TEntity entity);

    /// <summary>
    /// 构建 UPDATE 命令（子类应重写以指定列）
    /// </summary>
    /// <param name="entity">实体</param>
    /// <returns>SQL 和参数</returns>
    protected abstract (string sql, object parameters) BuildUpdateCommand(TEntity entity);
}
