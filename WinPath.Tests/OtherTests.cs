#pragma warning disable IDE0059 // Unnecessary assignment of a value

using Xunit;
using Xunit.Abstractions;

namespace WinPath.Tests
{
	public class OtherTests
	{
		private readonly ITestOutputHelper output;

		public OtherTests(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void AllValuesAreInOrder()
		{
			bool valuesAreInOrder =
				((int)HandleEventType.NoValue == 0)
			 && ((int)HandleEventType.UserPath == 1)
			 && ((int)HandleEventType.SystemPath == 2)
			 && ((int)HandleEventType.UserAndSystemPath == 3)
			 && ((int)HandleEventType.NoUserOrSystemPath == 4);
			
			output.WriteLine($"Individual results:");
			output.WriteLine($"{nameof(HandleEventType.NoValue)}: {(int)HandleEventType.NoValue == 0}");
			output.WriteLine($"{nameof(HandleEventType.UserPath)}: {(int)HandleEventType.UserPath == 1}");
			output.WriteLine($"{nameof(HandleEventType.SystemPath)}: {(int)HandleEventType.SystemPath == 2}");
			output.WriteLine($"{nameof(HandleEventType.UserAndSystemPath)}: {(int)HandleEventType.UserAndSystemPath == 3}");
			output.WriteLine($"{nameof(HandleEventType.NoUserOrSystemPath)}: {(int)HandleEventType.NoUserOrSystemPath == 4}");
			output.WriteLine($"Final: {valuesAreInOrder}");
			
			Assert.True(valuesAreInOrder);
		}

		[Fact]
		public void CreateOptions()
        {
			AddOptions addOptions = new AddOptions();

			UpdateOptions updateOptions = new UpdateOptions();

			BackupOptions.BackupApplyOptions backupApplyOptions = new BackupOptions.BackupApplyOptions();
			BackupOptions.BackupListOptions backupListOptions = new BackupOptions.BackupListOptions();
			BackupOptions.BackupCreateOptions backupCreateOptions = new BackupOptions.BackupCreateOptions();
			BackupOptions.BackupRemoveOptions backupRemoveOptions = new BackupOptions.BackupRemoveOptions();

			output.WriteLine(backupApplyOptions.BackupDirectory);
			output.WriteLine(backupCreateOptions.BackupDirectory);
			output.WriteLine(backupRemoveOptions.BackupDirectory);
		}
	}
}
