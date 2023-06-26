using System;
using Bomber.Interfaces;

namespace Bomber.Controllers
{
	public class BombGameKeyboardController : IBombGameKeyboardController
    {
        protected IBombGameLogger Logger { get; }
        private string KeyboardBuffer { get; set; } = string.Empty;

        // this is the array of event listeners that want to receive
        // keyboard input.
        private object handlerLock = new object();
        private HashSet<IBombGameKeyboardHandler> Handlers { get; } = new HashSet<IBombGameKeyboardHandler>();

        // constructor that is instantiated using IoC that requires the logger instance for the app.
        public BombGameKeyboardController(IBombGameLogger logger)
        {
            Logger = logger;
        }

        public void Subscribe(IBombGameKeyboardHandler handler)
        {
            Logger.Log(Constants.LoggingConstants.LogLevel.Debug, "BombGameKeyboardController.Subscribe");

            try
            {
                lock (handlerLock)
                {
                    Handlers.Add(handler);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, "BombGameKeyboardController.Subscribe");
            }
        }

        public void Unsubscribe(IBombGameKeyboardHandler handler)
        {
            Logger.Log(Constants.LoggingConstants.LogLevel.Debug, "BombGameKeyboardController.Unsubscribe");
            try
            {
                lock (handlerLock)
                {
                    Handlers.Remove(handler);
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex, "BombGameKeyboardController.Unsubscribe");
            }
        }

        // must implement this in the top level class.
        protected virtual Task<string> FetchInputAsync() => Task.FromResult(string.Empty);

        // function that acts as the main control input for the application
        // this just sits and waits for keyboard input.  If an exception occurs
        // it is caught, displayed and then the failure is returned to the top level.
        // The top level can decide to react to the failure however it chooses.
        public virtual async Task<bool> RunAsync()
        {
            var ret = true;
            try
            {
                // read the key in - this will block until a key has been read
                KeyboardBuffer = await FetchInputAsync().ConfigureAwait(false);

                // just call the event handler and let other classes process the key
                // that has been pressed
                // call each handler in return, don't both waiting for responses.
                foreach (var handler in Handlers)
                {
                    string param = KeyboardBuffer;
                    await handler.HandleKeyboardAsync(param).ConfigureAwait(false);
                }
            }
            catch(InvalidOperationException ex)
            {
                // just display the error and return to the calling function.
                Logger.Log(ex, "BombGameKeyboardController.RunAsync");
                KeyboardBuffer = string.Empty;
                ret = false;
            }

            return ret;
        }
    }
}

