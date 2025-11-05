namespace WSharp.Core.Shared.Guards;

/// <summary>
/// 参数验证辅助类
/// </summary>
public static class Guard
{
    /// <summary>
    /// 确保参数不为 null
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="value">参数值</param>
    /// <param name="parameterName">参数名</param>
    /// <returns>参数值</returns>
    /// <exception cref="ArgumentNullException">参数为 null 时抛出</exception>
    public static T NotNull<T>(T? value, string parameterName) where T : class
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }

        return value;
    }

    /// <summary>
    /// 确保字符串参数不为 null 或空白
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="parameterName">参数名</param>
    /// <returns>参数值</returns>
    /// <exception cref="ArgumentException">参数为 null 或空白时抛出</exception>
    public static string NotNullOrWhiteSpace(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("参数不能为 null 或空白", parameterName);
        }

        return value;
    }

    /// <summary>
    /// 确保字符串参数不为 null 或空
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="parameterName">参数名</param>
    /// <returns>参数值</returns>
    /// <exception cref="ArgumentException">参数为 null 或空时抛出</exception>
    public static string NotNullOrEmpty(string? value, string parameterName)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("参数不能为 null 或空", parameterName);
        }

        return value;
    }

    /// <summary>
    /// 确保集合参数不为 null 或空
    /// </summary>
    /// <typeparam name="T">集合元素类型</typeparam>
    /// <param name="value">参数值</param>
    /// <param name="parameterName">参数名</param>
    /// <returns>参数值</returns>
    /// <exception cref="ArgumentException">参数为 null 或空时抛出</exception>
    public static IEnumerable<T> NotNullOrEmpty<T>(IEnumerable<T>? value, string parameterName)
    {
        if (value is null || !value.Any())
        {
            throw new ArgumentException("参数不能为 null 或空", parameterName);
        }

        return value;
    }

    /// <summary>
    /// 确保数值参数在指定范围内
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="parameterName">参数名</param>
    /// <returns>参数值</returns>
    /// <exception cref="ArgumentOutOfRangeException">参数超出范围时抛出</exception>
    public static int InRange(int value, int min, int max, string parameterName)
    {
        if (value < min || value > max)
        {
            throw new ArgumentOutOfRangeException(parameterName, $"参数必须在 {min} 到 {max} 之间");
        }

        return value;
    }

    /// <summary>
    /// 确保数值参数大于指定值
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="min">最小值（不包含）</param>
    /// <param name="parameterName">参数名</param>
    /// <returns>参数值</returns>
    /// <exception cref="ArgumentOutOfRangeException">参数小于等于最小值时抛出</exception>
    public static int GreaterThan(int value, int min, string parameterName)
    {
        if (value <= min)
        {
            throw new ArgumentOutOfRangeException(parameterName, $"参数必须大于 {min}");
        }

        return value;
    }

    /// <summary>
    /// 确保数值参数大于等于指定值
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="min">最小值（包含）</param>
    /// <param name="parameterName">参数名</param>
    /// <returns>参数值</returns>
    /// <exception cref="ArgumentOutOfRangeException">参数小于最小值时抛出</exception>
    public static int GreaterThanOrEqual(int value, int min, string parameterName)
    {
        if (value < min)
        {
            throw new ArgumentOutOfRangeException(parameterName, $"参数必须大于等于 {min}");
        }

        return value;
    }

    /// <summary>
    /// 确保数值参数小于指定值
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="max">最大值（不包含）</param>
    /// <param name="parameterName">参数名</param>
    /// <returns>参数值</returns>
    /// <exception cref="ArgumentOutOfRangeException">参数大于等于最大值时抛出</exception>
    public static int LessThan(int value, int max, string parameterName)
    {
        if (value >= max)
        {
            throw new ArgumentOutOfRangeException(parameterName, $"参数必须小于 {max}");
        }

        return value;
    }

    /// <summary>
    /// 确保数值参数小于等于指定值
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="max">最大值（包含）</param>
    /// <param name="parameterName">参数名</param>
    /// <returns>参数值</returns>
    /// <exception cref="ArgumentOutOfRangeException">参数大于最大值时抛出</exception>
    public static int LessThanOrEqual(int value, int max, string parameterName)
    {
        if (value > max)
        {
            throw new ArgumentOutOfRangeException(parameterName, $"参数必须小于等于 {max}");
        }

        return value;
    }

    /// <summary>
    /// 确保条件为真
    /// </summary>
    /// <param name="condition">条件</param>
    /// <param name="message">错误消息</param>
    /// <exception cref="ArgumentException">条件为假时抛出</exception>
    public static void Against(bool condition, string message)
    {
        if (condition)
        {
            throw new ArgumentException(message);
        }
    }
}
