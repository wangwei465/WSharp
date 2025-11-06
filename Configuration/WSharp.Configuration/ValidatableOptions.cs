using System.ComponentModel.DataAnnotations;

namespace WSharp.Configuration;

/// <summary>
/// 支持验证的配置选项基类
/// </summary>
public abstract class ValidatableOptions
{
    /// <summary>
    /// 验证配置选项
    /// </summary>
    public virtual IEnumerable<ValidationResult> Validate()
    {
        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(this, context, results, validateAllProperties: true);
        return results;
    }

    /// <summary>
    /// 检查配置是否有效
    /// </summary>
    public virtual bool IsValid()
    {
        return !Validate().Any();
    }
}
