using System;
namespace Bomber.Interfaces
{
	public interface IBombGameKeyboardController
	{
		// utf encoded key that has been pressed.
		Task<bool> RunAsync();
		void Subscribe(IBombGameKeyboardHandler handler);
		void Unsubscribe(IBombGameKeyboardHandler handler);
	}
}

