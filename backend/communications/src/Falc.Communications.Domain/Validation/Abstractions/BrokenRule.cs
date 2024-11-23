namespace Falc.Communications.Domain.Validation.Abstractions;

public class BrokenRule(
    KeyValuePair<string, string> error,
    IReadOnlyDictionary<string, string> metadata)
{
    /// <summary>
    /// Validation error.
    /// </summary>
    public KeyValuePair<string, string> Error { get; } = error;

    /// <summary>
    /// Metadata related to the broken rule e.g. resource unique identifier.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; } = metadata;
}