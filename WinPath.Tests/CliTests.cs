using Xunit;
using Xunit.Abstractions;
using System.Runtime.Versioning;

using WinPath;
using WinPath.Library;

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
            /*
            Program.Main(new string[] {
                "add",
                "--user",
                "--backup",
                "--value",
                "CliTests_AddToUserPath"
            });
            //System.Threading.Tasks.Task.Delay(100);
            string path = System.Environment.GetEnvironmentVariable(
                "Path",
                System.EnvironmentVariableTarget.User
            );
            */
            UserPath userPath = new UserPath();
            System.Threading.Tasks.Task.Run(() => userPath.AddToPath("CliTests_AddToUserPath", false, null, true)).Wait();

            string path = UserPath.GetPathVariable();
            output.WriteLine(path);

            bool addedToPath = path.Contains("CliTests_AddToUserPath;");
            Assert.True(addedToPath /*? addedToPath : !addedToPath*/); // Temporary solution to fix this.
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
