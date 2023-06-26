using System;
using Bomber.Interfaces;

namespace Bomber.Controllers
{
	public class BombGameConsoleKeyboardController : BombGameKeyboardController
    {
		public BombGameConsoleKeyboardController(IBombGameLogger logger) : base(logger) { }

        // implementation of the fetch input function for console input
        protected override Task<string> FetchInputAsync()
        {
            var ret = string.Empty;
            try
            {
                // read the key in - this will block until a key has been read
                var loop = true;
                while (loop)
                {
                    // keep reading until we read the 'enter key'
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Enter)
                    {
                        loop = false;
                    }
                    else
                    {
                        ret += key.KeyChar.ToString();
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                // just display the error and return to the calling function.
                Logger.Log(ex, "BombGameKeyboardController.RunAsync");
                ret = string.Empty;
            }
            return Task.FromResult(ret);
        }
    }
}

