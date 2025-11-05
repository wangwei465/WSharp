namespace WSharp.Core.Shared.Results;

/// <summary>
/// 表示操作结果（无返回值）
/// </summary>
public class Result
{
    /// <summary>
    /// 获取操作是否成功
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// 获取操作是否失败
    /// </summary>
    public bool IsFailure => !this.IsSuccess;

    /// <summary>
    /// 获取错误信息
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// 获取错误代码
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// 初始化 <see cref="Result"/> 类的新实例
    /// </summary>
    /// <param name="isSuccess">是否成功</param>
    /// <param name="error">错误信息</param>
    /// <param name="errorCode">错误代码</param>
    protected Result(bool isSuccess, string? error, string? errorCode = null)
    {
        if (isSuccess && !string.IsNullOrWhiteSpace(error))
        {
            throw new ArgumentException("成功的结果不能包含错误信息", nameof(error));
        }

        if (!isSuccess && string.IsNullOrWhiteSpace(error))
        {
            throw new ArgumentException("失败的结果必须包含错误信息", nameof(error));
        }

        this.IsSuccess = isSuccess;
        this.Error = error;
        this.ErrorCode = errorCode;
    }

    /// <summary>
    /// 创建成功的结果
    /// </summary>
    /// <returns>成功的结果</returns>
    public static Result Success()
    {
        return new Result(true, null);
    }

    /// <summary>
    /// 创建失败的结果
    /// </summary>
    /// <param name="error">错误信息</param>
    /// <param name="errorCode">错误代码</param>
    /// <returns>失败的结果</returns>
    public static Result Failure(string error, string? errorCode = null)
    {
        return new Result(false, error, errorCode);
    }

    /// <summary>
    /// 创建成功的结果（带返回值）
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="value">返回值</param>
    /// <returns>成功的结果</returns>
    public static Result<T> Success<T>(T value)
    {
        return Result<T>.Success(value);
    }

    /// <summary>
    /// 创建失败的结果（带返回值）
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="error">错误信息</param>
    /// <param name="errorCode">错误代码</param>
    /// <returns>失败的结果</returns>
    public static Result<T> Failure<T>(string error, string? errorCode = null)
    {
        return Result<T>.Failure(error, errorCode);
    }
}

/// <summary>
/// 表示操作结果（带返回值）
/// </summary>
/// <typeparam name="T">返回值类型</typeparam>
public class Result<T> : Result
{
    private readonly T? _value;

    /// <summary>
    /// 获取返回值
    /// </summary>
    /// <exception cref="InvalidOperationException">操作失败时访问返回值</exception>
    public T Value
    {
        get
        {
            if (this.IsFailure)
            {
                throw new InvalidOperationException("不能访问失败结果的返回值");
            }

            return this._value!;
        }
    }

    /// <summary>
    /// 初始化 <see cref="Result{T}"/> 类的新实例
    /// </summary>
    /// <param name="isSuccess">是否成功</param>
    /// <param name="value">返回值</param>
    /// <param name="error">错误信息</param>
    /// <param name="errorCode">错误代码</param>
    protected Result(bool isSuccess, T? value, string? error, string? errorCode = null)
        : base(isSuccess, error, errorCode)
    {
        this._value = value;
    }

    /// <summary>
    /// 创建成功的结果
    /// </summary>
    /// <param name="value">返回值</param>
    /// <returns>成功的结果</returns>
    public new static Result<T> Success(T value)
    {
        return new Result<T>(true, value, null);
    }

    /// <summary>
    /// 创建失败的结果
    /// </summary>
    /// <param name="error">错误信息</param>
    /// <param name="errorCode">错误代码</param>
    /// <returns>失败的结果</returns>
    public new static Result<T> Failure(string error, string? errorCode = null)
    {
        return new Result<T>(false, default, error, errorCode);
    }

    /// <summary>
    /// 隐式转换为 T
    /// </summary>
    /// <param name="result">结果</param>
    public static implicit operator T(Result<T> result)
    {
        return result.Value;
    }
}
