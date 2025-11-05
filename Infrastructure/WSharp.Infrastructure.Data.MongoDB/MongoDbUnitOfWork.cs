using WSharp.Infrastructure.Data.UnitOfWork;

namespace WSharp.Infrastructure.Data.MongoDB;

/// <summary>
/// MongoDB 工作单元
/// </summary>
public class MongoDbUnitOfWork : UnitOfWorkBase
{
    /// <summary>
    /// 初始化工作单元
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    public MongoDbUnitOfWork(MongoDbContext dbContext) : base(dbContext)
    {
    }
}
