using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Falc.Communications.Domain.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Falc.Communications.Api.Authentication.Schemes;

public static class HmacAuthenticationScheme
{
    public const string Name = "Hmac";
    
    public class Configuration : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Private secret key used by the HMAC hashing algorithm.
        /// </summary>
        public string Secret { get; init; } = string.Empty;
    }

    public class Handler(
        IOptionsMonitor<Configuration> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder,
        IUserRepository userRepository)
        : AuthenticationHandler<Configuration>(options, logger, encoder)
    {
        private readonly IOptionsMonitor<Configuration> _options = options;

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var oneAndOnlyOneAuthenticationHeader = Request.Headers.Authorization.Count == 1;
            var authenticationSchemeIsHmac = Request.Headers.Authorization.SingleOrDefault()?.StartsWith("HMAC-SHA256") ?? false;
            if (oneAndOnlyOneAuthenticationHeader is false || authenticationSchemeIsHmac is false)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return AuthenticateResult.Fail("HMAC-SHA256 header is missing.");
            }

            var actualHmacSignature = Request.Headers.Authorization.Single()?.Split(" ").Last();
            if (actualHmacSignature is null or "")
            {
                Response.StatusCode = StatusCodes.Status403Forbidden;
                return AuthenticateResult.Fail("HMAC-SHA256 signature is missing.");
            }

            var routeIncludesUserId = Request.RouteValues.TryGetValue("userId", out var userIdRouteValue);
            var userIdRouteValueAsString = routeIncludesUserId && userIdRouteValue is string ? userIdRouteValue.ToString() : string.Empty;
            var userIdIsValid = Guid.TryParse(userIdRouteValueAsString, out var userId);
            var user = userIdIsValid ? await userRepository.GetAsync(userId, Context.RequestAborted) : null;
            if (user is null)
            {
                Response.StatusCode = StatusCodes.Status403Forbidden;
                return AuthenticateResult.Fail($"UserId parameter was either not present, considered invalid (value=${userIdRouteValue}) or did not map to a user. UserId must be a guid.");
            }
            
            var hashedEmailAddressBytes = SHA256.HashData(Encoding.UTF8.GetBytes(user.EmailAddress));
            var hashedEmailAddressBase64 = Convert.ToBase64String(hashedEmailAddressBytes);
            
            using var hmacSha256 = new HMACSHA256(Convert.FromBase64String(_options.CurrentValue.Secret));
            var hashedBytes = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(hashedEmailAddressBase64));
            var expectedHmacSignature = BitConverter.ToString(hashedBytes).Replace("-", string.Empty).ToLower();

            if (actualHmacSignature != expectedHmacSignature)
            {
                Response.StatusCode = StatusCodes.Status403Forbidden;
                return AuthenticateResult.Fail("HMAC-SHA256 signature is invalid.");
            }

            var principal = new ClaimsPrincipal(new ClaimsIdentity([], "Hmac"));
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}