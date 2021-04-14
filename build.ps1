function Remove-TestProject
{
    if ((Test-Path -Path ".\bin\temp\") -eq $false) {
        New-Item -Path ".\bin\temp" -ItemType "directory" -Force -Confirm
    }
    Copy-Item -Path ".\WinPath.sln" -Destination ".\bin\temp\WinPath.sln" -Force -Confirm
    dotnet sln remove ".\WinPath.Tests\WinPath.Tests.csproj"
}

function Add-TestProject
{
    Remove-Item -Path ".\WinPath.sln" -Force -Confirm
    Move-Item -Path ".\bin\temp\WinPath.sln"
}

function Restore-Packages
{
    Write-Host "Restoring packages..."
    dotnet restore --force
    Write-Host "Restored packages..."
}

function Build-Win64
{
    Write-Host "Building for Windows x64..."
    dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true -p:PublishSingleFile=true --self-contained false --nologo
    Write-Host "Built for Windows x64..."
}

function Build-Win32
{
    Write-Host "Building for Windows x86..."
    dotnet publish -c Release -r win-x86 -p:PublishReadyToRun=true -p:PublishSingleFile=true --self-contained false --nologo
    Write-Host "Built for Windows x86..."
}

function Build-WinArm
{
    Write-Host "Building for Windows Arm.."
    dotnet publish -c Release -r win-arm -p:PublishReadyToRun=true -p:PublishSingleFile=true --self-contained false --nologo
    Write-Host "Built for Windows Arm..."
}

function Build-WinArm64
{
    Write-Host "Building for Windows Arm 64..."
    dotnet publish -c Release -r win-arm64 -p:PublishReadyToRun=true -p:PublishSingleFile=true --self-contained false --nologo
    Write-Host "Built for Windows Arm 64..."
}

function Build-Win10_64 # One specific build for Windows 10 64-bit.
{
    Write-Host "Building for Windows 10 x64..."
    dotnet publish -c Release -r win10-x64 -p:PublishReadyToRun=true -p:PublishSingleFile=true --self-contained false --nologo
    Write-Host "Built for Windows 10 x64..."
}

function Compress-Executables
{
    # (maybe) TODO: Compress each executable in their own archive. 
}

function Rename-Executables
{
    [string[]] $executablesToRename =
        ".\bin\WinPath\net5.0\win-x64\publish\WinPath.exe",
        ".\bin\WinPath\net5.0\win-x86\publish\WinPath.exe",
        ".\bin\WinPath\net5.0\win-arm\publish\WinPath.exe",
        ".\bin\WinPath\net5.0\win-arm64\publish\WinPath.exe",
        ".\bin\WinPath\net5.0\win10-x64\publish\WinPath.exe"
    Rename-Item -Path $executablesToRename[0] -NewName "WinPath_win-x64.exe"
    Rename-Item -Path $executablesToRename[1] -NewName "WinPath_win-x86.exe"
    Rename-Item -Path $executablesToRename[2] -NewName "WinPath_win-arm.exe"
    Rename-Item -Path $executablesToRename[3] -NewName "WinPath_win-arm64.exe"
    Rename-Item -Path $executablesToRename[4] -NewName "WinPath_win10-x64.exe"
}

function Move-ToOneFolder
{
    if ((Test-Path -Path ".\bin\dist") -eq ($false)) { New-Item -Path ".\bin\dist\" -ItemType "directory" }
    Rename-Executables
    [string[]] $executables =
        ".\bin\WinPath\net5.0\win-x64\publish\WinPath_win-x64.exe",
        ".\bin\WinPath\net5.0\win-x86\publish\WinPath_win-x86.exe",
        ".\bin\WinPath\net5.0\win-arm\publish\WinPath_win-arm.exe",
        ".\bin\WinPath\net5.0\win-arm64\publish\WinPath_win-arm64.exe",
        ".\bin\WinPath\net5.0\win10-x64\publish\WinPath_win10-x64.exe",
        ".\bin\WinPath.Updater\net5.0\win-x86\"
    Move-Item -Path $executables -Destination ".\bin\dist\"
}

Remove-Item -Path ".\bin\dist\" -Recurse -Force -Confirm
Remove-TestProject
Restore-Packages
Build-Win64
Build-Win32
Build-WinArm
Build-WinArm64
Build-Win10_64
Move-ToOneFolder
Add-TestProject
#Compress-Executables
Write-Host "All tasks completed!"
