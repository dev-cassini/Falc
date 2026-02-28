namespace Falc.Communications.Api.ComponentTests.TestInfrastructure;

public static class TestJwtTokenFactory
{
    public static string CreateValidToken(Guid userId)
    {
        var claims = new List<Claim>
        {
            new("sub", userId.ToString()),
            new(ClaimTypes.Role, "admin")
        };

        return CreateToken(claims, TestJwtAuthHandler.SigningKey);
    }

    public static string CreateValidTokenWithoutRole(Guid userId)
    {
        var claims = new List<Claim>
        {
            new("sub", userId.ToString())
        };

        return CreateToken(claims, TestJwtAuthHandler.SigningKey);
    }

    public static string CreateInvalidToken(Guid userId)
    {
        var claims = new List<Claim>
        {
            new("sub", userId.ToString()),
            new(ClaimTypes.Role, "admin")
        };

        return CreateToken(claims, "a-different-signing-key-that-should-fail-validation");
    }

    private static string CreateToken(IEnumerable<Claim> claims, string signingKey)
    {
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: TestJwtAuthHandler.ValidIssuer,
            audience: TestJwtAuthHandler.ValidAudience,
            claims: claims,
            notBefore: DateTime.UtcNow.AddMinutes(-1),
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
