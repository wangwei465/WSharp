using MediatR;
using WSharp.Core.Shared.Results;

namespace WSharp.Core.Application.Queries;

/// <summary>
/// 查询处理器接口
/// </summary>
/// <typeparam name="TQuery">查询类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
