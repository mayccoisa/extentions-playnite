# Build and Deploy script for Retrospectiva anual
# 
# This script builds the extension and copies it to the Playnite extensions folder.

$playniteExtensions = "C:\Users\mayco\Downloads\Playnite\Extensions\785d9324-4173-420d-b17a-e2ace45bb317"
$msbuildPath = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"
$projectPath = ".\RetrospectiveSteam.csproj"
$outputPath = ".\bin\Debug"

Write-Host "Building project at $projectPath..." -ForegroundColor Cyan
& $msbuildPath $projectPath /t:Rebuild /p:Configuration=Debug

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "Deployment to $playniteExtensions..." -ForegroundColor Cyan
if (!(Test-Path $playniteExtensions)) {
    New-Item -ItemType Directory -Force -Path $playniteExtensions | Out-Null
}

# Copy binary
Copy-Item "$outputPath\RetrospectiveSteam.dll" $playniteExtensions -Force

# Copy assets
Copy-Item ".\bau_header.png" $playniteExtensions -Force
Copy-Item ".\extension.yaml" $playniteExtensions -Force
Copy-Item ".\icon.png" $playniteExtensions -Force

# Copy Localization folder
if (Test-Path ".\Localization") {
    Copy-Item ".\Localization" $playniteExtensions -Recurse -Force
}

Write-Host "Done! Please restart Playnite to see the changes." -ForegroundColor Green
