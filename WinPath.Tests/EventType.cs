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
				((int)HandleEventType.NoValue == 0)
			  + ((int)HandleEventType.UserPath == 1)
			  + ((int)HandleEventType.SystemPath == 2)
			  + ((int)HandleEventType.UserAndSystemPath == 3)
			  + ((int)HandleEventType.NoUserOrSystemPath == 4);
			Assert.True(valuesAreInOrder);
		}
	}
}
