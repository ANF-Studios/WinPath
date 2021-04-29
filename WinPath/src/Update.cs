using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace WinPath
{
    public class Update
    {
        private readonly bool includePrereleases;
        public readonly bool is32Or64BitOperatingSystem;
        private bool confirmDownload;
        private const string Releases = "https://api.github.com/repos/ANF-Studios/WinPath/releases";
        private static string downloadDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\WinPath\\temp\\download\\";
        
        public Update(bool includePrereleases, bool confirmDownload, bool is32Or64BitOperatingSystem)
        {
            this.includePrereleases = includePrereleases;
            this.confirmDownload = confirmDownload;
            this.is32Or64BitOperatingSystem = is32Or64BitOperatingSystem;
        }

        [DllImport("Shell32.dll", SetLastError = true)]
        private static extern bool IsUserAnAdmin();

        internal void DownloadWinPath(in ReleaseInfo releaseInfo, Action finalJob = null)
        {
            if (!confirmDownload)
            {
                Console.WriteLine("Release Information:\n"
                    + $"Title: {releaseInfo.ReleaseName}\n"
                    + $"Version: v{releaseInfo.TagName}"
                    + (releaseInfo.IsPrerelease ? " (Prerelease)\n" : "\n")
                    + $"Description: {releaseInfo.ReleaseDescription}\n"
                    + $"File: {releaseInfo.ReleaseAsset.ExecutableName}\n");
                Console.Write($"Confirm installation of WinPath v{releaseInfo.TagName} (y/n): ");
                
                if ((Console.ReadKey()).Key == ConsoleKey.Y)
                    confirmDownload = true;
                else
                    Environment.Exit(Environment.ExitCode);
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
                    UseShellExecute = !administratorPermissions,
                    Verb = administratorPermissions ? string.Empty : "runas",
                    CreateNoWindow = true
                };
                try
                {
                    var application = Process.Start(process);
                    if (application != null)
                    {
                        application.WaitForExit();
                        processExitCode = application.ExitCode;
                        Console.WriteLine(application.ExitCode);
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
                    var userPath = Program.GetUserPath();
                    var path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
                    if (path != null)
                    {
                        path = path.Replace("/", "\\");
                        if (Environment.Is64BitOperatingSystem)
                        {
                            if (!(path.Contains("%programfiles%\\winpath", StringComparison.CurrentCultureIgnoreCase) ||
                                  path.Contains("c:\\program files\\winpath",
                                      StringComparison.CurrentCultureIgnoreCase)))
                                userPath.AddToPath("%PROGRAMFILES%\\WinPath\\", true,
                                    DateTime.Now.ToFileTime().ToString());
                        }
                        else
                        {
                            if (!(path.Contains("%programfiles(x86)%\\winpath",
                                StringComparison.CurrentCultureIgnoreCase) || path.Contains(
                                "c:\\program files (x86)\\winpath", StringComparison.CurrentCultureIgnoreCase)))
                                userPath.AddToPath("%PROGRAMFILES(X86)%\\WinPath\\", true,
                                    DateTime.Now.ToFileTime().ToString());
                            Console.WriteLine("[STATUS] Installed WinPath successfully!");
                        }
                    }

                    Environment.ExitCode = 0;
                }
                else // If not.
                {
                    Console.WriteLine("[STATUS] Could not update WinPath! Please see the log file: " + downloadDirectory + "log.txt");
                    Environment.ExitCode = 1;
                }
                finalJob?.Invoke();
                Environment.Exit(Environment.ExitCode);
            }
        }

        internal List<Release> GetReleases()
        {
            try
            {
                string response;
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, "WinPath");
                    response = webClient.DownloadString(Update.Releases);
                    #if DEBUG
                        Console.WriteLine("Response: " + response);
                    #endif
                }

                return JsonSerializer.Deserialize<List<Release>>(response);
                //Console.WriteLine(releases[(releases.Count - 1)].Assets[4].DownloadUrl);
            }
            catch (WebException webException)
            {
                Console.WriteLine("Could not make web request!\n" + webException.Message);
                Environment.Exit(1);
            }
            return null;
        }

        internal Release FilterRelease(List<Release> releaseList)
        {
            // Reverse the order of the List so that newer releases
            // appear first in the foreach loop.
            releaseList.Reverse();

            foreach (var release in releaseList.Where(release => !release.IsDraft))
            {
                switch (release.IsPrerelease)
                {
                    case true when includePrereleases:
                        return release;
                    case false:
                        return release;
                }
            }
            // If by any chance (which shouldn't be) the foreach loop
            // could not do its task, return the default value; the
            // latest release regardless of if it's a prerelease or not.
            // And, return the latest one if it is not a Draft, but if it
            // is, then return the one before it.
            return (!releaseList[^1].IsDraft) ? releaseList[^1] : releaseList[^2];
        }

        internal Asset GetAssetForProcess(in Release release)
        {
            if (release.Assets.Count < 1)
            {
                Console.WriteLine($"There are no executables in release v{release.TagName}! Exiting...");
                Environment.Exit(1);
            }

            Asset? processedAsset = null;
            var architecture = GetArchitecture(RuntimeInformation.ProcessArchitecture).ToLower();
            release.Assets.ForEach((asset) => {
                if (OperatingSystem.IsWindowsVersionAtLeast(10) && Environment.Is64BitOperatingSystem)
                    if (asset.ExecutableName.Contains("win10-x64"))
                        processedAsset = asset;
                if (asset.ExecutableName.ToLower().Contains(architecture))
                    processedAsset = asset;
            });
            return (Asset) processedAsset;
        }

        private static string GetArchitecture(in Architecture processArchitecture)
            => processArchitecture switch
            {
                Architecture.X64 => Architecture.X64.ToString(),
                Architecture.X86 => Architecture.X86.ToString(),
                Architecture.Arm64 => Architecture.Arm64.ToString(),
                Architecture.Arm => Architecture.Arm.ToString(),
                _ => throw new PlatformNotSupportedException("WinPath does not support this architecture!")
            };
    }

    internal struct ReleaseInfo
    {
        public string ReleaseName { get; set; }

        public string TagName { get; set; }

        public bool IsPrerelease { get; set; }

        public string ReleaseDescription { get; set; }

        public Asset ReleaseAsset { get; set; }

        public Asset Updater { get; set; }
    }

    internal struct Release
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("assets_url")]
        public string AssetsUrl { get; set; }

        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }

        [JsonPropertyName("name")]
        public string ReleaseName { get; set; }

        [JsonPropertyName("draft")]
        public bool IsDraft { get; set; }

        [JsonPropertyName("prerelease")]
        public bool IsPrerelease { get; set; }

        [JsonPropertyName("published_at")]
        public string PublishedAt { get; set; }

        [JsonPropertyName("assets")]
        public List<Asset> Assets { get; set; }

        [JsonPropertyName("body")]
        public string Description { get; set; }
    }

    internal struct Asset
    {
        [JsonPropertyName("name")]
        public string ExecutableName { get; set; }

        [JsonPropertyName("browser_download_url")]
        public string DownloadUrl { get; set; }
    }
}
