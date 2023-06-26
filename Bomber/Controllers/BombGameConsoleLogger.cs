using System;
using Bomber.Constants;
using Bomber.Interfaces;

namespace Bomber.Controllers
{
	public class BombGameConsoleLogger : IBombGameLogger
	{
        private LoggingConstants.LogLevel LogLevel { get; set; }

        public BombGameConsoleLogger(LoggingConstants.LogLevel level)
        {
            LogLevel = level;
        }

        // log function that inspects the level that the log message
        // is set to and determines if it is within the scope of the logger
        // and logs the message out if it is.
        public void Log(LoggingConstants.LogLevel level, string log)
        {
            if (level > LogLevel)
                return;

            var msg = $"{DateTimeOffset.UtcNow} [{Thread.CurrentThread.ManagedThreadId}]({level.ToString()}) - {log}";
            Console.WriteLine(msg);
        }

        // simple wrapper that makes logging exceptions easier.
        public void Log(Exception ex, string log)
        {
            // format the exception into something that we can print out
            var inner = ex.InnerException?.Message ?? string.Empty;
            var msg = $"{ex.Message} (inner:{inner}) - {log}";

            // now just call the error level log function
            Log(LoggingConstants.LogLevel.Error, msg);
        }
    }
}

