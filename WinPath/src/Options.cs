using CommandLine;

namespace WinPath
{
    [Verb("add", HelpText = "Add a new value to your Path variable.")]
    public class AddOptions
    {
        [Option('v', "value", Default = null, Required = true, HelpText = "The variable to add to the path.")]
        public string Value { get; set; }

        [Option('u', "user", Default = false, Required = false, HelpText = "Add it to the user variables.")]
        public bool AddToUserVariables { get; set; }

        [Option('s', "system", Default = false, Required = false, HelpText = "Add it to the system variables.")]
        public bool AddToSystemVariables { get; set; }

        [Option('b', "backup", Default = false, Required = false, HelpText = "Weather to back up the Path variable to be restored if needed.")]
        public bool BackupPathVariable { get; set; }
    }

    [Verb("update", HelpText = "Update WinPath to the latest version, optioanlly with/without a prerelease.")]
    public class UpdateOptions
    {
        // TODO: Implement the code for it and then uncomment this section.
        //[Option('v', "version", HelpText = "Install a specific version of WinPath.")]
        //public string Version { get; set; }

        [Option('p', "prerelease", Required = false, HelpText = "Weather or not to download a prereleased version.")]
        public bool IncludePrereleases { get; set; }

        [Option('c', "confirm", Required = false, HelpText = "Confirm the installation and directly install without further confirmations.")]
        public bool ConfirmDownload { get; set; }
    }
}
