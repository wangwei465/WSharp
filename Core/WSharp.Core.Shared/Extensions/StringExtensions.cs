using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace WSharp.Core.Shared.Extensions;

/// <summary>
/// 字符串扩展方法
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 判断字符串是否为 null 或空
    /// </summary>
    /// <param name="value">字符串</param>
    /// <returns>如果字符串为 null 或空，则为 true；否则为 false</returns>
    public static bool IsNullOrEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// 判断字符串是否为 null 或空白
    /// </summary>
    /// <param name="value">字符串</param>
    /// <returns>如果字符串为 null 或空白，则为 true；否则为 false</returns>
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// 截取字符串，如果超过最大长度则添加省略号
    /// </summary>
    /// <param name="value">字符串</param>
    /// <param name="maxLength">最大长度</param>
    /// <param name="suffix">后缀（默认为"..."）</param>
    /// <returns>截取后的字符串</returns>
    public static string Truncate(this string value, int maxLength, string suffix = "...")
    {
        if (value.IsNullOrEmpty() || value.Length <= maxLength)
        {
            return value;
        }

        return value.Substring(0, maxLength) + suffix;
    }

    /// <summary>
    /// 转换为 PascalCase（大驼峰命名）
    /// </summary>
    /// <param name="value">字符串</param>
    /// <returns>PascalCase 字符串</returns>
    public static string ToPascalCase(this string value)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return value;
        }

        var words = value.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
        var result = new StringBuilder();

        foreach (var word in words)
        {
            result.Append(char.ToUpper(word[0], CultureInfo.InvariantCulture));
            result.Append(word.Substring(1).ToLower(CultureInfo.InvariantCulture));
        }

        return result.ToString();
    }

    /// <summary>
    /// 转换为 camelCase（小驼峰命名）
    /// </summary>
    /// <param name="value">字符串</param>
    /// <returns>camelCase 字符串</returns>
    public static string ToCamelCase(this string value)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return value;
        }

        var pascalCase = value.ToPascalCase();
        return char.ToLower(pascalCase[0], CultureInfo.InvariantCulture) + pascalCase.Substring(1);
    }

    /// <summary>
    /// 转换为 snake_case（蛇形命名）
    /// </summary>
    /// <param name="value">字符串</param>
    /// <returns>snake_case 字符串</returns>
    public static string ToSnakeCase(this string value)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return value;
        }

        var result = Regex.Replace(value, @"([a-z0-9])([A-Z])", "$1_$2");
        return result.ToLower(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// 转换为 kebab-case（短横线命名）
    /// </summary>
    /// <param name="value">字符串</param>
    /// <returns>kebab-case 字符串</returns>
    public static string ToKebabCase(this string value)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return value;
        }

        var result = Regex.Replace(value, @"([a-z0-9])([A-Z])", "$1-$2");
        return result.ToLower(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// 移除字符串中的所有空白字符
    /// </summary>
    /// <param name="value">字符串</param>
    /// <returns>移除空白后的字符串</returns>
    public static string RemoveWhiteSpace(this string value)
    {
        if (value.IsNullOrEmpty())
        {
            return value;
        }

        return Regex.Replace(value, @"\s+", string.Empty);
    }

    /// <summary>
    /// 将字符串转换为 Base64 编码
    /// </summary>
    /// <param name="value">字符串</param>
    /// <param name="encoding">编码（默认为 UTF-8）</param>
    /// <returns>Base64 编码的字符串</returns>
    public static string ToBase64(this string value, Encoding? encoding = null)
    {
        if (value.IsNullOrEmpty())
        {
            return value;
        }

        encoding ??= Encoding.UTF8;
        var bytes = encoding.GetBytes(value);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// 从 Base64 编码解码字符串
    /// </summary>
    /// <param name="value">Base64 编码的字符串</param>
    /// <param name="encoding">编码（默认为 UTF-8）</param>
    /// <returns>解码后的字符串</returns>
    public static string FromBase64(this string value, Encoding? encoding = null)
    {
        if (value.IsNullOrEmpty())
        {
            return value;
        }

        encoding ??= Encoding.UTF8;
        var bytes = Convert.FromBase64String(value);
        return encoding.GetString(bytes);
    }

    /// <summary>
    /// 判断字符串是否匹配正则表达式
    /// </summary>
    /// <param name="value">字符串</param>
    /// <param name="pattern">正则表达式模式</param>
    /// <returns>如果匹配，则为 true；否则为 false</returns>
    public static bool IsMatch(this string value, string pattern)
    {
        if (value.IsNullOrEmpty())
        {
            return false;
        }

        return Regex.IsMatch(value, pattern);
    }

    /// <summary>
    /// 判断字符串是否为有效的电子邮件地址
    /// </summary>
    /// <param name="value">字符串</param>
    /// <returns>如果是有效的电子邮件地址，则为 true；否则为 false</returns>
    public static bool IsValidEmail(this string value)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return false;
        }

        const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return value.IsMatch(pattern);
    }

    /// <summary>
    /// 判断字符串是否为有效的 URL
    /// </summary>
    /// <param name="value">字符串</param>
    /// <returns>如果是有效的 URL，则为 true；否则为 false</returns>
    public static bool IsValidUrl(this string value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// 反转字符串
    /// </summary>
    /// <param name="value">字符串</param>
    /// <returns>反转后的字符串</returns>
    public static string Reverse(this string value)
    {
        if (value.IsNullOrEmpty())
        {
            return value;
        }

        var charArray = value.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    /// <summary>
    /// 获取字符串的字节数（UTF-8 编码）
    /// </summary>
    /// <param name="value">字符串</param>
    /// <returns>字节数</returns>
    public static int GetByteCount(this string value)
    {
        if (value.IsNullOrEmpty())
        {
            return 0;
        }

        return Encoding.UTF8.GetByteCount(value);
    }
}
