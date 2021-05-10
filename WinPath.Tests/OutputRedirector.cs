using System;
using System.IO;
using System.Text;

using Xunit.Abstractions;

namespace WinPath.Tests
{
    public class OutputRedirector : TextWriter
    {
        private readonly ITestOutputHelper _output;

        public OutputRedirector(ITestOutputHelper output)
        {
            _output = output;
        }

        public override Encoding Encoding { get; }

        public override void WriteLine(string? value)
        {
            _output.WriteLine(value);
        }
    }
}
