# Security Policy and Procedures

This document outlines security procedures and general policies for this project.

<details>
<summary>Index</summary>

* [Supported Versions](#supported-versions)
* [Reporting a Vulnerability](#reporting-a-vulnerability)
* [Disclosure Policy](#disclosure-policy)
* [Service functionality](#service-functionality)
* [Comments on this Policy](#comments-on-this-policy)

</details>


## Supported Versions

WinPath's minimum supported .NET version is 5.0.x, this is because it calls some functions that are .NET 5 only.

| Version | Supported          |
| ------- | ------------------ |
| 5.1.x   | :white_check_mark: |
| 5.0.x   | :white_check_mark: |
| 4.8     | :x:                |
| ≤ 4.7   | :x:                |

## Reporting a Vulnerability

If you find a vulnerability/bug, you can open an issue in the [issues section](https://github.com/ANF-Studios/WinPath/issues/), especially if it's critical to minimize potential damage on other devices.

Once an issue is opened, it'll be reviewed by the code-owner(s). If it is accepted as a bug, a fix will be issued, possibly a bug fix release depending on how vulnerable it is. If it's not, the issue will be closed with a note on what can be done about it.

You can then update WinPath getting a release from the [releases section](https://github.com/ANF-Studios/WinPath/releases) or running `winpath update` in your terminal.

## Disclosure Policy

When the a security bug report is receieved, fix and release process will start, involving the following steps:

* Confirm the problem and determine the affected versions.
* Audit code to find any potential similar problems.
* Prepare fixes for all releases still under maintenance. These fixes will be released as fast as possible.

## Service functionality

WinPath does a number of stuff that raises concerns on whether it's safe or not. WinPath is designed to be as safe and as simple as possible.

As of v0.1, you can only add to your `Path` in user environment variables. Adding a new value to your `Path` is completely safe and has been tested in many ways with precaution measures. The `--backup` flag backs up the `Path` variable *before* adding a new value so that it could be restored if needed. These files are stored at `%APPDATA%\WinPath\PathBackup\`, these are not encrypted and files are stored in Long date string format.

WinPath does **NOT** do anything other than that.


## Comments on this Policy

If you have suggestions on how this process could be improved please submit a pull request.
