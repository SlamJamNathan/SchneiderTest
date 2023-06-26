using System;
namespace Bomber.Interfaces
{
	// interface that is responsible for displaying information about the
	// current game.
	public interface IBombGameView
	{
		// function that is responsible for displaying the current
		// state of the game
		bool Display(string display);
	}
}

