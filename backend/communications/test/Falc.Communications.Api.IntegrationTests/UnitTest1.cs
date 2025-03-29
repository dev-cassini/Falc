using System.Security.Cryptography;
using System.Text;

namespace Falc.Communications.Api.IntegrationTests;

public class Tests
{
    [Test]
    public void Test1()
    {
        var secretKey = new byte[64];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(secretKey); // replace with shared private key
            using (var hmac = new HMACSHA256(secretKey))
            {
                var hashValue = hmac.ComputeHash(Guid.NewGuid().ToByteArray()); // replace guid with email address
                Console.WriteLine($"hmac: {Convert.ToBase64String(hashValue)}");
            }
        }
        
        var test1 = "test.customer@test.com";
        var inputBytes = Encoding.UTF8.GetBytes(test1);
        var inputHash1 = SHA256.HashData(inputBytes);
        var result1 = Convert.ToHexString(inputHash1);
        
        var inputHash2 = SHA256.HashData(inputBytes);
        var result2 = Convert.ToHexString(inputHash2);
        
        Console.WriteLine($"sha 1: {result1}");
        Console.WriteLine($"sha 2: {result2}");
    }
}