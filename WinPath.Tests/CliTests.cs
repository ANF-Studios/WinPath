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
            System.Threading.Tasks.Task.Delay(100);
            Assert.Contains(
                "CliTests_AddToUserPath;", path
            );
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void AddToPathWithNoUserOrSystem()
        {
            Program.Main(new string[] {
                "add",
                "--backup",
                "--value",
                "CliTests_AddToPathWithNoUserOrSystem"
            });
        }
    }
}
