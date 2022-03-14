#Requires -RunAsAdministrator
# #Requires -Version 7.0

using namespace System.Runtime.InteropServices;
using namespace System.IO;

if ($IsWindows -eq $false)
{
    [Console]::WriteLine("You must be on Windows to run this script! Press any key to continue...");
    [Console]::ReadKey() | Out-Null;
    return;
}

$Is32or64BitOperatingSystem = ([RuntimeInformation]::OSArchitecture -eq [Architecture]::X64 -or [Architecture]::X86);
$iwr_header = @{ "User-Agent" = "WinPath_install_script" };
$64BitOutput = "C:\Program Files\WinPath\";
$32BitOutput = "C:\Program Files (x86)\WinPath\";
$version = "0.3.1" # Change this every new release.
$installed = $false;


if ([System.Environment]::Is64BitOperatingSystem)
{
    if (Test-Path -Path ([Path]::Combine($64BitOutput, "WinPath.exe")))
    {
        [Console]::Write("WinPath is already installed, run winpath update instead of this script!");
        [Console]::ReadKey() | Out-Null;
        return;
    }
}
else
{
    if (Test-Path -Path ([Path]::Combine($32BitOutput, "WinPath.exe")))
    {
        [Console]::Write("WinPath is already installed, run winpath update instead of this script!");
        [Console]::ReadKey() | Out-Null;
        return;
    }
}

try
{
    if ($Is32or64BitOperatingSystem)
    {
        if ([Environment]::Is64BitOperatingSystem) # x64
        {
            if ([Environment]::OSVersion.Version.Major -eq 10)
            {
                [Directory]::CreateDirectory($64BitOutput) | Out-Null;
                Invoke-WebRequest -Method "GET" -Headers $iwr_header -Uri "https://github.com/ANF-Studios/WinPath/releases/download/$version/WinPath_win10-x64.exe" -OutFile ($64BitOutput + "WinPath.exe");
                $installed = $true;
            }
            else
            {
                [Directory]::CreateDirectory($64BitOutput) | Out-Null;
                Invoke-WebRequest -Method "GET" -Headers $iwr_header -Uri "https://github.com/ANF-Studios/WinPath/releases/download/$version/WinPath_win-x64.exe" -OutFile ($64BitOutput + "WinPath.exe");
                $installed = $true;
            }
        }
        else # x86
        {
            [Directory]::CreateDirectory($32BitOutput) | Out-Null;
            Invoke-WebRequest -Method "GET" -Headers $iwr_header -Uri "https://github.com/ANF-Studios/WinPath/releases/download/$version/WinPath_win-x86.exe" -OutFile ($32BitOutput + "WinPath.exe");
            $installed = $true;
        }
    }
    else
    {
        if ([Environment]::Is64BitOperatingSystem) # Arm64
        {
            [Directory]::CreateDirectory($64BitOutput) | Out-Null;
            Invoke-WebRequest -Method "GET" -Headers $iwr_header -Uri "https://github.com/ANF-Studios/WinPath/releases/download/$version/WinPath_win-arm64.exe" -OutFile ($64BitOutput + "WinPath.exe");
            $installed = $true;
        }
        else # Arm
        {
            [Directory]::CreateDirectory($32BitOutput) | Out-Null;
            Invoke-WebRequest -Method "GET" -Headers $iwr_header -Uri "https://github.com/ANF-Studios/WinPath/releases/download/$version/WinPath_win-arm.exe" -OutFile ($32BitOutput + "WinPath.exe");
            $installed = $true;
        }
    }


}
catch [System.Exception]
{
    $installed = $false;
    [Console]::WriteLine("An error occured while installing WinPath:");
    [Console]::WriteLine($_);
    [Console]::ReadKey() | Out-Null;
}

if ($installed)
{
    $path = [Environment]::GetEnvironmentVariable("Path", [System.EnvironmentVariableTarget]::Machine);
    if ($path.EndsWith(";") -eq $false)
    {
        $path += ";";
    }

    if ([Environment]::Is64BitOperatingSystem)
    {
        [Environment]::SetEnvironmentVariable("Path", ($path + $64BitOutput), [System.EnvironmentVariableTarget]::Machine);
    }
    else
    {
        [Environment]::SetEnvironmentVariable("Path", ($path + $32BitOutput), [System.EnvironmentVariableTarget]::Machine);
    }
    
    [Console]::Write("WinPath is installed successfully! To ensure WinPath does work, restart your computer (optional).");
    [Console]::ReadKey() | Out-Null;
    return;
}
