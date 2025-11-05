using Microsoft.Extensions.Configuration;

namespace WSharp.Configuration;

/// <summary>
/// Configuration helper utilities
/// </summary>
public static class ConfigurationHelper
{
    /// <summary>
    /// Get configuration value with type conversion
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
    /// Get configuration section and bind to object
    /// </summary>
    public static T GetSection<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var section = configuration.GetSection(sectionName);
        var options = new T();
        section.Bind(options);
        return options;
    }

    /// <summary>
    /// Check if configuration section exists
    /// </summary>
    public static bool SectionExists(this IConfiguration configuration, string sectionName)
    {
        return configuration.GetSection(sectionName).Exists();
    }

    /// <summary>
    /// Get all configuration keys
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
    /// Get configuration as dictionary
    /// </summary>
    public static Dictionary<string, string?> ToDictionary(this IConfiguration configuration)
    {
        return GetAllKeys(configuration)
            .ToDictionary(key => key, key => configuration[key]);
    }
}
