using System;
using System.IO;

namespace WinPath.Updater
{
    class Program
    {
        private static readonly string executableDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\WinPath\\temp\\download\\WinPath.exe";
        private static readonly string updateStatusFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\WinPath\\temp\\update\\status.txt";

        public static void Main()
        {
            try
            {
                if (Environment.Is64BitOperatingSystem)
                    File.Move(
                        executableDirectory,
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.ProgramFiles
                        )
                        + "\\WinPath\\WinPath.exe",
                        true
                    );
                else
                    File.Move(
                        executableDirectory,
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.ProgramFilesX86
                        )
                        + "\\WinPath\\WinPath.exe",
                        true
                    );
                Directory.CreateDirectory(updateStatusFile.Trim("status.txt".ToCharArray()));
                File.WriteAllText(updateStatusFile, string.Empty);
                Console.WriteLine("WinPath is installed successfully!");
                Environment.ExitCode = 0;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Could not install WinPath: " + exception.Message);
                File.WriteAllText("log.txt", "Could not install WinPath: " + exception.Message);
                Environment.ExitCode = 1;
            }
            Environment.Exit(Environment.ExitCode);
        }
    }
}
