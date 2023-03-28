using System;

namespace Lib;

public sealed class NelderMeadMethod
{
	public delegate bool EvaluationStrategy(Statistics<Simplex> statistics);

	private readonly Coefficients _coefficients;
	private readonly Statistics<Simplex> _statistics;
	private readonly EvaluationStrategy _shouldWork;

	public NelderMeadMethod(Coefficients coefficients, EvaluationStrategy shouldWork)
	{
		_coefficients = coefficients;
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

		return FindMin(simplex, function).Item1;
	}

	private Simplex PerformIteration(Simplex simplex, RealMultivariableFunction function)
	{
		Console.WriteLine("/ Начинаем итерацию");

		var keyPoints = FindKeyPoints(simplex, function);

		var bestPoint = keyPoints.BestPoint;
		var bestValue = keyPoints.BestValue;

		var secondWorstPoint = keyPoints.SecondWorstPoint;
		var secondWorstValue = keyPoints.SecondWorstValue;

		var worstPoint = keyPoints.WorstPoint;
		var worstValue = keyPoints.WorstValue;

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
		MapPoint(reflected, basis, _coefficients.Reflection);

	private Point Expand(Point expanded, Point basis) =>
		MapPoint(expanded, basis, -_coefficients.Expansion);

	private Point Shrink(Point shrunk, Point basis) =>
		MapPoint(shrunk, basis, -_coefficients.Shrink);

	private delegate bool SavingCondition<in T>(T newValue, T currentValue);

	private static (Point, double) FindPoint(
		Simplex simplex,
		RealMultivariableFunction function,
		SavingCondition<double> shouldSaveNewValue,
		Point initialValue
	)
	{
		var currentPoint = initialValue;
		var currentValue = function.Calculate(currentPoint);

		for (var i = 0; i < simplex.Size; i++)
		{
			var point = simplex[i];
			var value = function.Calculate(point);

			if (shouldSaveNewValue(value, currentValue))
			{
				currentPoint = point;
				currentValue = value;
			}
		}

		return (currentPoint, currentValue);
	}

	private static (Point, double) FindPoint(
		Simplex simplex,
		RealMultivariableFunction function,
		SavingCondition<double> shouldSaveNewValue
	)
		=> FindPoint(simplex, function, shouldSaveNewValue, simplex[0]);

	private static (Point, double) FindMin(Simplex simplex, RealMultivariableFunction function)
		=> FindPoint(simplex, function, (newValue, currentValue) => newValue < currentValue);

	private static (Point, double) FindMax(Simplex simplex, RealMultivariableFunction function)
		=> FindPoint(simplex, function, (newValue, currentValue) => newValue > currentValue);

	// best, secondWorst, worst
	private static KeyPoints FindKeyPoints(Simplex simplex, RealMultivariableFunction function)
	{
		var (bestPoint, bestValue) = FindMin(simplex, function);
		var (worstPoint, worstValue) = FindMax(simplex, function);

		var (secondWorstPoint, secondWorstValue) = FindPoint(
			simplex,
			function,
			(newValue, currentValue) => currentValue < newValue && newValue < worstValue,
			bestPoint
		);

		return new KeyPoints
		{
			BestPoint = bestPoint,
			BestValue = bestValue,
			SecondWorstPoint = secondWorstPoint,
			SecondWorstValue = secondWorstValue,
			WorstPoint = worstPoint,
			WorstValue = worstValue
		};
	}

	private readonly struct KeyPoints
	{
		public Point BestPoint { get; init; }
		public double BestValue { get; init; }

		public Point SecondWorstPoint { get; init; }
		public double SecondWorstValue { get; init; }

		public Point WorstPoint { get; init; }
		public double WorstValue { get; init; }
	}
}
