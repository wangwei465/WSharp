using Microsoft.Extensions.Options;

namespace WSharp.Configuration;

/// <summary>
/// Configuration change notification interface
/// </summary>
public interface IConfigurationChangeNotifier<TOptions> where TOptions : class
{
    /// <summary>
    /// Subscribe to configuration changes
    /// </summary>
    IDisposable OnChange(Action<TOptions, string?> listener);

    /// <summary>
    /// Get current configuration value
    /// </summary>
    TOptions CurrentValue { get; }
}
