using System;
namespace Bomber.Interfaces
{
	public interface IBombGameController
	{
        Task InitialiseAsync();
        TaskCompletionSource? ControllerComplete { get; }
        bool ControllerTerminated { get; }

        // the physical board that we will play on.
        Dictionary<string, bool> Board { get; set; }
    }
}

