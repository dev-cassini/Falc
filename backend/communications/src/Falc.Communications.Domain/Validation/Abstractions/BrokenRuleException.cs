namespace Falc.Communications.Domain.Validation.Abstractions;

/// <summary>
/// Abstraction of an exception thrown when a specific business rule check fails.
/// </summary>
/// <param name="brokenRule">A summary of the rule that failed.</param>
public abstract class BrokenRuleException(BrokenRule brokenRule) : Exception
{
    /// <summary>
    /// A summary of the rule that failed.
    /// </summary>
    public BrokenRule BrokenRule { get; } = brokenRule;
}