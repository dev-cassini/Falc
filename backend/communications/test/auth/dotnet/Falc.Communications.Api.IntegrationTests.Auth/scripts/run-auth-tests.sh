#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
PROJECT_PATH="${PROJECT_DIR}/Falc.Communications.Api.IntegrationTests.Auth.csproj"
RESULTS_DIR="${PROJECT_DIR}/TestResults"

mkdir -p "${RESULTS_DIR}"

dotnet test "${PROJECT_PATH}" \
  --logger "trx;LogFileName=auth-integration.trx" \
  --results-directory "${RESULTS_DIR}" \
  "$@"
