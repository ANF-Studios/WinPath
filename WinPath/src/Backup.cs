using System;
using System.IO;
using System.Linq;

namespace WinPath
{
    public class Backup
    {
        public Backup() { }

        public static void ListBackups(
            in HandleEventType eventType,
            in string userBackupDirectory,
            in string systemBackupDirectory = null,
            in int range = 3
        )
        {
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
            string seperator = "-----------------------------------------"; // Default seperator.
            string spaces = string.Empty; // The number of spaces between `Filename` and `|`.

            if (userDirFileInfo.Length > 0)
            {
                seperator = string.Empty;
                for (int i = -2; i < (userDirFileInfo[0].Name + " | " + long.Parse(userDirFileInfo[0].Name)).Length; ++i)
                {
                    seperator += '-';
                }

                for (int i = 0; i < userDirFileInfo[0].Name.Length - "Filename".Length; ++i)
                    spaces += " ";
            }

            switch (eventType)
            {
                case HandleEventType.ListAllBackups:
                    Console.WriteLine("User Backups:");
                    Console.WriteLine("Filename" + spaces + " | Date of creation");
                    Console.WriteLine(seperator);

                    foreach (FileInfo backupFile in userDirFileInfo)
                        Console.WriteLine(
                            backupFile.Name
                                + " | "
                                + (
                                    long.TryParse(
                                        backupFile.Name,
                                        out temp
                                    )
                                        ? DateTime.FromFileTime(temp)
                                        : "<Parsing error>"
                                  )
                        );

                    Console.WriteLine(seperator + Console.Out.NewLine);

                    Console.WriteLine("System Backups:");
                    Console.WriteLine("Filename | Date of creation");
                    Console.WriteLine(seperator);

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
                                            ? DateTime.FromFileTime(temp)
                                            : "<Parsing error>"
                                      )
                            );
                    }
                    else*/ Console.WriteLine("System backups not yet supported by the API.");
                    Console.WriteLine(seperator);
                    break;

                case HandleEventType.ListBackups:
                case HandleEventType.ListLatestBackups:
                    if (range < BackupOptions.BackupListOptions.MinimumRange
                            || range > BackupOptions.BackupListOptions.MaximumRange)
                        Console.WriteLine($"Error: Range cannot be greater than "
                                            + BackupOptions.BackupListOptions.MaximumRange
                                            + " or less than "
                                            + BackupOptions.BackupListOptions.MinimumRange
                                            + $" (current: {range})");
                    else
                    {
                        Console.WriteLine("User Backups:");
                        Console.WriteLine("Filename" + spaces + " | Date of creation");
                        Console.WriteLine(seperator);

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
                                                ? DateTime.FromFileTime(temp)
                                                : "<Parsing error>"
                                          )
                                );
                            }
                            catch (IndexOutOfRangeException) { break; }
                        }

                        Console.WriteLine(seperator + Console.Out.NewLine);

                        Console.WriteLine("System Backups:");
                        Console.WriteLine("Filename | Date of creation");
                        Console.WriteLine(seperator);

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
                                                    ? DateTime.FromFileTime(temp)
                                                    : "<Parsing error>"
                                              )
                                    );
                                }
                                catch (IndexOutOfRangeException) { break; }
                            }
                        else*/
                        Console.WriteLine("System backups not yet supported by the API.");

                        Console.WriteLine(seperator);
                    }
                    break;

                default:
                    throw new ArgumentException(
                        $"{nameof(HandleEventType)} cannot be set to {nameof(eventType)}.",
                        nameof(eventType)
                    );
            }
        }
    
        public static void CreateBackup(in BackupOptions.BackupCreateOptions options)
        {
            // Invalid chars that may be in the provided directory.
            // For example:
            // When using a command argument `--directory "D:\backups\path\"` It takes the `\"` part
            // as a literal escape character which causes the Path to fail backing up.
            options.BackupDirectory = options.BackupDirectory.Trim(Path.GetInvalidFileNameChars());
            if (!options.BackupDirectory.EndsWith("\\") || !options.BackupDirectory.EndsWith("/"))
                options.BackupDirectory += "\\";

            string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
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
            else Console.WriteLine("Looks like something went wrong!");
        }
    }
}
