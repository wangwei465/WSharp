using WSharp.Infrastructure.Data.UnitOfWork;

namespace WSharp.Infrastructure.Data.Dapper;

/// <summary>
/// Dapper 工作单元
/// </summary>
public class DapperUnitOfWork : UnitOfWorkBase
{
    /// <summary>
    /// 初始化工作单元
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    public DapperUnitOfWork(DapperDbContext dbContext) : base(dbContext)
    {
    }
}
