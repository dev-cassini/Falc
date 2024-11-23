namespace Falc.Communications.Domain.Validation.Abstractions;

/// <summary>
/// A synchronous rule check applicable to resource of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Resource type.</typeparam>
public interface IRule<in T>
{
    /// <summary>
    /// The rule check. Throws on failure.
    /// </summary>
    /// <param name="resource">Resource the rule is applied to.</param>
    void Check(T resource);

    /// <summary>
    /// The rule check. Returns a broken rule on failure, else null.
    /// </summary>
    /// <param name="resource">Resource the rule is applied to.</param>
    /// <returns>A broken rule on failure, else null.</returns>
    BrokenRule? TryCheck(T resource);
}