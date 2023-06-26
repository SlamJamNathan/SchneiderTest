using System;
using Bomber.Constants;
using Bomber.Interfaces;
using Bomber.Menus;
using static System.Collections.Specialized.BitVector32;

namespace Bomber.Controllers
{
	public class BombGameController : IBombGameController, IBombGameKeyboardHandler
    {
        // the controller is the heartbeat of the game and is responsible
        // for interpreting keyboard input and managing the game depending
        // on teh keyboard input.
		private IBombGameModel Model { get; }
        private IBombGameView View { get; }
        private IBombGameLogger Logger { get; }
        private IBombGameKeyboardController Keyboard { get; }

        // help to determine when the game is complete.
        public TaskCompletionSource? ControllerComplete { get; } = new TaskCompletionSource();
        public bool ControllerTerminated => ControllerComplete?.Task?.IsCanceled ?? true;

        // this is the dictionary of menus that are displayed for the specific state
        // of the game
        private IBombGameMenu QuitMenu { get; } = new BombGameQuitMenu();
        private Dictionary<AppConstants.GameState, IBombGameMenu> Menus { get; } = new Dictionary<AppConstants.GameState, IBombGameMenu>
        {
            { AppConstants.GameState.NotStarted, new BombGameNotStartedMenu() },
            { AppConstants.GameState.InProgress, new BombGameInProgressMenu() },
            { AppConstants.GameState.FinishedFailure, new BombGameFinishedFailureMenu() },
            { AppConstants.GameState.FinishedSuccess, new BombGameFinishedSuccessMenu() },
        };

        // these are the different game handlers based on the current game state
        private Dictionary<AppConstants.GameState, Func<string, Task<bool>>> MessageHandlers = new Dictionary<AppConstants.GameState, Func<string, Task<bool>>>();

        // constructor that is instantiated by the SimpleIoC framework.
        public BombGameController(IBombGameModel model, IBombGameView view, IBombGameLogger logger, IBombGameKeyboardController keyboard)
		{
            Model = model;
            View = view;
            Logger = logger;
            Keyboard = keyboard;

            // subscribe for keyboard events.
            Keyboard.Subscribe(this);

            // put out some debug just for testing.
            Logger.Log(Constants.LoggingConstants.LogLevel.Debug, $"BombGameController.BombGameController");

            // initialise our game state handlers.
            MessageHandlers.Add(AppConstants.GameState.NotStarted, (action) => HandleGameStateNotStartedAsync(action));
            MessageHandlers.Add(AppConstants.GameState.InProgress, (action) => HandleGameStateInProgressAsync(action));
            MessageHandlers.Add(AppConstants.GameState.FinishedFailure, (action) => HandleGameFinishedAsync(action));
            MessageHandlers.Add(AppConstants.GameState.FinishedSuccess, (action) => HandleGameFinishedAsync(action));
        }

        // wrapper function for displaying menus
        private void HandleDisplayMenu()
        {
            try
            {
                // now display the menu to the user...
                var menuContent = string.Empty;
                if (true == Menus.TryGetValue(Model.CurrentState, out var menu))
                {
                    // found the menu to display
                    menuContent = menu.Display();
                }

                // quit menu is common for all menus, so we split
                // this out to make sure it always displays.
                var quitMenuContent = QuitMenu.Display();
                View.Display($"{Model.ToString()}\r\n" +
                    $"--------------------\r\n" +
                    $"{menuContent}\r\n" +
                    $"{quitMenuContent}");

            }
            catch (Exception ex)
            {
                Logger.Log(ex, "BombGameController.HandleDisplayMenu");
            }
        }

        //build the internal game board with the specified number of bombs
        public Dictionary<string, bool> Board { get; set; } = new Dictionary<string, bool>();
        private Task<bool> BuildGameBoardAsync(int bombs, int size)
        {
            Logger.Log(Constants.LoggingConstants.LogLevel.Debug, $"BombGameController.BuildGameBoardAsync bombs:{bombs}, squares:{size * size}");
            var start = 'A';
            var end = (char)('A' + size);

            // initialise the board
            Board = new Dictionary<string, bool>();
            for (var c = start; c < end; c++)
            {
                for(var i = 1; i <= size; i++)
                {
                    var key = $"{c.ToString()}{i.ToString()}";
                    Board.Add(key, false);
                }
            }

            // now just randomize the bombs and update the board status.
            var rnd = new Random();
            for(var i = 0; i < bombs; i++)
            {
                while (true)
                {
                    // loop until we find a square that is not a bomb
                    // and set it to the bomb.  This just stops us from duplicating
                    // squares.
                    var x = rnd.Next(0, size - 1);
                    var y = rnd.Next(1, size);
                    var c = (char)(start + x);
                    var key = $"{c.ToString()}{y.ToString()}";
                    if (Board[key] == false)
                    {
                        Board[key] = true;
                        break;
                    }
                }
            }

            // finally - reset the model back to the start
            Model.Reset(size);
            return Task.FromResult(true);
        }

        // called when the user presses a key when the game is in it's
        // finished state.
        private Task<bool> HandleGameFinishedAsync(string action)
        {
            Logger.Log(Constants.LoggingConstants.LogLevel.Debug, $"BombGameController.HandleGameFinishedAsync:{action}");
            if (0 == string.Compare(action, "y", true))
            {
                // new game - reset the model
                Model.Reset(0);
                Model.CurrentState = AppConstants.GameState.NotStarted;
            }
            else
            {
                // quit game.
                if (ControllerComplete is not null)
                    ControllerComplete.TrySetCanceled();
            }
            return Task.FromResult(true);
        }

        // ok, this is called when the user selects a menu item and the game state is
        // 'new game'
        private async Task<bool> HandleGameStateNotStartedAsync(string action)
        {
            Logger.Log(Constants.LoggingConstants.LogLevel.Debug, $"BombGameController.HandleGameStateNotStartedAsync:{action}");
            switch(action)
            {
                case AppConstants.NewGameEasy:
                    await BuildGameBoardAsync(10, 10).ConfigureAwait(false);
                    break;

                case AppConstants.NewGameMedium:
                    await BuildGameBoardAsync(30, 15).ConfigureAwait(false);
                    break;

                case AppConstants.NewGameHard:
                    await BuildGameBoardAsync(75, 20).ConfigureAwait(false);
                    break;

                case AppConstants.NewGameLegend:
                    await BuildGameBoardAsync(150, 26).ConfigureAwait(false);
                    break;
            }
            return true;
        }

        // this handles a game movement
        private Task<bool> HandleGameStateInProgressAsync(string action)
        {
            Logger.Log(Constants.LoggingConstants.LogLevel.Debug, $"BombGameController.HandleGameStateAsync:{action}");

            // make sure this is a valid move first
            if(action.Length < 2)
            {
                Logger.Log(Constants.LoggingConstants.LogLevel.Error, $"BombGameController.HandleGameStateAsync - invalid action, must be at least 2 characters.");
                return Task.FromResult(false);
            }

            // fetch the valid moves from the model.
            var moves = Model.ValidMoves;
            if(false == moves.Contains(action))
            {
                Logger.Log(Constants.LoggingConstants.LogLevel.Error, $"BombGameController.HandleGameStateAsync - {action} is not a valid move");
            }
            else if(action == Model.Location)
            {
                Logger.Log(Constants.LoggingConstants.LogLevel.Error, $"BombGameController.HandleGameStateAsync - {action} is not a valid move - you are already on {action}");
            }
            else
            {
                if(true == Board.TryGetValue(action, out var bomb))
                {
                    Model.Location = action;
                    Model.Moves++;
                    if (bomb)
                    {
                        Logger.Log(Constants.LoggingConstants.LogLevel.Error, $"BombGameController.HandleGameStateAsync - {action} is a BOMB - BOOOOOM");
                        Model.Lives--;
                    }
                    if (Model.Lives == 0)
                    {
                        // no more lives
                        Model.CurrentState = AppConstants.GameState.FinishedFailure;
                    }
                    else
                    {
                        if (action[0] == (char)('A' + (Model.Size-1)))
                        {
                            // reached the edge of the board.
                            Model.CurrentState = AppConstants.GameState.FinishedSuccess;
                        }
                    }
                }
                else
                {
                    // hmmm, not sure we will get here, just print out a message
                    Logger.Log(Constants.LoggingConstants.LogLevel.Error, $"BombGameController.HandleGameStateAsync - invalid board state");
                }
            }

            return Task.FromResult(true);
        }

        // this is the guts of the game and tries to handle updates to the
        // game base on the input and the current state.
        private async Task<bool> HandleGameStateAsync(string action)
        {
            Logger.Log(Constants.LoggingConstants.LogLevel.Debug, $"BombGameController.HandleGameStateAsync:{action}");
            var ret = MessageHandlers.TryGetValue(Model.CurrentState, out var handler);
            if ((true == ret) && (handler is not null))
                ret = await handler(action).ConfigureAwait(false);

            return ret;
        }

        // called to do a one-time initialisation of the bomb controller
        public Task InitialiseAsync()
        {
            Logger.Log(Constants.LoggingConstants.LogLevel.Debug, $"BombGameController.InitialiseAsync");

            // just force the first display of the display menu.
            HandleDisplayMenu();
            return Task.CompletedTask;
        }

        // implementation of the keyboard handler class that makes sense
        // of the keyboard buffer and progresses the game accordingly.
        public async Task<bool> HandleKeyboardAsync(string content)
        {
            Logger.Log(Constants.LoggingConstants.LogLevel.Debug, $"BombGameController.HandleKeyboardAsync: {content}");

            // handle the processing as long as the input is not empty.
            var ret = true;
            if (!string.IsNullOrEmpty(content))
            {
                // check to see if we have been asked to quit the game
                if (0 == string.Compare(content, AppConstants.QuitGame))
                {
                    // we have been asked to quit the game - just try to set our token source.
                    // NOTE: *could* use ControllerTerminated flag here to check but, compiler
                    // moans that we have not checked for null even though it happens within the
                    // ControllerTerminated check.
                    if (ControllerComplete is not null)
                        ControllerComplete.TrySetCanceled();
                }
                else
                {
                    // move the model on
                    ret = await HandleGameStateAsync(content).ConfigureAwait(false);

                    // now display the menu to the user...
                    HandleDisplayMenu();
                }
            }
            return ret;
        }
    }
}

