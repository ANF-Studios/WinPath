using System;
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

        //[Fact]
        //public void WinPathDoesUpdate()
        //{
        //    WinPath.Program.Main(new string[] { "update", "--prerelease", "--confirm" });
        //    Assert.True(System.IO.File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\WinPath\\temp\\update\\status.txt"));
        //}

        [Fact]
        public void WinPathIsInPath()
        {
            var path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
            var isOnPath = path.Contains("%PROGRAMFILES%\\WinPath") || path.Contains("C:\\Program Files\\WinPath");
            output.WriteLine("WinPath is added to the Path: " + isOnPath);
            Assert.False(isOnPath); // FIXME: Set up a proper testing solution.
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
    }
}
