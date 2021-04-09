using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace WinPath.Library
{
    public class Update
    {
        private readonly bool includePrereleases;
        private const string releases = "https://api.github.com/repos/ANF-Studios/WinPath/releases";

        public Update(bool includePrereleases)
        {
            this.includePrereleases = includePrereleases;
        }

        public void GetReleases()
        {
            string response = String.Empty;
            List<Release> releases;
            try
            {
                // TODO: Make a web request and download the appropriate arch, refer to:
                // https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.architecture

                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, "WinPath");
                    response = webClient.DownloadString("https://api.github.com/repos/ANF-Studios/WinPath/releases");
                    #if DEBUG
                        Console.WriteLine("Response: " + response);
                    #endif
                }

                releases = JsonSerializer.Deserialize<List<Release>>(response);
                Console.WriteLine(releases[(releases.Count - 1)].Assets[4].DownloadUrl);
            }
            catch (WebException webException)
            {
                Console.WriteLine("Could not make web request!\n" + webException.Message);
                Environment.Exit(exitCode: 1);
            }
        }
    }

    internal struct Release
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("assets_url")]
        public string AssetsUrl { get; set; }

        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }

        [JsonPropertyName("draft")]
        public bool IsDraft { get; set; }

        [JsonPropertyName("prerelease")]
        public bool IsPrerelease { get; set; }

        [JsonPropertyName("published_at")]
        public string PublishedAt { get; set; }

        [JsonPropertyName("assets")]
        public List<Assets> Assets { get; set; }

        [JsonPropertyName("body")]
        public string Description { get; set; }
    }

    internal struct Assets
    {
        [JsonPropertyName("name")]
        public string ExecutableName { get; set; }

        [JsonPropertyName("browser_download_url")]
        public string DownloadUrl { get; set; }
    }
}
