namespace Falc.Communications.Domain.Validation.Abstractions;

/// <summary>
/// An asynchronous rule check applicable to resource of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Resource type.</typeparam>
public interface IAsyncRule<in T>
{
    /// <summary>
    /// The rule check. Throws on failure.
    /// </summary>
    /// <param name="resource">Resource the rule is applied to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task CheckAsync(T resource, CancellationToken cancellationToken);

    /// <summary>
    /// The rule check. Returns a broken rule on failure, else null.
    /// </summary>
    /// <param name="resource">Resource the rule is applied to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A broken rule on failure, else null.</returns>
    Task<BrokenRule?> TryCheckAsync(T resource, CancellationToken cancellationToken);
}