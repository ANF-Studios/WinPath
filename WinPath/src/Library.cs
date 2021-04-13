using System;
using System.IO;

namespace WinPath.Library
{
    public class UserPath
    {
        private static string backupDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\WinPath\\PathBackup\\User\\";

        public static void AddToPath(string value, AddOptions opt = null)
        {
            if (opt != null && opt.BackupPathVariable)
                UserPath.BackupPath(Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User));
            var path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable
            (
                "Path",
                (path.EndsWith(";")
                    ? (path + value + ";")
                    : (";" + path + value + ";")),
                EnvironmentVariableTarget.User
            );
        }

        public static void BackupPath(string pathVariable)
        {
            #if DEBUG
                Console.WriteLine(backupDirectory);
            #endif
            Directory.CreateDirectory(backupDirectory);
            try
            {
                File.WriteAllText($"{backupDirectory}{DateTime.Now.ToFileTime()}", pathVariable);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Could not backup path!\n" + exception.Message);
            }
        }
    }
}
