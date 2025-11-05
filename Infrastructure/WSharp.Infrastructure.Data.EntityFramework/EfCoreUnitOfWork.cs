using WSharp.Infrastructure.Data.UnitOfWork;

namespace WSharp.Infrastructure.Data.EntityFramework;

/// <summary>
/// Entity Framework Core 工作单元
/// </summary>
public class EfCoreUnitOfWork : UnitOfWorkBase
{
    /// <summary>
    /// 初始化工作单元
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    public EfCoreUnitOfWork(EfCoreDbContext dbContext) : base(dbContext)
    {
    }
}
