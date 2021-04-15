﻿using System;

using CommandLine;

using WinPath.Library;

using Architecture = System.Runtime.InteropServices.Architecture;
using Runtime = System.Runtime.InteropServices.RuntimeInformation;

namespace WinPath
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!OperatingSystem.IsWindows())
                throw new PlatformNotSupportedException("WinPath is Windows only!");
            // Temporary debug code.
            //Library.UserPath.BackupPath(System.Environment.GetEnvironmentVariable("Path", System.EnvironmentVariableTarget.User));
            Parser.Default.ParseArguments<AddOptions, UpdateOptions>(args)
                .WithParsed<AddOptions>(options => {
                    if (options.Value == null) HandleArgument(HandleEventType.NoValue);
                    else if (options.Value != null)
                    {
                        if (options.AddToUserVariables && options.AddToSystemVariables)
                            HandleArgument(HandleEventType.UserAndSystemPath, options);
                        else if (options.AddToUserVariables)
                            HandleArgument(HandleEventType.UserPath, options);
                        else if (options.AddToSystemVariables)
                            HandleArgument(HandleEventType.SystemPath, options);
                        else
                            HandleArgument(HandleEventType.NoUserOrSystemPath);
                    }
                })
                .WithParsed<UpdateOptions>(options => {
                    Console.WriteLine("Updating WinPath...");
                    Update update = new Update
                    (
                        options.IncludePrereleases,
                        options.ConfirmDownload,
                        (Runtime.OSArchitecture == Architecture.X86
                            || Runtime.OSArchitecture == Architecture.X64)
                    );
                    Console.WriteLine("Fetching data from the server...");
                    var releases = update.GetReleases();
                    Console.WriteLine("Analyzing data...");
                    Release release = update.FilterRelease(releases);
                    Console.WriteLine("Parsing data...");
                    ReleaseInfo releaseInfo = new ReleaseInfo
                    {
                        ReleaseName = release.ReleaseName,
                        TagName = release.TagName,
                        IsPrerelease = release.IsPrerelease,
                        ReleaseDescription = release.Description,
                        ReleaseAsset = update.GetAssetForProcess(release)!,
                        Updater = release.Assets.Find((asset) => asset.ExecutableName == (
                            update.Is32Or64BitOperatingSystem
                                ? "WinPath.Updater_x86.exe"
                                : "WinPath.Updater_arm.exe" 
                            ))
                    };
                    update.DownloadWinPath(releaseInfo, () => {
                        foreach (string file in
                            System.IO.Directory.EnumerateFiles(
                                $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\WinPath\\temp\\download\\"
                        ))
                            try
                            {
                                System.IO.File.Delete(file);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error cleaning up: " + ex.Message);
                            }
                    });
                    //update.GetArchitecture(Runtime.ProcessArchitecture);
                });
        }

        static void HandleArgument(HandleEventType eventType, AddOptions options = null)
        {
            switch (eventType)
            {
                case HandleEventType.NoValue:
                    Console.WriteLine("Please provide a value to be added.");
                    break;

                case HandleEventType.UserPath:
                    UserPath.AddToPath(options.Value);
                    break;

                case HandleEventType.SystemPath:
                    throw new NotImplementedException("Cannot add to System Path as it's not implemented.");
                
                case HandleEventType.UserAndSystemPath:
                    throw new NotImplementedException("Cannot add to User and System Path as it's not implemented.");

                case HandleEventType.NoUserOrSystemPath:
                    Console.WriteLine("Did not modify any content, exiting...");
                    break;
            }
        }
    }
}
