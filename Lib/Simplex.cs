using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lib;

public sealed class Simplex : IEnumerable<Point>
{
	private readonly Point[] _simplex;
	public uint Size => (uint) _simplex.Length;

	public Simplex(IEnumerable<Point> simplex)
	{
		var uncheckedSimplex = simplex.ToArray();
		var pointCount = uncheckedSimplex.Length;

		if (uncheckedSimplex.Any(point => point.Dimension != pointCount - 1))
		{
			throw new ArgumentException(
				"Simplex size and its point dimensions are in disharmony",
				nameof(simplex)
			);
		}

		_simplex = uncheckedSimplex;
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

	public Point Centroid(Point? except = null)
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

	public IEnumerator<Point> GetEnumerator() =>
		((IEnumerable<Point>) _simplex).GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
