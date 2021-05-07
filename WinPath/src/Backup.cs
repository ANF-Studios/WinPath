using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WinPath
{
    public class Backup
    {
        public Backup() { }

        public static void ListBackups(
            in HandleEventType eventType,
            in string userBackupDirectory,
            in string systemBackupDirectory = null,
            in int? range = null
        )
        {
            DirectoryInfo userDirInfo = new DirectoryInfo(userBackupDirectory);
            DirectoryInfo systemDirInfo = systemBackupDirectory is null
                                            ? null
                                            : new DirectoryInfo(systemBackupDirectory);

            FileInfo[] userDirFileInfo = userDirInfo.GetFiles();
            FileInfo[] systemDirFileInfo = systemDirInfo?.GetFiles();

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
                    else Console.WriteLine("System backups not yet supported by the API.");
                    Console.WriteLine(seperator);
                    break;

                case HandleEventType.ListLatestBackups:
                    FileInfo[] reversedUserList = userDirFileInfo.Reverse().ToArray();
                    FileInfo[] reversedSystemList = systemDirFileInfo?.Reverse().ToArray();

                    Console.WriteLine("User Backups:");
                    Console.WriteLine("Filename" + spaces + " | Date of creation");
                    Console.WriteLine(seperator);

                    for (int i = 0; i < 3; ++i)
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

                    if (reversedSystemList is not null)
                        for (int i = 0; i < 3; ++i)
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
                    else Console.WriteLine("System backups not yet supported by the API.");

                    Console.WriteLine(seperator);
                    break;

                case HandleEventType.ListBackups:
                    break;

                default:
                    throw new ArgumentException(
                        $"{nameof(HandleEventType)} cannot be set to {nameof(eventType)}.",
                        nameof(eventType)
                    );
            }
        }
    }
}
