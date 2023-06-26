using System;
using Bomber.Interfaces;

namespace Bomber.Menus
{
	public class BombGameNotStartedMenu : IBombGameMenu
	{
        public string Display()
        {
            var ret = $"1: New Game - Easy\r\n" +
                $"2: New Game - Medium\r\n" +
                $"3: New Game - Hard\r\n" +
                $"4: New Game - Legend\r\n" +
                $"--------------------\r\n";
            return ret;
        }
    }
}

