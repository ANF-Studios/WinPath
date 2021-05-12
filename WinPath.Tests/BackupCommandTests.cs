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
            Directory.CreateDirectory(overrideDirectory);
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
        
        /*
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
        }*/

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
        public void ApplyEmptyBackup()
        {
            const string emptyFile = "EmptyFile.txt";
            Stream stream = File.Create(overrideDirectory + emptyFile);
            stream.Close();
            stream.Dispose();
            Backup.ApplyBackup
            (
                new BackupOptions.BackupApplyOptions
                {
                    BackupDirectory = overrideDirectory,
                    BackupFilename = emptyFile,
                    RestoreSystemVariables = false,
                    RestoreUserVariables = true
                }
            );
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void ApplyBackupFromNotExistentFile()
        {
            string fullPath = overrideDirectory + "NonExistingFile.txt";
            output.WriteLine($"{fullPath} exists: {File.Exists(fullPath)}");
            Backup.ApplyBackup
            (
                new BackupOptions.BackupApplyOptions
                {
                    BackupDirectory = overrideDirectory,
                    BackupFilename = "NonExistingFile.txt",
                    RestoreSystemVariables = false,
                    RestoreUserVariables = true
                }
            );
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
            System.Threading.Tasks.Task.Delay(100);
            bool fileExists = File.Exists(filename.FullName);
            output.WriteLine(fileExists);
            Assert.False(fileExists);
        }

        /*
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
        }*/

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

        /// <summary>
        /// Backups a directory that's extremely long.
        /// </summary>
        /// <remarks>
        /// One thing to keep note of is that an exception
        /// shouldn't be thrown at all on Windows, which is
        /// what WinPath is targeted at. This is because
        /// paths above ~260 are handled by .NET Core which
        /// means it shouldn't throw an exception. Please see
        /// https://stackoverflow.com/a/5188559/15443828 for
        /// more information.
        /// 
        /// <para>
        /// If by any chance it doesn't load/appear, here's a
        /// quote from the answer:
        /// 
        /// .NET Core Solution
        /// It just works because the framework adds the long path syntax for you.
        /// </para>
        /// </remarks>
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

            bool exceptionThrown = false;
            FileInfo filename = new FileInfo(Directory.EnumerateFiles(overrideDirectory).ToArray().FirstOrDefault());
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
                Assert.False(exceptionThrown); // Since it's handled by .NET Core.
            }
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void RemoveBackupWhileFileIsInUse()
        {
            Backup.CreateBackup(new BackupOptions.BackupCreateOptions
            {
                BackupDirectory = overrideDirectory,
                BackupUserVariables = true,
                BackupSystemVariables = false
            });

            bool exceptionThrown = false;
            FileInfo filename = new FileInfo(Directory.EnumerateFiles(overrideDirectory).ToArray().FirstOrDefault());

            output.WriteLine(filename.Name);
            output.WriteLine(filename.DirectoryName);

            StreamReader streamReader = new StreamReader(filename.FullName);
            streamReader.ReadToEnd();

            try
            {
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
