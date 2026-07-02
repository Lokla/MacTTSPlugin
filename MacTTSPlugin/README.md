# MacTTSPlugin for ACT

A low-latency ACT plugin for macOS/CrossOver setups.

## Attribution

This project was inspired by the LinuxTTSPlugin project and uses it as a blueprint
for ACT plugin structure and workflow patterns:

- LinuxTTSPlugin: https://github.com/Minizbot2012/LinuxTTSPlugin

This repository contains a macOS-focused implementation with its own code and
adaptations for CrossOver/macOS TTS behavior.

## What it does

- Replaces ACT's `PlayTtsMethod` with a direct call to `say`.
- Uses a background queue to avoid process storms and reduce jitter.
- Avoids shell scripts/batch wrappers during normal speech calls.

## Prerequisites

1. .NET Framework 4.6.1+ build environment for ACT plugins.
2. Access to `Advanced Combat Tracker.exe` for compile-time reference.

## Reproducible Build Setup

1. Copy `build.local.env.example` to `build.local.env`.
2. Set `ACT_REFERENCE_PATH` to your local ACT executable path.
3. Do not commit `build.local.env` (it is ignored by git).

Example values in `build.local.env`:

ACT_REFERENCE_PATH="/Users/yourname/Library/Application Support/CrossOver/Bottles/YourBottle/drive_c/Program Files (x86)/Advanced Combat Tracker/Advanced Combat Tracker.exe"
BUILD_CONFIGURATION="Release"
BUILD_TOOL=""

## Build

On macOS or Linux shell:

1. Ensure `msbuild` or `xbuild` is installed.
2. Run:

./build.sh

On Windows PowerShell:

1. Ensure `msbuild` or `xbuild` is installed.
2. Run:

./build.ps1

Output DLL:

bin/Release/MacTTSPlugin.dll

Optional one-off override (without editing config):

./build.ps1 -ActReferencePath "C:\Path\To\Advanced Combat Tracker.exe"

## GitHub Actions (Online Build)

This repository includes a CI workflow at `.github/workflows/build.yml`.

How it works:

1. Builds a lightweight compile-time ACT API stub (`tools/ActReferenceStub`).
2. Builds `MacTTSPlugin` against that stub in CI.
3. Uploads `MacTTSPlugin.dll` as a workflow artifact.

Why a stub is used:

1. `Advanced Combat Tracker.exe` is proprietary and must not be redistributed in this repository.
2. CI still validates compile health without shipping ACT binaries.

Note:

For real user releases, build locally against an actual ACT installation path.

## GitHub Actions (Release Packaging)

This repository includes `.github/workflows/release.yml`.

Triggers:

1. Manual run (`workflow_dispatch`) with optional `version` input.
2. Push tags starting with `v` (for example `v1.0.0`).
3. Push to `main`/`master`.

Outputs:

1. `dist/MacTTSPlugin-<version>.zip`
2. Unzipped package folder as an artifact for inspection.

On tag builds (`v*`), the workflow also attaches the zip file to a GitHub Release.

## Install in ACT

1. Open ACT -> Plugins.
2. Browse and add `MacTTSPlugin.dll`.
3. In plugin settings, verify:
   - Binary path: `Z:\usr\bin\say`
   - Rate: start around `260`
   - Extra args: `{text}`
4. Press `Test speech`.

## Tuning for lower latency

- Keep `Use queue` enabled.
- Use shorter callouts.
- Increase `Rate` to reduce total speech duration.
- If needed, set Voice to a compact voice like `Alex`.

## Notes

- Voice names depend on macOS installed voices.
- If `say` cannot be started, enable `Fallback` to use ACT default TTS temporarily.

## Publishing on GitHub and Licensing

Yes, you can publish this project.

This repository uses the MIT License (see `../LICENSE`).

Important points:

1. Do not commit or redistribute `Advanced Combat Tracker.exe`.
2. Keep ACT as an external compile-time reference only.
3. Avoid shipping ACT assets, icons, or proprietary binaries in releases.
4. Document that users must supply their own ACT installation path.

Recommended release artifact:

1. `MacTTSPlugin.dll`
2. `README.md`
3. `build.local.env.example`
