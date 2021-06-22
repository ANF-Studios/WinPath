using System;
using System.IO;
using System.Diagnostics;

namespace WinPath.Updater
{
    class Program
    {
        private static string executableDirectory = Path.Combine(
                                                                Path.GetTempPath(),
                                                                "WinPath\\download\\WinPath.exe");
        private static readonly string logDirectory = Path.Combine(
                                                        Path.GetTempPath(),
                                                        "WinPath\\logs\\log.txt");
        private const string launchingFromWinPath = "launching_from_winpath";
        private const string errorMessage = "Could not install WinPath because of an error: ";

        public static void Main(string[] args)
        {
            Console.Title = AppDomain.CurrentDomain.FriendlyName;
            string currentVersion = GetInstalledWinPathVersion();

            if (currentVersion is null)
                Environment.Exit(-1);

            if (currentVersion == "0.2.0") // Backwards compatibility support.
                executableDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\WinPath\\temp\\download\\WinPath.exe";

            if (int.Parse(currentVersion[2].ToString()) > 2)
            {
                if (args.Length < 1) // To prevent crashing if args is 0 in the next if-statement.
                    return;
                if (args[0] != launchingFromWinPath)
                    return;
            }
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
                //Console.ReadKey();
                Directory.CreateDirectory(logDirectory.Replace("log.txt", string.Empty));
                File.AppendAllText(
                    logDirectory,
                    DateTime.Today.ToLongDateString()
                    + "\n"
                    + errorMessage
                    + exception.Message
                    + "\n"
                    + "Please report this to the developer."
                    + "\n"
                );
                Console.WriteLine("Exception logged at " + logDirectory);
                Environment.ExitCode = 1;
            }
            Environment.Exit(Environment.ExitCode);
        }

        public static string GetInstalledWinPathVersion()
        {
            FileVersionInfo winPathVersion = null;
            string installationPath = Environment.Is64BitOperatingSystem
                                                ? "C:\\Program Files\\WinPath\\WinPath.exe"
                                                : "C:\\Program Files (x86)\\WinPath\\WinPath.exe";
            if (File.Exists(installationPath))
                winPathVersion = FileVersionInfo.GetVersionInfo(installationPath);
            else
                return null;
            return $"{winPathVersion.FileMajorPart}.{winPathVersion.FileMinorPart}.{winPathVersion.FileBuildPart}";
        }
    }
}
