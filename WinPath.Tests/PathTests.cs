using System;
using Xunit;
using Xunit.Abstractions;

namespace WinPath.Tests
{
    public class PathTests
    {
        private readonly ITestOutputHelper output;

        public PathTests(ITestOutputHelper output)
        {
            this.output = output;
        }


        [Fact]
        public void AddToUserPath()
        {
            WinPath._Library.UserPath.AddToPath("foo", new AddOptions { AddToUserVariables = true, });
            string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
            bool isAddedToThePath = path.EndsWith("foo;");
            output.WriteLine(isAddedToThePath ? "Variable is added to the path" : "Variable is NOT added to the path");
            Xunit.Assert.True(isAddedToThePath);
        }

        [Fact]
        public void AddToUserPathWithBackup()
        {
            WinPath._Library.UserPath.AddToPath("foo", new AddOptions { AddToUserVariables = true, BackupPathVariable = true });
            string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
            bool isAddedToThePath = path.EndsWith("foo;");
            output.WriteLine(isAddedToThePath ? "Variable is added to the path" : "Variable is NOT added to the path");
            
            Assert.True(isAddedToThePath);
            Assert.True(System.IO.Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\WinPath\\PathBackup\\User\\"));
        }
    }
}
