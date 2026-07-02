#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PLUGIN_DIR="$ROOT_DIR/MacTTSPlugin"
OUT_ROOT="$ROOT_DIR/dist"
VERSION="${1:-dev}"
PACKAGE_DIR="$OUT_ROOT/MacTTSPlugin-$VERSION"
ZIP_PATH="$OUT_ROOT/MacTTSPlugin-$VERSION.zip"

mkdir -p "$PACKAGE_DIR"
rm -f "$ZIP_PATH"

DLL_PATH="$PLUGIN_DIR/bin/Release/MacTTSPlugin.dll"

if [[ ! -f "$DLL_PATH" ]]; then
  echo "Error: plugin DLL not found at:"
  echo "  $DLL_PATH"
  exit 1
fi

# Guard against packaging a DLL compiled against the CI stub (ACT 0.0.0.0, no strong-name key).
if command -v monodis >/dev/null 2>&1; then
  REF_DUMP="$(monodis --assemblyref "$DLL_PATH")"
  if perl -0777 -e 'exit(($ARGV[0] =~ /Version=0\.0\.0\.0\s+Name=Advanced Combat Tracker/s) ? 0 : 1)' "$REF_DUMP"; then
    if [[ "${ALLOW_STUB_BUILD:-0}" != "1" ]]; then
      echo "Error: MacTTSPlugin.dll references stub ACT assembly identity (0.0.0.0)."
      echo "Build against your real Advanced Combat Tracker.exe before packaging."
      echo "Set ALLOW_STUB_BUILD=1 only for non-runtime compile-check artifacts."
      exit 1
    fi
  fi
fi

cp "$DLL_PATH" "$PACKAGE_DIR/"
cp "$PLUGIN_DIR/README.md" "$PACKAGE_DIR/"
cp "$PLUGIN_DIR/build.local.env.example" "$PACKAGE_DIR/"
cp "$ROOT_DIR/LICENSE" "$PACKAGE_DIR/"

cat > "$PACKAGE_DIR/VERSION.txt" <<EOF
$VERSION
EOF

(
  cd "$OUT_ROOT"
  zip -r "$(basename "$ZIP_PATH")" "$(basename "$PACKAGE_DIR")"
)

echo "Packaged release: $ZIP_PATH"
