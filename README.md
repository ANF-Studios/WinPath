<div align="center">
    <h1 >WinPath</h1>
    <p>A simple tool to add variables to your <code>Path</code>!</p>
</div>


## Usage
Using WinPath can be a bit tricky as modifying environment variables can cause corruption of your `Path`. **For this reason I will not be responsible for any loss**, I highly recommend you to make a [backup of your registry](https://support.microsoft.com/en-us/topic/how-to-back-up-and-restore-the-registry-in-windows-855140ad-e318-2a13-2829-d428a2ab0692) *and* also always run the `--backup` flag when adding another value to your registry.

### Adding WinPath to the Path
There are two ways to do so, you can either:
* Move `WinPath.exe` to `C:\Windows\`.

OR

* Move `WinPath.exe` to a directory such as `C:\Program Files\WinPath\` (`C:\Program Files (x86)\WinPath` for x86_64).

Then, either manually add it, using the [Environment Variables window](https://superuser.com/a/284351) or simply use `WinPath.exe` itself:

1. Open up `Command Prompt` in the same directory you've stored `WinPath.exe`.
2. Run:
    ```ps1
    winpath.exe --system --value path\to\winpath.exe
    ```
3. Open up a **new instance** of your terminal and run `winpath`.

### Adding a new value to your path

#### User variables
Run:
```ps1
winpath --user --value ValueToAdd
```

#### System variables
Run:
```ps1
winpath --system --value ValueToAdd
```

## Building and Running
To build WinPath, you are strongly recommended to be on Windows (though building for other platforms is possible by removing checks that throw an exception if the platform is not Windows). You also require .NET Core 5 to be installed.

Once you are on `Windows`, have `.NET 5.0` SDK, run:
```pwsh
dotnet restore
```

Then, run:
```pwsh
dotnet build
# OR, what I use to publish a release:
dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true -p:PublishSingleFile=true --self-contained false
# AND, for 32-bit:
dotnet publish -c Release -r win-x86 -p:PublishReadyToRun=true -p:PublishSingleFile=true --self-contained false
```

To run the executable:
```pwsh
dotnet run
# OR, after you ran dotnet build:
.\bin\Debug\net5.0\WinPath.exe
```

## Help and Support
If you find a bug or something is not working as expected, open an [issue](/issues/new), especially if it's critical to minimize harm/damage if any.

You can also meet me at my Discord server where I am mostly active:

[![Discord server](https://discord.com/api/guilds/732064655396044840/embed.png?style=banner3)](https://discord.gg/fKWpK7A)

<!-- Publish using: dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true -p:PublishSingleFile=true --self-contained false -->
