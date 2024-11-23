namespace Falc.Communications.Domain.Tooling.Abstractions;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}