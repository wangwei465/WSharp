using Microsoft.Extensions.Options;

namespace WSharp.Configuration;

/// <summary>
/// Configuration change notifier implementation
/// </summary>
public class ConfigurationChangeNotifier<TOptions> : IConfigurationChangeNotifier<TOptions>
    where TOptions : class
{
    private readonly IOptionsMonitor<TOptions> _optionsMonitor;

    public ConfigurationChangeNotifier(IOptionsMonitor<TOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
    }

    public IDisposable OnChange(Action<TOptions, string?> listener)
    {
        return _optionsMonitor.OnChange(listener);
    }

    public TOptions CurrentValue => _optionsMonitor.CurrentValue;
}
