using Xunit;
using Xunit.Abstractions;
using System.Runtime.Versioning;

using WinPath;

namespace WinPath.Tests
{
    public class CliTests
    {
        private readonly ITestOutputHelper output;

        public CliTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void AddToUserPath()
        {
            Program.Main(new string[] {
                "add",
                "--user",
                "--backup",
                "--value",
                "CliTests_AddToUserPath"
            });
            string path = System.Environment.GetEnvironmentVariable(
                "Path",
                System.EnvironmentVariableTarget.User
            );
            output.WriteLine(path);
            Assert.Contains(
                "CliTests_AddToUserPath;", path
            );
        }
    }
}
