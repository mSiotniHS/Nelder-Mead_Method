using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Lib;

public class Point : IEquatable<Point>
{
	public double[] Coordinates { get; }
	public uint Dimension => (uint) Coordinates.Length;

	public Point(IEnumerable<double> coords)
	{
		Coordinates = coords.ToArray();
	}

	public Point(params double[] coords)
	{
		Coordinates = coords.ToArray();
	}

	public double this[int key]
	{
		get => Coordinates[key];
		set => Coordinates[key] = value;
	}

	private static void DimensionAssert(Point first, Point second)
	{
		DimensionAssert(first.Dimension, second.Dimension);
	}

	private static void DimensionAssert(uint first, uint second)
	{
		if (first != second)
		{
			throw new Exception("Размерности точек не совпадают");
		}
	}

	public void DimensionAssert(uint dimension)
	{
		DimensionAssert(dimension, Dimension);
	}

	public void DimensionAssert(Point other) =>
		DimensionAssert(this, other);

	public static Point operator +(Point point) =>
		point;

	public static Point operator -(Point point) =>
		new(point.Coordinates.Select(coordinate => -coordinate));

	public static Point operator +(Point left, Point right)
	{
		DimensionAssert(left, right);

		return new Point(
			left.Coordinates.Zip(right.Coordinates, (x, y) => x + y)
		);
	}

	public static Point operator -(Point left, Point right) =>
		left + -right;

	public static Point operator *(Point left, double right) =>
		new(left.Coordinates.Select(coordinate => coordinate * right));

	public static Point operator *(double left, Point right) =>
		right * left;

	public static Point operator /(Point left, double right) =>
		left * (1 / right);

	public static Point Zero(uint dimension) => new(Enumerable.Repeat(0.0, (int) dimension));

	public bool Equals(Point? other)
	{
		if (other == null) return false;

		return Coordinates
			.Zip(
				other.Coordinates,
				(first, second) => Math.Abs(first - second) < 0.001
			)
			.All(Utilities.Identity);
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as Point);
	}

	public override int GetHashCode()
	{
		return Coordinates.GetHashCode();
	}

	public override string ToString() => $"({string.Join(", ", Coordinates)})";
}
