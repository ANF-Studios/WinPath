using System;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Versioning;
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
        [SupportedOSPlatform("windows")]
        public void AddToUserPath()
        {
            new UserPath().AddToPath("LibraryTests_AddToUserPath", false);

            Task.Delay(1000);

            string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
            bool isAddedToThePath = path.Contains("LibraryTests_AddToUserPath;");
            
            output.WriteLine(isAddedToThePath ? "Variable is added to the path" : "Variable is NOT added to the path");
            Assert.True(isAddedToThePath);
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void AddToUserPathWithBackup()
        {
            var userPath = new UserPath();
            userPath.AddToPath("LibraryTests_AddToUserPathWithBackup", true);

            Task.Delay(1000);            

            string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
            bool isAddedToThePath = path.Contains("LibraryTests_AddToUserPathWithBackup;");
            bool backupExists = File.Exists(userPath.BackupDirectory + userPath.BackupFilename);

            output.WriteLine(isAddedToThePath ? "Variable is added to the path" : "Variable is NOT added to the path");
            output.WriteLine(backupExists ? "Path is backed up" : "Path is NOT backed up");
            output.WriteLine(userPath.BackupDirectory + userPath.BackupFilename);
            Assert.True((isAddedToThePath && backupExists));
        }

        [Fact]
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public void ChangeBackupDirectory()
        {
            const string backupDir = "C:\\backup\\";
            var userPath = new UserPath(backupDir);
            Assert.True(userPath.BackupDirectory == backupDir);
        }

        [Fact]
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public void ChangeBackupFilenameAndDirectory()
        {
            const string backupDir = "C:\\backup\\";
            const string backupFilename = "backupFile.txt";
            var userPath = new UserPath(backupDir, backupFilename);
            Assert.True((userPath.BackupDirectory == backupDir) && (userPath.BackupFilename == backupFilename));
        }
    }
}
