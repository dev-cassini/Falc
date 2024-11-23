using Falc.Communications.Domain.Tooling.Abstractions;

namespace Falc.Communications.Domain.Tooling;

public class DefaultDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}