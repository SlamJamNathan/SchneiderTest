using System;
namespace Bomber.Constants
{
	public static class AppConstants
	{
		// the different stats that the game can exist in.
		public enum GameState
		{
			NotStarted, ChooseGameSize, FinishedSuccess, FinishedFailure, InProgress
		}

		public const int MaxLives = 3;
		public const string QuitGame = "q";
		public const string NewGameEasy = "1";
        public const string NewGameMedium = "2";
        public const string NewGameHard = "3";
        public const string NewGameLegend = "4";
    }
}

