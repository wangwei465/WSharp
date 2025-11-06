using Microsoft.Extensions.Configuration;

namespace WSharp.Configuration;

/// <summary>
/// 配置辅助工具类
/// </summary>
public static class ConfigurationHelper
{
    /// <summary>
    /// 获取配置值并进行类型转换
    /// </summary>
    public static T GetValue<T>(this IConfiguration configuration, string key, T defaultValue = default!)
    {
        var value = configuration[key];

        if (value == null)
            return defaultValue;

        try
        {
            if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), value, ignoreCase: true);
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// 获取配置节并绑定到对象
    /// </summary>
    public static T GetSection<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var section = configuration.GetSection(sectionName);
        var options = new T();
        section.Bind(options);
        return options;
    }

    /// <summary>
    /// 检查配置节是否存在
    /// </summary>
    public static bool SectionExists(this IConfiguration configuration, string sectionName)
    {
        return configuration.GetSection(sectionName).Exists();
    }

    /// <summary>
    /// 获取所有配置键
    /// </summary>
    public static IEnumerable<string> GetAllKeys(this IConfiguration configuration)
    {
        return GetAllKeysRecursive(configuration, null);
    }

    private static IEnumerable<string> GetAllKeysRecursive(IConfiguration configuration, string? prefix)
    {
        foreach (var child in configuration.GetChildren())
        {
            var key = string.IsNullOrEmpty(prefix) ? child.Key : $"{prefix}:{child.Key}";

            if (child.Value != null)
            {
                yield return key;
            }

            foreach (var subKey in GetAllKeysRecursive(child, key))
            {
                yield return subKey;
            }
        }
    }

    /// <summary>
    /// 将配置转换为字典
    /// </summary>
    public static Dictionary<string, string?> ToDictionary(this IConfiguration configuration)
    {
        return GetAllKeys(configuration)
            .ToDictionary(key => key, key => configuration[key]);
    }
}
