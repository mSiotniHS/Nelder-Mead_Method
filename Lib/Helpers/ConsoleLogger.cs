using System;

namespace Lib.Helpers;

public sealed class ConsoleLogger : ILogger
{
	public void Log(string log) => Console.WriteLine(log);
}
