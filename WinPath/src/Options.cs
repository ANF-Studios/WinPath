using CommandLine;
using WinPath.Extensions;

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

    [Verb("backup", HelpText = "A group of options to manage backups of your Path variable.")]
    [ChildVerbs(typeof(BackupListOptions), typeof(BackupApplyOptions), typeof(BackupCreateOptions), typeof(BackupRemoveOptions))]
    public class BackupOptions
    {
        [Verb("list", HelpText = "Display a list of backups.")]
        public class BackupListOptions
        {
            [Option("all", HelpText = "Print all the backups")]
            public bool ListAllBackups { get; set; }

            [Option("latest", HelpText = "Print the latest backup, along with the previous two")]
            public bool ListLatest { get; set; }

            [Option("range", /* Min = 1, Max = int.MaxValue, */ Default = 10, HelpText = "Print a specific range of values starting from the latest to the minimum of that range.")]
            public int Range { get; set; }                     // Providing Min and Max values throw an exception at
                                                               // ParserVerbExtensions.ParseVerbs(Parser, IEnumerable<string>, Type[]). If
            public const int MinimumRange = 1;                 // you find a solution to this, please open a pull request, it'll help a lot.
            public const int MaximumRange = int.MaxValue - 1;  // For now, however, these are the ways to declare the minimum and maxmimum values.
        }

        [Verb("apply", HelpText = "Apply a path value from a backup.")]
        public class BackupApplyOptions
        {
            [Option('n', "name", HelpText = "The ilename of the backup file.", Required = true)]
            public string BackupFilename { get; set; }

            [Option('u', "user", HelpText = "Search user path? True if user, false if system.")]
            public bool UserBackup { get; set; } = false;
        }

        [Verb("create", HelpText = "Create a new backup of your Path variable.")]
        public class BackupCreateOptions
        {
            [Option('u', "user", Default = false, Required = false, HelpText = "Backup user variables.")]
            public bool BackupUserVariables { get; set; }
            
            [Option('s', "system", Default = false, Required = false, HelpText = "Backup system variables.")]
            public bool BackupSystemVariables { get; set; }

            [Option('d', "directory", HelpText = "An override of the directory to backup to. Note that the backup list method might not locate this backup.")]
            public string BackupDirectory { get; set; } = string.Empty;

            // TODO: Might or might not implement it.
            //[Option("encrypt", HelpText = "Encrypt the backup of Path, only possible when --directory is provided.")]
            //public bool EncryptBackup { get; set; }
        }

        [Verb("remove", HelpText = "Deletes a backup from the computer permanently.")]
        public class BackupRemoveOptions
        {
            [Option('n', "name", HelpText = "The filename of the backup file.", Required = true)]
            public string BackupFilename { get; set; }

            [Option('u', "user", HelpText = "Search user path? True if user, false if system.")]
            public bool UserBackup { get; set; } = false;
        }
    }

    [Verb("update", HelpText = "Update WinPath to the latest version, optioanlly with/without a prerelease. This command can also be used to freshly install, update, and reinstall WinPath.")]
    public class UpdateOptions
    {
        // TODO: Implement the code for it and then uncomment this section.
        //[Option('v', "version", HelpText = "Install a specific version of WinPath.")]
        //public string Version { get; set; }

        [Option('p', "prerelease", Required = false, HelpText = "Whether or not to download a prereleased version.")]
        public bool IncludePrereleases { get; set; }

        [Option('y', "confirm", Required = false, HelpText = "Confirm the installation and directly install without further confirmations.")]
        public bool ConfirmDownload { get; set; }
    }
}
