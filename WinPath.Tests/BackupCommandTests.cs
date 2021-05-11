using System;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;

using Xunit;
using Xunit.Abstractions;

using WinPath;

namespace WinPath.Tests
{
    public class BackupCommandTests
    {
        private readonly ITestOutputHelper output;
        private readonly System.IO.TextWriter initialOutput = Console.Out;

        private const string overrideDirectory = "C:\\dev\\";

        public BackupCommandTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void ListBackupDefaultCase()
        {
            bool exceptionThrown = false;
            try
            {
                Backup.ListBackups(HandleEventType.NoValue, overrideDirectory);
            }
            catch
            {
                exceptionThrown = true;
            }
            finally
            {
                Assert.True(exceptionThrown);
            }
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void ListAllBackups()
        {
            Console.SetOut(new OutputRedirector(output));
            Program.Main(new string[] {
                "add",
                "--user",
                "--backup",
                "--value",
                "BackupTests_ListAllBackups"
            });
            Program.Main(
                new string[] { "backup", "list", "--all" }
            );
            Console.SetOut(initialOutput);
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void ListLatestBackups()
        {
            Console.SetOut(new OutputRedirector(output));
            Program.Main(new string[] {
                "add",
                "--user",
                "--backup",
                "--value",
                "BackupTests_ListLatestBackups"
            });
            Program.Main(
                new string[] { "backup", "list", "--latest" }
            );
            Console.SetOut(initialOutput);
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void ListRangedBackups()
        {
            Program.Main(new string[] {
                "add",
                "--user",
                "--backup",
                "--value",
                "BackupTests_ListRangedBackups"
            });
            Program.Main(
                new string[] { "backup", "list", "--range", new Random().Next(0, 100).ToString() }
            );
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void ListRangedBackupsWithInvalidRange()
        {
            Program.Main(new string[] {
                "add",
                "--user",
                "--backup",
                "--value",
                "BackupTests_ListRangedBackupsWithInvalidRange"
            });
            Program.Main(
                new string[] { "backup", "list", "--range", "-1337" }
            );
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void ListBackupsDefaultCase()
        {
            Program.Main(new string[] {
                "add",
                "--user",
                "--backup",
                "--value",
                "BackupTests_ListBackupsDefaultCase"
            });
            bool exceptionThrown = false;
            try
            {
                Backup.ListBackups(HandleEventType.NoValue, "...");
            }
            catch
            {
                exceptionThrown = true;
            }

            Assert.True(exceptionThrown);
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void DeleteBackupDirectoryAndListBackups()
        {
            string backupDirectory = Program.GetUserPath().BackupDirectory;
            if (System.IO.Directory.Exists(backupDirectory))
                System.IO.Directory.Delete(backupDirectory, true);
            Backup.ListBackups(HandleEventType.ListAllBackups, backupDirectory);
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void CreateBackupClass()
        {
            Backup backup = new Backup();
            output.WriteLine(backup.ToString());
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void CreateUserBackup()
        {
            Program.Main(
                new string[]
                {
                    "backup",
                    "create",
                    "--user"
                }
            );
        }
        [Fact]
        [SupportedOSPlatform("windows")]
        public void CreateUserBackupWithArgumentsAfterDirectory()
        {
            Program.Main(
                new string[]
                {
                    "backup",
                    "create",
                    "--directory",
                    overrideDirectory,
                    "--user"
                }
            );
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void CreateSystemBackup()
        {
            Program.Main(
                new string[]
                {
                    "backup",
                    "create",
                    "--system"
                }
            );
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void CreateUserAndSystemBackup()
        {
            Program.Main(
                new string[]
                {
                    "backup",
                    "create",
                    "--user",
                    "--system"
                }
            );
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void CreateUserBackupInASpecifiedDirectory()
        {
            Program.Main(
                new string[]
                {
                    "backup",
                    "create",
                    "--directory",
                    overrideDirectory,
                    "--user"
                }
            );

            Program.Main(
                 new string[]
                 {
                    "backup",
                    "create",
                    "--user",
                    "--directory",
                    overrideDirectory
                 }
            );
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void ApplyUserBackup()
        {
            CreateUserBackupInASpecifiedDirectory();
            var backup = Directory.EnumerateFiles(overrideDirectory).ToArray().FirstOrDefault();
            var backupFile = new FileInfo(backup);

            output.WriteLine(backup);
            output.WriteLine(backupFile.Name);

            Program.Main(
                new string[]
                {
                        "backup",
                        "apply",
                        "--user",
                        "--name",
                        backupFile.Name,
                        "--directory",
                        overrideDirectory
                }
            );
            Assert.True(File.Exists(Path.Combine(Path.GetTempPath(), "WinPath", "u_backup.txt")));
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void ApplySystemBackup()
        {
            CreateUserBackup();
            var backup = Directory.EnumerateFiles(overrideDirectory).ToArray().FirstOrDefault();

            output.WriteLine(backup);

            Program.Main(
                new string[]
                {
                    "backup",
                    "apply",
                    "--system",
                    "--name",
                    backup,
                    "--directory",
                    overrideDirectory
                }
            );
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void ApplyUserAndSystemBackup()
        {
            CreateUserBackup();
            var backup = Directory.EnumerateFiles(overrideDirectory).ToArray().FirstOrDefault();

            output.WriteLine(backup);

            Program.Main(
                new string[]
                {
                    "backup",
                    "apply",
                    "--system",
                    "--user",
                    "--name",
                    backup,
                    "--directory",
                    overrideDirectory
                }
            );
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void RemoveBackup()
        {
            Backup.CreateBackup(new BackupOptions.BackupCreateOptions
            {
                BackupDirectory = overrideDirectory,
                BackupUserVariables = true,
                BackupSystemVariables = false
            });

            var filename = new FileInfo(Directory.EnumerateFiles(overrideDirectory).ToArray().FirstOrDefault());

            output.WriteLine(filename.Name);
            output.WriteLine(filename.DirectoryName);

            Program.Main(new string[]
            {
                "backup",
                "remove",
                "--name",
                filename.Name,
                "--directory",
                filename.DirectoryName
            });
            Assert.False(File.Exists(filename.FullName));
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void RemoveBackupWithArgumentsAfterDirectory()
        {
            Backup.CreateBackup(new BackupOptions.BackupCreateOptions
            {
                BackupDirectory = overrideDirectory,
                BackupUserVariables = true,
                BackupSystemVariables = false
            });

            var filename = new FileInfo(Directory.EnumerateFiles(overrideDirectory).ToArray().FirstOrDefault());

            output.WriteLine(filename.Name);
            output.WriteLine(filename.DirectoryName);

            Program.Main(new string[]
            {
                "backup",
                "remove",
                "--directory",
                filename.DirectoryName,
                "--name",
                filename.Name
            });
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void RemoveBackupWithInvalidDirectory()
        {
            Backup.CreateBackup(new BackupOptions.BackupCreateOptions
            {
                BackupDirectory = overrideDirectory,
                BackupUserVariables = true,
                BackupSystemVariables = false
            });

            var filename = new FileInfo(Directory.EnumerateFiles(overrideDirectory).ToArray().FirstOrDefault());
            var exceptionThrown = false;

            output.WriteLine(filename.Name);
            output.WriteLine(filename.DirectoryName);

            try
            {
                Program.Main(new string[]
                {
                    "backup",
                    "remove",
                    "--name",
                    filename.Name,
                    "--directory",
                    "filename.DirectoryName", // Invalid directory.
                });
            }
            catch
            {
                exceptionThrown = true;
            }
            finally
            {
                Assert.False(exceptionThrown); // Since it's handled within the code.
            }
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void RemoveBackupWithTooLongDirectory()
        {
            Backup.CreateBackup(new BackupOptions.BackupCreateOptions
            {
                BackupDirectory = overrideDirectory,
                BackupUserVariables = true,
                BackupSystemVariables = false
            });

            var filename = new FileInfo(Directory.EnumerateFiles(overrideDirectory).ToArray().FirstOrDefault());
            var exceptionThrown = false;
            const string tooLongDir = @"C:\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\"
                                    + @"foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\"
                                    + @"foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\"
                                    + @"foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\"
                                    + @"foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\"
                                    + @"foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\"
                                    + @"foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\"
                                    + @"foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\foobar\";

            Directory.CreateDirectory(tooLongDir);

            output.WriteLine(filename.Name);
            output.WriteLine(filename.DirectoryName);

            var longPathsEnabled = Microsoft.Win32.Registry.GetValue(
                "HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\FileSystem",
                "LongPathsEnabled",
                "default"
            );

            output.WriteLine(longPathsEnabled.ToString());

            try
            {
                Program.Main(new string[]
                {
                    "backup",
                    "remove",
                    "--name",
                    filename.Name,
                    "--directory",
                    tooLongDir, // Too long directory.
                });
            }
            catch
            {
                exceptionThrown = true;
            }
            finally
            {
                Assert.False(exceptionThrown); // Since it's handled within the code.
            }
        }
    }
}
