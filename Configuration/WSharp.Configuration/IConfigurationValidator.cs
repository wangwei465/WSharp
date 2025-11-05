using System.ComponentModel.DataAnnotations;

namespace WSharp.Configuration;

/// <summary>
/// Configuration validator interface
/// </summary>
public interface IConfigurationValidator
{
    /// <summary>
    /// Validate configuration object
    /// </summary>
    IEnumerable<ValidationResult> Validate<TOptions>(TOptions options) where TOptions : class;

    /// <summary>
    /// Check if configuration is valid
    /// </summary>
    bool IsValid<TOptions>(TOptions options) where TOptions : class;

    /// <summary>
    /// Validate and throw exception if invalid
    /// </summary>
    void ValidateAndThrow<TOptions>(TOptions options) where TOptions : class;
}
