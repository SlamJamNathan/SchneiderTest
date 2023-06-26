using System;
using Bomber.Interfaces;

namespace Bomber.Menus
{
    public class BombGameFinishedSuccessMenu : IBombGameMenu
    {
        public string Display() => $"Congratulations - you have won! - do you want to play again (y/n)";
    }
}

