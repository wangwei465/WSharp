using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace WSharp.Infrastructure.Caching.Memory;

/// <summary>
/// 内存缓存键跟踪器，用于支持模式匹配删除
/// </summary>
public class MemoryKeyTracker
{
    private readonly ConcurrentDictionary<string, byte> _keys;

    public MemoryKeyTracker()
    {
        _keys = new ConcurrentDictionary<string, byte>();
    }

    /// <summary>
    /// 添加键
    /// </summary>
    public void AddKey(string key)
    {
        _keys.TryAdd(key, 0);
    }

    /// <summary>
    /// 移除键
    /// </summary>
    public void RemoveKey(string key)
    {
        _keys.TryRemove(key, out _);
    }

    /// <summary>
    /// 检查键是否存在
    /// </summary>
    public bool ContainsKey(string key)
    {
        return _keys.ContainsKey(key);
    }

    /// <summary>
    /// 获取所有键
    /// </summary>
    public IEnumerable<string> GetAllKeys()
    {
        return _keys.Keys.ToList();
    }

    /// <summary>
    /// 根据模式获取匹配的键
    /// </summary>
    /// <param name="pattern">模式字符串，支持 * 和 ? 通配符</param>
    /// <returns>匹配的键列表</returns>
    public IEnumerable<string> GetKeysByPattern(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            return Enumerable.Empty<string>();
        }

        // 如果模式是精确匹配（不包含通配符）
        if (!pattern.Contains('*') && !pattern.Contains('?'))
        {
            return _keys.ContainsKey(pattern) ? new[] { pattern } : Enumerable.Empty<string>();
        }

        // 将通配符模式转换为正则表达式
        var regexPattern = ConvertWildcardToRegex(pattern);

        try
        {
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return _keys.Keys.Where(key => regex.IsMatch(key)).ToList();
        }
        catch (ArgumentException)
        {
            // 如果正则表达式无效，返回空列表
            return Enumerable.Empty<string>();
        }
    }

    /// <summary>
    /// 清空所有键
    /// </summary>
    public void Clear()
    {
        _keys.Clear();
    }

    /// <summary>
    /// 获取键的数量
    /// </summary>
    public int Count => _keys.Count;

    /// <summary>
    /// 将通配符模式转换为正则表达式
    /// </summary>
    /// <param name="pattern">通配符模式（* 匹配任意字符，? 匹配单个字符）</param>
    /// <returns>正则表达式模式</returns>
    private static string ConvertWildcardToRegex(string pattern)
    {
        // 转义正则表达式特殊字符
        var escaped = Regex.Escape(pattern);

        // 将转义后的 \* 和 \? 替换为对应的正则表达式
        escaped = escaped.Replace("\\*", ".*")   // * 匹配任意字符（0 或多个）
                        .Replace("\\?", ".");     // ? 匹配单个字符

        // 添加开始和结束锚点
        return $"^{escaped}$";
    }

    /// <summary>
    /// 根据前缀获取键
    /// </summary>
    /// <param name="prefix">前缀</param>
    /// <returns>匹配的键列表</returns>
    public IEnumerable<string> GetKeysByPrefix(string prefix)
    {
        if (string.IsNullOrEmpty(prefix))
        {
            return GetAllKeys();
        }

        return _keys.Keys.Where(key => key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    /// <summary>
    /// 根据后缀获取键
    /// </summary>
    /// <param name="suffix">后缀</param>
    /// <returns>匹配的键列表</returns>
    public IEnumerable<string> GetKeysBySuffix(string suffix)
    {
        if (string.IsNullOrEmpty(suffix))
        {
            return GetAllKeys();
        }

        return _keys.Keys.Where(key => key.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    /// <summary>
    /// 根据包含的字符串获取键
    /// </summary>
    /// <param name="substring">子字符串</param>
    /// <returns>匹配的键列表</returns>
    public IEnumerable<string> GetKeysContaining(string substring)
    {
        if (string.IsNullOrEmpty(substring))
        {
            return GetAllKeys();
        }

        return _keys.Keys.Where(key => key.Contains(substring, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}
