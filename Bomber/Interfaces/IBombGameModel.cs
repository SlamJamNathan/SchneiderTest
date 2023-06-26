using System;
using static Bomber.Constants.AppConstants;

namespace Bomber.Interfaces
{
	// interface that defines the bomb game model that
	// is used to store the status of the current game.
	public interface IBombGameModel
	{
		// the number of lives remaining.
		int Lives { get; set; }

		// the number of moves that have been taken
		int Moves { get; set; }

		// the board size that was initialised
		int Size { get; set; }

		// the current location within the grid.
		string Location { get; set; }

		// determines the state of the game
		bool IsFinished { get; }

        // the actual state of the game
        GameState CurrentState { get; set; }

		// simple function that resets the internal counters
		void Reset(int size);

		// determine the valid moves that can happen next.
		HashSet<string> ValidMoves { get; }
    }
}

