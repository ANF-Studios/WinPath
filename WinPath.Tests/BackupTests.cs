﻿using Xunit;
using Xunit.Abstractions;
using System;
using System.Runtime.Versioning;

using WinPath;

namespace WinPath.Tests
{
    public class BackupTests
    {
        private readonly ITestOutputHelper output;

        public BackupTests(ITestOutputHelper output)
        {
            this.output = output;
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
        public void DeleteBackupDirectoryAndListBackups()
        {
            string backupDirectory = Program.GetUserPath().BackupDirectory;
            if (System.IO.Directory.Exists(backupDirectory))
                System.IO.Directory.Delete(backupDirectory, true);
            Backup.ListBackups(HandleEventType.ListAllBackups, backupDirectory);
        }

        [Fact]
        public void CreateBackupClass()
        {
            Backup backup = new Backup();
            output.WriteLine(backup.ToString());
        }
    }
}