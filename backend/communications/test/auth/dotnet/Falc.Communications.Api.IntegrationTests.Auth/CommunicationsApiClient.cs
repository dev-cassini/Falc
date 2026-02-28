using System.Net;
using System.Net.Http.Json;

namespace Falc.Communications.Api.IntegrationTests.Auth;

public sealed class CommunicationsApiClient(HttpClient httpClient)
{
    public async Task<HttpResponseMessage> SearchUsersAsync(object payload, CancellationToken cancellationToken)
    {
        return await httpClient.PostAsJsonAsync("/api/communications/users/search", payload, cancellationToken);
    }

    public static async Task<SearchUsersResponse?> ReadSearchUsersResponseAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.StatusCode != HttpStatusCode.OK)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<SearchUsersResponse>(cancellationToken: cancellationToken);
    }

    public sealed record SearchUsersResponse(List<UserDto> Users, int PageNumber, int PageSize, int TotalCount);
    public sealed record UserDto(Guid Id, string Email, MarketingPreferencesDto MarketingPreferences);
    public sealed record MarketingPreferencesDto(bool Email, bool Phone, bool Sms);
}
