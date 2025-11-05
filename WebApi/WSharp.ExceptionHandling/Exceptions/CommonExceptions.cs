using System.Net;

namespace WSharp.ExceptionHandling.Exceptions;

/// <summary>
/// 验证异常
/// </summary>
public class ValidationException : BusinessException
{
    /// <summary>
    /// 验证错误列表
    /// </summary>
    public Dictionary<string, string[]> Errors { get; set; }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation errors occurred.", "VALIDATION_ERROR", HttpStatusCode.BadRequest)
    {
        Errors = errors;
    }

    public ValidationException(string field, string error)
        : base($"Validation failed for field '{field}'.", "VALIDATION_ERROR", HttpStatusCode.BadRequest)
    {
        Errors = new Dictionary<string, string[]>
        {
            [field] = new[] { error }
        };
    }
}

/// <summary>
/// 未找到资源异常
/// </summary>
public class NotFoundException : BusinessException
{
    public NotFoundException(string resource, object key)
        : base($"Resource '{resource}' with key '{key}' was not found.", "NOT_FOUND", HttpStatusCode.NotFound)
    {
    }

    public NotFoundException(string message)
        : base(message, "NOT_FOUND", HttpStatusCode.NotFound)
    {
    }
}

/// <summary>
/// 未授权异常
/// </summary>
public class UnauthorizedException : BusinessException
{
    public UnauthorizedException(string message = "Unauthorized access.")
        : base(message, "UNAUTHORIZED", HttpStatusCode.Unauthorized)
    {
    }
}

/// <summary>
/// 禁止访问异常
/// </summary>
public class ForbiddenException : BusinessException
{
    public ForbiddenException(string message = "Access forbidden.")
        : base(message, "FORBIDDEN", HttpStatusCode.Forbidden)
    {
    }
}

/// <summary>
/// 冲突异常（如重复数据）
/// </summary>
public class ConflictException : BusinessException
{
    public ConflictException(string message)
        : base(message, "CONFLICT", HttpStatusCode.Conflict)
    {
    }
}
