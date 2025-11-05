using FluentValidation;
using MediatR;
using WSharp.Core.Shared.Constants;
using WSharp.Core.Shared.Results;

namespace WSharp.Core.Application.Behaviors;

/// <summary>
/// 验证行为管道
/// </summary>
/// <typeparam name="TRequest">请求类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// 初始化验证行为
    /// </summary>
    /// <param name="validators">验证器集合</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        this._validators = validators;
    }

    /// <summary>
    /// 处理请求
    /// </summary>
    /// <param name="request">请求</param>
    /// <param name="next">下一个处理器</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>响应</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!this._validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            this._validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));
            return (TResponse)(object)Result.Failure(errorMessage, ErrorCodes.VALIDATION_FAILED);
        }

        return await next();
    }
}
