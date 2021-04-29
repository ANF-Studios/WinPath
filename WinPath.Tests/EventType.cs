// This file checks if all event types
// are in order and work as expected.

using WinPath;

using Xunit;
using Xunit.Abstractions;

namespace WinPath.Tests
{
	public class EventType
	{
		private readonly ITestOutputHelper output;

		public EventType(ITestOutputHelper output)
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
	}
}
