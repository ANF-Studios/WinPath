using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace WinPath.Library
{
    /// <summary>
    /// Contains basic methods to add values to system <c>Path</c>.
    /// </summary>
    [SupportedOSPlatform("windows")]
    [UnsupportedOSPlatform("browser")]
    public class SystemPath : IPath
    {
        /// <summary>
        /// The directory to backup the <c>Path</c> variable when creating it.
        /// </summary>
        /// <remarks>
        /// The directory to backup the <c>Path</c> variable when creating it. This variable will be used when
        /// the <see cref="AddToPath(string, bool, string, bool)"/> method is used (instead of
        /// <see cref="AddToPath(string, string, string, bool, bool)"/>). Its value defaults to <c>%APPDATA%\Path\SystemBackups\</c>.
        /// </remarks>
        public string BackupDirectory { get; set; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Path\\SystemBackups\\";

        /// <summary>
        /// The default backup filename. It's used when no filename
        /// is provided in method(s) that use it.
        /// </summary>
        public string BackupFilename { get; set; } = "backup.txt";

        /// <summary>
        /// Registry path to system environment variables.
        /// </summary>
        private const string EnvironmentVariablesPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
        
        /// <summary>
        /// Default empty constructor.
        /// </summary>
        public SystemPath() { }

        /// <summary>
        /// Constructor that takes an override for the <see cref="BackupDirectory"/>.
        /// </summary>
        /// <param name="backupDirectory">This variable will be override <see cref="BackupDirectory"/></param>
        public SystemPath(string backupDirectory)
        {
            this.BackupDirectory = backupDirectory;
        }

        /// <summary>
        /// Constructor that takes an override for the <see cref="BackupDirectory"/> and <see cref="BackupFilename"/>.
        /// </summary>
        /// <param name="backupDirectory">This variable will be override <see cref="BackupDirectory"/></param>
        /// <param name="filename"></param>
        public SystemPath(string backupDirectory, string filename)
        {
            this.BackupDirectory = backupDirectory;
            this.BackupFilename = filename;
        }

        /// <summary>
        /// Adds a new value to the system Path, optionally backing up the initial path.
        /// </summary>
        /// <remarks>
        /// This method takes in a string value which will be added to the <c>Path</c>. It also
        /// optionally takes in a <paramref name="backupFilename"/> which is the name of the file
        /// to backup. For the directory, <see cref="BackupDirectory"/> will be used. If you want
        /// to override the directory, either change <see cref="BackupDirectory"/> or use
        /// <see cref="AddToPath(string, string, string, bool, bool)"/> instead. A backup will not be created
        /// if <paramref name="backup"/> is false.
        /// </remarks>
        /// <param name="value">The content of the new value to be added to the <c>Path</c>.</param>
        /// <param name="backup">Whether to backup the initial <c>Path</c> or not.</param>
        /// <param name="backupFilename">The name of the file to backup, no need to provide it if you use <see cref="BackupFilename"/>.</param>
        /// <param name="force">Ignore if path/value is already added and add the given value regardless. By default (false), it will throw an exception.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="value"/> is null or empty.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when value already exists in the path. Can be ignored by setting <paramref name="force"/> to true.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown when SetValue permissions are not granted.
        /// </exception>
        public async Task AddToPath(string value, bool backup = false, string backupFilename = null, bool force = false)
        {
            if (value != null || value != string.Empty)
            {
                string initialPath = SystemPath.GetPathVariable();
                if (SystemPath.IsOnPath(value) && force == false)
                    throw new InvalidOperationException("Value is already added to the path.");
                if (backup)
                    await BackupPath(initialPath, backupFilename);
                string path = initialPath.EndsWith(";")     // If the initial path does end with a semicolon,
                    ? (initialPath + value + ";")           // Add the initial path without a semicolon.
                    : (initialPath + ";" + value + ";");    // Otherwise add it to the Path starting with a semicolon.
<<<<<<< HEAD
                Registry.SetValue(EnvironmentVariablesPath, "Path", path, RegistryValueKind.ExpandString);
=======
                Registry.SetValue(EnvironmentVariablesPath, "Path", path);
>>>>>>> Fixed a bug messing up semicolons when adding path variables
            }
            else
                throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Adds a new value to the system Path, optionally backing up the initial path.
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
        /// <param name="force">Ignore if path/value is already added and add the given value regardless. By default (false), it will throw an exception.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="value"/> is null or empty.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when value already exists in the path. Can be ignored by setting <paramref name="force"/> to true.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown when SetValue permissions are not granted.
        /// </exception>
        public async Task AddToPath(string value, string backupFilename = null, string backupDirectory = null, bool backup = false, bool force = false)
        {
            if (value != null || value != string.Empty)
            {
                string initialPath = SystemPath.GetPathVariable();
                if (SystemPath.IsOnPath(value) && force == false)
                    throw new InvalidOperationException("Value is already added to the path.");
                if (backup)
                    await BackupPath(initialPath, backupFilename, backupDirectory);
                string path = initialPath.EndsWith(";")     // If the initial path does end with a semicolon,
                    ? (initialPath + value + ";")           // Add the initial path without a semicolon.
                    : (initialPath + ";" + value + ";");    // Otherwise add it to the Path starting with a semicolon.
                Registry.SetValue(EnvironmentVariablesPath, "Path", path, RegistryValueKind.ExpandString);
            }
            else
                throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Backups the system Path <c>Path</c> to a file.
        /// </summary>
        /// <remarks>
        /// Backups the <c>Path</c> to a file. This method calls <see cref="File.WriteAllTextAsync(string, string?, System.Text.Encoding, CancellationToken)"/>
        /// providing either the values from the overload, or defaults to <see cref="BackupDirectory"/> and <see cref="BackupFilename"/>.
        /// This method can also be overridden to suit your needs and backup as you like.
        /// </remarks>
        /// <param name="path">The <c>Path</c> variable.</param>
        /// <param name="filename">The name of the file to backup, no need to provide it if you use <see cref="BackupFilename"/>.</param>
        /// <param name="backupDirectory">The directory path to backup to, no need to provide it if you use <see cref="BackupDirectory"/>.</param>
        /// <param name="cancellationToken">This method would not do anything if <see cref="CancellationToken.IsCancellationRequested"/> is true.</param>
        public async Task BackupPath(string path, string filename = null, string backupDirectory = null, CancellationToken? cancellationToken = null)
        {
            // If cancellationToken is null i.e, it's not provided, continue -- because
            // IsCancellationRequested is null and hence not true which means this code
            // would never run.
            // However, if IsCancellationRequested is true, and of course, cancellationToken
            // is not null, it would not execute, which is what we want.
            if (cancellationToken == null || !cancellationToken.Value.IsCancellationRequested)
            {
                if (!Directory.Exists(backupDirectory ?? this.BackupDirectory))
                    Directory.CreateDirectory(backupDirectory ?? this.BackupDirectory);
                await File.WriteAllTextAsync((backupDirectory ?? this.BackupDirectory) + (filename ?? this.BackupFilename), path, System.Text.Encoding.UTF8);
            }
        }

        /// <summary>
        /// Accesses the system <c>Path</c> value in environment variables.
        /// </summary>
        /// <returns>The <c>Path</c> variable.</returns>
        static string GetPathVariable()
            => Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine);

        /// <summary>
        /// Checks whether a given value is already on the system path.
        /// </summary>
        /// <param name="value">The value to look for in the path.</param>
        /// <returns>True if it finds the value in the path, false if not.</returns>
        static bool IsOnPath(string value)
            => SystemPath.GetPathVariable().Contains(value);
    }
}
