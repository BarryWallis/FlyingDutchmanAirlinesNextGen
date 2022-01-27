using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FlyingDutchmanAirlines.ControllerLayer.JsonData;
public class BookingData : IValidatableObject
{
    private string? _firstName;
    public string FirstName
    {
        get
        {
            Debug.Assert(_firstName is not null);
            return _firstName;
        }

        set => _firstName = ValidateName(value, nameof(FirstName));
    }

    private string? _lasttName;
    public string LastName
    {
        get
        {
            Debug.Assert(_lasttName is not null);
            return _lasttName;
        }

        set => _lasttName = ValidateName(value, nameof(LastName));
    }

    private static string ValidateName(string name, string propertyName)
        => string.IsNullOrWhiteSpace(name) ? throw new InvalidOperationException($"Could not set {propertyName}") : name;

    /// <inheritdoc/>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        List<ValidationResult> validationResults = new();
        if (FirstName is null && LastName is null)
        {
            validationResults.Add(new ValidationResult("All given data points are null"));
        }
        else if (FirstName is null || LastName is null)
        {
            validationResults.Add(new ValidationResult("One of the given data points is null"));
        }

        return validationResults;
    }
}
