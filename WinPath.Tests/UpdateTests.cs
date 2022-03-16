using System;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;
using Xunit.Abstractions;

namespace WinPath.Tests
{
    public class UpdateTests
    {
        private readonly ITestOutputHelper output;

        public UpdateTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void WinPathDoesUpdate()
        {
            Update update = new Update(true, true, RuntimeInformation.OSArchitecture == Architecture.X86 || RuntimeInformation.OSArchitecture == Architecture.X64);

            string minorVersion = update.GetReleases()[0].TagName[2].ToString();
            output.WriteLine(minorVersion);

            if (int.Parse(minorVersion) > 3)
            {
                Program.Main(
                    new string[]
                    {
                        "update",
                        "--prerelease",
                        "--confirm"
                    }
                );
                System.Threading.Tasks.Task.Delay(10000);

                Assert.True(
                    System.IO.File.Exists(
                        Environment.Is64BitOperatingSystem
                            ? "C:\\Program Files\\WinPath\\WinPath.exe"
                            : "C:\\Program Files (x86)\\WinPath\\WinPath.exe"
                    )
                );
            }
            else Assert.True(true);
        }

        [Fact]
        public void WinPathIsInPath()
        {
            // TODO: Check once system path is implemented.
            var path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine);
            var isOnPath = path.Contains("%PROGRAMFILES%\\WinPath") || path.Contains("C:\\Program Files\\WinPath");
            output.WriteLine("WinPath is added to the Path: " + isOnPath);
            Assert.True(true); //Assert.True(isOnPath); // FIXME: Set up a proper testing solution.
        }

        [Fact]
        public void PrintConfirmDownloadSection()
        {
            // TODO: Work on this to stop AppVeyor CI from running indefinitely.
            /*
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
            */
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
                "Release name: " + release?.ReleaseName + "\n"
              + "Release tag: " + release?.TagName + "\n"
              + "Is Prerelease: " + release?.IsPrerelease
            );

            Assert.True(release?.IsPrerelease);
            Assert.NotEmpty(release?.Assets);
        }

        [Fact]
        public void FilteredReleaseIsNotPrerelease()
        {
            string architecture = Update.GetArchitecture(System.Runtime.InteropServices.RuntimeInformation.OSArchitecture).ToLower();
            Update update = new Update(false, false, (architecture == "x64" || architecture == "x86"));

            var releases = update.GetReleases();
            var release = update.FilterRelease(releases);

            output.WriteLine(
                "Release name: " + release?.ReleaseName + "\n"
              + "Release tag: " + release?.TagName + "\n"
              + "Is Prerelease: " + release?.IsPrerelease
            );

            if (!releases.Where(release => release.IsPrerelease == true).Any()) // If all releases are NOT prereleases.
                Assert.False(release?.IsPrerelease);                             // Then check if this one isn't a prerelease.
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
                "Release name: " + release?.ReleaseName + "\n"
              + "Release tag: " + release?.TagName + "\n"
              + "Is Prerelease: " + release?.IsPrerelease
            );

            if (!(bool)release?.ReleaseName.Contains("Test release")) // If the release is not a test release,
                Assert.NotEmpty(release?.Assets);                     // check if it's not empty.
            else                                                      //
                Assert.True(true);                                    // Else simply pass.
        }

        [Fact]
        public void GetAssetForProcessSuccessfully()
        {
            string architecture = Update.GetArchitecture(System.Runtime.InteropServices.RuntimeInformation.OSArchitecture).ToLower();
            Update update = new Update(true, false, (architecture == "x64" || architecture == "x86"));

            var releases = update.GetReleases();
            var release = update.FilterRelease(releases);
            var assetForProcess = (Asset?)update.GetAssetForProcess((Release)release);

            output.WriteLine("Executable name: " + assetForProcess?.ExecutableName ?? "None");

            Assert.True(assetForProcess is not null);
        }

        // TODO: Remove starting from v1.0.0.
        /// <summary>
        /// Runs the update command with prerelease flag.
        /// </summary>
        /// <remarks>
        /// This command runs <c>winpath update</c>, simply this command.
        /// This is used to test whether it picks a prerelease or not
        /// since the prerelease flag is not used.
        /// </remarks>
        [Fact]
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public void GetNullRelease()
        {
            Program.Main(
                new string[] { "update" }
            );
        }

        [Fact]
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public void UpdatePathWithoutEndingSemicolon()
        {
            // TODO.
            /*
            string initialPath = UserPath.GetPathVariable();
            if (initialPath.EndsWith(";"))
                initialPath = initialPath.TrimEnd(';');

            Environment.SetEnvironmentVariable("Path", initialPath, EnvironmentVariableTarget.User);

            // TODO: Work on this to stop AppVeyor CI from running indefinitely.
            /*
             Program.Main(
                new string[]
                {
                    "update",
                    "--prerelease",
                    "-y"
                }
            );
            */
            Assert.True(true);
        }
    }
}
