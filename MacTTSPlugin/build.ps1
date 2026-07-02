param(
  [string]$Configuration = "Release",
  [string]$ActReferencePath = ""
)

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectFile = Join-Path $ScriptDir "MacTTSPlugin.csproj"
$LocalEnvFile = Join-Path $ScriptDir "build.local.env"
$DefaultActPath = Join-Path $ScriptDir "ExternalLibraries\Advanced Combat Tracker.exe"

if (Test-Path $LocalEnvFile) {
  Get-Content $LocalEnvFile | ForEach-Object {
    if ($_ -match '^\s*#' -or $_ -match '^\s*$') { return }
    if ($_ -match '^\s*([A-Za-z_][A-Za-z0-9_]*)\s*=\s*"?(.*?)"?\s*$') {
      $name = $matches[1]
      $value = $matches[2]
      Set-Variable -Name $name -Value $value -Scope Script
    }
  }
}

if ([string]::IsNullOrWhiteSpace($ActReferencePath)) {
  if (Get-Variable -Name ACT_REFERENCE_PATH -Scope Script -ErrorAction SilentlyContinue) {
    $ActReferencePath = $ACT_REFERENCE_PATH
  } else {
    $ActReferencePath = $DefaultActPath
  }
}

if (Get-Variable -Name BUILD_CONFIGURATION -Scope Script -ErrorAction SilentlyContinue) {
  $Configuration = $BUILD_CONFIGURATION
}

$buildTool = ""
if (Get-Variable -Name BUILD_TOOL -Scope Script -ErrorAction SilentlyContinue) {
  $buildTool = $BUILD_TOOL
}
if ([string]::IsNullOrWhiteSpace($buildTool)) {
  $msbuild = Get-Command msbuild -ErrorAction SilentlyContinue
  if ($msbuild) {
    $buildTool = "msbuild"
  } else {
    $xbuild = Get-Command xbuild -ErrorAction SilentlyContinue
    if ($xbuild) {
      $buildTool = "xbuild"
    } else {
      throw "Neither msbuild nor xbuild was found in PATH."
    }
  }
}

if (-not (Test-Path $ActReferencePath)) {
  throw "ACT reference not found: $ActReferencePath"
}

Write-Host "Using build tool: $buildTool"
Write-Host "Project: $ProjectFile"
Write-Host "Configuration: $Configuration"
Write-Host "ACT reference: $ActReferencePath"

& $buildTool $ProjectFile "/p:Configuration=$Configuration" "/p:ACTReferencePath=$ActReferencePath"

if ($LASTEXITCODE -ne 0) {
  exit $LASTEXITCODE
}

Write-Host "Build completed. Output DLL:"
Write-Host "  $(Join-Path $ScriptDir "bin\$Configuration\MacTTSPlugin.dll")"
