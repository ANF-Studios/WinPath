﻿using System;
using System.Runtime.Versioning;

using Xunit;
using Xunit.Abstractions;

using WinPath;

namespace WinPath.Tests
{
    public class BackupCommandTests
    {
        private readonly ITestOutputHelper output;

        public BackupCommandTests(ITestOutputHelper output)
        {
            this.output = output;
            Console.SetOut(new OutputRedirector(output));
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void ListAllBackups()
        {
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
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public void ListLatestBackups()
        {
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
                    "C:\\dev",
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
                    "C:\\dev"
                 }
            );
        }
    }
}