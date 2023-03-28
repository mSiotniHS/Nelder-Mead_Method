using System.IO;

namespace Lib.Helpers;

public sealed class FileLogger : ILogger
{
	private readonly StreamWriter _writer;

	public FileLogger(StreamWriter writer)
	{
		_writer = writer;
	}

	public void Log(string log) => _writer.WriteLine(log);
}
