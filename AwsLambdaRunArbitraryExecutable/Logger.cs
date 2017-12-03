using System;

namespace AwsLambdaRunArbitraryExecutable
{
	public enum LogLevel
	{
		Info = 0,
		Error = 1,
		Critical = 2
	}

	public interface ILogger
	{
		void LogInfo(string message);

		void LogCritical(string message);

		void LogError(string message);

		void LogError(string message, Exception ex);
	}

	/// <summary>
	/// A simple AWS Lambda Logger
	/// </summary>
	public class Logger : ILogger
	{
		public void LogInfo(string message)
		{
			LogInternal(LogLevel.Info, message);
		}

		public void LogCritical(string message)
		{
			LogInternal(LogLevel.Critical, $"CRITICAL {message}");
		}

		public void LogError(string message)
		{
			LogInternal(LogLevel.Error, $"ERROR {message}");
		}

		public void LogError(string message, Exception ex)
		{
			LogInternal(LogLevel.Error, $"ERROR {message} : {ex.Message} {ex.StackTrace}");
		}

		private static void LogInternal(LogLevel logLevel, string message)
		{
			Console.WriteLine(message); // this will go into CloudWatch log stream
		}
	}
}