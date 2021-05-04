using System;
using System.IO;

namespace WinPath.Updater
{
    class Program
    {
        private static readonly string executableDirectory = $"{Path.GetTempPath()}WinPath\\download\\WinPath.exe";

        public static void Main()
        {
            Console.Title = AppDomain.CurrentDomain.FriendlyName;
            try
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    Directory.CreateDirectory(
                        //Environment.GetFolderPath(
                        //    Environment.SpecialFolder.ProgramFiles
                        //)
                        "C:\\Program Files" + "\\WinPath"
                    );
                    File.Move(
                        executableDirectory,
                        //Environment.GetFolderPath(
                        //    Environment.SpecialFolder.ProgramFiles
                        //)
                        "C:\\Program Files" + "\\WinPath\\WinPath.exe",
                        true
                    );
                }
                else
                {
                    Directory.CreateDirectory(
                        //Environment.GetFolderPath(
                        //    Environment.SpecialFolder.ProgramFilesX86
                        //)
                        "C:\\Program Files (x86)" + "\\WinPath"
                    );
                    File.Move(
                        executableDirectory,
                        //Environment.GetFolderPath(
                        //    Environment.SpecialFolder.ProgramFilesX86
                        //)
                        "C:\\Program Files (x86)" + "\\WinPath\\WinPath.exe",
                        true
                    );
                }
                Console.WriteLine("WinPath is installed successfully!");
                Environment.ExitCode = 0;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Could not install WinPath: " + exception.Message);
                Console.ReadKey();
                File.WriteAllText(Directory.GetCurrentDirectory() + "log.txt", "Could not install WinPath: " + exception.Message);
                Environment.ExitCode = 1;
            }
            Environment.Exit(Environment.ExitCode);
        }
    }
}
