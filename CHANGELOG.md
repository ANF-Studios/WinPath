# Changelog

## [v0.3.1](https://github.com/ANF-Studios/WinPath/releases/tag/0.3.1)
* Minor fix in update command; conflicting paths (#49).
* List command should display the table of backups without inconsistencies (#52).
* `WinPath.Updater` (manages updates) will now log errors correctly (#54).
* Running `winpath backup create` will now give the correct error instead of "Something went wrong!" (#56).
* Minor bug fix when prerelease flag was not run (#72).
* [Other/Non-bug fix change] `WinPath.Updater` now has backwards compatibility support with v0.2.0 and v0.3.0 (#78).
* [Other/Non-bug fix change] You can now simply download `install.ps1` instead of going through a mess of manual downloading for your first installation! (#85).

## [v0.3.0](https://github.com/ANF-Studios/WinPath/releases/tag/0.3.0)
* WinPath.Library.UserPath now targets async tasks instead of sync voids.
* Changed download directory (of update command) to `%TEMP%\WinPath\download` from `%APPDATA%\WinPath\temp\download`.
* Console title now sets when WinPath Cli is used.
* Added some checks in the update command to detect AppVeyor -- do not have an environment variable called `APPVEYOR`.
* Fixed a bug where the path slash wasn't reversed.
* Added additional logging.
* Improved message clearity.
* WinPath.Updater now doesn't filter prereleases when prerelease flag is not run.
* Massively decreased size of WinPath.exe (Microsoft.Win32.Registry dependency dropped).
* WinPath now uses the core lib; `WinPath.Library` for its general operations.
* WinPath.Updater no longer reads a key when it throws an exception.
* Update class within WinPath is now fully public.
* WinPath doesn't directly exit in some parts now.
* Added backup command.
    * Added backup create command.
    * Added backup remove command.
    * Added backup apply command.
    * Added backup list command.
* WinPath.Updater now takes specific arguments to verify that a user is not launching it which can cause runtime exceptions.


## [v0.2.0](https://github.com/ANF-Studios/WinPath/releases/tag/0.2.0)
* Fix typo in help command (help text of system flag).
* Added update command.
* Moved add to path option to its own section (`winpath add`).
* Fix typo in help text of update command's command line flag.
* Set up CI/CD properly.
* Created solution file.
* Added tests (project).
* Created `SECURITY.md` file.
* Created WinPath.Updater project which helps updating WinPath.

## [v0.1.0](https://github.com/ANF-Studios/WinPath/releases/tag/0.1.0)
* Basic feature; can add user Path variables.
* Can backup Path (user).
* Help Command.
