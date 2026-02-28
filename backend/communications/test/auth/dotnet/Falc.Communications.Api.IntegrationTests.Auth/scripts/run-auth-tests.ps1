param(
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$DotNetTestArgs
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectDir = Split-Path -Parent $ScriptDir
$ProjectPath = Join-Path $ProjectDir "Falc.Communications.Api.IntegrationTests.Auth.csproj"
$ResultsDir = Join-Path $ProjectDir "TestResults"

New-Item -ItemType Directory -Force -Path $ResultsDir | Out-Null

dotnet test $ProjectPath `
    --logger "trx;LogFileName=auth-integration.trx" `
    --results-directory $ResultsDir `
    @DotNetTestArgs
