using CommandLine;

namespace WinPath
{
    public class Options
    {
        [Option('v', "value", Default = null, Required = true, HelpText = "The variable to add to the path.")]
        public string Value { get; set; }

        [Option('u', "user", Default = false, Required = false, HelpText = "Add it to the user variables.")]
        public bool AddToUserVariables { get; set; }

        [Option('s', "system", Default = false, Required = false, HelpText = "Add it to the sytem variables.")]
        public bool AddToSystemVariables { get; set; }

        [Option('b', "backup", Default = false, Required = false, HelpText = "Weather to back up the Path variable to be restored if needed.")]
        public bool BackupPathVariable { get; set; }
    }
}
