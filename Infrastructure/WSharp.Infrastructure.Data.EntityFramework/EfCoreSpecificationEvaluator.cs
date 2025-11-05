using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WSharp.Core.Domain.Specifications;

namespace WSharp.Infrastructure.Data.EntityFramework;

/// <summary>
/// Entity Framework Core 规约评估器
/// </summary>
public static class EfCoreSpecificationEvaluator
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
            .Aggregate(query, (current, include) => current.Include(include));

        // 应用包含导航属性（字符串形式）
        query = specification.IncludeStrings
            .Aggregate(query, (current, includeString) => current.Include(includeString));

        // 应用排序
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        // 应用分组
        if (specification.GroupBy != null)
        {
            query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
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
}
