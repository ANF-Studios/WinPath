using System;

using CommandLine;

using WinPath.Library;

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
                    if (System.IO.File.Exists(Update.UpdateStatusFile))
                        System.IO.File.Delete(Update.UpdateStatusFile);
                    Console.WriteLine("Updating WinPath...");
                    Update update = new Update(options.IncludePrereleases, options.ConfirmDownload);
                    var releases = update.GetReleases();
                    Release release = update.FilterRelease(releases);
                    Console.WriteLine(release.TagName);
                    Console.WriteLine(update.GetAssetForProcess(release).ExecutableName);
                    ReleaseInfo releaseInfo = new ReleaseInfo
                    {
                        ReleaseName = release.ReleaseName,
                        TagName = release.TagName,
                        IsPrerelease = release.IsPrerelease,
                        ReleaseDescription = release.Description,
                        ReleaseAsset = update.GetAssetForProcess(release)!,
                        Updater = release.Assets.Find((asset) => asset.ExecutableName == "WinPath.Updater.exe")
                    };
                    update.DownloadWinPath(releaseInfo);
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
