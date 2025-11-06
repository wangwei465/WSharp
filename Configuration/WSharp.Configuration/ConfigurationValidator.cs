using System.ComponentModel.DataAnnotations;

namespace WSharp.Configuration;

/// <summary>
/// 默认配置验证器实现
/// </summary>
public class ConfigurationValidator : IConfigurationValidator
{
    public IEnumerable<ValidationResult> Validate<TOptions>(TOptions options) where TOptions : class
    {
        var context = new ValidationContext(options);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(options, context, results, validateAllProperties: true);
        return results;
    }

    public bool IsValid<TOptions>(TOptions options) where TOptions : class
    {
        return !Validate(options).Any();
    }

    public void ValidateAndThrow<TOptions>(TOptions options) where TOptions : class
    {
        var validationResults = Validate(options).ToList();

        if (validationResults.Any())
        {
            var errors = string.Join(Environment.NewLine,
                validationResults.Select(r => r.ErrorMessage));

            throw new ValidationException(
                $"{typeof(TOptions).Name} 配置验证失败:{Environment.NewLine}{errors}");
        }
    }
}
