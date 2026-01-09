using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.Helpers;

public class ValidationHelper
{
    public static (bool isValid, IReadOnlyCollection<ValidationResult> errors) ValidateObject(object instance)
    {
        ArgumentNullException.ThrowIfNull(instance);

        ValidationContext validationContext = new ValidationContext(instance);
        IList<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject(instance, validationContext, validationResults, validateAllProperties: true);

        return (isValid, validationResults.AsReadOnly());
    }
}
