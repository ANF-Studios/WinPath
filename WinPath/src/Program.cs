using System;
using System.Runtime.Versioning;

using CommandLine;

using WinPath.Library;
using WinPath.Extensions;

using Architecture = System.Runtime.InteropServices.Architecture;
using Runtime = System.Runtime.InteropServices.RuntimeInformation;

namespace WinPath
{
    public class Program
    {
        private static readonly UserPath userPath = new UserPath
        {
            BackupDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\{System.AppDomain.CurrentDomain.FriendlyName}\\UserBackups\\"
        };

        [SupportedOSPlatform("windows")]
        [UnsupportedOSPlatform("browser")]
        public static void Main(string[] args)
        {
            Console.Title = AppDomain.CurrentDomain.FriendlyName;
            if (!OperatingSystem.IsWindows())
                throw new PlatformNotSupportedException("WinPath is Windows only!");
            Parser.Default.ParseVerbs<AddOptions, BackupOptions, UpdateOptions>(args)
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
                //.WithParsed<BackupOptions>(options =>
                //{
                //    Doesn't react/trigger this section.
                //    This is because of child verbs.
                //})
                .WithParsed<BackupOptions.BackupListOptions>(options =>
                {
                    if (options.ListAllBackups)
                        ListBackups();
                    else if (options.ListLatest)
                        ListBackups();
                    else
                        ListBackups();
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
                                $"{System.IO.Path.GetTempPath()}WinPath\\download\\"
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
                    userPath.AddToPath(
                        options.Value,
                        options.BackupPathVariable,
                        DateTime.Now.ToFileTime().ToString()
                    );
                    if (Environment.GetEnvironmentVariable(
                            "Path",
                            EnvironmentVariableTarget.User)
                        .EndsWith(
                            $"{options.Value};"
                        )
                    ) Console.WriteLine($"Successfully added `{options.Value}` to the Path!");
                    else
                        Console.WriteLine(
                            "There seems to be an error, we could not verify if that value is actually added to the Path or not, it's nothing to worry about though!"
                        );
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

        public static UserPath GetUserPath()
            => userPath;
    }
}
