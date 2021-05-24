using System;
using System.IO;

namespace WinPath.Updater
{
    class Program
    {
        private static readonly string executableDirectory = Path.Combine(
                                                                Path.GetTempPath(),
                                                                "WinPath\\download\\WinPath.exe");
        private static readonly string logDirectory = Path.Combine(
                                                        Path.GetTempPath(),
                                                        "WinPath\\logs\\log.txt");
        private const string launchingFromWinPath = "launching_from_winpath";
        private const string errorMessage = "Could not install WinPath because of an error: ";

        public static void Main(string[] args)
        {
            if (args.Length < 1) // To prevent crashing if args is 0 in the next if-statement.
                return;
            if (args[0] != launchingFromWinPath)
                return;
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
                File.AppendAllText(
                    logDirectory,
                    DateTime.Today.ToLongDateString()
                      + (File.Exists(logDirectory)
                        ? "\n" + errorMessage
                        : errorMessage)
                  + exception.Message
                  + "\n"
                  + "Please report this to the developer."
                );
                Console.WriteLine("Exception logged at " + logDirectory);
                Environment.ExitCode = 1;
            }
            Environment.Exit(Environment.ExitCode);
        }
    }
}
