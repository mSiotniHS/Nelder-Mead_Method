using System;

namespace Lib;

public sealed class NelderMeadMethod
{
	public delegate bool EvaluationStrategy(Statistics<Simplex> statistics);

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

	public Point FindMinimum(RealMultivariableFunction function, Simplex initialSimplex)
	{
		var dimension = function.Dimension;
		var simplex = initialSimplex;

		if (simplex.Size != dimension + 1)
		{
			throw new ArgumentException($"Initial simplex is of wrong size: got {simplex.Size}, require {dimension}",
				nameof(initialSimplex));
		}

		_statistics.Save(simplex);

		while (_shouldWork(_statistics))
		{
			Console.WriteLine("\n\nCurrent simplex");
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
		Console.WriteLine("/ Начинаем итерацию");

		var (bestPoint, secondWorstPoint, worstPoint) = FindKeyPoints(
			simplex, function,
			out var bestValue, out var secondWorstValue, out var worstValue
		);

		Console.WriteLine("| Ключевые точки:");
		Console.WriteLine($"| *) Лучшая: {bestPoint} [{bestValue}]");
		Console.WriteLine($"| *) Second worst: {secondWorstPoint} [{secondWorstValue}]");
		Console.WriteLine($"| *) Худшая: {worstPoint} [{worstValue}]");

		var centroid = simplex.Centroid(except: worstPoint);
		Console.WriteLine($"| Центр масс всех, кроме худшей: {centroid}");

		var reflection = Reflect(worstPoint, centroid);
		var reflectionValue = function.Calculate(reflection);
		Console.WriteLine($"| Reflection: {reflection} [{reflectionValue}]");

		if (reflectionValue < bestValue)
		{
			Console.WriteLine("| reflectionValue лучше bestValue!");

			var expansion = Expand(reflection, centroid);
			var expansionValue = function.Calculate(expansion);
			Console.WriteLine($"\\ Expansion: {expansion} [{expansionValue}]");

			return simplex.Replace(
				worstPoint,
				expansionValue < reflectionValue ? expansion : reflection
			);
		}

		Console.WriteLine("| reflectionValue хуже bestValue");

		if (reflectionValue < secondWorstValue) // between!
		{
			Console.WriteLine("\\ reflectionValue лучше secondWorstValue!");
			return simplex.Replace(worstPoint, reflection);
		}

		Console.WriteLine("| reflectionValue хуже secondWorstValue");

		var (betterPoint, betterValue) = reflectionValue < worstValue
			? (reflection, reflectionValue)
			: (worstPoint, worstValue);
		Console.WriteLine($"| Лучшая между reflection и worst: {betterPoint} [{betterValue}]");

		var shrunk = Shrink(betterPoint, centroid);
		var shrunkValue = function.Calculate(shrunk);
		Console.WriteLine($"| Shrunk: {shrunk} [{shrunkValue}]");

		if (shrunkValue < betterValue)
		{
			Console.WriteLine("\\ shrunkValue лучше betterValue!");
			return simplex.Replace(worstPoint, shrunk);
		}

		Console.WriteLine("\\ Никакая из вычисленных не лучше --- global shrink");
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
		var worstPoint = simplex[0];

		bestValue = function.Calculate(bestPoint);
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
				worstPoint = point;
				worstValue = value;
			}
		}

		var secondWorstPoint = bestPoint;
		secondWorstValue = function.Calculate(secondWorstPoint);

		for (var i = 0; i < simplex.Size; i++)
		{
			var point = simplex[i];
			var value = function.Calculate(point);

			if (secondWorstValue < value && value < worstValue)
			{
				secondWorstPoint = point;
				secondWorstValue = value;
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
