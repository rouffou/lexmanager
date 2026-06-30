using System.Text.RegularExpressions;
using LexManager.SharedKernel.Domain;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.Identity.Domain.Clients;

/// <summary>A validated email address.</summary>
public sealed partial class Email : ValueObject
{
    private Email(string value) => Value = value;

    public string Value { get; }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !EmailRegex().IsMatch(value))
        {
            throw new BusinessRuleValidationException(ClientErrors.InvalidEmail);
        }

        return new Email(value.Trim().ToLowerInvariant());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.CultureInvariant)]
    private static partial Regex EmailRegex();
}
