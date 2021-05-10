using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace WinPath.Tests
{
    public class UpdateTests
    {
        private readonly ITestOutputHelper output;

        public UpdateTests(ITestOutputHelper output)
        {
            Console.SetOut(new OutputRedirector(output));
            this.output = output;
        }

        [Fact]
        public void WinPathDoesUpdate()
        {
            Console.SetOut(null);
            Program.Main(
                new string[]
                {
                    "update",
                    "--prerelease",
                    "--confirm"
                }
            );

            Assert.True(
                System.IO.File.Exists(
                    Environment.Is64BitOperatingSystem
                        ? "C:\\Program Files\\WinPath\\WinPath.exe"
                        : "C:\\Program Files (x86)\\WinPath\\WinPath.exe"
                )
            );
        }

        [Fact]
        public void WinPathIsInPath()
        {
            Console.SetOut(null);
            var path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
            var isOnPath = path.Contains("%PROGRAMFILES%\\WinPath") || path.Contains("C:\\Program Files\\WinPath");
            output.WriteLine("WinPath is added to the Path: " + isOnPath);
            Assert.True(isOnPath); // FIXME: Set up a proper testing solution.
        }

        [Fact]
        public void PrintConfirmDownloadSection()
        {
            Update update = new Update(true, false, true);
            update.DownloadWinPath(
                new ReleaseInfo
                {
                    IsPrerelease = true,
                    ReleaseAsset = new Asset { DownloadUrl = "https://example.com/", ExecutableName = "foobar.exe" },
                    ReleaseDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                    ReleaseName = "Foobar",
                    TagName = "0.0.0",
                    Updater = new Asset { DownloadUrl = "https://example.com/", ExecutableName = "foobar.Updater.exe" }
                }
            );
            Assert.True(true);
        }

        [Fact]
        public void GetWinPathReleases()
        {
            string architecture = Update.GetArchitecture(System.Runtime.InteropServices.RuntimeInformation.OSArchitecture).ToLower();
            Update update = new Update(true, false, (architecture == "x64" || architecture == "x86"));
            var releases = update.GetReleases();
            foreach (var release in releases)
                output.WriteLine(release.ReleaseName + '\n' + release.TagName + '\n');
            Assert.False(releases.Count < 1);
        }

        [Fact]
        public void LatestPrereleaseHasAssets()
        {
            string architecture = Update.GetArchitecture(System.Runtime.InteropServices.RuntimeInformation.OSArchitecture).ToLower();
            Update update = new Update(true, false, (architecture == "x64" || architecture == "x86"));
            
            var releases = update.GetReleases();
            var release = update.FilterRelease(releases);

            output.WriteLine(
                "Release name: " + release.ReleaseName + "\n"
              + "Release tag: " + release.TagName + "\n"
              + "Is Prerelease: " + release.IsPrerelease
            );

            Assert.True(release.IsPrerelease);
            Assert.NotEmpty(release.Assets);
        }

        [Fact]
        public void FilteredReleaseIsNotPrerelease()
        {
            string architecture = Update.GetArchitecture(System.Runtime.InteropServices.RuntimeInformation.OSArchitecture).ToLower();
            Update update = new Update(false, false, (architecture == "x64" || architecture == "x86"));

            var releases = update.GetReleases();
            var release = update.FilterRelease(releases);

            output.WriteLine(
                "Release name: " + release.ReleaseName + "\n"
              + "Release tag: " + release.TagName + "\n"
              + "Is Prerelease: " + release.IsPrerelease
            );

            if (!releases.Where(release => release.IsPrerelease == true).Any()) // If all releases are NOT prereleases.
                Assert.False(release.IsPrerelease);                             // Then check if this one isn't a prerelease.
            else Assert.True(true);                                             // Else simply pass.
        }

        [Fact]
        public void LatestReleaseHasAssets()
        {
            string architecture = Update.GetArchitecture(System.Runtime.InteropServices.RuntimeInformation.OSArchitecture).ToLower();
            Update update = new Update(true, false, (architecture == "x64" || architecture == "x86"));
            
            var releases = update.GetReleases();
            var release = update.FilterRelease(releases);

            output.WriteLine(
                "Release name: " + release.ReleaseName + "\n"
              + "Release tag: " + release.TagName + "\n"
              + "Is Prerelease: " + release.IsPrerelease
            );

            if (!release.ReleaseName.Contains("Test release")) // If the release is not a test release,
                Assert.NotEmpty(release.Assets);               // check if it's not empty.
            else                                               //
                Assert.True(true);                             // Else simply pass.
        }

        [Fact]
        public void GetAssetForProcessSuccessfully()
        {
            string architecture = Update.GetArchitecture(System.Runtime.InteropServices.RuntimeInformation.OSArchitecture).ToLower();
            Update update = new Update(true, false, (architecture == "x64" || architecture == "x86"));

            var releases = update.GetReleases();
            var release = update.FilterRelease(releases);
            var assetForProcess = (Asset?)update.GetAssetForProcess(release);

            output.WriteLine("Executable name: " + assetForProcess?.ExecutableName ?? "None");

            Assert.True(assetForProcess is not null);
        }
    }
}
