using System;
using System.IO;

namespace WinPath.Library
{
    public class UserPath
    {
        public string BackupDirectory { get; set; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Path\\UserBackups";
        
        public void AddToPath(string value, bool backup = false)
        {
            if (value != null || value != string.Empty)
            {
                string initialPath = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
                if (backup)
                    BackupPath(initialPath);
                Environment.SetEnvironmentVariable(
                    "Path",
                    (initialPath.EndsWith(";")                 // If the initial path does end with a semicolon,
                        ? (initialPath + value + ";")          // Add the initial path without a semicolon.
                        : (";" + initialPath + value + ";")),  // Otherwise add it to the Path starting with a semicolon.
                    EnvironmentVariableTarget.User
                );
            }
            else
                throw new ArgumentNullException(nameof(value));
        }

        public void AddToPath(string value, string backupDirectory, bool backup = false)
        {
            if (value != null || value != string.Empty)
            {
                string initialPath = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
                if (backup)
                    BackupPath(initialPath, backupDirectory);
                Environment.SetEnvironmentVariable(
                    "Path",
                    (initialPath.EndsWith(";")                 // If the initial path does end with a semicolon,
                        ? (initialPath + value + ";")          // Add the initial path without a semicolon.
                        : (";" + initialPath + value + ";")),  // Otherwise add it to the Path starting with a semicolon.
                    EnvironmentVariableTarget.User
                );
            }
            else
                throw new ArgumentNullException(nameof(value));
        }

        public void BackupPath(string path, string backupDirectory = null)
        {
            // TODO: Implement.
        }
    }
}
