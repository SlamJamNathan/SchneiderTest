using System;
namespace Bomber.Interfaces
{
	public interface IBombGameKeyboardHandler
	{
		Task<bool> HandleKeyboardAsync(string content);
	}
}

