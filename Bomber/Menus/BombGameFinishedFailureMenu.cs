using System;
using Bomber.Interfaces;

namespace Bomber.Menus
{
	public class BombGameFinishedFailureMenu : IBombGameMenu
	{
        public string Display() => $"You have failed - do you want to play again (y/n)";
    }
}

