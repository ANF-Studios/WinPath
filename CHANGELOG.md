# Changelog

## WIP
* WinPath.Library.UserPath now targets async tasks instead of sync voids.
* Changed download directory (of update command) to `%TEMP%\WinPath\download` from `%APPDATA%\WinPath\temp\download`.
* Console title now sets when WinPath Cli is used.
* Added some checks in the update command to detect AppVeyor -- do not have an environment variable called `APPVEYOR`.
* Fixed a bug where the path slash wasn't reversed.
* Added additional logging.
* Improved message clearity.

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
