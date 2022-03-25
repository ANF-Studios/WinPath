using System;
using System.IO;
using System.Runtime.Versioning;

using CommandLine;

using WinPath.Library;
using WinPath.Extensions;

using Architecture = System.Runtime.InteropServices.Architecture;
using Runtime = System.Runtime.InteropServices.RuntimeInformation;

namespace WinPath
{
    /// <summary>
    /// Main class for the entire program,
    /// contains the main method as well.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// A default instance of <see cref="UserPath"/> used
        /// across the entire program.
        /// </summary>
        internal static readonly UserPath UserPath = new UserPath
        {
            BackupDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\{System.AppDomain.CurrentDomain.FriendlyName}\\UserBackups\\"
        };

        /// <summary>
        /// Main method, the entry point of the program.
        /// </summary>
        /// <param name="args">Arguments provided from the cli.</param>
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
                .WithParsed<BackupOptions.BackupPrintOptions>(options => Backup.PrintPath(options))
                .WithParsed<BackupOptions.BackupListOptions>(options =>
                {
                    if (options.ListAllBackups)
                        Backup.ListBackups(
                            HandleEventType.ListAllBackups,
                            UserPath.BackupDirectory
                        );
                    else if (options.ListLatest)
                        Backup.ListBackups(
                            HandleEventType.ListLatestBackups,
                            UserPath.BackupDirectory
                        );
                    else
                        Backup.ListBackups(
                            HandleEventType.ListBackups,
                            UserPath.BackupDirectory,
                            null,
                            options.Range
                        );
                })
                .WithParsed<BackupOptions.BackupApplyOptions>(options => Backup.ApplyBackup(options))
                .WithParsed<BackupOptions.BackupCreateOptions>(options => Backup.CreateBackup(options))
                .WithParsed<BackupOptions.BackupRemoveOptions>(options => Backup.RemoveBackup(options))
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
                    Release? release = update.FilterRelease(releases);
                    
                    // To be removed in v1.0.0.
                    if (release is null)
                    {
                        Console.WriteLine("There is no stable release at the moment, please run this command again with the --prerelease flag.");
                        return;
                    }
                    ReleaseInfo releaseInfo = new ReleaseInfo
                    {
                        ReleaseName = release?.ReleaseName,
                        TagName = release?.TagName,
                        IsPrerelease = (bool)(release?.IsPrerelease),
                        ReleaseDescription = release?.Description,
                        ReleaseAsset = update.GetAssetForProcess((Release)release)!,
                        Updater = (Asset)release?.Assets.Find((asset) => asset.ExecutableName == Update.InstallationTool)
                    };
                    update.DownloadWinPath(releaseInfo/*, () => {
                        foreach (string file in
                            Directory.EnumerateFiles(
                                $"{Path.GetTempPath()}WinPath\\download\\"
                        ))
                            try
                            {
                                File.Delete(file);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error cleaning up: " + ex.Message);
                            }
                    }*/);
                });
        }

        // TODO: Refactor it soon.
        /// <summary>
        /// Handle arguments relating to the <c>Path</c>.
        /// </summary>
        /// <param name="eventType">The type of the event to handle.</param>
        /// <param name="options">Options for to be used according to <paramref name="eventType"/>.</param>
        static void HandleArgument(HandleEventType eventType, AddOptions options = null)
        {
            switch (eventType)
            {
                case HandleEventType.NoValue:
                    Console.WriteLine("Please provide a value to be added.");
                    break;

                case HandleEventType.UserPath:
                    UserPath.AddToPath(
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

        /// <summary>
        /// Get the default <see cref="UserPath"/> instance.
        /// </summary>
        /// <remarks>
        /// When the application starts, <see cref="Program"/> creates
        /// an instance of <see cref="UserPath"/> called <see cref="UserPath"/>,
        /// this method returns this global instance, it contains data that's
        /// later on used.
        /// </remarks>
        /// <returns>The default instance created at the start of the program.</returns>
        public static UserPath GetUserPath()
            => UserPath;
    }
}
