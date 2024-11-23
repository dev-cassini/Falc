using Falc.Communications.Domain.Tooling.Abstractions;
using Falc.Communications.Domain.Validation.Validators;

namespace Falc.Communications.Domain.Model;

public class User
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public Guid Id { get; }
    
    /// <summary>
    /// When the user was created in UTC.
    /// </summary>
    public DateTimeOffset CreationTimestampUtc { get; }
    
    /// <summary>
    /// Email address.
    /// </summary>
    public string EmailAddress { get; }

    public User(
        Guid id, 
        string emailAddress,
        IDateTimeProvider dateTimeProvider)
    {
        Id = id;
        CreationTimestampUtc = dateTimeProvider.UtcNow;
        EmailAddress = emailAddress.Trim().ToLower();
        
        new UserValidator(this).Validate();
    }
    
    #region EF Constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private User() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    #endregion
}