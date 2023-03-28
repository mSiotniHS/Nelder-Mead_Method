using System.Collections.Generic;

namespace Lib;

public sealed class Statistics<T>
{
	public List<T> Trace { get; }
	public uint IterationCount => (uint) Trace.Count;

	public Statistics()
	{
		Trace = new List<T>();
	}

	public void Save(T thing)
	{
		Trace.Add(thing);
	}
}
