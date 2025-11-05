using MediatR;
using WSharp.Core.Shared.Results;

namespace WSharp.Core.Application.Commands;

/// <summary>
/// 命令接口（无返回值）
/// </summary>
public interface ICommand : IRequest<Result>
{
}

/// <summary>
/// 命令接口（带返回值）
/// </summary>
/// <typeparam name="TResponse">响应类型</typeparam>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
