using System;
using Bomber.Controllers;
using Bomber.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace BomberTests
{
    // override the main controller to be able to specify our keys instead of reading
    // from teh console.
	public class BombGameTestKeyboard : BombGameKeyboardController
    {
        public BombGameTestKeyboard(IBombGameLogger logger) : base(logger) { }
        public string KeysToSend { get; set; } = string.Empty;

        // implementation of the fetch input function for console input
        protected override Task<string> FetchInputAsync() => Task.FromResult(KeysToSend);
    }
}

