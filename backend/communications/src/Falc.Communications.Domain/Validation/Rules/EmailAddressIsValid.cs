using Falc.Communications.Domain.Model;
using Falc.Communications.Domain.Validation.Abstractions;

namespace Falc.Communications.Domain.Validation.Rules;

/// <summary>
/// User must have a valid email address.
/// </summary>
public class EmailAddressIsValidRule : IRule<User>
{
    /// <inheritdoc />
    /// <exception cref="EmailAddressIsValidException">User email address is invalid.</exception>
    public void Check(User user)
    {
        var brokenRule = TryCheck(user);
        if (brokenRule is not null)
        {
            throw new EmailAddressIsValidException(brokenRule);
        }
    }

    /// <inheritdoc />
    public BrokenRule? TryCheck(User user)
    {
        var firstAtIndex = user.EmailAddress.IndexOf('@');
        var lastAtIndex = user.EmailAddress.LastIndexOf('@');
        var noAtSymbol = firstAtIndex == -1;
        var multipleAtSymbols = firstAtIndex != lastAtIndex;
        var startsWithAt = firstAtIndex == 0;
        var endsWithAt = lastAtIndex == user.EmailAddress.Length - 1;

        var localPart = noAtSymbol || multipleAtSymbols ? string.Empty : user.EmailAddress[..firstAtIndex];
        var domainPart = noAtSymbol || multipleAtSymbols ? string.Empty : user.EmailAddress[(firstAtIndex + 1)..];

        var localPartIsValid = localPart.Length > 0 && localPart.Split('.').All(x => x != string.Empty);
        var domainPartIsValid = domainPart.Length > 0 && domainPart.IndexOf('.') > 0 && domainPart.Split('.').All(x => x != string.Empty);

        var emailAddressIsValid =
            noAtSymbol is false &&
            multipleAtSymbols is false &&
            startsWithAt is false &&
            endsWithAt is false &&
            localPartIsValid &&
            domainPartIsValid;

        return emailAddressIsValid ? null : new EmailAddressIsValidBrokenRule();
    }
}

public class EmailAddressIsValidException(BrokenRule brokenRule) : BrokenRuleException(brokenRule);

public class EmailAddressIsValidBrokenRule() : BrokenRule(
    new KeyValuePair<string, string>("EmailAddress", "User has an invalid email address."),
    new Dictionary<string, string>());