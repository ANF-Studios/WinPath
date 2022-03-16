using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WinPath.Library
{
    interface IPath
    {
        /// <summary>
        /// The directory to backup the <c>Path</c> variable when creating it.
        /// </summary>
        /// <remarks>
        /// The directory to backup the <c>Path</c> variable when creating it. This variable will be used when
        /// the <see cref="AddToPath(string, bool, string, bool)"/> method is used (instead of
        /// <see cref="AddToPath(string, string, string, bool, bool)"/>). Its value defaults to <c>%APPDATA%\Path\UserBackups\</c>.
        /// </remarks>
        string BackupDirectory { get; set; }

        /// <summary>
        /// The default backup filename. It's used when no filename
        /// is provided in method(s) that use it.
        /// </summary>
        string BackupFilename { get; set; }

        /// <summary>
        /// Adds a new value to the Path, optionally backing up the initial path.
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
        Task AddToPath(string value, bool backup = false, string backupFilename = null, bool force = false);

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
        /// <param name="force">Ignore if path/value is already added and add the given value regardless. By default (false), it will throw an exception.</param>
        Task AddToPath(string value, string backupFilename = null, string backupDirectory = null, bool backup = false, bool force = false);

        /// <summary>
        /// Backups the <c>Path</c> to a file.
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
        Task BackupPath(string path, string filename = null, string backupDirectory = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Accesses the <c>Path</c> value in environment variables.
        /// </summary>
        /// <returns>The <c>Path</c> variable.</returns>
        static Task<string> GetPathVariable()
            => throw new NotImplementedException();

        /// <summary>
        /// Checks whether a given value is already on the path.
        /// </summary>
        /// <param name="value">The value to look for in the path.</param>
        /// <returns>True if it finds the value in the path, false if not.</returns>
        static Task<bool> IsOnPath(string value)
            => throw new NotImplementedException();
    }
}
