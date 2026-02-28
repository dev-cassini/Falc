# Falc.Communications.Api.IntegrationTests.Auth

Remote integration tests for authn/authz behavior of the communications API.

## Scope

Endpoint under test:

- `POST /api/communications/users/search`

Scenarios:

- `401` when token is missing
- `401` when token is invalid
- `403` when role claim is missing
- `200` when role claim contains `admin`

## Config Precedence

Settings are loaded in this order (highest to lowest priority):

1. `appsettings.local.json` (optional)
2. Environment variables (`FALC_AUTH_*`)
3. AWS Secrets Manager (`falc/communications/auth-tests/dev` by default)

## Required Settings

Top-level JSON keys expected from AWS secret or local settings file:

- `baseUrl`
- `idpTokenUrl`
- `idpClientId`
- `idpClientSecret`
- `adminUserIdentifier`
- `nonAdminUserIdentifier`

Optional keys:

- `idpImpersonationGrantType` (default: `urn:ietf:params:oauth:grant-type:token-exchange`)
- `idpImpersonationFieldName` (default: `subject`)
- `requiredRole` (default: `admin`)
- `timeoutSeconds` (default: `30`)
- `environmentName` (default: `dev`)
- `awsSecretId`
- `awsRegion` (default: `eu-west-2`)
- `awsProfile`

## Run

### macOS / Linux

```bash
./scripts/run-auth-tests.sh
```

### Windows PowerShell

```powershell
./scripts/run-auth-tests.ps1
```

Both commands write TRX test output to `TestResults/auth-integration.trx`.
