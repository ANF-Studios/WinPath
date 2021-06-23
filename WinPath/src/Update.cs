using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WinPath
{
    /// <summary>
    /// The main class handler for the
    /// update command.
    /// </summary>
    public class Update
    {
        /// <summary>
        /// Whether to include prereleases from
        /// the github <seealso cref="releases"/>
        /// section or not.
        /// </summary>
        private readonly bool includePrereleases;
        /// <summary>
        /// If the current operating system is 32-bit
        /// or 6-bit, or not. If it's not, the possibility
        /// is that it's Arm or Arm64.
        /// </summary>
        public readonly bool Is32Or64BitOperatingSystem;
        /// <summary>
        /// To confirm the download or not. If true,
        /// WinPath won't ask for a confirmation to
        /// download
        /// </summary>
        private bool confirmDownload;
        /// <summary>
        /// The link to fetch github releases from.
        /// </summary>
        private const string releases = "https://api.github.com/repos/ANF-Studios/WinPath/releases";
        /// <summary>
        /// The directory to download WinPath from the releases page.
        /// </summary>
        private static readonly string downloadDirectory = $"{Path.GetTempPath()}WinPath\\download\\";
        /// <summary>
        /// The directory <c>WinPath.Updater</c> logs on failure.
        /// </summary>
        private static readonly string logDirectory = Path.Combine(
                                                        Path.GetTempPath(),
                                                        "WinPath\\logs\\log.txt");

        /// <summary>
        /// Default constructor, all overloads are required.
        /// </summary>
        /// <param name="includePrereleases">See <see cref="includePrereleases"/>.</param>
        /// <param name="confirmDownload">See <see cref="confirmDownload"/>.</param>
        /// <param name="is32Or64BitOperatingSystem">See <see cref="Is32Or64BitOperatingSystem"/>.</param>
        public Update(bool includePrereleases, bool confirmDownload, bool is32Or64BitOperatingSystem)
        {
            this.includePrereleases = includePrereleases;
            this.confirmDownload = confirmDownload;
            this.Is32Or64BitOperatingSystem = is32Or64BitOperatingSystem;
        }

        /// <summary>
        /// Tests whether the current user is a member of the Administrator's group.
        /// </summary>
        /// <remarks>
        /// This function is a wrapper for CheckTokenMembership. It is recommended to
        /// call that function directly to determine Administrator group status rather
        /// than calling IsUserAnAdmin.
        /// 
        ///     <list type="table">
        ///         <para>
        ///             <listheader>
        ///                 <term>Minimum supported client</term>
        ///                 <description>Windows XP [desktop apps only]</description>
        ///             </listheader>
        ///         </para>
        ///         
        ///         <para>
        ///             <listheader>
        ///                 <term>Minimum supported server</term>
        ///                 <description>Windows Server 2003 [desktop apps only]</description>
        ///             </listheader>
        ///         </para>
        ///         
        ///         <para>
        ///             <listheader>
        ///                 <term>Target Platform</term>
        ///                 <description>Windows</description>
        ///             </listheader>
        ///         </para>
        ///         
        ///         <para>
        ///             <listheader>
        ///                 <term>Header</term>
        ///                 <description>shlobj_core.h (include Shlobj.h)</description>
        ///             </listheader>
        ///         </para>
        ///         
        ///         <para>
        ///             <listheader>
        ///                 <term>Library</term>
        ///                 <description>Shell32.lib</description>
        ///             </listheader>
        ///         </para>
        ///         
        ///         <para>
        ///             <listheader>
        ///                 <term>DLL</term>
        ///                 <description>Shell32.dll (version 5.0 or later)</description>
        ///             </listheader>
        ///         </para>
        ///         
        ///         <para>
        ///             <listheader>
        ///                 <term>API set</term>
        ///                 <description>ext-ms-win-shell-shell32-l1-2-1 (introduced in Windows 10, version 10.0.10240)</description>
        ///             </listheader>
        ///         </para>
        ///     </list>
        /// 
        /// For more information, visit https://docs.microsoft.com/en-us/windows/win32/api/shlobj_core/nf-shlobj_core-isuseranadmin.
        /// </remarks>
        /// <returns>Returns true if the user is a member of the Administrator's group; otherwise, false.</returns>
        [DllImport("Shell32.dll", SetLastError = true)]
        private static extern bool IsUserAnAdmin();

        /// <summary>
        /// Downloads an installs WinPath.
        /// </summary>
        /// <param name="releaseInfo">General information on the release to fetch.</param>
        /// <param name="finalJob">An optional <see cref="Action"/> to invoke at the end.</param>
        public void DownloadWinPath(in ReleaseInfo releaseInfo, Action finalJob = null)
        {
            bool appveyor = Environment.GetEnvironmentVariable("APPVEYOR", EnvironmentVariableTarget.Process) == "True";
            if (!confirmDownload)
            {
                Console.WriteLine("Release Information:\n"
                    + $"Title: {releaseInfo.ReleaseName}\n"
                    + $"Version: v{releaseInfo.TagName}"
                    + (releaseInfo.IsPrerelease ? " (Prerelease)\n" : "\n")
                    + $"File: {releaseInfo.ReleaseAsset.ExecutableName}\n");
                Console.Write($"Confirm installation of WinPath v{releaseInfo.TagName} (y/n): ");

                if (!appveyor)
                    if ((Console.ReadKey()).Key == ConsoleKey.Y)
                        confirmDownload = true;
                    else
                        Environment.Exit(Environment.ExitCode);
                else
                    return;
            }

            Directory.CreateDirectory(downloadDirectory);

            Console.WriteLine("\nDownloading " + releaseInfo.ReleaseAsset.ExecutableName + "...");
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, "WinPath");
                    webClient.DownloadFile(releaseInfo.ReleaseAsset.DownloadUrl, downloadDirectory + "WinPath.exe");
                    webClient.DownloadFile(releaseInfo.Updater.DownloadUrl, downloadDirectory + "WinPath.Updater.exe");
                }
            }
            catch (WebException exception)
            {
                Console.WriteLine("Couldn't download WinPath due to an error in networking:\n" + exception);
                Environment.Exit(1);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Couldn't download WinPath:\n" + exception.Message);
                Environment.Exit(1);
            }
            finally
            {
                Console.WriteLine("Downloaded WinPath v" + releaseInfo.TagName + "...");
                Console.WriteLine("Installing WinPath...");

                bool administratorPermissions = IsUserAnAdmin();
                int processExitCode = 1; // Default to unsuccessful.

                ProcessStartInfo process = new ProcessStartInfo
                {
                    FileName = downloadDirectory + "WinPath.Updater.exe",
                    Arguments = "launching_from_winpath", // To tell WinPath.Updater that a user isn't launching it.
                    UseShellExecute = !administratorPermissions,
                    Verb = administratorPermissions ? string.Empty : "runas",
                    CreateNoWindow = true
                };
                try
                {
                    Console.WriteLine("Starting update...");
                    if (appveyor)
                    {
                        const string installationPath = "C:\\Program Files\\WinPath\\";
                        Directory.CreateDirectory(installationPath);
                        File.Move(downloadDirectory + "\\WinPath.exe", installationPath + "WinPath.exe");
                        processExitCode = 0;
                    }
                    else
                    {
                        var application = Process.Start(process);
                        application.WaitForExit();
                        processExitCode = application.ExitCode;
                        Console.WriteLine("Installer exited with code: " + application.ExitCode);
                    }
                }
                catch (System.ComponentModel.Win32Exception exception)
                {
                    if (exception.NativeErrorCode == 1223)
                        Console.WriteLine("Could not install WinPath because administrator permissions were not provided!");
                    else
                        Console.WriteLine("Could not install WinPath: " + exception.Message);
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Could not update WinPath: " + exception.Message);
                }
                if (processExitCode == 0) // If application exited successfully.
                {
                    WinPath.Library.UserPath userPath = WinPath.Program.GetUserPath();
                    string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
                    if (!path.EndsWith(";"))
                        Environment.SetEnvironmentVariable("Path", (path += ";"), EnvironmentVariableTarget.User);
                    path = path.Replace("/", "\\");
                    if (Environment.Is64BitOperatingSystem)
                       if (!(path.Contains("%programfiles%\\winpath", StringComparison.CurrentCultureIgnoreCase) || path.Contains("c:\\program files\\winpath", StringComparison.CurrentCultureIgnoreCase)))
                           userPath.AddToPath("%PROGRAMFILES%\\WinPath\\", true, DateTime.Now.ToFileTime().ToString());
                    else
                       if (!(path.Contains("%programfiles(x86)%\\winpath", StringComparison.CurrentCultureIgnoreCase) || path.Contains("c:\\program files (x86)\\winpath", StringComparison.CurrentCultureIgnoreCase)))
                           userPath.AddToPath("%PROGRAMFILES(X86)%\\WinPath\\", true, DateTime.Now.ToFileTime().ToString());
                    Console.WriteLine("[STATUS] Installed WinPath successfully!");
                    Environment.ExitCode = 0;
                }
                else // If not.
                {
                    Console.WriteLine("[STATUS] Could not update WinPath! Please see the log file: " + logDirectory + "log.txt");
                    Environment.ExitCode = 1;
                }
                finalJob?.Invoke();
            }
        }

        /// <summary>
        /// Fetches the releases data file from
        /// GitHub's API servers.
        /// </summary>
        /// <returns>A list of <see cref="Release"/> that contains data on every release.</returns>
        public List<Release> GetReleases()
        {
            string response = String.Empty;
            List<Release> releases;
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, "WinPath");
                    response = webClient.DownloadString(Update.releases);
                    #if DEBUG
                        Console.WriteLine("Response: " + response);
                    #endif
                }

                releases = JsonSerializer.Deserialize<List<Release>>(response);
                return releases;
                //Console.WriteLine(releases[(releases.Count - 1)].Assets[4].DownloadUrl);
            }
            catch (WebException webException)
            {
                Console.WriteLine("Could not make web request!\n" + webException.Message);
                Environment.Exit(exitCode: 1);
            }
            return null;
        }

        /// <summary>
        /// Filters the desired release.
        /// </summary>
        /// <param name="releases">release from filtered from flags.</param>
        /// <returns></returns>
        public Release? FilterRelease(List<Release> releases)
        {
            // TO be removed in v1.0.0.
            if (!this.includePrereleases)
                if (releases.TrueForAll(release => release.IsPrerelease))
                    return null; // Next will be handled by the rest of the code.

            // Reverse the order of the List so that newer releses
            // appear first in the foreach loop.
            if (releases[0].TagName == "0.1.0") // First release.
                releases.Reverse();

            if (releases[0].IsDraft)
                return releases[1];

            foreach (Release release in releases)
            {
                if (release.IsPrerelease && includePrereleases)
                    return release;
                else if (!release.IsPrerelease && !includePrereleases)
                    return release;
            }
            // If by any chance (which shouldn't be) the foreach loop
            // could not do its task, return the default value; the
            // latest release regardless of if it's a prerelease or not.
            // And, return the latest one if it is not a Draft, but if it
            // is, then return the one before it.
            return (!releases[^1].IsDraft) ? releases[^1] : releases[^2];
        }

        /// <summary>
        /// Gets the appropriate WinPath executable from a certain release.
        /// </summary>
        /// <param name="release">The release to filter assets from.</param>
        /// <returns>An <see cref="Asset"/> that contains the release.</returns>
        public Asset GetAssetForProcess(in Release release)
        {
            if (release.Assets.Count < 1)
            {
                Console.WriteLine($"There are no executables in release v{release.TagName}! Exiting...");
                Environment.Exit(1);
            }

            Asset? processedAsset = null;
            string architecture = GetArchitecture(RuntimeInformation.ProcessArchitecture).ToLower();
            release.Assets.ForEach((asset) => {
                if (OperatingSystem.IsWindowsVersionAtLeast(10) && Environment.Is64BitOperatingSystem)
                    if (asset.ExecutableName.Contains("win10-x64"))
                        processedAsset = asset;
                if (asset.ExecutableName.ToLower().Contains(architecture))
                    processedAsset = asset;
            });
            return (Asset)processedAsset;
        }

        /// <summary>
        /// Returns a stringified version of the supplied architecture.
        /// </summary>
        /// <param name="processArchitecture">The architecture to stringify.</param>
        /// <returns>A stringified version of the provided architecture.</returns>
        public static string GetArchitecture(in Architecture processArchitecture)
            => processArchitecture switch
            {
                Architecture.X64 => Architecture.X64.ToString(),
                Architecture.X86 => Architecture.X86.ToString(),
                Architecture.Arm64 => Architecture.Arm64.ToString(),
                Architecture.Arm => Architecture.Arm.ToString(),
                _ => throw new PlatformNotSupportedException("WinPath does not support this architecture!")
            };
    }

    /// <summary>
    /// General information on a release.
    /// </summary>
    public struct ReleaseInfo
    {
        /// <summary>
        /// The name of the release.
        /// </summary>
        public string ReleaseName { get; set; }

        /// <summary>
        /// The tag of the release.
        /// </summary>
        public string TagName { get; set; }
        
        /// <summary>
        /// Whether the release is a prerelease or not.
        /// </summary>
        public bool IsPrerelease { get; set; }

        /// <summary>
        /// Description of the release.
        /// </summary>
        public string ReleaseDescription { get; set; } // To be removed.

        /// <summary>
        /// WinPath (executable) asset of the release.
        /// </summary>
        public Asset ReleaseAsset { get; set; }

        /// <summary>
        /// The updater asset for this release
        /// </summary>
        public Asset Updater { get; set; }
    }

    /// <summary>
    /// Proprerties of a release.
    /// </summary>
    public struct Release
    {
        /// <summary>
        /// The url of the release.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// The url of assets of the release.
        /// </summary>
        [JsonPropertyName("assets_url")]
        public string AssetsUrl { get; set; }

        /// <summary>
        /// The name of the tag of the release.
        /// </summary>
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }

        /// <summary>
        /// The name of the release.
        /// </summary>
        [JsonPropertyName("name")]
        public string ReleaseName { get; set; }

        /// <summary>
        /// Whether the release is a draft or not.
        /// </summary>
        [JsonPropertyName("draft")]
        public bool IsDraft { get; set; }

        /// <summary>
        /// Whether the release is a prerelease or not.
        /// </summary>
        [JsonPropertyName("prerelease")]
        public bool IsPrerelease { get; set; }

        /// <summary>
        /// When the release was published at.
        /// </summary>
        [JsonPropertyName("published_at")]
        public string PublishedAt { get; set; }

        /// <summary>
        /// All the assets of the release.
        /// </summary>
        [JsonPropertyName("assets")]
        public List<Asset> Assets { get; set; }

        /// <summary>
        /// The description of the release.
        /// </summary>
        [JsonPropertyName("body")]
        public string Description { get; set; } // To be removed.
    }

    /// <summary>
    /// A standard asset with a name
    /// and a download url.
    /// </summary>
    public struct Asset
    {
        /// <summary>
        /// The name of the asset.
        /// </summary>
        [JsonPropertyName("name")]
        public string ExecutableName { get; set; }

        /// <summary>
        /// The download link of the asset.
        /// </summary>
        [JsonPropertyName("browser_download_url")]
        public string DownloadUrl { get; set; }
    }
}
