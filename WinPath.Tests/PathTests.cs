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
            WinPath.Library.UserPath.AddToPath("foo", new Options { AddToUserVariables = true, });
            string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
            bool isAddedToThePath = path.EndsWith("foo;");
            output.WriteLine(isAddedToThePath ? "Variable is added to the path" : "Variable is NOT added to the path");
            Xunit.Assert.True(isAddedToThePath);
        }
    }
}
