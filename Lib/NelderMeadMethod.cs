using System;
using System.Collections.Generic;

namespace Lib;

public sealed class NelderMeadMethod
{
	private readonly double _reflectionCoef;
	private readonly double _shrinkCoef;
	private readonly double _expansionCoef;

	private readonly Statistics<Simplex> _statistics;
	private readonly EvaluationStrategy _shouldWork;

	public NelderMeadMethod(Coefficients coefficients, EvaluationStrategy shouldWork)
	{
		_reflectionCoef = coefficients.Reflection;
		_shrinkCoef = coefficients.Shrink;
		_expansionCoef = coefficients.Expansion;

		_statistics = Statistics<Simplex>.Classic();
		_shouldWork = shouldWork;
	}

	// TODO strategy?
	private static IEnumerable<Point> GeneratePoints(uint count, uint dimension) =>
		Utilities.Generate(count, () => Utilities.RandomPoint(dimension));

	public Point FindMinimum(RealMultivariableFunction function)
	{
		var dimension = function.Dimension;
		var simplex = new Simplex(GeneratePoints(dimension + 1, dimension));

		_statistics.Save(simplex);

		while (_shouldWork(_statistics))
		{
			Console.WriteLine("Current simplex");
			for (var i = 0; i < simplex.Size; i++)
			{
				Console.WriteLine($"{i + 1}) {simplex[i]} ({function.Calculate(simplex[i])})");
			}

			Console.WriteLine();

			simplex = PerformIteration(simplex, function);
			_statistics.Save(simplex);
		}

		return FindMin(simplex, function);
	}

	private Simplex PerformIteration(Simplex simplex, RealMultivariableFunction function)
	{
		var (bestPoint, _, worstPoint) = FindKeyPoints(
			simplex, function,
			out var bestValue, out var secondWorstValue, out var worstValue
		);

		var centroid = simplex.Centroid(except: worstPoint);

		var reflection = Reflect(worstPoint, centroid);
		var reflectionValue = function.Calculate(reflection);

		if (reflectionValue < bestValue)
		{
			var expansion = Expand(reflection, centroid);
			var expansionValue = function.Calculate(expansion);

			return simplex.Replace(
				worstPoint,
				expansionValue < reflectionValue ? expansion : reflection
			);
		}

		if (reflectionValue < secondWorstValue) // between!
		{
			return simplex.Replace(worstPoint, reflection);
		}

		var shrunk = Shrink(reflectionValue < worstValue ? reflection : worstPoint, centroid);
		var shrunkValue = function.Calculate(shrunk);

		if (shrunkValue < reflectionValue)
		{
			return simplex.Replace(worstPoint, shrunk);
		}

		return simplex.Map(point => Shrink(point, bestPoint));
	}

	private static Point MapPoint(Point mapped, Point basis, double coef) =>
		basis + coef * (basis - mapped);

	private Point Reflect(Point reflected, Point basis) =>
		MapPoint(reflected, basis, _reflectionCoef);

	private Point Expand(Point expanded, Point basis) =>
		MapPoint(expanded, basis, -_expansionCoef);

	private Point Shrink(Point shrunk, Point basis) =>
		MapPoint(shrunk, basis, -_shrinkCoef);

	// best, secondWorst, worst
	private static (Point, Point, Point) FindKeyPoints(
		Simplex simplex, RealMultivariableFunction function,
		out double bestValue, out double secondWorstValue, out double worstValue)
	{
		var bestPoint = simplex[0];
		var secondWorstPoint = simplex[0];
		var worstPoint = simplex[0];

		bestValue = function.Calculate(bestPoint);
		secondWorstValue = function.Calculate(secondWorstPoint);
		worstValue = function.Calculate(worstPoint);

		for (var i = 1; i < simplex.Size; i++)
		{
			var point = simplex[i];
			var value = function.Calculate(point);

			if (value < bestValue)
			{
				bestPoint = point;
				bestValue = value;
			}

			if (value > worstValue)
			{
				secondWorstPoint = worstPoint;
				secondWorstValue = worstValue;

				worstPoint = point;
				worstValue = value;
			}
		}

		return (bestPoint, secondWorstPoint, worstPoint);
	}

	private static Point FindMin(Simplex simplex, RealMultivariableFunction function)
	{
		var (minPoint, _, _) = FindKeyPoints(simplex, function, out _, out _, out _);
		return minPoint;
	}
}
