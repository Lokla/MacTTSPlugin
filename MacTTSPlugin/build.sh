#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_FILE="$SCRIPT_DIR/MacTTSPlugin.csproj"
LOCAL_ENV_FILE="$SCRIPT_DIR/build.local.env"

ACT_REFERENCE_PATH_DEFAULT="$SCRIPT_DIR/ExternalLibraries/Advanced Combat Tracker.exe"
BUILD_CONFIGURATION_DEFAULT="Release"

if [[ -f "$LOCAL_ENV_FILE" ]]; then
  # shellcheck source=/dev/null
  source "$LOCAL_ENV_FILE"
fi

ACT_REFERENCE_PATH="${ACT_REFERENCE_PATH:-$ACT_REFERENCE_PATH_DEFAULT}"
BUILD_CONFIGURATION="${BUILD_CONFIGURATION:-$BUILD_CONFIGURATION_DEFAULT}"
BUILD_TOOL="${BUILD_TOOL:-}"

if [[ -z "$BUILD_TOOL" ]]; then
  if command -v msbuild >/dev/null 2>&1; then
    BUILD_TOOL="msbuild"
  elif command -v xbuild >/dev/null 2>&1; then
    BUILD_TOOL="xbuild"
  else
    echo "Error: neither msbuild nor xbuild was found in PATH."
    exit 1
  fi
fi

if [[ ! -f "$ACT_REFERENCE_PATH" ]]; then
  echo "Error: ACT reference not found:"
  echo "  $ACT_REFERENCE_PATH"
  echo "Set ACT_REFERENCE_PATH in build.local.env"
  exit 1
fi

echo "Using build tool: $BUILD_TOOL"
echo "Project: $PROJECT_FILE"
echo "Configuration: $BUILD_CONFIGURATION"
echo "ACT reference: $ACT_REFERENCE_PATH"

"$BUILD_TOOL" "$PROJECT_FILE" \
  /p:Configuration="$BUILD_CONFIGURATION" \
  /p:ACTReferencePath="$ACT_REFERENCE_PATH"

echo "Build completed. Output DLL:"
echo "  $SCRIPT_DIR/bin/$BUILD_CONFIGURATION/MacTTSPlugin.dll"
