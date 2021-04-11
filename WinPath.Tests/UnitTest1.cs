using System;
using Xunit;
using Xunit.Abstractions;

namespace WinPath.Tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }


        [Fact]
        public void Test1()
        {
            output.WriteLine("Hello World!");
            WinPath.Library.UserPath.AddToPath("foo", new Options { AddToUserVariables = true, });
            string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
            Xunit.Assert.True(path.EndsWith("foo;"));
        }
    }
}
