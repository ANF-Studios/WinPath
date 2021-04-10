using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WinPath.Library
{
    public class Update
    {
        private readonly bool includePrereleases;
        private readonly bool confirmDownload;
        private const string releases = "https://api.github.com/repos/ANF-Studios/WinPath/releases";

        public Update(bool includePrereleases, bool confirmDownload)
        {
            this.includePrereleases = includePrereleases;
            this.confirmDownload = confirmDownload;
        }

        internal void DownloadWinPath(in ReleaseInfo releaseInfo)
        {
            // TODO: Implement.
            if (!confirmDownload)
            {
                Console.WriteLine("Release Information:");
                Console.WriteLine($"Title: {releaseInfo.ReleaseName}");
                Console.WriteLine($"Version: v{releaseInfo.TagName}"
                    + (releaseInfo.IsPrerelease ? " (Prerelease)" : ""));
                Console.WriteLine($"Description: {releaseInfo.ReleaseDescription}");
                Console.WriteLine($"File: {releaseInfo.ReleaseAsset.ExecutableName}");
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
                    //response = webClient.DownloadString(Update.releases);
                    response = System.IO.File.ReadAllText("C:\\Users\\ANF-Studios\\Desktop\\_.json");
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
            return releases[(releases.Count - 1)];
        }

        public string GetArchitecture(in Architecture processArchitecture)
        {
            return processArchitecture switch
            {
                Architecture.X64 => Architecture.X64.ToString(),
                Architecture.X86 => Architecture.X86.ToString(),
                Architecture.Arm64 => Architecture.Arm64.ToString(),
                Architecture.Arm => Architecture.Arm.ToString(),
                _ => throw new PlatformNotSupportedException("WinPath does not support this architecture!")
            };
        }
    }

    internal struct ReleaseInfo
    {
        public string ReleaseName { get; set; }

        public string TagName { get; set; }

        public bool IsPrerelease { get; set; }

        public string ReleaseDescription { get; set; }

        public Asset ReleaseAsset { get; set; }
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
