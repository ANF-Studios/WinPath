using Xunit;
using Xunit.Abstractions;

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
        public void AddToUserPath()
        {
            Program.Main(new string[] {
                "add",
                "--user",
                "--backup",
                "--value",
                "foobar"
            });
            Assert.True(System.Environment.GetEnvironmentVariable(
                    "Path",
                    System.EnvironmentVariableTarget.User
                ).EndsWith("foobar;"));
        }
    }
}