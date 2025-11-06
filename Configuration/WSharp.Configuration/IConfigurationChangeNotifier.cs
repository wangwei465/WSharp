using Microsoft.Extensions.Options;

namespace WSharp.Configuration;

/// <summary>
/// 配置变更通知接口
/// </summary>
public interface IConfigurationChangeNotifier<TOptions> where TOptions : class
{
    /// <summary>
    /// 订阅配置变更
    /// </summary>
    IDisposable OnChange(Action<TOptions, string?> listener);

    /// <summary>
    /// 获取当前配置值
    /// </summary>
    TOptions CurrentValue { get; }
}
