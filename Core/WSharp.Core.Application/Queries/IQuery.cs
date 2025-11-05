using MediatR;
using WSharp.Core.Shared.Results;

namespace WSharp.Core.Application.Queries;

/// <summary>
/// 查询接口
/// </summary>
/// <typeparam name="TResponse">响应类型</typeparam>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
