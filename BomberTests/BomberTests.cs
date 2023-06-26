using GalaSoft.MvvmLight.Ioc;
using Bomber.Interfaces;
using Bomber.Controllers;
using Bomber.Models;
using Bomber.Views;
using Bomber.Constants;

namespace BomberTests;

[TestClass]
public class BomberTests
{
    protected async Task BoardSetupTest(string choice, int bombsExpected, int boardSizeExpected)
    {
        // initialise our simpleIoc framework with the interfaces and
        // concrete classes. - NOTE that the only difference here is that
        // we define our test keyboad so that we can send our tests to the board.
        var level = LoggingConstants.LogLevel.Debug;
        var logger = new BombGameConsoleLogger(level);
        var model = new BombGameModel();
        var view = new BombGameConsoleView();
        var keyboard = new BombGameTestKeyboard(logger);
        var controller = new BombGameController(model, view, logger, keyboard);

        // technically it is not really possible to end up in the situation where
        // the above objects/interfaces are null but, this is just belt and braces to make sure...
        // You can never tell how the application will be modified in future and this adds
        // a level of protection to ensure.
        Assert.IsNotNull(logger);
        Assert.IsNotNull(keyboard);
        Assert.IsNotNull(controller);
        Assert.IsNotNull(model);
        Assert.IsNotNull(view);

        // call the one-time initialisation of the controller.
        await controller.InitialiseAsync().ConfigureAwait(false);

        // now start an easy game.
        keyboard.KeysToSend = choice;
        var ret = await keyboard.RunAsync().ConfigureAwait(false);

        Assert.AreEqual(ret, true);
        Assert.AreEqual(model.CurrentState, AppConstants.GameState.InProgress);
        Assert.AreEqual(model.Lives, AppConstants.MaxLives);
        Assert.AreEqual(model.Moves, 0);
        Assert.AreEqual(controller.Board.Count, boardSizeExpected);

        var bombs = controller.Board.Values.ToList().FindAll(b => b == true);
        Assert.AreEqual(bombs.Count, bombsExpected);
    }

    [TestMethod]
    public async Task TestNewGameEasyMenuChoice()
    {
        await BoardSetupTest("1", 10, 10 * 10).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task TestNewGameMediumMenuChoice()
    {
        await BoardSetupTest("2", 30, 15 * 15).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task TestNewGameHardMenuChoice()
    {
        await BoardSetupTest("3", 75, 20 * 20).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task TestNewGameLegendMenuChoice()
    {
        await BoardSetupTest("4", 150, 26 * 26).ConfigureAwait(false);
    }

    protected async Task TestValidPosition(string choice)
    {
        // initialise our simpleIoc framework with the interfaces and
        // concrete classes. - NOTE that the only difference here is that
        // we define our test keyboad so that we can send our tests to the board.
        var level = LoggingConstants.LogLevel.Debug;
        var logger = new BombGameConsoleLogger(level);
        var model = new BombGameModel();
        var view = new BombGameConsoleView();
        var keyboard = new BombGameTestKeyboard(logger);
        var controller = new BombGameController(model, view, logger, keyboard);

        // technically it is not really possible to end up in the situation where
        // the above objects/interfaces are null but, this is just belt and braces to make sure...
        // You can never tell how the application will be modified in future and this adds
        // a level of protection to ensure.
        Assert.IsNotNull(logger);
        Assert.IsNotNull(keyboard);
        Assert.IsNotNull(controller);
        Assert.IsNotNull(model);
        Assert.IsNotNull(view);

        // call the one-time initialisation of the controller.
        await controller.InitialiseAsync().ConfigureAwait(false);

        // now start an easy game.
        keyboard.KeysToSend = "1";
        var ret = await keyboard.RunAsync().ConfigureAwait(false);

        // now set our initial position.
        keyboard.KeysToSend = choice;
        int currentMoves = model.Moves;
        ret = await keyboard.RunAsync().ConfigureAwait(false);

        Assert.AreEqual(ret, true);
        Assert.AreEqual(model.CurrentState, AppConstants.GameState.InProgress);
        Assert.AreEqual(model.Moves, currentMoves + 1);
        Assert.AreEqual(model.Location, choice);
    }

    protected async Task TestInValidPosition(string choice)
    {
        // initialise our simpleIoc framework with the interfaces and
        // concrete classes. - NOTE that the only difference here is that
        // we define our test keyboad so that we can send our tests to the board.
        var level = LoggingConstants.LogLevel.Debug;
        var logger = new BombGameConsoleLogger(level);
        var model = new BombGameModel();
        var view = new BombGameConsoleView();
        var keyboard = new BombGameTestKeyboard(logger);
        var controller = new BombGameController(model, view, logger, keyboard);

        // technically it is not really possible to end up in the situation where
        // the above objects/interfaces are null but, this is just belt and braces to make sure...
        // You can never tell how the application will be modified in future and this adds
        // a level of protection to ensure.
        Assert.IsNotNull(logger);
        Assert.IsNotNull(keyboard);
        Assert.IsNotNull(controller);
        Assert.IsNotNull(model);
        Assert.IsNotNull(view);

        // call the one-time initialisation of the controller.
        await controller.InitialiseAsync().ConfigureAwait(false);

        // now start an easy game.
        keyboard.KeysToSend = "1";
        var ret = await keyboard.RunAsync().ConfigureAwait(false);

        // now set our initial position.
        keyboard.KeysToSend = choice;
        int currentMoves = model.Moves;
        ret = await keyboard.RunAsync().ConfigureAwait(false);

        Assert.AreEqual(ret, true);
        Assert.AreEqual(model.CurrentState, AppConstants.GameState.InProgress);
        Assert.AreEqual(model.Moves, currentMoves);
        Assert.AreNotEqual(model.Location, choice);
    }

    [TestMethod]
    public async Task TestValidStartPosition()
    {
        await TestValidPosition("A1").ConfigureAwait(false);
        await TestValidPosition("A2").ConfigureAwait(false);
        await TestValidPosition("A3").ConfigureAwait(false);
        await TestValidPosition("A4").ConfigureAwait(false);
        await TestValidPosition("A5").ConfigureAwait(false);
        await TestValidPosition("A1").ConfigureAwait(false);
        await TestValidPosition("A6").ConfigureAwait(false);
        await TestValidPosition("A7").ConfigureAwait(false);
        await TestValidPosition("A8").ConfigureAwait(false);
        await TestValidPosition("A9").ConfigureAwait(false);
        await TestValidPosition("A10").ConfigureAwait(false);
    }

    [TestMethod]
    public async Task TestInValidStartPosition()
    {
        await TestInValidPosition("B1").ConfigureAwait(false);
    }

}
