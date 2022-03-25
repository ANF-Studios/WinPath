using System;
using System.IO;
using System.Linq;
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
        /// Print the current path or a backed up value of it.
        /// </summary>
        /// <param name="options">Configuration for the backup to cutomize parts of it.</param>
        public static void PrintPath(BackupOptions.BackupPrintOptions options)
        {
            if (options.UserPath)
            {
                FileInfo[] userDirFileInfo = new DirectoryInfo(Program.UserPath.BackupDirectory).GetFiles();
                if (!string.IsNullOrEmpty(options.Filename) && userDirFileInfo.Any(backupFile => backupFile.Name == options.Filename))
                {
                    string pathBackup = File.ReadAllText(Path.Combine(Program.UserPath.BackupDirectory, options.Filename));
                    Console.WriteLine("Backup date: " + DateTime.FromFileTime(Convert.ToInt64(options.Filename)));
                    Console.WriteLine(UserPath.GetPathVariable() == pathBackup ? "Up to date with current path" : "Not up to date with current path");
                    Console.WriteLine(pathBackup);
                }
                else
                {
                    Console.WriteLine("Current:");
                    Console.WriteLine(UserPath.GetPathVariable());
                }
            }
            else if (options.SystemPath)
            {
                // To be implemented.
            }
            else
                Console.WriteLine("Neither --user or --system flags were chosen. Exiting...");
        }

        /// <summary>
        /// List backups, behavoir is
        /// defined by arguments.
        /// </summary>
        /// <param name="eventType">The type of the event i.e, type of listing to list.</param>
        /// <param name="userBackupDirectory">The directory of user variable backups.</param>
        /// <param name="systemBackupDirectory">The directory of system variable backups.</param>
        /// <param name="range">The range of backups to display.</param>
        public static void ListBackups(
            in HandleEventType eventType,
            in string userBackupDirectory,
            in string systemBackupDirectory = null,
            in int range = 3
        )
        {
            // The value that defines the location of the desired format
            // in the index of dateTimeObject.GetGetDateTimeFormats();.
            // It strictly looks like: 01-Jan-20 12:00 PM.
            const uint formatIndex = 12;

            DirectoryInfo userDirInfo = new DirectoryInfo(userBackupDirectory);
            DirectoryInfo systemDirInfo = systemBackupDirectory is null
                                            ? null
                                            : new DirectoryInfo(systemBackupDirectory);

            if (!userDirInfo.Exists)
            {
                Console.WriteLine("No backups found...");
                return;
            }
            //if (!systemDirInfo.Exists)
            //{
            // Will be implemented when system backups are supported.
            //}

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

                    Console.WriteLine(separator + Console.Out.NewLine);

                    Console.WriteLine("System Backups:");
                    Console.WriteLine("Filename | Date of creation");
                    Console.WriteLine(separator);

                    /*
                    if (systemDirFileInfo is not null)
                    {
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
                    }
                    else*/
                    Console.WriteLine("System backups not yet supported by the API.");
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

                        Console.WriteLine(separator + Console.Out.NewLine);

                        Console.WriteLine("System Backups:");
                        Console.WriteLine("Filename | Date of creation");
                        Console.WriteLine(separator);

                        /*
                        if (reversedSystemList is not null)
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
                        else*/
                        Console.WriteLine("System backups not yet supported by the API.");

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
            // Seems like this method isn't effected unlike others.
            //if (options.BackupDirectory.Contains("-u") || options.BackupDirectory.Contains("-s"))
            //{
            //    Console.WriteLine("Whoops, seems like there's an error on our end. Please use --user (-u) and --system (-s) flags before --directory (-d).");
            //    return;
            //}

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

            string path = UserPath.GetPathVariable();
            string filename = DateTime.Now.ToFileTime().ToString();
            string finalPath = Path.Combine(options.BackupDirectory, filename);

            if (options.BackupSystemVariables)
                Console.WriteLine("System variables are not supported by the API.");
            else if (options.BackupUserVariables)
                Program.GetUserPath().BackupPath(
                    path,
                    filename,
                    options.BackupDirectory
                );
            if (File.Exists(finalPath))
                Console.WriteLine("Successfully backed up Path at: " + finalPath);
            else
                Console.WriteLine("Looks like something went wrong!");
        }

        /// <summary>
        /// Removed or deletes (permanently) a backup
        /// from the local computer.
        /// </summary>
        /// <param name="options">Configuration for the backup to cutomize parts of it.</param>
        public static void RemoveBackup(in BackupOptions.BackupRemoveOptions options)
        {
            // Seems like this not needed, but we're not sure just yet.
            //if (options.BackupDirectory.Contains("-n"))
            //{
            //    Console.WriteLine("Whoops, seems like there's an issue on our end. Please use the --name (-n) flag before --directory (-d).");
            //    return;
            //}

            // Invalid chars that may be in the provided directory.
            // For example:
            // When using a command argument `--directory "D:\backups\path\"` It takes the `\"` part
            // as a literal escape character which causes the value to be invalid.
            options.BackupDirectory = options.BackupDirectory.Trim(Path.GetInvalidFileNameChars());

            string file = Path.Combine(options.BackupDirectory, options.BackupFilename);
            try
            {
                File.Delete(file);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("`" + file + "` was not found!");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Oh no, there was an error: " + exception.Message);
                Console.WriteLine("Please report to the developer.");
            }

            if (!File.Exists(file))
                Console.WriteLine("Successfully removed backup!");
            else
                Console.WriteLine("Could not remove " + file + "!");
        }

        /// <summary>
        /// Applys a backup from a file to
        /// the <c>Path</c> variable.
        /// </summary>
        /// <param name="options">Configuration for the backup to cutomize parts of it.</param>
        public static void ApplyBackup(in BackupOptions.BackupApplyOptions options)
        {
            // Seems like this not needed, but we're not sure just yet.
            //if (options.BackupDirectory.Contains("-n"))
            //{
            //    Console.WriteLine("Whoops, seems like there's an issue on our end. Please use the --name (-n) flag before --directory (-d).");
            //    return;
            //}

            if (options.RestoreUserVariables && options.RestoreSystemVariables)
            {
                Console.WriteLine("Both user and system variables cannot be restored at the same time (this is to protect you).");
                return;
            }

            if (options.RestoreSystemVariables)
            {
                Console.WriteLine("System variables are not yet supported by the API.");
                return;
            }

            // Invalid chars that may be in the provided directory.
            // For example:
            // When using a command argument `--directory "D:\backups\path\"` It takes the `\"` part
            // as a literal escape character which causes the value to be invalid.
            options.BackupDirectory = options.BackupDirectory.Trim(Path.GetInvalidFileNameChars());

            string file = Path.Combine(options.BackupDirectory, options.BackupFilename);
            string initialUserPath = options.RestoreUserVariables
                                        ? UserPath.GetPathVariable()
                                        : null;
            string initialSystemPath = options.RestoreSystemVariables
                                        ? Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine)
                                        : null;

            if (!File.Exists(file))
            {
                Console.WriteLine(file + " not found! Aborting restore.");
                return;
            }
            else
            {
                string newPath = File.ReadAllText(file);

                if (string.IsNullOrEmpty(newPath))
                {
                    Console.WriteLine("Given file is empty! Aborting restore.");
                    return;
                }

                if (options.RestoreUserVariables)
                {
                    string tempDir = Path.Combine(Path.GetTempPath(), "WinPath");

                    try
                    {
                        if (!Directory.Exists(tempDir))
                            Directory.CreateDirectory(tempDir);
                        File.WriteAllText(userinitialBackup, initialUserPath);
                    }
                    catch (UnauthorizedAccessException) { Console.WriteLine("Whoops, we do not have enough permissions to create a backup before replacing, it's okay though!"); }
                    catch (Exception exception) { Console.WriteLine("There seems to be an error backing up the path before replacing, it's okay though!\nDetails: " + exception.Message); }

                    Environment.SetEnvironmentVariable("Path", newPath, EnvironmentVariableTarget.User);

                    Console.WriteLine("Successfully restored file as new Path!");
                    Console.WriteLine("In case if you changed your mind, there is a backup at: " + userinitialBackup);
                }
                if (options.RestoreSystemVariables)
                {
                    File.WriteAllText(systeminitialBackup, initialSystemPath);

                    Environment.SetEnvironmentVariable("Path", newPath, EnvironmentVariableTarget.Machine);

                    Console.WriteLine("Successfully restored file as new Path!");
                    Console.WriteLine("In case if you changed your mind, there is a backup at: " + systeminitialBackup);
                }
            }
        }
    }
}
