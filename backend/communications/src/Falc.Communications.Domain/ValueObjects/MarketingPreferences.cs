namespace Falc.Communications.Domain.ValueObjects;

public record MarketingPreferences(bool Email, bool Phone, bool Sms)
{
    public bool Email { get; set; } = Email;
    public bool Phone { get; set; } = Phone;
    public bool Sms { get; set; } = Sms;
}