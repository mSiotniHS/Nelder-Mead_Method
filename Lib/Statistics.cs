using System.Collections.Generic;

namespace Lib;

public sealed class Statistics<T>
{
	public List<T> Trace { get; }
	public uint IterationCount { get; private set; }

	private readonly uint _savingFrequency;
	private readonly uint _maxTraceLength;

	private uint _counter;

	public Statistics(uint savingFrequency, uint maxTraceLength)
	{
		Trace = new List<T>();

		_savingFrequency = savingFrequency;
		_maxTraceLength = maxTraceLength;

		_counter = 0;
		IterationCount = 0;
	}

	public static Statistics<T> Classic() =>
		new(1, uint.MaxValue);

	public void Save(T thing)
	{
		IterationCount++;

		_counter++;
		if (_counter != _savingFrequency) return;

		Trace.Add(thing);
		if (Trace.Count > _maxTraceLength) Trace.RemoveAt(0);
		_counter = 0;
	}
}
