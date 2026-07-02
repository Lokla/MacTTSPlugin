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

cp "$PLUGIN_DIR/bin/Release/MacTTSPlugin.dll" "$PACKAGE_DIR/"
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
