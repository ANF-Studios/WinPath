<div align="center">
    <h1 >WinPath</h1>
    <p>A simple tool to add variables to your <code>Path</code>!</p>
    <a href="https://github.com/ANF-Studios/WinPath/actions/workflows/dotnet.yml" alt="GitHub Actions | .NET" target="_blank"><img src="https://github.com/ANF-Studios/WinPath/actions/workflows/dotnet.yml/badge.svg" /></a>
    <a href="https://codecov.io/gh/ANF-Studios/WinPath" alt="CodeCov | Code Coverage" target="_blank"><img src="https://codecov.io/gh/ANF-Studios/WinPath/branch/master/graph/badge.svg?token=ZDA5DTLQMF" /></a>
    <a href="https://github.com/ANF-Studios/WinPath/actions/workflows/codeql.yml" alt="GitHub CodeQL | Security Scan" target="_blank"><img src="https://github.com/ANF-Studios/WinPath/actions/workflows/codeql.yml/badge.svg" /></a>
    <a href="https://ci.appveyor.com/project/ANF-Studios/winpath" alt="AppVeyor Build | .NET" target="_blank"><img src="https://ci.appveyor.com/api/projects/status/um85ms51acjnnux4/branch/master?svg=true" /></a>
</div>


<h2 align="center">Installation</h2>

**Before installing WinPath, make sure you have *at least* [`.NET 5`](https://dotnet.microsoft.com/download/dotnet/5.0/runtime)'s *runtime* installed. If not, WinPath will NOT work.**

### CLI Installation
To install WinPath download `WinPath_win<optioanl version>-<arch>` where `<optional version>` can either be `10` or nothing. And `<arch>` can be either x86, x64, arm or arm64. If you don't know your pc architecture, you can get help from [this guide](https://winaero.com/check-if-processor-is-32-bit-64-bit-or-arm-in-windows-10/). It's recommended that you download `WinPath_win10-x64` if you're on a 64-bit Windows 10 computer.

After you've downloaded WinPath, you can either manually install it by moving WinPath and adding it to the `Path`, or, you can let WinPath handle that for you by running the `update` command. To do so, open up command prompt in the same directory you have WinPath downloaded and type `.\<the name of the executable> update -p -y` and let everything be handled for you.

**Keep note** that if you already have WinPath installed, you only need to run `winpath update` as it's already installed and do not require downloading anything.

### Manual Installation
In the second option, there are two ways to do so, you can either:
* Move `WinPath.exe` to `C:\Windows\`.

OR

* Move `WinPath.exe` to a directory such as `C:\Program Files\WinPath\` (`C:\Program Files (x86)\WinPath\` for x86).

Then, either manually add it, using the [Environment Variables window](https://superuser.com/a/284351) or simply use `WinPath.exe` itself:

1. Open up `Command Prompt` in the same directory you've stored `WinPath.exe`.
2. Run:
    ```ps1
    > winpath.exe --system --value path\to\winpath.exe
    ```
3. Open up a **new instance** of your terminal and run `winpath`.

<h2 align="center">Usage</h2>

### Usage demo:
![WinPath's usage demo](https://user-images.githubusercontent.com/68814933/115187380-35f77f00-a0b1-11eb-815e-3cf75d275d12.gif)

### Adding variables to the `Path`
Adding variables to the `Path` isn't hard, at all! It's focused on being simple, secure, and safe. We've even set up extra precautions to make sure nothing goes wrong, but you don't need to use those because you're safe, 95% (yes, I actually mesured it using unit tests from every CI run) of the times!

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
You can also backup your *initial* `Path` variable(s) before modifying so that if you're from one of those 5%, you can restore your `Path`. You can do so simply by adding the `--backup` flag when running that command. Currently, as of v0.2.0, WinPath offers no way to restore your `Path` variable(s), you have to do it manually. I also highly recommend you to make a [backup of your registry](https://support.microsoft.com/en-us/topic/how-to-back-up-and-restore-the-registry-in-windows-855140ad-e318-2a13-2829-d428a2ab0692). They're stored in `%APPDATA%\WinPath\` divided into sub folders, you can look at it for yourself. If you need assistance regarding this matter, please do not hesitate to open an issue.

### Backup Command
#### Creating a backup
A backup every once in a while is a good idea to make, a reliable and working system really helps with it since you don't need to manually back up your registry or copy the path and write to a file yourself, WinPath does it for you instead.

To backup your variables, run
```ps1
# For user variabes only.
> winpath backup create --user

# For system variables only.
> winpath backup create --system

# For both user and system variables.
> winpath backup create --user --system

# You can also use -u or -s for short.
```

Do note that if you leave it empty, it wouldn't backup anything so at least one of these flags are required.

If you want to backup to a specific directory, you can easily do so by appending the directory flag:
```ps1
> winpath backup create --system --directory D:\backup\path\system

# You can use -d for short.
```

These backups by default are stored in `%APPDATA%\WinPath\UserBackups` for user variables and `%APPDATA%\WinPath\SystemBackups` for system variables.

#### Applying a backup
If you want to restore your `Path` variable to an earlier point from your backup collection, you can run the `apply` command. To do so, run:
```ps1
# For user backups:
> winpath backup apply --user --name filename

# For system backups:
> winpath backup apply --system --name 

# You can't use both together.
```

By default, it will search in `%APPDATA%\WinPath\UserBackups` for user variables and `%APPDATA%\WinPath\SystemBackups` for system variables. But since file names are based on the current local time in file time format, you need to know their exact name, for that, you can [list backups](#listing-backups). You can use the directory flag to override the location to search for the file:

```ps1
> winpath backup apply --user --name filename --directory path
```

Once you start restoring a file backup, WinPath will first check your flags if they're proper, once that's passed, it will check if the actual file to restore exists, if true, it will proceed to read it. If it's empty, it won't continue and stop the process, this is for safety purposes.

Before the final stage of replacing it, It will create a backup to `%TEMP%\WinPath` just in case you change your mind, it will write to `u_backup.txt` for user backups and `s_backup.txt` for system backups, if you run the command twice by any chance, you can not recover it back unless you have a backup from an earlier time. And if it by any chance fails to write this file, the process would not crash, it would simply ignore this. There is no way to change this behavior including stopping it to make a backup before replacing your path.

Once everything is done, it will replace the path environment variable.

#### Removing a backup
Removing a backup is simple. You only need to provide the file name:
```ps1
> winpath backup remove --name filename
```

You do not need a fully qualified file name, this is because by default it searches in `%APPDATA%\WinPath\UserBackups` for user variables and `%APPDATA%\WinPath\SystemBackups` for system variables. But since file names are based on the current local time in file time format, you need to know their exact name, for that, you can [list backups](#listing-backups).

If you want to override this directory, you can run the directory flag:
```ps1
> winpath backup remove --name filename --directory directory
```

Funny how in this sense, it can be used as a way to delete any kind of a file.

#### Listing backups
##### Listing every backup
If you want to list every backup there is, simply run:
```ps1
> winpath backup list --all
```

##### Listing specified amount of backups
If you want to list the latest backups, run
```ps1
> winpath backup list
```

This will list the first 10 backups or till 10 backups -- as much as it can if less than 10. If you want to override this value, you can run
```ps1
> winpath backup list --range 42
```

This will list 42 backups. And you might notice that you can't override the directory to search for, unlike in other commands. This is because other directories might include files not of interest, it's best that it remains in a constant directory. However, I do have plans to change that in the future.

##### Listing the latest backups
If you don't want this and just simply want to see your most recent backups, simply run
```ps1
> winpath backup list --latest
```

This will list the first three backups or until it reaches how many backups there -- if there are less than this amount.


### Updating WinPath
Updating WinPath is easier than ever. With v0.2.0, you have the `update` command which acts as all the installer, updater, and reinstaller. You can update WinPath by running `winpath update`, it'll ask you for a confirmation, if you want to confirm the installationm, you can add the `-y` (or `--confirm`) flag. If you want to update to a beta/prerelease, you can add the `--prerelease` flag. Prereleases are revisions of WinPath that are generally less stable and are not meant to handle errors properly compared to a distribution build, it may also be a featureless, under-work build. 

<h2 align="center">Building and Running</h2>

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
```ps1
> dotnet run
# OR, after you ran dotnet build:
> .\bin\Debug\net5.0\WinPath.exe
```

<h2 align="center">Help and Support</h2>

<h3 align="center">FAQ</h3>

#### Where can I find information on `WinPath.Library`?
You can learn about WinPath.Library [here](https://github.com/ANF-Studios/WinPath/tree/master/WinPath.Library).

#### What is WinPath?
WinPath is a simple tool to add variables to your [`Path`](https://en.wikipedia.org/wiki/PATH_(variable)). It focuses on your safety and ease of use primarily, because sometimes, you might change something not wanted or change (for example delete a value or the entire `Path` itself) or... just find it hard to modify variables. WinPath covers that for you.

#### But, for such a simple task? Why?
Small and simple things make a difference, it doesn't matter to me by how much. Besides, I started this project originally for myself.

#### Why should I use WinPath?
You don't need to, it's not one of those "must have" tools, but it's really helpful. ***Especially if you don't have administrator permissions***. That means you won't need any permission to add another variable to your `Path`. That really helps out and is super useful for some of you folks out there.

##### But wait, doesn't that mean WinPath will require UAC access too?
No. It doesn't. Because Win32 API allows modification of `Path` freely.

#### Why is WinPath Windows only?
Because of some complexities which will require more time, work, and testing, I will officially not support WinPath builds for platforms other than Windows. *However*, you can always use `WinPath.Library` for all platforms.

#### Is it safe?
Of course it is. I [dogfood](https://en.wikipedia.org/wiki/Eating_your_own_dog_food) it too, as if I am the client! I just put it here on GitHub so that others could use it too! If that doesn't satisfy you, you can compile your own version and look through the code, it's completely *safe*. Even our [CodeQL reports](https://github.com/ANF-Studios/WinPath/actions/workflows/codeql.yml) don't find any vulnerabilities either.

#### Why is X like so? Why not Y?
Because that's how I decided to implement X. If you have a better idea/proposal, please, do open an [issue](https://github.com/ANF-Studios/WinPath/issues/new/choose), but before that, search for similar issues.

#### Should I star WinPath?
Yes, starring WinPath motivates me to do better and keep working on it, that's also how I know that you appreciate my work.

<br />
<br />

If you find a bug or something is not working as expected, open an [issue](https://github.com/ANF-Studios/WinPath/issues/new/choose), especially if it's critical to minimize harm/damage if any. Refer to [SECURITY.md](/SECURITY.md) for more information.

You can also meet me at my Discord server where I am mostly active:

[![Discord server](https://discord.com/api/guilds/732064655396044840/embed.png?style=banner3)](https://discord.gg/fKWpK7A)

<!-- Publish using: dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true -p:PublishSingleFile=true --self-contained false -->
