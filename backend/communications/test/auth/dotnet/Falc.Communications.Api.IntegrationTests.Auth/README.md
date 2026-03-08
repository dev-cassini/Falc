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

## Config Loading

Settings are loaded from local JSON only:

- `appsettings.local.json` in current working directory, then project directory
- or file path in `FALC_AUTH_LOCAL_SETTINGS_PATH`

## Required Settings

Top-level JSON keys expected from local settings file:

- `baseUrl`
- `idpTokenUrl`
- `idpClientId`
- `idpClientSecret`
- `adminUserIdentifier`
- `customerUserIdentifier`

Optional keys:

- `idpDelegationPasswordGrantUsername` (default: `admin`)
- `idpDelegationPasswordGrantPassword` (default: `Pa$$word123`)
- `idpPasswordGrantScope` (default: `openid profile email`)
- `idpImpersonationGrantType` (default: `delegation`)
- `idpImpersonationFieldName` (default: `subject`)
- `timeoutSeconds` (default: `30`)
- `environmentName` (default: `dev`)

Use `appsettings.local.example.json` as a template to create your local settings file.

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
