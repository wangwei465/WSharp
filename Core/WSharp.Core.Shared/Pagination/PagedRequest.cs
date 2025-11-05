namespace WSharp.Core.Shared.Pagination;

/// <summary>
/// 分页请求参数
/// </summary>
public class PagedRequest
{
    private int _pageNumber = 1;
    private int _pageSize = 10;

    /// <summary>
    /// 获取或设置页码（从1开始）
    /// </summary>
    public int PageNumber
    {
        get => this._pageNumber;
        set => this._pageNumber = value < 1 ? 1 : value;
    }

    /// <summary>
    /// 获取或设置每页大小
    /// </summary>
    public int PageSize
    {
        get => this._pageSize;
        set => this._pageSize = value < 1 ? 10 : (value > 100 ? 100 : value);
    }

    /// <summary>
    /// 获取跳过的记录数
    /// </summary>
    public int Skip => (this.PageNumber - 1) * this.PageSize;

    /// <summary>
    /// 获取获取的记录数
    /// </summary>
    public int Take => this.PageSize;

    /// <summary>
    /// 初始化 <see cref="PagedRequest"/> 类的新实例
    /// </summary>
    public PagedRequest()
    {
    }

    /// <summary>
    /// 初始化 <see cref="PagedRequest"/> 类的新实例
    /// </summary>
    /// <param name="pageNumber">页码</param>
    /// <param name="pageSize">每页大小</param>
    public PagedRequest(int pageNumber, int pageSize)
    {
        this.PageNumber = pageNumber;
        this.PageSize = pageSize;
    }
}

/// <summary>
/// 分页结果
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// 获取或设置当前页的数据
    /// </summary>
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

    /// <summary>
    /// 获取或设置当前页码
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// 获取或设置每页大小
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 获取或设置总记录数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 获取总页数
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(this.TotalCount / (double)this.PageSize);

    /// <summary>
    /// 获取是否有上一页
    /// </summary>
    public bool HasPreviousPage => this.PageNumber > 1;

    /// <summary>
    /// 获取是否有下一页
    /// </summary>
    public bool HasNextPage => this.PageNumber < this.TotalPages;

    /// <summary>
    /// 初始化 <see cref="PagedResult{T}"/> 类的新实例
    /// </summary>
    public PagedResult()
    {
    }

    /// <summary>
    /// 初始化 <see cref="PagedResult{T}"/> 类的新实例
    /// </summary>
    /// <param name="items">当前页的数据</param>
    /// <param name="pageNumber">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <param name="totalCount">总记录数</param>
    public PagedResult(IEnumerable<T> items, int pageNumber, int pageSize, int totalCount)
    {
        this.Items = items.ToList().AsReadOnly();
        this.PageNumber = pageNumber;
        this.PageSize = pageSize;
        this.TotalCount = totalCount;
    }

    /// <summary>
    /// 从 <see cref="PagedList{T}"/> 创建分页结果
    /// </summary>
    /// <param name="pagedList">分页列表</param>
    /// <returns>分页结果</returns>
    public static PagedResult<T> FromPagedList(PagedList<T> pagedList)
    {
        return new PagedResult<T>(
            pagedList.Items,
            pagedList.PageNumber,
            pagedList.PageSize,
            pagedList.TotalCount);
    }

    /// <summary>
    /// 创建空的分页结果
    /// </summary>
    /// <returns>空的分页结果</returns>
    public static PagedResult<T> Empty()
    {
        return new PagedResult<T>(Enumerable.Empty<T>(), 1, 10, 0);
    }
}
