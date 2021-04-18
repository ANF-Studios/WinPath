using System;
using System.IO;
using System.Runtime.Versioning;

namespace WinPath.Library
{
    /// <summary>
    /// Contains basic methods to add values to user <c>Path</c>.
    /// </summary>
    [SupportedOSPlatform("windows")]
    [UnsupportedOSPlatform("browser")]
    public class UserPath
    {
        /// <summary>
        /// The directory to backup the <c>Path</c> variable when creating it.
        /// </summary>
        /// <remarks>
        /// The directory to backup the <c>Path</c> variable when creating it. This variable will be used when
        /// the <see cref="AddToPath(string, bool, string)"/> method is used (instead of
        /// <see cref="AddToPath(string, string, string, bool)"/>). Its value defaults to <c>%APPDATA%\Path\UserBackups\</c>.
        /// </remarks>
        public string BackupDirectory { get; set; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Path\\UserBackups\\";

        /// <summary>
        /// The default backup filename. It's used when no filename
        /// is provided in method(s) that use it.
        /// </summary>
        public string BackupFilename { get; set; } = "backup.txt";

        /// <summary>
        /// Default empty constructor.
        /// </summary>
        public UserPath() { }

        /// <summary>
        /// Constructor that takes an override for the <see cref="BackupDirectory"/>.
        /// </summary>
        /// <param name="backupDirectory">This variable will be override <see cref="BackupDirectory"/></param>
        public UserPath(string backupDirectory)
        {
            this.BackupDirectory = backupDirectory;
        }

        /// <summary>
        /// Constructor that takes an override for the <see cref="BackupDirectory"/> and <see cref="BackupFilename"/>.
        /// </summary>
        /// <param name="backupDirectory">This variable will be override <see cref="BackupDirectory"/></param>
        /// <param name="filename"></param>
        public UserPath(string backupDirectory, string filename)
        {
            this.BackupDirectory = backupDirectory;
            this.BackupFilename = filename;
        }

        /// <summary>
        /// Adds a new value to the Path, optionally backing up the initial path.
        /// </summary>
        /// <remarks>
        /// This method takes in a string value which will be added to the <c>Path</c>. It also
        /// optionally takes in a <paramref name="backupFilename"/> which is the name of the file
        /// to backup. For the directory, <see cref="BackupDirectory"/> will be used. If you want
        /// to override the directory, either change <see cref="BackupDirectory"/> or use
        /// <see cref="AddToPath(string, string, string, bool)"/> instead. A backup will not be created
        /// if <paramref name="backup"/> is false.
        /// </remarks>
        /// <param name="value">The content of the new value to be added to the <c>Path</c>.</param>
        /// <param name="backup">Whether to backup the initial <c>Path</c> or not.</param>
        /// <param name="backupFilename">The name of the file to backup, no need to provide it if you use <see cref="BackupFilename"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// Exception is thrown when <paramref name="value"/> is null or empty.
        /// </exception>
        public void AddToPath(string value, bool backup = false, string backupFilename = null)
        {
            if (value != null || value != string.Empty)
            {
                string initialPath = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
                if (backup)
                    BackupPath(initialPath, backupFilename);
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

        /// <summary>
        /// Adds a new value to the Path, optionally backing up the initial path.
        /// </summary>
        /// <remarks>
        /// This method takes in a string value which will be added to the <c>Path</c>. It also optionally
        /// takes in <paramref name="backupFilename"/>, <paramref name="backupDirectory"/> and whether to
        /// <paramref name="backup"/> or not. If you provide the former two, they will be overridden and 
        /// if and when <paramref name="backup"/> is set to true. These two values override <see cref="BackupFilename"/>
        /// and <see cref="BackupDirectory"/>. A backup will not be created if <paramref name="backup"/> is false.
        /// </remarks>
        /// <param name="value">The content of the new value to be added to the <c>Path</c>.</param>
        /// <param name="backupFilename">The name of the file to backup, no need to provide it if you use <see cref="BackupFilename"/>.</param>
        /// <param name="backupDirectory">The directory path to backup to, no need to provide it if you use <see cref="BackupDirectory"/>.</param>
        /// <param name="backup">Whether to backup the initial <c>Path</c> or not.</param>
        /// <exception cref="ArgumentNullException">
        /// Exception is thrown when <paramref name="value"/> is null or empty.
        /// </exception>
        public void AddToPath(string value, string backupFilename = null, string backupDirectory = null, bool backup = false)
        {
            if (value != null || value != string.Empty)
            {
                string initialPath = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
                if (backup)
                    BackupPath(initialPath, backupFilename, backupDirectory);
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

        /// <summary>
        /// Backups the <c>Path</c> to a file.
        /// </summary>
        /// <remarks>
        /// Backups the <c>Path</c> to a file. This method calls <see cref="File.WriteAllText(string, string?, System.Text.Encoding)"/>
        /// providing either the values from the overload, or defaults to <see cref="BackupDirectory"/> and <see cref="BackupFilename"/>.
        /// This method can also be overridden to suit your needs and backup as you like.
        /// </remarks>
        /// <param name="path">The <c>Path</c> variable.</param>
        /// <param name="filename">The name of the file to backup, no need to provide it if you use <see cref="BackupFilename"/>.</param>
        /// <param name="backupDirectory">The directory path to backup to, no need to provide it if you use <see cref="BackupDirectory"/>.</param>
        public virtual void BackupPath(string path, string filename = null, string backupDirectory = null)
            => File.WriteAllText((backupDirectory ?? this.BackupDirectory) + (filename ?? this.BackupFilename), path, System.Text.Encoding.UTF8);
    }
}
