using System;
using Bomber.Interfaces;

namespace Bomber.Menus
{
	public class BombGameInProgressMenu : IBombGameMenu
	{
        public string Display()
        {
            var ret = $"Enter square to visit (ie. 'C2').  " +
                $"You can only move to adjacent squares and you must start in column 'A'" +
                $"\r\n--------------------\r\n";
            return ret;
        }
    }
}

