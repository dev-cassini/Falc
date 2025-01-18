# FALC Communications #

ASP.NET Core 8 Web API for managing communications.

## Local Development

### HMAC-SHA256 Authentication

Implementation loosely based on [this](https://learn.microsoft.com/en-us/azure/communication-services/tutorials/hmac-header-tutorial?pivots=programming-language-csharp) Azure example.

Use the following code to generate a secret 64 bytes long:

```csharp
var secretKey = new byte[64];
using (var rng = RandomNumberGenerator.Create())
{
    rng.GetBytes(secretKey);
}
```

Navigate to the API project directory and add the HMAC secret to dotnet user secrets manager:

```shell
dotnet user-secrets set "Authentication:Schemes:Hmac:Secret" "SECRET_KEY"
```

Postman pre-request script to generate HMAC signature:

```javascript
var CryptoJS = require('crypto-js');

const clientKey = 'SECRET_KEY';
const emailAddress = 'EMAIL_ADDRESS';

var emailAddressHash = CryptoJS.SHA256(emailAddress).toString(CryptoJS.enc.Base64);
var clientKeyWordArray = CryptoJS.enc.Base64.parse(clientKey);
var hmac = CryptoJS.HmacSHA256(emailAddressHash, clientKeyWordArray).toString(CryptoJS.enc.Hex);

pm.request.headers.add({
    key: 'Authorization',
    value: 'HMAC-SHA256 ' + hmac
});
```

See [here](https://code.google.com/archive/p/crypto-js/) for more info about the crypto-js package.