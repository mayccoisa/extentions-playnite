# Build and Deploy script for Retrospective Steam 2025
# 
# This script builds the extension and copies it to the Playnite extensions folder.
# Adjust $playniteExtensions if your Playnite is installed elsewhere.

$playniteExtensions = "$env:AppData\Playnite\Extensions\785d9324-4173-420d-b17a-e2ace45bb317"
$msbuildPath = "C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Current\Bin\MSBuild.exe"
$projectPath = "c:\Users\WEON-ADMIN\Downloads\extensao playnite\RetrospectiveSteam\RetrospectiveSteam\RetrospectiveSteam.csproj"
$outputPath = "c:\Users\WEON-ADMIN\Downloads\extensao playnite\RetrospectiveSteam\RetrospectiveSteam\bin\Debug"

Write-Host "Building project..." -ForegroundColor Cyan
& $msbuildPath $projectPath /t:Rebuild /p:Configuration=Debug

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "Deployment..." -ForegroundColor Cyan
if (!(Test-Path $playniteExtensions)) {
    New-Item -ItemType Directory -Force -Path $playniteExtensions | Out-Null
}

Copy-Item "$outputPath\RetrospectiveSteam.dll" $playniteExtensions -Force
Copy-Item "c:\Users\WEON-ADMIN\Downloads\extensao playnite\RetrospectiveSteam\RetrospectiveSteam\bau_header.png" $playniteExtensions -Force
Copy-Item "c:\Users\WEON-ADMIN\Downloads\extensao playnite\RetrospectiveSteam\RetrospectiveSteam\extension.yaml" $playniteExtensions -Force
Copy-Item "c:\Users\WEON-ADMIN\Downloads\extensao playnite\RetrospectiveSteam\RetrospectiveSteam\icon.png" $playniteExtensions -Force

Write-Host "Done! Please restart Playnite to see the changes." -ForegroundColor Green
