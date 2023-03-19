using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib;

public sealed class Simplex
{
	private readonly Point[] _simplex;
	public uint Size => (uint) _simplex.Length;

	public Simplex(IEnumerable<Point> simplex)
	{
		_simplex = simplex.ToArray();
	}

	public Point this[Index idx]
	{
		get => _simplex[idx];
		set => _simplex[idx] = value;
	}

	public Simplex Replace(Point replaced, Point added)
	{
		var newSimplex = _simplex.ToArray();
		var idx = Array.IndexOf(newSimplex, replaced);

		if (idx == -1)
		{
			throw new ArgumentException("Point is not in simplex", nameof(replaced));
		}

		newSimplex[idx] = added;

		return new Simplex(newSimplex);
	}

	public Simplex Map(Func<Point, Point> map)
		=> new(_simplex.Select(map));

	public Point Centroid(Point except)
	{
		var centroid = Point.Zero(Size - 1);
		var count = 0;

		foreach (var point in _simplex)
		{
			if (point.Equals(except)) continue;

			centroid += point;
			count++;
		}

		return centroid / count;
	}
}
