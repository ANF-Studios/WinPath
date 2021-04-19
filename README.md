<div align="center">
    <h1 >WinPath</h1>
    <p>A simple tool to add variables to your <code>Path</code>!</p>
    <a href="https://github.com/ANF-Studios/WinPath/actions/workflows/dotnet.yml" alt="GitHub Actions | .NET" target="_blank"><img src="https://github.com/ANF-Studios/WinPath/actions/workflows/dotnet.yml/badge.svg" /></a>
    <a href="https://codecov.io/gh/ANF-Studios/WinPath" alt="CodeCov | Code Coverage" target="_blank"><img src="https://codecov.io/gh/ANF-Studios/WinPath/branch/master/graph/badge.svg?token=ZDA5DTLQMF" /></a>
    <a href="https://github.com/ANF-Studios/WinPath/actions/workflows/codeql.yml" alt="GitHub CodeQL | Security Scan" target="_blank"><img src="https://github.com/ANF-Studios/WinPath/actions/workflows/codeql.yml/badge.svg" /></a>
    <a href="https://ci.appveyor.com/project/ANF-Studios/winpath" alt="AppVeyor Build | .NET" target="_blank"><img src="https://ci.appveyor.com/api/projects/status/um85ms51acjnnux4?svg=true" /></a>
</div>


## Installation
### CLI Installation
To install WinPath download `WinPath_win<optioanl version>-<arch>` where `<optional version>` can either be `10` or nothing. And `<arch>` can be either x86, x64, arm or arm64. If you don't know your pc architecture, you can get help from [this guide](https://winaero.com/check-if-processor-is-32-bit-64-bit-or-arm-in-windows-10/). It's recommended that you download `WinPath_win10-x64` if you're on a 64-bit Windows 10 computer.

After you've downloaded WinPath, you can either manually install it by moving WinPath and adding it to the `Path`, or, you can let WinPath handle that for you by running the `update` command. To do so, open up command prompt in the same directory you have WinPath downloaded and type `.\<the name of the executable> update -p -y` and let everything be handled for you.

Keep note that if you already have WinPath installed, you only need to run `winpath update` as it's already installed and do not require downloading anything.

Using WinPath can be a bit tricky as modifying environment variables can cause corruption of your `Path`. **For this reason I will not be responsible for any loss**, I highly recommend you to make a [backup of your registry](https://support.microsoft.com/en-us/topic/how-to-back-up-and-restore-the-registry-in-windows-855140ad-e318-2a13-2829-d428a2ab0692) *and* also always run the `--backup` flag when adding another value to your registry.

### Manual Installation
In the second option, there are two ways to do so, you can either:
* Move `WinPath.exe` to `C:\Windows\`.

OR

* Move `WinPath.exe` to a directory such as `C:\Program Files\WinPath\` (`C:\Program Files (x86)\WinPath\` for x86).

Then, either manually add it, using the [Environment Variables window](https://superuser.com/a/284351) or simply use `WinPath.exe` itself:

1. Open up `Command Prompt` in the same directory you've stored `WinPath.exe`.
2. Run:
    ```ps1
    winpath.exe --system --value path\to\winpath.exe
    ```
3. Open up a **new instance** of your terminal and run `winpath`.

## Usage
### Usage demo:
![WinPath's usage demo](https://user-images.githubusercontent.com/68814933/115187380-35f77f00-a0b1-11eb-815e-3cf75d275d12.gif)

### Adding variables to the `Path`
Adding variables to the `Path` isn't hard, at all! It's focused on being simple, secure, and safe. We've even set up extra precautions to make sure nothing goes wrong, but you don't need to use those because you're safe, 95% (approx.) of the times!

To add a variable to your `Path`, simply run:
```ps1
> winpath add --value foo
```
But that won't change anything and you'll get a warning saying `Did not modify any content, exiting...`. That's because you have to run at least either of `--user` or `--system` flag. These flags define which variable type/location to add to. There are two types, concerning WinPath itself, one being the `User` and other being `Machine` or `System`. `User` variables are only limited to the account, whereas `System` variables work on all accounts globally. You can add both as well, but you must supply at least one of them.
```ps1
# Add a variable to your user Path.
> winpath add --user --value foo
# Add a variable to your system Path.
> winpath add --system --value bar
# Add a variable to both user and system Paths.
> winpath add --system --user --value baz
```
You can also backup your *initial* `Path` variable(s) before modifying so that if you're from one of those 5%, you can restore your `Path`. You can do so simply by adding the `--backup` flag when running that command. Currently, as of v0.2.0, WinPath offers no way to restore your `Path` variable(s), you have to do it manually. They're stored in `%APPDATA%\WinPath\` divided into sub folders, you can look at it for yourself. If you need assistance regarding this matter, please do not hesitate to open an issue.

### Updating WinPath
Updating WinPath is easier than ever. With v0.2.0, you have the `update` command which acts as all the installer, updater, and reinstaller. You can update WinPath by running `winpath update`, it'll ask you for a confirmation, if you want to confirm the installationm, you can add the `-y` (or `--confirm`) flag. If you want to update to a beta/prerelease, you can add the `--prerelease` flag. Prereleases are revisions of WinPath that are generally less stable and are not meant to handle errors properly compared to a distribution build, it may also be a featureless, under-work build. 

## Building and Running
To build WinPath, you are strongly recommended to be on Windows (though building for other platforms is possible by removing checks that throw an exception if the platform is not Windows). You also require .NET Core 5 to be installed.

Once you are on `Windows`, have `.NET 5.0` SDK, run:
```ps1
> dotnet restore
```

Then, run:
```ps1
> dotnet build
# OR, what I use to publish a release:
> dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true -p:PublishSingleFile=true --self-contained false
# AND, for 32-bit:
> dotnet publish -c Release -r win-x86 -p:PublishReadyToRun=true -p:PublishSingleFile=true --self-contained false
```

To run the executable:
```
> dotnet run
# OR, after you ran dotnet build:
> .\bin\Debug\net5.0\WinPath.exe
```

## Help and Support
If you find a bug or something is not working as expected, open an [issue](https://github.com/ANF-Studios/WinPath/issues/new), especially if it's critical to minimize harm/damage if any. Refer to [SECURITY.md](/SECURITY.md) for more information.

You can also meet me at my Discord server where I am mostly active:

[![Discord server](https://discord.com/api/guilds/732064655396044840/embed.png?style=banner3)](https://discord.gg/fKWpK7A)

<!-- Publish using: dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true -p:PublishSingleFile=true --self-contained false -->
