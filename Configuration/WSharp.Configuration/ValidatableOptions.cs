using System.ComponentModel.DataAnnotations;

namespace WSharp.Configuration;

/// <summary>
/// Base class for configuration options with validation support
/// </summary>
public abstract class ValidatableOptions
{
    /// <summary>
    /// Validate the configuration options
    /// </summary>
    public virtual IEnumerable<ValidationResult> Validate()
    {
        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(this, context, results, validateAllProperties: true);
        return results;
    }

    /// <summary>
    /// Check if configuration is valid
    /// </summary>
    public virtual bool IsValid()
    {
        return !Validate().Any();
    }
}
