using System;
using Bomber.Constants;

namespace Bomber.Interfaces
{
	public interface IBombGameLogger
	{
		void Log(LoggingConstants.LogLevel level, string log);
		void Log(Exception ex, string log);
    }
}

