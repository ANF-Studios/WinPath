using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WinPath.Library
{
    public class Update
    {
        private readonly bool includePrereleases;
        private bool confirmDownload;
        private const string releases = "https://api.github.com/repos/ANF-Studios/WinPath/releases";
        private static string downloadDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\WinPath\\temp\\download\\";
        private static string updateStatusFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\WinPath\\temp\\update\\status.txt";

        public Update(bool includePrereleases, bool confirmDownload)
        {
            this.includePrereleases = includePrereleases;
            this.confirmDownload = confirmDownload;
        }

        internal void DownloadWinPath(in ReleaseInfo releaseInfo)
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
                    webClient.DownloadFile(releaseInfo.Updater.DownloadUrl, downloadDirectory + releaseInfo.Updater.ExecutableName);
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
                ProcessStartInfo process = new ProcessStartInfo
                {
                    FileName = downloadDirectory + "WinPath.Updater.exe",
                    UseShellExecute = true,
                    Verb = "runas",
                    CreateNoWindow = true
                };
                try
                {
                    Process.Start(process).WaitForExit();
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
                if (File.Exists(updateStatusFile))
                {
                    Console.WriteLine("[STATUS] Installed WinPath successfully!");
                    Environment.ExitCode = 0;
                }
                else
                {
                    Console.WriteLine("[STATUS] Could not update WinPath!");
                    Environment.ExitCode = 1;
                }
                Environment.Exit(Environment.ExitCode);
            }
        }

        internal List<Release> GetReleases()
        {
            string response = String.Empty;
            List<Release> releases;
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, "WinPath");
                    // FIXME: Change back to its original state.
                    response = webClient.DownloadString(Update.releases);
                    //response = System.IO.File.ReadAllText("C:\\Users\\ANF-Studios\\Desktop\\_.json");
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

        internal Release FilterRelease(List<Release> releases)
        {
            // Reverse the order of the List so that newer releses
            // appear first in the foreach loop.
            releases.Reverse();

            foreach (Release release in releases)
            {
                if (!release.IsDraft)
                    continue;
                if (release.IsPrerelease && includePrereleases)
                    return release;
                else if (!(release.IsPrerelease && includePrereleases))
                    return release;
            }
            // If by any chance (which shouldn't be) the foreach loop
            // could not do its task, return the default value; the
            // latest release regardless of if it's a prerelease or not.
            // And, return the latest one if it is not a Draft, but if it
            // is, then return the one before it.
            return (!releases[^1].IsDraft) ? releases[^1] : releases[^2];
        }

        internal Asset GetAssetForProcess(in Release release)
        {
            if (release.Assets.Count < 1)
            {
                Console.WriteLine($"There are no executables in release v{release.TagName}! Exiting...");
                Environment.Exit(1);
            }

            Asset? processedAsset = null;
            string architecture = GetArchitecture(RuntimeInformation.ProcessArchitecture).ToLower();
            release.Assets.ForEach((asset) => {
                if (IsWindows10() && Environment.Is64BitOperatingSystem)
                    if (asset.ExecutableName.Contains("win10-x64"))
                        processedAsset = asset;
                if (asset.ExecutableName.ToLower().Contains(architecture))
                    processedAsset = asset;
            });
            return (Asset)processedAsset;
        }

        public string GetArchitecture(in Architecture processArchitecture)
            => processArchitecture switch
            {
                Architecture.X64 => Architecture.X64.ToString(),
                Architecture.X86 => Architecture.X86.ToString(),
                Architecture.Arm64 => Architecture.Arm64.ToString(),
                Architecture.Arm => Architecture.Arm.ToString(),
                _ => throw new PlatformNotSupportedException("WinPath does not support this architecture!")
            };

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public static bool IsWindows10()
            => ((string)(Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion")).GetValue("ProductName")).StartsWith("Windows 10");

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
