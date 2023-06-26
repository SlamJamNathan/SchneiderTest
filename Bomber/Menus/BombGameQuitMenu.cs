using System;
using Bomber.Interfaces;

namespace Bomber.Menus
{
	public class BombGameQuitMenu : IBombGameMenu
	{
        public string Display() => $"Q: Quit Game";
    }
}

