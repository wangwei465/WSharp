using System.ComponentModel.DataAnnotations;

namespace WSharp.Configuration;

/// <summary>
/// 配置验证器接口
/// </summary>
public interface IConfigurationValidator
{
    /// <summary>
    /// 验证配置对象
    /// </summary>
    IEnumerable<ValidationResult> Validate<TOptions>(TOptions options) where TOptions : class;

    /// <summary>
    /// 检查配置是否有效
    /// </summary>
    bool IsValid<TOptions>(TOptions options) where TOptions : class;

    /// <summary>
    /// 验证并在无效时抛出异常
    /// </summary>
    void ValidateAndThrow<TOptions>(TOptions options) where TOptions : class;
}
