using System.Linq.Expressions;

namespace WSharp.Core.Domain.Specifications;

/// <summary>
/// 规约接口
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// 获取条件表达式
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// 获取包含导航属性的表达式列表
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// 获取包含字符串格式的导航属性列表
    /// </summary>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// 获取排序表达式
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// 获取降序排序表达式
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// 获取分组表达式
    /// </summary>
    Expression<Func<T, object>>? GroupBy { get; }

    /// <summary>
    /// 获取跳过记录数
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// 获取获取记录数
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// 获取是否启用分页
    /// </summary>
    bool IsPagingEnabled { get; }
}

/// <summary>
/// 规约基类
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public abstract class Specification<T> : ISpecification<T>
{
    /// <summary>
    /// 获取条件表达式
    /// </summary>
    public Expression<Func<T, bool>>? Criteria { get; private set; }

    /// <summary>
    /// 获取包含导航属性的表达式列表
    /// </summary>
    public List<Expression<Func<T, object>>> Includes { get; } = new();

    /// <summary>
    /// 获取包含字符串格式的导航属性列表
    /// </summary>
    public List<string> IncludeStrings { get; } = new();

    /// <summary>
    /// 获取排序表达式
    /// </summary>
    public Expression<Func<T, object>>? OrderBy { get; private set; }

    /// <summary>
    /// 获取降序排序表达式
    /// </summary>
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    /// <summary>
    /// 获取分组表达式
    /// </summary>
    public Expression<Func<T, object>>? GroupBy { get; private set; }

    /// <summary>
    /// 获取跳过记录数
    /// </summary>
    public int? Skip { get; private set; }

    /// <summary>
    /// 获取获取记录数
    /// </summary>
    public int? Take { get; private set; }

    /// <summary>
    /// 获取是否启用分页
    /// </summary>
    public bool IsPagingEnabled { get; private set; }

    /// <summary>
    /// 添加条件
    /// </summary>
    /// <param name="criteria">条件表达式</param>
    protected void AddCriteria(Expression<Func<T, bool>> criteria)
    {
        this.Criteria = criteria;
    }

    /// <summary>
    /// 添加包含导航属性
    /// </summary>
    /// <param name="includeExpression">包含表达式</param>
    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        this.Includes.Add(includeExpression);
    }

    /// <summary>
    /// 添加包含导航属性（字符串格式）
    /// </summary>
    /// <param name="includeString">包含字符串</param>
    protected void AddInclude(string includeString)
    {
        this.IncludeStrings.Add(includeString);
    }

    /// <summary>
    /// 应用排序
    /// </summary>
    /// <param name="orderByExpression">排序表达式</param>
    protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        this.OrderBy = orderByExpression;
    }

    /// <summary>
    /// 应用降序排序
    /// </summary>
    /// <param name="orderByDescendingExpression">降序排序表达式</param>
    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        this.OrderByDescending = orderByDescendingExpression;
    }

    /// <summary>
    /// 应用分组
    /// </summary>
    /// <param name="groupByExpression">分组表达式</param>
    protected void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
    {
        this.GroupBy = groupByExpression;
    }

    /// <summary>
    /// 应用分页
    /// </summary>
    /// <param name="skip">跳过记录数</param>
    /// <param name="take">获取记录数</param>
    protected void ApplyPaging(int skip, int take)
    {
        this.Skip = skip;
        this.Take = take;
        this.IsPagingEnabled = true;
    }
}
