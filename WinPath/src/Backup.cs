using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using WinPath.Library;

namespace WinPath
{
    /// <summary>
    /// The main class handler for the
    /// backup command's sub commands.
    /// </summary>
    public class Backup
    {
        /// <summary>
        /// The directory of the initial user backup.
        /// </summary>
        private readonly static string userinitialBackup = Path.Combine(Path.GetTempPath(), "WinPath\\u_backup.txt");
        /// <summary>
        /// The directory of the initial system backup.
        /// </summary>
        private readonly static string systeminitialBackup = Path.Combine(Path.GetTempPath(), "WinPath\\s_backup.txt");

        /// <summary>
        /// Default empty constructor.
        /// </summary>
        public Backup() { }

        /// <summary>
        /// List backups. Count of backups to display can be altered.
        /// </summary>
        /// <param name="eventType">The type of the event i.e, type of listing to list.</param>
        /// <param name="userBackupDirectory">The directory of user variable backups.</param>
        /// <param name="systemBackupDirectory">The directory of system variable backups.</param>
        /// <param name="range">The range of backups to display.</param>
        public static void ListBackups(
            in HandleEventType eventType,
            in string userBackupDirectory,
            in string systemBackupDirectory,
            in int range = 3
        )
        {
            // The value that defines the location of the desired format
            // in the index of dateTimeObject.GetGetDateTimeFormats();.
            // It strictly looks like: 01-Jan-20 12:00 PM.
            const uint formatIndex = 12;

            DirectoryInfo userDirInfo = new DirectoryInfo(userBackupDirectory);
            DirectoryInfo systemDirInfo = new DirectoryInfo(systemBackupDirectory);

            FileInfo[] userDirFileInfo = userDirInfo.GetFiles();
            FileInfo[] systemDirFileInfo = systemDirInfo?.GetFiles();

            FileInfo[] reversedUserList = userDirFileInfo.Reverse().ToArray();
            FileInfo[] reversedSystemList = systemDirFileInfo?.Reverse().ToArray();

            long temp; // For long.TryParse(..., out temp);
            string separator = "-----------------------------------------"; // Default separator.
            string spaces = string.Empty; // The number of spaces between `Filename` and `|`.

            //var now = DateTime.Now;
            //for (int i = 0; i < now.GetDateTimeFormats().Length; ++i)
            //    Console.WriteLine(i + " " + now.GetDateTimeFormats()[i]);

            if (userDirFileInfo.Length > 0)
            {
                separator = string.Empty;
                for (int i = 0; i < (userDirFileInfo[0].Name + " | " + DateTime.FromFileTime(long.Parse(userDirFileInfo[0].Name)).GetDateTimeFormats()[formatIndex]).Length; ++i)
                    separator += '-';

                for (int i = 0; i < userDirFileInfo[0].Name.Length - "Filename".Length; ++i)
                    spaces += " ";
            }

            switch (eventType)
            {
                case HandleEventType.ListAllBackups:
                    Console.WriteLine("User Backups:");
                    Console.WriteLine("Filename" + spaces + " | Date of creation");
                    Console.WriteLine(separator);

                    if (userDirFileInfo.Length > 0)
                        foreach (FileInfo backupFile in userDirFileInfo)
                            Console.WriteLine(
                                backupFile.Name
                                    + " | "
                                    + (
                                        long.TryParse(
                                            backupFile.Name,
                                            out temp
                                        )
                                            ? DateTime.FromFileTime(temp).GetDateTimeFormats()[formatIndex]
                                            : "<Parsing error>"
                                      )
                            );
                    else
                        Console.WriteLine("No backups found");

                    Console.WriteLine(separator + Console.Out.NewLine);

                    Console.WriteLine("System Backups:");
                    Console.WriteLine("Filename" + spaces + " | Date of creation");
                    Console.WriteLine(separator);

                    if (systemDirFileInfo.Length > 0)
                        foreach (FileInfo backupFile in systemDirFileInfo)
                            Console.WriteLine(
                                backupFile.Name
                                    + " | "
                                    + (
                                        long.TryParse(
                                            backupFile.Name,
                                            out temp
                                        )
                                            ? DateTime.FromFileTime(temp).GetDateTimeFormats()[formatIndex]
                                            : "<Parsing error>"
                                      )
                            );
                    else
                        Console.WriteLine("No backups found");
                    Console.WriteLine(separator);
                    break;

                case HandleEventType.ListBackups:
                case HandleEventType.ListLatestBackups:
                    if (range < BackupOptions.BackupListOptions.MinimumRange
                            || range > BackupOptions.BackupListOptions.MaximumRange)
                        Console.WriteLine($"Range cannot be greater than "
                                            + BackupOptions.BackupListOptions.MaximumRange
                                            + " or less than "
                                            + BackupOptions.BackupListOptions.MinimumRange
                                            + $" (current: {range})");
                    else
                    {
                        Console.WriteLine("User Backups:");
                        Console.WriteLine("Filename" + spaces + " | Date of creation");
                        Console.WriteLine(separator);

                        if (reversedUserList.Length > 0)
                            for (int i = 0; i < range; ++i)
                            {
                                try
                                {
                                    Console.WriteLine(
                                        reversedUserList[i].Name
                                            + " | "
                                            + (
                                                long.TryParse(
                                                    reversedUserList[i].Name,
                                                    out temp
                                                )
                                                    ? DateTime.FromFileTime(temp).GetDateTimeFormats()[formatIndex]
                                                    : "<Parsing error>"
                                              )
                                    );
                                }
                                catch (IndexOutOfRangeException) { break; }
                            }
                        else
                            Console.WriteLine("No backups found");

                        Console.WriteLine(separator + Console.Out.NewLine);

                        Console.WriteLine("System Backups:");
                        Console.WriteLine("Filename" + spaces + " | Date of creation");
                        Console.WriteLine(separator);

                        if (reversedSystemList.Length > 0)
                            for (int i = 0; i < range; ++i)
                            {
                                try
                                {
                                    Console.WriteLine(
                                        reversedSystemList[i].Name
                                            + " | "
                                            + (
                                                long.TryParse(
                                                    reversedSystemList[i].Name,
                                                    out temp
                                                )
                                                    ? DateTime.FromFileTime(temp).GetDateTimeFormats()[formatIndex]
                                                    : "<Parsing error>"
                                              )
                                    );
                                }
                                catch (IndexOutOfRangeException) { break; }
                            }
                        else
                            Console.WriteLine("No backups found");
                        Console.WriteLine(separator);
                    }
                    break;

                default:
                    throw new ArgumentException(
                        $"{nameof(HandleEventType)} cannot be set to {nameof(eventType)}.",
                        nameof(eventType)
                    );
            }
        }

        /// <summary>
        /// Creates a backup of the <c>Path</c>
        /// variable in a text file.
        /// </summary>
        /// <param name="options">Configuration for the backup to cutomize parts of it.</param>
        public static void CreateBackup(in BackupOptions.BackupCreateOptions options)
        {
            if (!options.BackupUserVariables && !options.BackupSystemVariables)
            {
                Console.WriteLine("Did not modify any content because neither user or system flag is provided, exiting...");
                return;
            }

            // Invalid chars that may be in the provided directory.
            // For example:
            // When using a command argument `--directory "D:\backups\path\"` It takes the `\"` part
            // as a literal escape character which causes the value to be invalid.
            options.BackupDirectory = options.BackupDirectory.Trim(Path.GetInvalidFileNameChars());
            if (!options.BackupDirectory.EndsWith("\\") || !options.BackupDirectory.EndsWith("/"))
                options.BackupDirectory += "\\";

            string filename = DateTime.Now.ToFileTime().ToString();
            if (!Path.IsPathFullyQualified(options.BackupDirectory))
                options.BackupDirectory = null;

            if (options.BackupSystemVariables)
                _ = Program.GetSystemPath().BackupPath(
                    SystemPath.GetPathVariable(),
                    options.BackupDirectory == null ? filename : "s_" + filename,
                    options.BackupDirectory ?? Program.GetSystemPath().BackupDirectory
                );
            if (options.BackupUserVariables)
                _ = Program.GetUserPath().BackupPath(
                    UserPath.GetPathVariable(),
                    options.BackupDirectory == null ? filename : "u_" + filename,
                    options.BackupDirectory ?? Program.GetUserPath().BackupDirectory
                );
            Console.WriteLine("Path should be backed up now!");
        }

        /// <summary>
        /// Removed or deletes (permanently) a backup
        /// from the local computer.
        /// </summary>
        /// <param name="options">Configuration for the backup to cutomize parts of it.</param>
        public static void RemoveBackup(in BackupOptions.BackupRemoveOptions options)
        {
            string file = (options.UserBackup ? Program.GetUserPath().BackupDirectory : Program.GetSystemPath().BackupDirectory) + options.BackupFilename;
            
            if (!File.Exists(file))
            {
                Console.WriteLine("Backup doesn't exist!");
                return;
            }

            try
            {
                File.Delete(file);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Oh no, there was an error: " + exception.Message);
                Console.WriteLine("Please report to the developer.");
            }

            if (!File.Exists(file))
                Console.WriteLine("Successfully removed backup!");
        }

        /// <summary>
        /// Applys a backup from a file to
        /// the <c>Path</c> variable.
        /// </summary>
        /// <param name="options">Configuration for the backup to cutomize parts of it.</param>
        public static void ApplyBackup(in BackupOptions.BackupApplyOptions options)
        {
            string file = (options.UserBackup ? Program.GetUserPath().BackupDirectory : Program.GetSystemPath().BackupDirectory) + options.BackupFilename;
            if (!File.Exists(file))
            {
                Console.WriteLine(file + " not found! Aborting restore.");
                return;
            }
            else
            {
                string newPath = File.ReadAllText(file);

                if (string.IsNullOrEmpty(newPath) || string.IsNullOrWhiteSpace(newPath))
                {
                    Console.WriteLine("Given file is empty! Aborting restore.");
                    return;
                }

                if (options.UserBackup)
                {
                    Backup.CreateBackup(new BackupOptions.BackupCreateOptions { BackupUserVariables = true });
                    Environment.SetEnvironmentVariable("Path", newPath, EnvironmentVariableTarget.User);

                    Console.WriteLine("Successfully applied file as new Path!");
                    Console.WriteLine("Path has been backed up regardless of success before replacing it.");
                }
                else
                {
                    Backup.CreateBackup(new BackupOptions.BackupCreateOptions { BackupSystemVariables = true });

                    bool success;
                    try
                    {
                        Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment", "Path", newPath, RegistryValueKind.ExpandString);
                        success = true;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("Failed to update system path: " + exception.Message);
                        success = false;
                    }

                    if (success == true)
                        Console.WriteLine("Successfully applied file as new Path!");
                    Console.WriteLine("Path has been backed up regardless of success before replacing it.");
                }
            }
        }
    }
}
