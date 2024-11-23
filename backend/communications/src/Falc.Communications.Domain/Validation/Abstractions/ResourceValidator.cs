namespace Falc.Communications.Domain.Validation.Abstractions;

/// <summary>
/// An abstraction of a resource validator consisting of a collection of rules applicable
/// to the resource of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Resource type.</typeparam>
public abstract class ResourceValidator<T>(T resource)
{
    private T Resource { get; } = resource;
    protected abstract IEnumerable<IRule<T>> Rules { get; }
    protected abstract IEnumerable<IAsyncRule<T>> AsyncRules { get; }

    /// <summary>
    /// Apply each rule synchronously, aggregate any broken rules and throw, else pass.
    /// </summary>
    /// <remarks>If validator includes async rules then call <see cref="ValidateAsync"/> instead.</remarks>
    /// <exception cref="AggregatedBrokenRuleException">One or more <see cref="Rules"/> failed.</exception>
    public void Validate()
    {
        var brokenRules = TryValidate();
        if (brokenRules.Count is not 0)
        {
            throw new AggregatedBrokenRuleException(brokenRules);
        }
    }
    
    /// <summary>
    /// Apply each rule, aggregate any broken rules and throw, else pass.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="AggregatedBrokenRuleException">One or more <see cref="Rules"/> failed.</exception>
    public async Task ValidateAsync(CancellationToken cancellationToken)
    {
        var brokenRules = await TryValidateAsync(cancellationToken);
        if (brokenRules.Count is not 0)
        {
            throw new AggregatedBrokenRuleException(brokenRules);
        }
    }

    /// <summary>
    /// Apply each rule synchronously and aggregate any rules that fail into a list.
    /// </summary>
    /// <remarks>If validator includes async rules then call <see cref="TryValidateAsync"/> instead.</remarks>
    /// <returns>A list of rules that failed.</returns>
    public IReadOnlyList<BrokenRule> TryValidate()
    {
        var brokenRules = new List<BrokenRule>();
        foreach (var rule in Rules)
        {
            var brokenRule = rule.TryCheck(Resource);
            if (brokenRule is not null)
            {
                brokenRules.Add(brokenRule);
            }
        }
        
        foreach (var asyncRule in AsyncRules)
        {
            var brokenRule = asyncRule.TryCheckAsync(Resource, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
            
            if (brokenRule is not null)
            {
                brokenRules.Add(brokenRule);
            }
        }

        return brokenRules;
    }
    
    /// <summary>
    /// Apply each rule and aggregate any rules that fail into a list.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of rules that failed.</returns>
    public async Task<IReadOnlyList<BrokenRule>> TryValidateAsync(CancellationToken cancellationToken)
    {
        var brokenRules = new List<BrokenRule>();
        foreach (var rule in Rules)
        {
            var brokenRule = rule.TryCheck(Resource);
            if (brokenRule is not null)
            {
                brokenRules.Add(brokenRule);
            }
        }
        
        foreach (var asyncRule in AsyncRules)
        {
            var brokenRule = await asyncRule.TryCheckAsync(Resource, cancellationToken);
            if (brokenRule is not null)
            {
                brokenRules.Add(brokenRule);
            }
        }

        return brokenRules;
    }
}