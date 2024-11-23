using Falc.Communications.Domain.Model;
using Falc.Communications.Domain.Validation.Abstractions;
using Falc.Communications.Domain.Validation.Rules;

namespace Falc.Communications.Domain.Validation.Validators;

/// <summary>
/// A collection of <see cref="IRule{T}"/> and <see cref="IAsyncRule{T}"/> applicable
/// to a <see cref="User"/>.
/// </summary>
/// <param name="user">User the rules are applied to.</param>
public class UserValidator(User user) : ResourceValidator<User>(user)
{
    protected override IEnumerable<IRule<User>> Rules { get; } =
    [
        new EmailAddressIsValidRule()
    ];

    protected override IEnumerable<IAsyncRule<User>> AsyncRules { get; } = [];
}