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

        [Fact]
        public void WinPathDoesUpdate()
        {
            WinPath.Program.Main(new string[] { "update", "--prerelease", "--confirm" });
            Assert.True(System.IO.File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\WinPath\\temp\\update\\status.txt"));
        }

        [Fact]
        private void WinPathIsInPath()
        {
            var path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
            var isOnPath = path.Contains("%PROGRAMFILES%\\WinPath") || path.Contains("C:\\Program Files\\WinPath");
            output.WriteLine("WinPath is added to the Path: " + isOnPath);
            Assert.True(isOnPath);
        }
    }
}
