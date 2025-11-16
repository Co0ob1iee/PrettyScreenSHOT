# Script do automatycznego budowania instalatora PrettyScreenSHOT
# Użycie: .\build-installer.ps1 -Method MSIX|WiX|InnoSetup
# Uwaga: Skrypt należy uruchomić z katalogu głównego projektu lub build/installer/

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("MSIX", "WiX", "InnoSetup")]
    [string]$Method,

    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [bool]$SelfContained = $false
)

$ErrorActionPreference = "Stop"

# Navigate to project root if running from build/installer/
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
if ($scriptDir -match "build[\\/]installer$") {
    $projectRoot = (Get-Item $scriptDir).Parent.Parent.FullName
    Set-Location $projectRoot
}
$projectRoot = Get-Location

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "PrettyScreenSHOT Installer Builder" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Method: $Method" -ForegroundColor Yellow
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Runtime: $Runtime" -ForegroundColor Yellow
Write-Host ""

# Krok 1: Clean
Write-Host "[1/4] Cleaning previous builds..." -ForegroundColor Green
dotnet clean -c $Configuration
if (Test-Path "bin\$Configuration") {
    Remove-Item -Recurse -Force "bin\$Configuration"
}
if (Test-Path "obj") {
    Remove-Item -Recurse -Force "obj"
}

# Krok 2: Restore
Write-Host "[2/4] Restoring packages..." -ForegroundColor Green
dotnet restore

# Krok 3: Build & Publish
Write-Host "[3/4] Building and publishing..." -ForegroundColor Green
$publishArgs = @(
    "publish",
    "-c", $Configuration,
    "-r", $Runtime,
    "--self-contained", $SelfContained.ToString().ToLower(),
    "-p:PublishSingleFile=true",
    "-p:IncludeNativeLibrariesForSelfExtract=true"
)

if ($SelfContained) {
    $publishArgs += "-p:PublishTrimmed=true"
}

dotnet @publishArgs

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Krok 4: Create Installer
Write-Host "[4/4] Creating installer..." -ForegroundColor Green

$publishPath = "bin\$Configuration\net10.0-windows\$Runtime\publish"

switch ($Method) {
    "MSIX" {
        Write-Host "Creating MSIX package..." -ForegroundColor Cyan
        
        if (-not (Test-Path "Package.appxmanifest")) {
            Write-Host "Error: Package.appxmanifest not found!" -ForegroundColor Red
            exit 1
        }
        
        # Wymaga Windows SDK
        $makeAppx = Get-Command "makeappx.exe" -ErrorAction SilentlyContinue
        if (-not $makeAppx) {
            Write-Host "Warning: makeappx.exe not found. Install Windows SDK." -ForegroundColor Yellow
            Write-Host "You can create MSIX package manually in Visual Studio." -ForegroundColor Yellow
        } else {
            $msixPath = "installer\PrettyScreenSHOT_1.0.0.0_x64.msix"
            New-Item -ItemType Directory -Force -Path "installer" | Out-Null
            
            # Create appx package
            & $makeAppx pack /d $publishPath /p $msixPath
            
            Write-Host "MSIX package created: $msixPath" -ForegroundColor Green
        }
    }
    
    "WiX" {
        Write-Host "Creating WiX installer..." -ForegroundColor Cyan
        
        $candle = Get-Command "candle.exe" -ErrorAction SilentlyContinue
        $light = Get-Command "light.exe" -ErrorAction SilentlyContinue
        
        if (-not $candle -or -not $light) {
            Write-Host "Error: WiX Toolset not found!" -ForegroundColor Red
            Write-Host "Install WiX from: https://wixtoolset.org/" -ForegroundColor Yellow
            exit 1
        }
        
        $wxsPath = "build\installer\Installer.wxs"
        if (-not (Test-Path $wxsPath)) {
            Write-Host "Error: Installer.wxs not found at $wxsPath!" -ForegroundColor Red
            exit 1
        }

        New-Item -ItemType Directory -Force -Path "installer" | Out-Null

        # Compile
        & $candle $wxsPath -ext WixUtilExtension -out "installer\"
        if ($LASTEXITCODE -ne 0) {
            Write-Host "WiX compilation failed!" -ForegroundColor Red
            exit 1
        }
        
        # Link
        & $light "installer\Installer.wixobj" -ext WixUIExtension -ext WixUtilExtension -out "installer\PrettyScreenSHOT-Setup.msi"
        if ($LASTEXITCODE -ne 0) {
            Write-Host "WiX linking failed!" -ForegroundColor Red
            exit 1
        }
        
        Write-Host "MSI installer created: installer\PrettyScreenSHOT-Setup.msi" -ForegroundColor Green
    }
    
    "InnoSetup" {
        Write-Host "Creating Inno Setup installer..." -ForegroundColor Cyan
        
        $iscc = Get-Command "iscc.exe" -ErrorAction SilentlyContinue
        
        if (-not $iscc) {
            Write-Host "Error: Inno Setup not found!" -ForegroundColor Red
            Write-Host "Install Inno Setup from: https://jrsoftware.org/isinfo.php" -ForegroundColor Yellow
            Write-Host "Make sure iscc.exe is in PATH" -ForegroundColor Yellow
            exit 1
        }
        
        $issPath = "build\installer\Installer.iss"
        if (-not (Test-Path $issPath)) {
            Write-Host "Error: Installer.iss not found at $issPath!" -ForegroundColor Red
            exit 1
        }

        # Update source path in ISS file
        $issContent = Get-Content $issPath -Raw
        $issContent = $issContent -replace 'Source: "bin\\Release\\net10\.0-windows\\\*"', "Source: `"$publishPath\*`""
        $issTempPath = "installer\Installer_temp.iss"
        Set-Content $issTempPath -Value $issContent

        # Compile
        & $iscc $issTempPath
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Inno Setup compilation failed!" -ForegroundColor Red
            exit 1
        }
        
        Write-Host "EXE installer created in installer\ folder" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

