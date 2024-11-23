namespace Falc.Communications.Domain.Validation.Abstractions;

/// <summary>
/// A collection of <see cref="BrokenRule"/>s wrapped in an aggregate exception.
/// </summary>
/// <param name="brokenRules">A collection of broken rules.</param>
public class AggregatedBrokenRuleException(IReadOnlyList<BrokenRule> brokenRules) : Exception
{
    /// <summary>
    /// A collection of broken rules.
    /// </summary>
    public IReadOnlyList<BrokenRule> BrokenRules { get; } = brokenRules;
}