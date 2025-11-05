using System.Linq.Expressions;
using WSharp.Core.Domain.Specifications;

namespace WSharp.Infrastructure.Data.Specifications;

/// <summary>
/// 规约评估器（用于将规约转换为查询）
/// </summary>
public static class SpecificationEvaluator
{
    /// <summary>
    /// 应用规约到查询
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="inputQuery">输入查询</param>
    /// <param name="specification">规约</param>
    /// <returns>应用规约后的查询</returns>
    public static IQueryable<T> GetQuery<T>(
        IQueryable<T> inputQuery,
        ISpecification<T> specification) where T : class
    {
        var query = inputQuery;

        // 应用条件
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // 应用包含导航属性（表达式形式）
        query = specification.Includes
            .Aggregate(query, (current, include) => ApplyInclude(current, include));

        // 应用包含导航属性（字符串形式）
        query = specification.IncludeStrings
            .Aggregate(query, (current, includeString) => ApplyInclude(current, includeString));

        // 应用排序
        if (specification.OrderBy != null)
        {
            query = ApplyOrderBy(query, specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = ApplyOrderByDescending(query, specification.OrderByDescending);
        }

        // 应用分组
        if (specification.GroupBy != null)
        {
            query = ApplyGroupBy(query, specification.GroupBy);
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
                query = query.Take(specification.Take.Value);
            }
        }

        return query;
    }

    /// <summary>
    /// 应用包含导航属性（表达式形式）
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">查询</param>
    /// <param name="include">包含表达式</param>
    /// <returns>应用后的查询</returns>
    private static IQueryable<T> ApplyInclude<T>(
        IQueryable<T> query,
        Expression<Func<T, object>> include) where T : class
    {
        // 子类需要重写此方法以支持 EF Core 的 Include
        // 默认返回原查询
        return query;
    }

    /// <summary>
    /// 应用包含导航属性（字符串形式）
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">查询</param>
    /// <param name="includeString">包含字符串</param>
    /// <returns>应用后的查询</returns>
    private static IQueryable<T> ApplyInclude<T>(
        IQueryable<T> query,
        string includeString) where T : class
    {
        // 子类需要重写此方法以支持 EF Core 的 Include
        // 默认返回原查询
        return query;
    }

    /// <summary>
    /// 应用排序
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">查询</param>
    /// <param name="orderBy">排序表达式</param>
    /// <returns>应用后的查询</returns>
    private static IQueryable<T> ApplyOrderBy<T>(
        IQueryable<T> query,
        Expression<Func<T, object>> orderBy) where T : class
    {
        return query.OrderBy(orderBy);
    }

    /// <summary>
    /// 应用降序排序
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">查询</param>
    /// <param name="orderByDescending">降序排序表达式</param>
    /// <returns>应用后的查询</returns>
    private static IQueryable<T> ApplyOrderByDescending<T>(
        IQueryable<T> query,
        Expression<Func<T, object>> orderByDescending) where T : class
    {
        return query.OrderByDescending(orderByDescending);
    }

    /// <summary>
    /// 应用分组
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">查询</param>
    /// <param name="groupBy">分组表达式</param>
    /// <returns>应用后的查询</returns>
    private static IQueryable<T> ApplyGroupBy<T>(
        IQueryable<T> query,
        Expression<Func<T, object>> groupBy) where T : class
    {
        return query.GroupBy(groupBy).SelectMany(x => x);
    }
}
