using Bomber.Controllers;
using Bomber.Interfaces;
using Bomber.Models;
using Bomber.Views;
using GalaSoft.MvvmLight.Ioc;

namespace Bomber;
class Program
{
    // the app completion source that is set when the user presses a specific
    // key (Q) to quit the application
    private static TaskCompletionSource AppComplete { get; } = new TaskCompletionSource();
    private static bool AppTerminated => AppComplete.Task.IsCanceled;

    // same as Main, but async.
    static async Task MainAsync(string[] args)
    {
        // define the logging level based on whether this is production or debug.
        var level = Constants.LoggingConstants.LogLevel.Error;
#if DEBUG
        level = Constants.LoggingConstants.LogLevel.Debug;
#endif

        // initialise our simpleIoc framework with the interfaces and
        // concrete classes.
        SimpleIoc.Default.Register<IBombGameLogger>(() => new BombGameConsoleLogger(level));
        SimpleIoc.Default.Register<IBombGameModel, BombGameModel>();
        SimpleIoc.Default.Register<IBombGameView, BombGameConsoleView>();
        SimpleIoc.Default.Register<IBombGameController, BombGameController>();
        SimpleIoc.Default.Register<IBombGameKeyboardController, BombGameKeyboardController>();

        // create our game controller instance, keyboard, and logger.
        var controller = SimpleIoc.Default.GetInstance<IBombGameController>();
        var keyboad = SimpleIoc.Default.GetInstance<IBombGameKeyboardController>();
        var logger = SimpleIoc.Default.GetInstance<IBombGameLogger>();

        // technically it is not really possible to end up in the situation where
        // the above objects/interfaces are null but, this is just belt and braces to make sure...
        // You can never tell how the application will be modified in future and this adds
        // a level of protection to ensure.
        if(logger is null)
        {
            Console.WriteLine("Cannot instantiate logger - exiting");
            return;
        }
        else if(keyboad is null)
        {
            logger.Log(Constants.LoggingConstants.LogLevel.Error, "Cannot instantiate keyboard - exiting");
            return;
        }
        else if (controller is null)
        {
            logger.Log(Constants.LoggingConstants.LogLevel.Error, "Cannot instantiate controller - exiting");
            return;
        }

        // call the one-time initialisation of the controller.
        await controller.InitialiseAsync().ConfigureAwait(false);

        // loop until the terminate application - The controllerterminated source
        // will be set by the controller
        var loop = true;
        while (loop)
        {
            // check to see if the controller has been terminated
            if(controller.ControllerTerminated)
            {
                // the controller has stopped - most likely the user has pressed 'Q' to quit the app.
                logger.Log(Constants.LoggingConstants.LogLevel.Info, "controller terminated - exiting");
                loop = false;
                continue;
            }

            // configure await to ensure that we are not forced to return on the same thread.
            // in practice this does not make a blind bit of difference in a console app because
            // of the lack of synchronisation context however, it is good practice and allows easier
            // porting of code to apps that require the synchronisation - such as WPF/Xamarin UI apps.
            if (false == await keyboad.RunAsync().ConfigureAwait(false))
            {
                // ok, we have most likely had an exception within the keyboard class - exit the application.
                logger.Log(Constants.LoggingConstants.LogLevel.Info, "false == keyboad.RunAsync() - exiting");
                loop = false;
            }
        }
    }

    // main entry point to the application, only job here is to get us
    // into async/await methodology by calling the mainasync function.
    static void Main(string[] args)
    {
        MainAsync(args).GetAwaiter().GetResult();
    }
}

