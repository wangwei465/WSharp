namespace WSharp.Core.Shared.Pagination;

/// <summary>
/// 分页列表
/// </summary>
/// <typeparam name="T">列表项类型</typeparam>
public class PagedList<T>
{
    /// <summary>
    /// 获取当前页的数据
    /// </summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>
    /// 获取当前页码（从1开始）
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// 获取每页大小
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// 获取总记录数
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// 获取总页数
    /// </summary>
    public int TotalPages { get; }

    /// <summary>
    /// 获取是否有上一页
    /// </summary>
    public bool HasPreviousPage => this.PageNumber > 1;

    /// <summary>
    /// 获取是否有下一页
    /// </summary>
    public bool HasNextPage => this.PageNumber < this.TotalPages;

    /// <summary>
    /// 初始化 <see cref="PagedList{T}"/> 类的新实例
    /// </summary>
    /// <param name="items">当前页的数据</param>
    /// <param name="pageNumber">当前页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <param name="totalCount">总记录数</param>
    public PagedList(IEnumerable<T> items, int pageNumber, int pageSize, int totalCount)
    {
        this.Items = items.ToList().AsReadOnly();
        this.PageNumber = pageNumber;
        this.PageSize = pageSize;
        this.TotalCount = totalCount;
        this.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    /// <summary>
    /// 创建空的分页列表
    /// </summary>
    /// <returns>空的分页列表</returns>
    public static PagedList<T> Empty()
    {
        return new PagedList<T>(Enumerable.Empty<T>(), 1, 10, 0);
    }

    /// <summary>
    /// 从列表创建分页列表
    /// </summary>
    /// <param name="source">源列表</param>
    /// <param name="pageNumber">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <returns>分页列表</returns>
    public static PagedList<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return new PagedList<T>(items, pageNumber, pageSize, count);
    }
}
