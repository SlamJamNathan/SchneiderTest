using System;
using Bomber.Interfaces;

namespace Bomber.Views
{
	public class BombGameConsoleView : IBombGameView
    {
        // function that is responsible for displaying the content specified
        public bool Display(string display)
        {
            Console.WriteLine(display);
            return true;
        }
    }
}

