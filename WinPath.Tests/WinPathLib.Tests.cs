using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

using WinPath.Library;

namespace WinPath.Tests
{
    public class LibraryTests
    {
        private readonly ITestOutputHelper output;

        public LibraryTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        
        [Fact]
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public void AddToUserPath()
        {
            new UserPath().AddToPath("LibraryTests_AddToUserPath", false);

            string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
            bool isAddedToThePath = path.EndsWith("LibraryTests_AddToUserPath;");
            
            output.WriteLine(isAddedToThePath ? "Variable is added to the path" : "Variable is NOT added to the path");
            Assert.True(isAddedToThePath);
        }

        [Fact]
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public void AddToUserPathWithBackup()
        {
            var userPath = new UserPath();
            userPath.AddToPath("LibraryTests_AddToUserPathWithBackup", true);

            string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
            bool isAddedToThePath = path.EndsWith("LibraryTests_AddToUserPathWithBackup;");
            bool backupExists = File.Exists(userPath.BackupDirectory + userPath.BackupFilename);

            output.WriteLine(isAddedToThePath ? "Variable is added to the path" : "Variable is NOT added to the path");
            output.WriteLine(backupExists ? "Path is backed up" : "Path is NOT backed up");
            output.WriteLine(userPath.BackupDirectory + userPath.BackupFilename);
            Assert.True((isAddedToThePath && backupExists));
        }

        [Fact]
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public void TestMethod()
        {
            var userPath = new UserPath("C:\\backup\\");
            userPath.AddToPath("test", true);
            Assert.True(true);
        }
    }
}
