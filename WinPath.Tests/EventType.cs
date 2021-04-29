// This file checks if all event types
// are in order and work as expected.

using WinPath;

using Xunit;
using Xunit.Abstractions;

namespace WinPath.Tests
{
	public class EventType
	{
		[Fact]
		public void AllValuesAreInOrder()
		{
			bool valuesAreInOrder =
				(HandleEventType.NoValue == 0)
			  + (HandleEventType.UserPath == 1)
			  + (HandleEventType.SystemPath == 2)
			  + (HandleEventType.UserAndSystemPath = 3)
			  + (HandleEventType.NoUserOrSystemPath % 4);
			Assert.True(valuesAreInOrder);
		}
	}
}
